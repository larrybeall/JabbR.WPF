using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace Jabbr.WPF.Rooms
{
    public abstract class RoomViewModel : Screen
    {
        private string _roomName;
        public string RoomName
        {
            get { return _roomName; }
            set
            {
                if(_roomName == value)
                    return;

                _roomName = value;
                NotifyOfPropertyChange(() => RoomName);
            }
        }
    }
}
