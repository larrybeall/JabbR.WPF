using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Jabbr.WPF.Authentication;
using Jabbr.WPF.Infrastructure;
using Jabbr.WPF.Rooms;

namespace Jabbr.WPF
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private readonly ServiceLocator _serviceLocator;
        private readonly JabbrManager _jabbrManager;
        private readonly LoginViewModel _loginViewModel;
        private readonly RoomSelectionViewModel _roomSelectionViewModel;
        private readonly ChatWindowViewModel _chatWindowViewModel;

        private bool _loggedIn;
        private bool _hasPreviouslyJoinedRooms;

        public ShellViewModel(
            ServiceLocator serviceLocator, 
            JabbrManager jabbrManager,
            LoginViewModel loginViewModel,
            RoomSelectionViewModel roomSelectionViewModel,
            ChatWindowViewModel chatWindowViewModel)
        {
            _serviceLocator = serviceLocator;
            _jabbrManager = jabbrManager;
            _loginViewModel = loginViewModel;
            _roomSelectionViewModel = roomSelectionViewModel;
            _chatWindowViewModel = chatWindowViewModel;

            Initialize();
        }

        private void Initialize()
        {
            _jabbrManager.LoggedIn += JabbrManagerOnLoggedIn;
            _jabbrManager.JoinedRoom += JabbrManagerOnJoinedRoom;
            _jabbrManager.RoomsReceived += JabbrManagerOnRoomsReceived;

            Items.Add(_loginViewModel);
            Items.Add(_roomSelectionViewModel);
            Items.Add(_chatWindowViewModel);

            ActivateItem(_loginViewModel);
        }

        private void JabbrManagerOnRoomsReceived(object sender, RoomsReceivedEventArgs roomsReceivedEventArgs)
        {
            _jabbrManager.RoomsReceived -= JabbrManagerOnRoomsReceived;

            if(!_loggedIn)
                throw new InvalidOperationException();

            if(!_hasPreviouslyJoinedRooms)
                ActivateItem(_roomSelectionViewModel);
        }

        private void JabbrManagerOnJoinedRoom(object sender, RoomDetailsEventArgs roomDetailsEventArgs)
        {
            _jabbrManager.JoinedRoom -= JabbrManagerOnJoinedRoom;

            if(!_loggedIn)
                throw new InvalidOperationException();

            if(_hasPreviouslyJoinedRooms)
                ActivateItem(_chatWindowViewModel);
        }

        private void JabbrManagerOnLoggedIn(object sender, LoggedInEventArgs loggedInEventArgs)
        {
            _loggedIn = true;
            _hasPreviouslyJoinedRooms = loggedInEventArgs.Rooms.Any();

            if (!_hasPreviouslyJoinedRooms)
                _jabbrManager.JoinedRoom -= JabbrManagerOnJoinedRoom;
            else
                _jabbrManager.RoomsReceived -= JabbrManagerOnRoomsReceived;
        }
    }
}
