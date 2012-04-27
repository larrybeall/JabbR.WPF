using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Caliburn.Micro;
using Jabbr.WPF.Infrastructure;
using Jabbr.WPF.Messages;
using Jabbr.WPF.Users;
using System.ComponentModel;
using Jabbr.WPF.Infrastructure.Models;
using System.Windows;
using Jabbr.WPF.Infrastructure.Services;

namespace Jabbr.WPF.Rooms
{
    public class RoomViewModel : Screen
    {
        private readonly JabbrManager _jabbrManager;
        private readonly MessageService _messageService;
        private readonly RoomService _roomService;
        private readonly UserService _userService;
        private readonly IObservableCollection<MessageViewModel> _messages; 
        private readonly IObservableCollection<RoomUserViewModel> _users;
        private IEnumerable<string> _owners; 

        private bool _isPrivate;
        private int _userCount;
        private int _unreadMessageCount;
        private string _topic;
        private JoinState _joinState;

        public RoomViewModel(
            JabbrManager jabbrManager, 
            MessageService messageService,
            RoomService roomService,
            UserService userService)
        {
            _jabbrManager = jabbrManager;
            _messageService = messageService;
            _roomService = roomService;
            _userService = userService;
            _messages = new BindableCollection<MessageViewModel>();
            _users = new BindableCollection<RoomUserViewModel>();
        }

        public int UserCount
        {
            get { return _userCount; }
            set
            {
                if (_userCount == value)
                    return;

                _userCount = value;
                NotifyOfPropertyChange(() => UserCount);
            }
        }

        public JoinState JoinState
        {
            get { return _joinState; }
            set
            {
                if(_joinState == value)
                    return;

                _joinState = value;
                NotifyOfPropertyChange(() => JoinState);
            }
        }

        public bool IsPrivate
        {
            get { return _isPrivate; }
            set
            {
                if (_isPrivate == value)
                    return;

                _isPrivate = value;
                NotifyOfPropertyChange(() => IsPrivate);
            }
        }

        public string RoomName
        {
            get { return DisplayName; }
            set
            {
                if (DisplayName == value || value == null)
                    return;

                DisplayName = value.ToUpper();
                NotifyOfPropertyChange(() => RoomName);
            }
        }

        public int UnreadMessageCount
        {
            get { return _unreadMessageCount; }
            set
            {
                if(_unreadMessageCount == value)
                    return;

                _unreadMessageCount = value;
                NotifyOfPropertyChange(() => UnreadMessageCount);
                NotifyOfPropertyChange(() => HasUnreadMessages);
            }
        }

        public bool HasUnreadMessages
        {
            get { return UnreadMessageCount > 0; }
        }

        public string Topic
        {
            get { return _topic; }
            set
            {
                if(_topic == value)
                    return;

                _topic = value;
                NotifyOfPropertyChange(() => Topic);
            }
        }

        public IObservableCollection<MessageViewModel> Messages
        {
            get { return _messages; }
        }

        public IObservableCollection<RoomUserViewModel> Users
        {
            get { return _users; }
        }

        internal void Populate(JabbR.Client.Models.Room roomDetails)
        {
            SetIsNotifying(false);

            RoomName = roomDetails.Name;
            IsPrivate = roomDetails.Private;
            UserCount = roomDetails.Count;
            Topic = roomDetails.Topic;
            _owners = roomDetails.Owners;

            if (roomDetails.Users != null)
            {
                int userCount = 0;
                foreach (var user in roomDetails.Users)
                {
                    AddUser(user);
                    userCount = userCount + 1;
                }

                UserCount = userCount;
            }

            if (roomDetails.RecentMessages != null && roomDetails.RecentMessages.Any())
            {
                var messageViewModels = _messageService.ProcessMessages(roomDetails.RecentMessages);
                foreach (var chatMessageViewModel in messageViewModels)
                {
                    ProcessMessage(chatMessageViewModel, true);
                }
            }

            SetIsNotifying(true);
        }

        public void LeaveRoom()
        {
            _roomService.LeaveRoom(this);

            TryClose();
        }

        internal void OnJoined(JabbR.Client.Models.Room roomDetails)
        {
            _jabbrManager.RoomCountChanged += JabbrManagerOnRoomCountChanged;
            _jabbrManager.RoomTopicChanged += JabbrManagerOnRoomTopicChanged;
            _userService.UserJoined += UserServiceOnUserJoined;
            _messageService.MessageProcessed += MessageProcessingServiceOnMessageProcessed;

            SetIsNotifying(false);

            Populate(roomDetails);
            JoinState = JoinState.Joined;

            SetIsNotifying(true, true);
        }

        private void SetIsNotifying(bool isNotifying, bool forceRefresh = false)
        {
            IsNotifying = isNotifying;
            _users.IsNotifying = isNotifying;
            _messages.IsNotifying = isNotifying;

            if (forceRefresh)
            {
                _users.Refresh();
                _messages.Refresh();
                Refresh();
            }
        }

        private void UserServiceOnUserJoined(object sender, UserJoinedEventArgs userJoinedEventArgs)
        {
            if (!VerifyRoomName(userJoinedEventArgs.Room))
                return;

            if(_users.Any(usr => usr.User == userJoinedEventArgs.UserViewModel))
                return;

            AddUser(userJoinedEventArgs.UserViewModel);
        }

        private void MessageProcessingServiceOnMessageProcessed(object sender, MessageProcessedEventArgs messageProcessedEventArgs)
        {
            if (!VerifyRoomName(messageProcessedEventArgs.Room))
                return;

            ProcessMessage(messageProcessedEventArgs.MessageViewModel);
        }

        private void AddUser(JabbR.Client.Models.User user)
        {
            var userVm = _userService.GetUserViewModel(user);

            AddUser(userVm);
        }

        private void AddUser(UserViewModel userViewModel)
        {
            var userVm = new RoomUserViewModel(userViewModel);
            userVm.IsOwner = _owners.Any(x => x.Equals(userVm.Name));

            _users.Add(userVm);
        }

        private bool VerifyRoomName(string roomName)
        {
            return roomName.Equals(RoomName, StringComparison.InvariantCultureIgnoreCase);
        }

        private void JabbrManagerOnRoomTopicChanged(object sender, RoomDetailsEventArgs roomDetailsEventArgs)
        {
            if(VerifyRoomName(roomDetailsEventArgs.Room.Name))
                Topic = roomDetailsEventArgs.Room.Topic;
        }

        private void JabbrManagerOnRoomCountChanged(object sender, RoomCountEventArgs roomCountEventArgs)
        {
            if(VerifyRoomName(roomCountEventArgs.Room.Name))
                UserCount = roomCountEventArgs.Count;
        }

        private void ProcessMessage(ChatMessageViewModel viewModel, bool isInitializing = false)
        {
            viewModel.HasBeenSeen = IsRoomVisible(isInitializing);

            ChatMessageGroupViewModel lastGroupMessage = _messages.LastOrDefault() as ChatMessageGroupViewModel;
            if (lastGroupMessage == null || !lastGroupMessage.TryAddMessage(viewModel))
            {
                ChatMessageGroupViewModel groupViewModel = new ChatMessageGroupViewModel(viewModel);
                _messages.Add(groupViewModel);
            }

            UpdateUnreadMessageCount();
        }

        private bool IsRoomVisible(bool isRoomInitializing = false)
        {
            if (isRoomInitializing)
                return true;

            if (Application.Current.MainWindow.IsActive == false || Application.Current.MainWindow.WindowState == WindowState.Minimized)
                return false;

            return IsActive;
        }

        private void UpdateUnreadMessageCount()
        {
            int hasNotBeenSeenCount = _messages.OfType<IHasBeenSeen>().Count(x => !x.HasBeenSeen);
            UnreadMessageCount = _messages.OfType<ICanHaveUnreadMessages>().Sum(x => x.UnreadMessageCount) +
                                 hasNotBeenSeenCount;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            // capture point in time
            var hasUnreadMessags = _messages.OfType<ICanHaveUnreadMessages>().Where(x => x.HasUnreadMessages).ToList();
            hasUnreadMessags.ForEach(x => x.SetAllMessagesRead());

            // capture point in time
            var unseenMessages = _messages.OfType<IHasBeenSeen>().Where(x => !x.HasBeenSeen).ToList();
            unseenMessages.ForEach(x => x.HasBeenSeen = true);

            UnreadMessageCount = 0;
        }
    }
}
