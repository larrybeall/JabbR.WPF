using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jabbr.WPF.Infrastructure.Models;
using Jabbr.WPF.Messages;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Markup;
using HtmlToXamlConversion;
using System.Threading.Tasks;
using Jabbr.WPF.Users;
using JabbrModels = JabbR.Client.Models;

namespace Jabbr.WPF.Infrastructure.Services
{
    public class MessageProcessingService
    {
        private readonly SynchronizationContext _uiContext;
        private readonly ServiceLocator _serviceLocator;

        public MessageProcessingService(ServiceLocator serviceLocator)
        {
            _uiContext = SynchronizationContext.Current;
            _serviceLocator = serviceLocator;
        }

        public event EventHandler<MessageProcessedEventArgs> MessageProcessed;

        public void ProcessMessageAsync(string room, JabbrModels.Message message)
        {
            Message msg = new Message(message);
            ProcessMessageAsync(room, msg);
        }

        public void ProcessMessageAsync(string room, Message message)
        {
            InternalProcessMessage(room, message);
        }

        public ChatMessageViewModel ProcessMessage(JabbrModels.Message message)
        {
            Message msg = new Message(message);
            return ProcessMessage(msg);
        }

        public ChatMessageViewModel ProcessMessage(Message message)
        {
            var task = InternalProcessMessage(null, message, true);
            task.Wait();

            return task.Result.MessageViewModel;
        }

        private Task<MessageProcessedEventArgs> InternalProcessMessage(string room, Message message, bool processSynchronously = false)
        {
            Task<MessageProcessedEventArgs> task = new Task<MessageProcessedEventArgs>(() =>
            {
                string content = ProcessEmoji(message.Content);
                var inlines = ParseMessage(content);
                var msgVm = _serviceLocator.GetViewModel<ChatMessageViewModel>();
                var usrVm = _serviceLocator.GetViewModel<UserViewModel>();

                usrVm.Initialize(message.User, false);

                msgVm.Content = inlines;
                msgVm.RawContent = message.Content;
                msgVm.MessageDateTime = message.When.LocalDateTime;
                msgVm.MessageId = message.Id;
                msgVm.User = usrVm;

                return new MessageProcessedEventArgs(msgVm, room);
            });

            if (!processSynchronously)
            {
                task.ContinueWith((completedTask) => OnMessageProcessed(completedTask.Result, _uiContext),
                                  TaskContinuationOptions.OnlyOnRanToCompletion);
                task.ContinueWith((errorTask) =>
                {
                    AggregateException exception = errorTask.Exception;

                    if (exception == null)
                        return;

                    foreach (var innerException in exception.InnerExceptions)
                    {
                        System.Diagnostics.Trace.WriteLine(innerException.Message);
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);
            }

            task.Start();

            return task;
        }

        private static string ProcessEmoji(string content)
        {
            return content;
        }

        private static InlineCollection ParseMessage(string content)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            string xamlString = HtmlToXamlConverter.ConvertHtmlToXaml(content, false);
            var parsedXaml = XamlReader.Parse(xamlString);
            var xamlLines = ((Paragraph)((Section)parsedXaml).Blocks.FirstBlock).Inlines;

            return xamlLines;
        }

        private void OnMessageProcessed(MessageProcessedEventArgs args, SynchronizationContext context = null)
        {
            if (context != null)
            {
                context.Post(d => OnMessageProcessed(args), null);
                return;
            }

            var handler = MessageProcessed;
            if (handler != null)
                handler(this, args);
        }
    }
}
