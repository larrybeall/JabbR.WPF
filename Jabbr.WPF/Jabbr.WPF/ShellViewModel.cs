using System.Windows.Input;
using Caliburn.Micro;
using Jabbr.WPF.Authentication;
using Jabbr.WPF.Infrastructure.Services;
using Jabbr.WPF.Rooms;

namespace Jabbr.WPF
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private readonly AuthenticationService _authenticationService;
        private readonly ChatWindowViewModel _chatWindowViewModel;
        private readonly LoginViewModel _loginViewModel;

        public ShellViewModel(
            AuthenticationService authenticationService,
            LoginViewModel loginViewModel,
            ChatWindowViewModel chatWindowViewModel)
        {
            _authenticationService = authenticationService;
            _loginViewModel = loginViewModel;
            _chatWindowViewModel = chatWindowViewModel;

            Initialize();
        }

        private void Initialize()
        {
            _authenticationService.SignInComplete += AuthenticationServiceOnSignInComplete;

            Items.Add(_loginViewModel);
            Items.Add(_chatWindowViewModel);

            ActivateItem(_loginViewModel);
        }

        private void AuthenticationServiceOnSignInComplete(object sender, LoginCompleteEventArgs loginCompleteEventArgs)
        {
            if (loginCompleteEventArgs.HasJoinedRooms)
                ActivateItem(_chatWindowViewModel);
            // TODO: implement code to handle situations where a user does not have joined rooms
        }

        public void MouseDown(MouseButtonEventArgs args)
        {
            args.Handled = true;
        }
    }
}