using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Jabbr.WPF.Infrastructure;
using Jabbr.WPF.Messages;
using Jabbr.WPF.Users;
using System.ComponentModel;
using Jabbr.WPF.Infrastructure.Models;

namespace Jabbr.WPF.Rooms
{
    public class RoomViewModel : Screen
    {
        private readonly JabbrManager _jabbrManager;
        private readonly ServiceLocator _serviceLocator;
        private readonly IObservableCollection<ChatMessageViewModel> _messages;
        private readonly IObservableCollection<UserViewModel> _users;
        private readonly AutoRefreshCollectionViewSource _ownerUsersSource;
        private readonly AutoRefreshCollectionViewSource _activeUsersSource;
        private readonly AutoRefreshCollectionViewSource _awayUsersSource;
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
            _ownerUsersSource = new AutoRefreshCollectionViewSource {Source = _users};
            _activeUsersSource = new AutoRefreshCollectionViewSource {Source = _users};
            _awayUsersSource = new AutoRefreshCollectionViewSource {Source = _users};

            Owners.Filter = FilterOwnerUsers;
            ActiveUsers.Filter = FilterActiveUsers;
            AwayUsers.Filter = FilterAwayUsers;
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

        public ICollectionView Owners
        {
            get { return _ownerUsersSource.View; }
        }

        public ICollectionView ActiveUsers
        {
            get { return _activeUsersSource.View; }
        }

        public ICollectionView AwayUsers
        {
            get { return _awayUsersSource.View; }
        }

        internal void Initialize(RoomDetailsEventArgs roomDetailsEventArgs)
        {
            _jabbrManager.MessageReceived += JabbrManagerOnMessageReceived;
            _jabbrManager.RoomCountChanged += JabbrManagerOnRoomCountChanged;
            _jabbrManager.RoomTopicChanged += JabbrManagerOnRoomTopicChanged;
            _jabbrManager.UserJoinedRoom += JabbrManagerOnUserJoinedRoom;

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
        }

        private void AddUser(User user)
        {
            var userVm = _serviceLocator.GetViewModel<UserViewModel>();
            bool isOwner = _owners.Any(x => x.Equals(user.Name));
            userVm.Initialize(user, isOwner);

            _users.Add(userVm);
        }

        private void JabbrManagerOnUserJoinedRoom(object sender, UserRoomSpecificEventArgs userRoomSpecificEventArgs)
        {
            if(!userRoomSpecificEventArgs.Room.Equals(RoomName, StringComparison.InvariantCultureIgnoreCase))
                return;

            string userName = userRoomSpecificEventArgs.User.Name;
            if(_users.Any(x => x.Name.Equals(userName, StringComparison.InvariantCultureIgnoreCase)))
                return;

            AddUser(userRoomSpecificEventArgs.User);
        }

        private void JabbrManagerOnRoomTopicChanged(object sender, RoomDetailsEventArgs roomDetailsEventArgs)
        {
            if (RoomName.Equals(roomDetailsEventArgs.Room.Name, StringComparison.InvariantCultureIgnoreCase))
                Topic = roomDetailsEventArgs.Room.Topic;
        }

        private void JabbrManagerOnRoomCountChanged(object sender, RoomCountEventArgs roomCountEventArgs)
        {
            if (RoomName.Equals(roomCountEventArgs.Room.Name, StringComparison.InvariantCultureIgnoreCase))
                UserCount = roomCountEventArgs.Count;
        }

        private void JabbrManagerOnMessageReceived(object sender, MessageReceivedEventArgs messageReceivedEventArgs)
        {
            if(!RoomName.Equals(messageReceivedEventArgs.Room, StringComparison.InvariantCultureIgnoreCase))
                return;

            var msgVm = _serviceLocator.GetViewModel<ChatMessageViewModel>();
            // TODO: set is room visible properly
            msgVm.Initialize(messageReceivedEventArgs, false);
            Messages.Add(msgVm);

            UnreadMessageCount = Messages.Count(msg => !msg.Seen);
        }

        private bool FilterOwnerUsers(object data)
        {
            UserViewModel userVm = data as UserViewModel;
            if (userVm == null)
                return false;

            return userVm.IsOwner;
        }

        private bool FilterAwayUsers(object data)
        {
            UserViewModel userVm = data as UserViewModel;
            if (userVm == null)
                return false;

            return !userVm.IsOwner && userVm.IsAway;
        }

        private bool FilterActiveUsers(object data)
        {
            UserViewModel userVm = data as UserViewModel;
            if (userVm == null)
                return false;

            return !userVm.IsOwner && !userVm.IsAway;
        }
    }
}
