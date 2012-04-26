using System;
using System.Collections.Generic;
using System.Linq;
using Jabbr.WPF.Messages;
using System.Threading;
using HtmlToXamlConversion;
using System.Threading.Tasks;
using JabbR.Client.Models;

namespace Jabbr.WPF.Infrastructure.Services
{
    public class MessageService
    {
        private readonly SynchronizationContext _uiContext;
        private readonly ServiceLocator _serviceLocator;
        private readonly UserService _userService;

        public MessageService(ServiceLocator serviceLocator, UserService userService)
        {
            _uiContext = SynchronizationContext.Current;
            _serviceLocator = serviceLocator;
            _userService = userService;
        }

        public event EventHandler<MessageProcessedEventArgs> MessageProcessed;

        public void ProcessMessageAsync(string room, Message message)
        {
            Task<MessageProcessedEventArgs> task = new Task<MessageProcessedEventArgs>(() =>
            {
                var msgVm = CreateMessageViewModel(message);

                return new MessageProcessedEventArgs(msgVm, room);
            });

            task.ContinueWith(completedTask => OnMessageProcessed(completedTask.Result, _uiContext),
                                TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith(ProcessTaskExceptions, TaskContinuationOptions.OnlyOnFaulted);

            task.Start();
        }

        public ChatMessageViewModel ProcessMessage(Message message)
        {
            return CreateMessageViewModel(message);
        }

        public IEnumerable<ChatMessageViewModel> ProcessMessages(IEnumerable<Message> messages)
        {
            var result = messages.AsParallel().Select(CreateMessageViewModel);

            return result.OrderBy(x => x.MessageDateTime);
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
    }
}
