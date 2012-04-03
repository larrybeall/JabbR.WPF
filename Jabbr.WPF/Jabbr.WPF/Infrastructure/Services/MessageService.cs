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
using System.Text.RegularExpressions;

namespace Jabbr.WPF.Infrastructure.Services
{
    public class MessageService
    {
        private readonly SynchronizationContext _uiContext;
        private readonly ServiceLocator _serviceLocator;
        private readonly UserService _userService;
        private readonly Regex _tagRegex;

        public MessageService(ServiceLocator serviceLocator, UserService userService)
        {
            _uiContext = SynchronizationContext.Current;
            _serviceLocator = serviceLocator;
            _userService = userService;
            _tagRegex = new Regex(@"<[^>]+>");
        }

        public event EventHandler<MessageProcessedEventArgs> MessageProcessed;

        public void ProcessMessageAsync(string room, JabbrModels.Message message)
        {
            Message msg = new Message(message);
            ProcessMessageAsync(room, msg);
        }

        public void ProcessMessageAsync(string room, Message message)
        {
            Task<MessageProcessedEventArgs> task = new Task<MessageProcessedEventArgs>(() =>
            {
                var msgVm = CreateMessageViewModel(message);

                return new MessageProcessedEventArgs(msgVm, room);
            });

            task.ContinueWith((completedTask) => OnMessageProcessed(completedTask.Result, _uiContext),
                                TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith(ProcessTaskExceptions, TaskContinuationOptions.OnlyOnFaulted);

            task.Start();
        }

        public ChatMessageViewModel ProcessMessage(JabbrModels.Message message)
        {
            Message msg = new Message(message);
            return ProcessMessage(msg);
        }

        public ChatMessageViewModel ProcessMessage(Message message)
        {
            return CreateMessageViewModel(message);
        }

        public IEnumerable<ChatMessageViewModel> ProcessMessages(IEnumerable<Message> messages)
        {
            List<Task<ChatMessageViewModel>> parsingTasks = new List<Task<ChatMessageViewModel>>();
            foreach (var message in messages)
            {
                Message toProcess = message;
                var task = Task.Factory.StartNew<ChatMessageViewModel>(() => CreateMessageViewModel(toProcess));
                task.ContinueWith(ProcessTaskExceptions, TaskContinuationOptions.OnlyOnFaulted);

                parsingTasks.Add(task);
            }

            var waitTasks = parsingTasks.ToArray();
            Task.WaitAll(waitTasks);

            return parsingTasks.Select(x => x.Result);
        }

        private void ProcessTaskExceptions(Task errorTask)
        {
            AggregateException exception = errorTask.Exception;

            if (exception == null)
                return;

            foreach (var innerException in exception.InnerExceptions)
            {
                System.Diagnostics.Trace.WriteLine(innerException.Message);
            }
        }

        private ChatMessageViewModel CreateMessageViewModel(Message message)
        {
            string content = ProcessEmoji(message.Content);
            var msgVm = _serviceLocator.GetViewModel<ChatMessageViewModel>();
            var userVm = _userService.GetUserViewModel(message.User);

            msgVm.IsNotifying = false;

            msgVm.RawContent = message.Content;
            msgVm.MessageDateTime = message.When.LocalDateTime;
            msgVm.MessageId = message.Id;
            msgVm.RichContent = ConvertToXaml(content);
            msgVm.User = userVm;

            msgVm.IsNotifying = true;

            return msgVm;
        }

        private static string ProcessEmoji(string content)
        {
            return content;
        }

        private static string ConvertToXaml(string content)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            return HtmlToXamlConverter.ConvertHtmlToXaml(content, false);
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

        private static bool ContentContainsHtml(string content, Regex tagRegex)
        {
            if (string.IsNullOrEmpty(content))
                return false;

            return tagRegex.IsMatch(content);
        }

        private static Inline[] CreateInlineArray(string xamlString)
        {
            try
            {
                var parsedXaml = XamlReader.Parse(xamlString);
                var inlineCollection = ((Paragraph)((Section)parsedXaml).Blocks.FirstBlock).Inlines;
                Inline[] inlines = new Inline[inlineCollection.Count];
                inlineCollection.CopyTo(inlines, 0);

                return inlines;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
