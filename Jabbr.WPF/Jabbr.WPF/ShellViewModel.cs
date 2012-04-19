using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Jabbr.WPF.Authentication;
using Jabbr.WPF.Infrastructure;
using Jabbr.WPF.Rooms;
using System.Windows.Input;
using Jabbr.WPF.Infrastructure.Services;

namespace Jabbr.WPF
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private readonly AuthenticationService _authenticationService;
        private readonly LoginViewModel _loginViewModel;
        private readonly RoomSelectionViewModel _roomSelectionViewModel;
        private readonly ChatWindowViewModel _chatWindowViewModel;

        public ShellViewModel(
            AuthenticationService authenticationService,
            LoginViewModel loginViewModel,
            RoomSelectionViewModel roomSelectionViewModel,
            ChatWindowViewModel chatWindowViewModel)
        {
            _authenticationService = authenticationService;
            _loginViewModel = loginViewModel;
            _roomSelectionViewModel = roomSelectionViewModel;
            _chatWindowViewModel = chatWindowViewModel;

            Initialize();
        }

        private void Initialize()
        {
            _authenticationService.SignInComplete += AuthenticationServiceOnSignInComplete;

            Items.Add(_loginViewModel);
            Items.Add(_roomSelectionViewModel);
            Items.Add(_chatWindowViewModel);

            ActivateItem(_loginViewModel);
        }

        private void AuthenticationServiceOnSignInComplete(object sender, LoginCompleteEventArgs loginCompleteEventArgs)
        {
            if(loginCompleteEventArgs.HasJoinedRooms)
                ActivateItem(_chatWindowViewModel);
            else
                ActivateItem(_roomSelectionViewModel);
        }

        public void MouseDown(MouseButtonEventArgs args)
        {
            args.Handled = true;
        }
    }
}
