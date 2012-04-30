using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HtmlToXamlConversion;
using JabbR.Client;
using JabbR.Client.Models;
using Jabbr.WPF.Messages;
using Jabbr.WPF.Rooms;
using Jabbr.WPF.Users;

namespace Jabbr.WPF.Infrastructure.Services
{
    public class MessageService : BaseService
    {
        private readonly JabbRClient _client;
        private readonly RoomService _roomService;
        private readonly ServiceLocator _serviceLocator;
        private readonly UserService _userService;

        public MessageService(
            ServiceLocator serviceLocator,
            UserService userService,
            JabbRClient client,
            RoomService roomService)
        {
            _serviceLocator = serviceLocator;
            _userService = userService;
            _roomService = roomService;
            _client = client;

            _client.MessageReceived += OnMessageReceived;
        }

        public Task ProcessMessageAsync(string room, Message message)
        {
            var task = new Task<ChatMessageViewModel>(() =>
            {
                ChatMessageViewModel msgVm = CreateMessageViewModel(message);
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
            ParallelQuery<ChatMessageViewModel> result = messages.AsParallel().Select(CreateMessageViewModel);

            return result.OrderBy(x => x.MessageDateTime);
        }

        private void ProcessTaskExceptions(Task errorTask)
        {
            AggregateException exception = errorTask.Exception;

            if (exception == null)
                return;

            foreach (Exception innerException in exception.InnerExceptions)
            {
                Trace.WriteLine(innerException.Message);
            }
        }

        private ChatMessageViewModel CreateMessageViewModel(Message message)
        {
            string content = ProcessEmoji(message.Content);
            var msgVm = _serviceLocator.GetViewModel<ChatMessageViewModel>();
            UserViewModel userVm = _userService.GetUserViewModel(message.User);

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
            RoomViewModel roomViewModel = _roomService.GetRoom(room);

            PostOnUi(() => roomViewModel.ProcessMessage(messageViewModel));
        }

        private void OnMessageReceived(Message message, string room)
        {
            ProcessMessageAsync(room, message);
        }
    }
}