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

            Items.Add(_loginViewModel);
            Items.Add(_roomSelectionViewModel);
            Items.Add(_chatWindowViewModel);

            ActivateItem(_loginViewModel);
        }

        private void JabbrManagerOnLoggedIn(object sender, LoggedInEventArgs loggedInEventArgs)
        {
            if(loggedInEventArgs.Rooms.Any())
                ActivateItem(_chatWindowViewModel);
            else
                ActivateItem(_roomSelectionViewModel);
        }
    }
}
