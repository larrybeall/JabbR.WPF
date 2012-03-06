using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace Jabbr.WPF.Rooms
{
    public class RoomDetailViewModel : PropertyChangedBase
    {
        private string _roomName;
        private bool _isPrivate;
        private int _userCount;

        public int UserCount
        {
            get { return _userCount; }
            set
            {
                if(_userCount == value)
                    return;

                _userCount = value;
                NotifyOfPropertyChange(() => DisplayValue);
            }
        }

        public bool IsPrivate
        {
            get { return _isPrivate; }
            set
            {
                if(_isPrivate == value)
                    return;

                _isPrivate = value;
                NotifyOfPropertyChange(()  => IsPrivate);
            }
        }

        public string RoomName
        {
            get { return _roomName; }
            set
            {
                if(_roomName == value)
                    return;
                
                _roomName = value;
                NotifyOfPropertyChange(() => DisplayValue);
            }
        }

        public string DisplayValue
        {
            get { return string.Format("{0} ({1})", RoomName, UserCount); }
        }
    }
}
