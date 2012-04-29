using System;
using System.Collections.Generic;
using System.Linq;
using Jabbr.WPF.Messages;
using System.Threading;
using HtmlToXamlConversion;
using System.Threading.Tasks;
using JabbR.Client.Models;
using JabbR.Client;
using Jabbr.WPF.Rooms;

namespace Jabbr.WPF.Infrastructure.Services
{
    public class MessageService : BaseService
    {
        private readonly ServiceLocator _serviceLocator;
        private readonly UserService _userService;
        private readonly RoomService _roomService;
        private readonly JabbRClient _client;

        public MessageService(
            ServiceLocator serviceLocator, 
            UserService userService, 
            JabbRClient client,
            RoomService roomService)
            :base()
        {
            _serviceLocator = serviceLocator;
            _userService = userService;
            _roomService = roomService;
            _client = client;

            _client.MessageReceived += OnMessageReceived;
        }

        public Task ProcessMessageAsync(string room, Message message)
        {
            Task<ChatMessageViewModel> task = new Task<ChatMessageViewModel>(() =>
            {
                var msgVm = CreateMessageViewModel(message);
                return msgVm;
            });

            task.ContinueWith(completedTask => OnMessageProcessed(completedTask.Result, room),
                                TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith(ProcessTaskExceptions, TaskContinuationOptions.OnlyOnFaulted);

            task.Start();
            return task;
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

        private void OnMessageProcessed(ChatMessageViewModel messageViewModel, string room)
        {
            var roomViewModel = _roomService.GetRoom(room);

            PostOnUi(() => roomViewModel.ProcessMessage(messageViewModel));
        }

        private void OnMessageReceived(Message message, string room)
        {
            ProcessMessageAsync(room, message);
        }
    }
}
