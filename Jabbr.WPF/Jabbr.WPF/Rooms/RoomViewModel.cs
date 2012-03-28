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

namespace Jabbr.WPF.Rooms
{
    public class RoomViewModel : Screen
    {
        private readonly JabbrManager _jabbrManager;
        private readonly ServiceLocator _serviceLocator;
        private readonly IObservableCollection<ChatMessageViewModel> _messages;
        private readonly IObservableCollection<UserViewModel> _users;
        private readonly AutoRefreshCollectionViewSource _usersSource;
        private IEnumerable<string> _owners; 

        private bool _isPrivate;
        private int _userCount;
        private int _unreadMessageCount;
        private string _topic;

        public RoomViewModel(JabbrManager jabbrManager, ServiceLocator serviceLocator)
        {
            _jabbrManager = jabbrManager;
            _serviceLocator = serviceLocator;
            _messages = new BindableCollection<ChatMessageViewModel>();
            _users = new BindableCollection<UserViewModel>();
            _usersSource = new AutoRefreshCollectionViewSource {Source = _users};

            _usersSource.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
            _usersSource.SortDescriptions.Add(new SortDescription("Group", ListSortDirection.Ascending));
            _usersSource.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
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

        public IObservableCollection<ChatMessageViewModel> Messages
        {
            get { return _messages; }
        }

        public ICollectionView Users
        {
            get { return _usersSource.View; }
        }

        internal void Initialize(RoomDetailsEventArgs roomDetailsEventArgs)
        {
            _jabbrManager.MessageReceived += JabbrManagerOnMessageReceived;
            _jabbrManager.RoomCountChanged += JabbrManagerOnRoomCountChanged;
            _jabbrManager.RoomTopicChanged += JabbrManagerOnRoomTopicChanged;
            _jabbrManager.UserJoinedRoom += JabbrManagerOnUserJoinedRoom;
            _jabbrManager.NoteChanged += JabbrManagerOnNoteChanged;

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

            foreach (var recentMessage in roomDetailsEventArgs.Room.RecentMessages)
            {
                ProcessMessage(recentMessage, true);
            }
        }

        private void AddUser(User user)
        {
            var userVm = _serviceLocator.GetViewModel<UserViewModel>();
            bool isOwner = _owners.Any(x => x.Equals(user.Name));
            userVm.Initialize(user, isOwner);

            _users.Add(userVm);
        }

        private bool VerifyRoomName(string roomName)
        {
            return roomName.Equals(RoomName, StringComparison.InvariantCultureIgnoreCase);
        }

        private void JabbrManagerOnNoteChanged(object sender, UserRoomSpecificEventArgs userRoomSpecificEventArgs)
        {
            if(!VerifyRoomName(userRoomSpecificEventArgs.Room))
                return;

            var userVm = _users.SingleOrDefault(x => x.Name.Equals(userRoomSpecificEventArgs.User.Name));
            if(userVm == null)
                return;

            userVm.SetNote(userRoomSpecificEventArgs.User);
        }

        private void JabbrManagerOnUserJoinedRoom(object sender, UserRoomSpecificEventArgs userRoomSpecificEventArgs)
        {
            if(!VerifyRoomName(userRoomSpecificEventArgs.Room))
                return;

            string userName = userRoomSpecificEventArgs.User.Name;
            if(_users.Any(x => x.Name.Equals(userName, StringComparison.InvariantCultureIgnoreCase)))
                return;

            AddUser(userRoomSpecificEventArgs.User);
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

        private void JabbrManagerOnMessageReceived(object sender, MessageReceivedEventArgs messageReceivedEventArgs)
        {
            if(!VerifyRoomName(messageReceivedEventArgs.Room))
                return;

            ProcessMessage(messageReceivedEventArgs.Message);
        }

        private void ProcessMessage(Jabbr.WPF.Infrastructure.Models.Message message, bool isInitializing = false)
        {
            var msgVm = _serviceLocator.GetViewModel<ChatMessageViewModel>();
            
            msgVm.Initialize(message, IsRoomVisible(isInitializing));
            Messages.Add(msgVm);
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
            UnreadMessageCount = Messages.Count(msg => !msg.Seen);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            var unseenMessages = Messages.Where(x => x.Seen == false).ToList();
            foreach (var chatMessageViewModel in unseenMessages)
            {
                chatMessageViewModel.Seen = true;
            }
            UpdateUnreadMessageCount();
        }
    }
}
