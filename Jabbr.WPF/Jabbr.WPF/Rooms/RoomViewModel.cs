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
        private readonly ServiceLocator _serviceLocator;
        private readonly MessageService _messageService;
        private readonly UserService _userService;
        private readonly IObservableCollection<ChatMessageViewModel> _messages;
        private readonly IObservableCollection<IUserViewModel> _users;
        private readonly AutoRefreshCollectionViewSource _usersSource;
        private readonly AutoRefreshCollectionViewSource _messagesSource;
        private IEnumerable<string> _owners; 

        private bool _isPrivate;
        private int _userCount;
        private int _unreadMessageCount;
        private string _topic;

        public RoomViewModel(
            JabbrManager jabbrManager, 
            ServiceLocator serviceLocator, 
            MessageService messageService,
            UserService userService)
        {
            _jabbrManager = jabbrManager;
            _serviceLocator = serviceLocator;
            _messageService = messageService;
            _userService = userService;
            _messages = new BindableCollection<ChatMessageViewModel>();
            _users = new BindableCollection<IUserViewModel>();
            _usersSource = new AutoRefreshCollectionViewSource {Source = _users};
            _messagesSource = new AutoRefreshCollectionViewSource {Source = _messages};

            _usersSource.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
            _usersSource.SortDescriptions.Add(new SortDescription("Group", ListSortDirection.Ascending));
            _usersSource.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            _messagesSource.SortDescriptions.Add(new SortDescription("MessageDateTime", ListSortDirection.Ascending));
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
                if (DisplayName == value)
                    return;

                DisplayName = value;
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
            }
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

        public ICollectionView Messages
        {
            get { return _messagesSource.View; }
        }

        public ICollectionView Users
        {
            get { return _usersSource.View; }
        }

        internal void Initialize(RoomDetailsEventArgs roomDetailsEventArgs)
        {
            _jabbrManager.RoomCountChanged += JabbrManagerOnRoomCountChanged;
            _jabbrManager.RoomTopicChanged += JabbrManagerOnRoomTopicChanged;
            _userService.UserJoined += UserServiceOnUserJoined;
            _messageService.MessageProcessed += MessageProcessingServiceOnMessageProcessed;

            var roomDetails = roomDetailsEventArgs.Room;
            RoomName = roomDetails.Name;
            UserCount = roomDetails.Users.Count();
            IsPrivate = roomDetails.Private;
            Topic = roomDetails.Topic;
            _owners = roomDetails.Owners;

            foreach (var user in roomDetails.Users)
            {
                AddUser(user);
            }

            var messageViewModels = _messageService.ProcessMessages(roomDetails.RecentMessages);
            foreach (var chatMessageViewModel in messageViewModels)
            {
                ProcessMessage(chatMessageViewModel, true);
            }
        }

        private void UserServiceOnUserJoined(object sender, UserJoinedEventArgs userJoinedEventArgs)
        {
            if (!VerifyRoomName(userJoinedEventArgs.Room))
                return;

            if(_users.Contains(userJoinedEventArgs.UserViewModel))
                return;

            AddUser(userJoinedEventArgs.UserViewModel);
        }

        private void MessageProcessingServiceOnMessageProcessed(object sender, MessageProcessedEventArgs messageProcessedEventArgs)
        {
            if (!VerifyRoomName(messageProcessedEventArgs.Room))
                return;

            ProcessMessage(messageProcessedEventArgs.MessageViewModel);
        }

        private void AddUser(User user)
        {
            var userVm = _userService.GetUserViewModel(user);

            AddUser(userVm);
        }

        private void AddUser(IUserViewModel userViewModel)
        {
            var userVm = userViewModel;
            if (_owners.Any(x => x.Equals(userVm.Name)))
            {
                userVm = new OwnerViewModel(userViewModel);
            }

            _users.Add(userVm);
        }

        private bool VerifyRoomName(string roomName)
        {
            return roomName.Equals(RoomName, StringComparison.InvariantCultureIgnoreCase);
        }

        private void JabbrManagerOnUserJoinedRoom(object sender, UserRoomSpecificEventArgs userRoomSpecificEventArgs)
        {
            
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

        private void ProcessMessage(Jabbr.WPF.Infrastructure.Models.Message message, bool isInitializing = false)
        {
            var msgVm = _messageService.ProcessMessage(message);
            ProcessMessage(msgVm, isInitializing);
        }

        private void ProcessMessage(ChatMessageViewModel viewModel, bool isInitializing = false)
        {
            viewModel.HasBeenSeen = IsRoomVisible(isInitializing);

            _messages.Add(viewModel);
            UpdateUnreadMessageCount();
        }

        private bool IsRoomVisible(bool isRoomInitializing = false)
        {
            if (Application.Current.MainWindow.IsActive == false || Application.Current.MainWindow.WindowState == WindowState.Minimized)
                return false;

            if (isRoomInitializing)
                return true;

            return IsActive;
        }

        private void UpdateUnreadMessageCount()
        {
            UnreadMessageCount = _messages.Count(msg => !msg.HasBeenSeen);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            var unseenMessages = _messages.Where(x => x.HasBeenSeen == false).ToList();
            foreach (var chatMessageViewModel in unseenMessages)
            {
                chatMessageViewModel.HasBeenSeen = true;
            }
            UpdateUnreadMessageCount();
        }
    }
}
