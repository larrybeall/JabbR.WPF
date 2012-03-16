using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Jabbr.WPF.Messages;

namespace Jabbr.WPF.OldRooms
{
    public abstract class RoomViewModel : Screen
    {
        private string _roomName;
        private RoomActionsViewModel _roomActions;
        private string _roomCommand;
        private IObservableCollection<MessageViewModel> _messages;
 
        public IObservableCollection<MessageViewModel> Messages
        {
            get { return _messages; }
            set
            {
                if(_messages == value)
                    return;

                _messages = value;
                NotifyOfPropertyChange(() => Messages);
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
                NotifyOfPropertyChange(() => RoomName);
            }
        }

        public RoomActionsViewModel RoomActions
        {
            get { return _roomActions; }
            set
            {
                if(_roomActions == value)
                    return;

                _roomActions = value;
                NotifyOfPropertyChange(() => RoomActions);
            }
        }

        public string RoomCommand
        {
            get { return _roomCommand; }
            set
            {
                if (_roomCommand == value)
                    return;

                _roomCommand = value;
                NotifyOfPropertyChange(() => RoomCommand);
            }
        }

        public virtual bool CanSendCommand(string command)
        {
            return !string.IsNullOrEmpty(command);
        }

        public abstract void SendCommand(string command);
    }
}
