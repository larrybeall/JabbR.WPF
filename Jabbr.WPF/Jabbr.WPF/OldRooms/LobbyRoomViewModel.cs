using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.Windows;
using Jabbr.WPF.Infrastructure;
using Jabbr.WPF.Messages;

namespace Jabbr.WPF.OldRooms
{
    public class LobbyRoomViewModel : RoomViewModel
    {
        private readonly JabbrManager _jabbrManager;
        private readonly ServiceLocator _serviceLocator;
        private IObservableCollection<RoomDetailViewModel> _rooms;
        private bool _initialized;

        public LobbyRoomViewModel(
            JabbrManager jabbrManager, 
            ServiceLocator serviceLocator, 
            RoomActionsViewModel roomActions)
        {
            _rooms = new BindableCollection<RoomDetailViewModel>();
            Messages = new BindableCollection<MessageViewModel>();
            _jabbrManager = jabbrManager;
            _serviceLocator = serviceLocator;
            RoomActions = roomActions;
            SubscribeToEvents();
        }
 
        public IObservableCollection<RoomDetailViewModel> Rooms
        {
            get { return _rooms; }
            set
            {
                if(_rooms == value)
                    return;

                _rooms = value;
                NotifyOfPropertyChange(() => Rooms);
            }
        }

        public override void SendCommand(string command)
        {
            //bool result = ((ShellViewModel) Parent).SendCommand(command);
            //if (!result)
            //    MessageBox.Show("send failed");
            //RoomCommand = null;
        }

        protected override void OnActivate()
        {
            if (!_initialized)
            {
                _initialized = true;
                RoomName = "Lobby";
                Messages.Add(new LobbyMessageViewModel { Content = "Welcome to JabbR", MessageTime = DateTime.Now });
                Messages.Add(new LobbyMessageViewModel { Content = "Type /help to see the list of commands", MessageTime = DateTime.Now });
            }

            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            if(close)
                UnSubscribeToEvents();

            base.OnDeactivate(close);
        }

        private void SubscribeToEvents()
        {
            _jabbrManager.RoomCountChanged += JabbrManagerOnRoomCountChanged;
            _jabbrManager.RoomsReceived += JabbrManagerOnRoomsReceived;
            _jabbrManager.LoggedIn += JabbrManagerOnLoggedIn;
            _jabbrManager.LeftRoom += JabbrManagerOnLeftRoom;
        }

        private void UnSubscribeToEvents()
        {
            _jabbrManager.RoomCountChanged -= JabbrManagerOnRoomCountChanged;
            _jabbrManager.RoomsReceived -= JabbrManagerOnRoomsReceived;
            _jabbrManager.LoggedIn -= JabbrManagerOnLoggedIn;
        }

        private void JabbrManagerOnLeftRoom(object sender, RoomEventArgs roomEventArgs)
        {
            Messages.Add(new LobbyMessageViewModel
                             {
                                 Content = string.Format("You have left {0}", roomEventArgs.Room),
                                 MessageTime = DateTime.Now
                             });
        }

        private void JabbrManagerOnLoggedIn(object sender, UserEventArgs userEventArgs)
        {
            Messages.Add(new LobbyMessageViewModel
                             {
                                 Content = string.Format("Welcome back {0}", userEventArgs.User.Name),
                                 MessageTime = DateTime.Now
                             });
            Messages.Add(new LobbyMessageViewModel
                             {
                                 Content = "Type /rooms to  list all available rooms",
                                 MessageTime = DateTime.Now
                             });
            Messages.Add(new LobbyMessageViewModel
                             {
                                 Content = "Type /logout to logout of chat",
                                 MessageTime = DateTime.Now
                             });
        }

        private void JabbrManagerOnRoomsReceived(object sender, RoomsReceivedEventArgs roomsReceivedEventArgs)
        {
            _jabbrManager.RoomsReceived -= JabbrManagerOnRoomsReceived;
            var rooms = roomsReceivedEventArgs.Rooms;

            foreach (var room in rooms.OrderByDescending(room => room.Count))
            {
                var roomVm = _serviceLocator.GetViewModel<RoomDetailViewModel>();
                roomVm.IsPrivate = room.Private;
                roomVm.RoomName = room.Name;
                roomVm.UserCount = room.Count;

                Rooms.Add(roomVm);
            }

            Messages.Add(new LobbyMessageViewModel
                             {
                                 Content = "You can join any of the rooms on the right",
                                 MessageTime = DateTime.Now
                             });
        }

        private void JabbrManagerOnRoomCountChanged(object sender, RoomCountEventArgs roomCountEventArgs)
        {
            var roomName = roomCountEventArgs.Room.Name;
            var toUpdate = Rooms.FirstOrDefault(room => room.RoomName.Equals(roomName));
            if (toUpdate != null)
                toUpdate.UserCount = roomCountEventArgs.Count;
        }
    }
}
