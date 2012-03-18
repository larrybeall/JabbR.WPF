using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Jabbr.WPF.Infrastructure;
using Jabbr.WPF.Messages;

namespace Jabbr.WPF.Rooms
{
    public class RoomViewModel : Screen
    {
        private readonly JabbrManager _jabbrManager;
        private readonly ServiceLocator _serviceLocator;
        private readonly IObservableCollection<ChatMessageViewModel> _messages;

        private bool _isPrivate;
        private int _userCount;
        private int _unreadMessageCount;
        private string _topic;

        public RoomViewModel(JabbrManager jabbrManager, ServiceLocator serviceLocator)
        {
            _jabbrManager = jabbrManager;
            _serviceLocator = serviceLocator;
            _messages = new BindableCollection<ChatMessageViewModel>();
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

        internal void Initialize(RoomDetailsEventArgs roomDetailsEventArgs)
        {
            _jabbrManager.MessageReceived += JabbrManagerOnMessageReceived;
            _jabbrManager.RoomCountChanged += JabbrManagerOnRoomCountChanged;
            _jabbrManager.RoomTopicChanged += JabbrManagerOnRoomTopicChanged;

            var roomDetails = roomDetailsEventArgs.Room;
            RoomName = roomDetails.Name;
            UserCount = roomDetails.Users.Count();
            IsPrivate = roomDetails.Private;
            Topic = roomDetails.Topic;
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
    }
}
