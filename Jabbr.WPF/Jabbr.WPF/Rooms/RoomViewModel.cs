using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Jabbr.WPF.Infrastructure;

namespace Jabbr.WPF.Rooms
{
    public class RoomViewModel : Screen
    {
        private readonly JabbrManager _jabbrManager;

        private bool _isPrivate;
        private int _userCount;
        private int _unreadMessageCount;

        public RoomViewModel(JabbrManager jabbrManager)
        {
            _jabbrManager = jabbrManager;
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

        internal void Initialize(RoomDetailsEventArgs roomDetailsEventArgs)
        {
            _jabbrManager.MessageReceived += JabbrManagerOnMessageReceived;

            var roomDetails = roomDetailsEventArgs.Room;
            RoomName = roomDetails.Name;
            UserCount = roomDetails.Count;
            IsPrivate = roomDetails.Private;
        }

        private void JabbrManagerOnMessageReceived(object sender, MessageReceivedEventArgs messageReceivedEventArgs)
        {
            
        }
    }
}
