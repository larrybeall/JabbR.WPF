using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using JabbR.Client.Models;
using Jabbr.WPF.Infrastructure.Services;
using Jabbr.WPF.Messages;
using Jabbr.WPF.Users;

namespace Jabbr.WPF.Rooms
{
    public class RoomViewModel : Screen
    {
        #region fields

        private readonly MessageService _messageService;
        private readonly IObservableCollection<MessageViewModel> _messages;
        private readonly RoomService _roomService;
        private readonly UserService _userService;
        private readonly IObservableCollection<RoomUserViewModel> _users;
        private DispatcherTimer _typingTimer;

        private bool _isPrivate;
        private JoinState _joinState;
        private List<string> _owners;
        private string _topic;
        private int _unreadMessageCount;
        private int _userCount;
        private bool _isTyping;
        private bool _isRequstingPreviousMessages;

        #endregion

        #region constructors

        public RoomViewModel(
            MessageService messageService,
            RoomService roomService,
            UserService userService)
        {
            _messageService = messageService;
            _roomService = roomService;
            _userService = userService;
            _messages = new BindableCollection<MessageViewModel>();
            _users = new BindableCollection<RoomUserViewModel>();
        }

        #endregion

        #region properties

        public bool IsRequestingPreviousMessages
        {
            get { return _isRequstingPreviousMessages; }
            set
            {
                if(_isRequstingPreviousMessages == value)
                    return;

                _isRequstingPreviousMessages = value;
                NotifyOfPropertyChange(() => IsRequestingPreviousMessages);
            }
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
                if (_joinState == value)
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
                if (_unreadMessageCount == value)
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
                if (_topic == value)
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

        #endregion

        #region internal methods

        internal void Populate(Room roomDetails, bool suppressNotification = true)
        {
            if (suppressNotification)
                SetIsNotifying(false);

            RoomName = roomDetails.Name;
            IsPrivate = roomDetails.Private;
            UserCount = roomDetails.Count;
            Topic = roomDetails.Topic;

            if (roomDetails.Owners != null)
                _owners = new List<string>(roomDetails.Owners);

            if (roomDetails.Users != null)
            {
                int userCount = 0;
                foreach (User user in roomDetails.Users)
                {
                    AddUser(user);
                    userCount = userCount + 1;
                }

                UserCount = userCount;
            }

            if (roomDetails.RecentMessages != null && roomDetails.RecentMessages.Any())
            {
                IEnumerable<ChatMessageViewModel> messageViewModels =
                    _messageService.ProcessMessages(roomDetails.RecentMessages);
                foreach (ChatMessageViewModel chatMessageViewModel in messageViewModels)
                {
                    ProcessMessage(chatMessageViewModel, true);
                }
            }

            if (suppressNotification)
                SetIsNotifying(true);
        }

        internal void OnJoined(Room roomDetails)
        {
            Populate(roomDetails, false);

            // ReSharper disable UseObjectOrCollectionInitializer
            _typingTimer = new DispatcherTimer();
            // ReSharper restore UseObjectOrCollectionInitializer
            _typingTimer.Interval = TimeSpan.FromSeconds(3);
            _typingTimer.Tick += TypingExpired;

            JoinState = JoinState.Joined;
        }

        internal void AddUser(UserViewModel userViewModel)
        {
            if (_users.Any(x => x.User.Equals(userViewModel)))
                return;

            var userVm = new RoomUserViewModel(userViewModel);
            userVm.IsOwner = _owners.Any(x => x.Equals(userVm.User.Name));

            _users.Add(userVm);

            // TODO: Implement notification to the room of the joined user
        }

        internal void SetTopic(string topic)
        {
            Topic = topic;

            // TODO: Implement notification to the room of topic change.
        }

        internal void UserLeft(UserViewModel user)
        {
            RoomUserViewModel leftUser = _users.FirstOrDefault(x => x.User.Equals(user));
            if (leftUser != null)
                _users.Remove(leftUser);

            // TODO: Implement notification to the room of left user.
        }

        internal void AddOwner(string userName)
        {
            if (_owners.Contains(userName))
                return;

            _owners.Add(userName);

            RoomUserViewModel userVm = _users.FirstOrDefault(x => x.User.Name.Equals(userName));
            if (userVm != null)
                userVm.IsOwner = true;
        }

        internal void RemoveOwner(string userName)
        {
            if (!_owners.Contains(userName))
                return;

            _owners.Remove(userName);

            RoomUserViewModel userVm = _users.FirstOrDefault(x => x.User.Name.Equals(userName));
            if (userVm != null)
                userVm.IsOwner = false;
        }

        internal void SetUserTyping(UserViewModel user)
        {
            RoomUserViewModel userVm = _users.FirstOrDefault(x => x.User.Equals(user));
            if (userVm != null)
                userVm.IsTyping = true;
        }

        internal void ProcessMessage(ChatMessageViewModel viewModel, bool suppressUnreadNotifications = false)
        {
            viewModel.HasBeenSeen = IsRoomVisible(suppressUnreadNotifications);

            var lastGroupMessage = _messages.LastOrDefault() as ChatMessageGroupViewModel;
            if (lastGroupMessage == null || !lastGroupMessage.TryAddMessage(viewModel))
            {
                var groupViewModel = new ChatMessageGroupViewModel(viewModel);
                _messages.Add(groupViewModel);
            }

            UpdateUnreadMessageCount();
        }

        internal void AddPreviousMessages(IEnumerable<ChatMessageViewModel> messages)
        {
            bool hasUpdates = false;

            _messages.IsNotifying = false;

            foreach (var chatMessage in messages.Reverse())
            {
                chatMessage.HasBeenSeen = true;
                var firstMessageGroup = _messages.FirstOrDefault() as ChatMessageGroupViewModel;
                if (firstMessageGroup == null || !firstMessageGroup.TryAddPreviousMessage(chatMessage))
                {
                    var groupMessage = new ChatMessageGroupViewModel(chatMessage);
                    _messages.Insert(0, groupMessage);

                    hasUpdates = true;
                }
            }

            _messages.IsNotifying = true;

            if(hasUpdates)
                _messages.Refresh();

            IsRequestingPreviousMessages = false;
        }

        internal void Kicked()
        {
            LeaveRoom();

            // TODO: Display warning that user has been kicked.
        }

        internal void SetTyping()
        {
            if(_isTyping)
                return;

            _isTyping = true;
            _typingTimer.Start();
            _roomService.SetTyping(RoomName);
        }

        #endregion

        #region public methods

        public void LeaveRoom()
        {
            _roomService.LeaveRoom(this);

            TryClose();
        }

        public void RequestPreviousMessages()
        {
            if(IsRequestingPreviousMessages)
                return;

            var firstMessage = _messages.FirstOrDefault();
            if (firstMessage == null)
                return;

            IsRequestingPreviousMessages = true;
            _messageService.RequestPreviousMessages(firstMessage.MessageId, this);
        }

        #endregion

        #region private methods

        private void TypingExpired(object sender, EventArgs eventArgs)
        {
            _typingTimer.Stop();
            _isTyping = false;
        }

        private void SetIsNotifying(bool isNotifying)
        {
            IsNotifying = isNotifying;
            _users.IsNotifying = isNotifying;
            _messages.IsNotifying = isNotifying;
        }

        private void AddUser(User user)
        {
            UserViewModel userVm = _userService.GetUserViewModel(user);

            AddUser(userVm);
        }

        private bool IsRoomVisible(bool isRoomInitializing = false)
        {
            if (isRoomInitializing)
                return true;

            if (Application.Current.MainWindow.IsActive == false ||
                Application.Current.MainWindow.WindowState == WindowState.Minimized)
                return false;

            return IsActive;
        }

        private void UpdateUnreadMessageCount()
        {
            int hasNotBeenSeenCount = _messages.OfType<IHasBeenSeen>().Count(x => !x.HasBeenSeen);
            UnreadMessageCount = _messages.OfType<ICanHaveUnreadMessages>().Sum(x => x.UnreadMessageCount) +
                                 hasNotBeenSeenCount;
        }

        #endregion

        #region overrides

        protected override void OnActivate()
        {
            base.OnActivate();

            // capture point in time
            List<ICanHaveUnreadMessages> hasUnreadMessags =
                _messages.OfType<ICanHaveUnreadMessages>().Where(x => x.HasUnreadMessages).ToList();
            hasUnreadMessags.ForEach(x => x.SetAllMessagesRead());

            // capture point in time
            List<IHasBeenSeen> unseenMessages = _messages.OfType<IHasBeenSeen>().Where(x => !x.HasBeenSeen).ToList();
            unseenMessages.ForEach(x => x.HasBeenSeen = true);

            UnreadMessageCount = 0;
        }

        #endregion
    }
}