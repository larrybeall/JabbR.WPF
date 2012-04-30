using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using Jabbr.WPF.Infrastructure.Services;
using Phoenix.Windows.Engage;

namespace Jabbr.WPF.Authentication
{
    public class LoginViewModel : Screen
    {
        private readonly AuthenticationService _authenticationService;

        private string _avatar;
        private bool _canLogin;
        private bool _isAuthenticating;
        private string _password;
        private bool _showOpenIdPopup;
        private string _username;

        public LoginViewModel(AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public string Username
        {
            get { return _username; }
            set
            {
                if (_username == value)
                    return;

                _username = value;
                NotifyOfPropertyChange(() => Username);
                Validate();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (_password == value)
                    return;

                _password = value;
                NotifyOfPropertyChange(() => Password);
                Validate();
            }
        }

        public bool CanLogin
        {
            get { return _canLogin; }
            set
            {
                if (_canLogin == value)
                    return;

                _canLogin = value;
                NotifyOfPropertyChange(() => CanLogin);
            }
        }

        public string Avatar
        {
            get { return _avatar; }
            set
            {
                if (_avatar == value)
                    return;

                _avatar = value;
                NotifyOfPropertyChange(() => Avatar);
            }
        }

        public bool ShowOpenIdPopup
        {
            get { return _showOpenIdPopup; }
            set
            {
                if (_showOpenIdPopup == value)
                    return;

                _showOpenIdPopup = value;
                NotifyOfPropertyChange(() => ShowOpenIdPopup);
            }
        }

        public bool IsAuthenticating
        {
            get { return _isAuthenticating; }
            set
            {
                if (_isAuthenticating == value)
                    return;

                _isAuthenticating = value;
                NotifyOfPropertyChange(() => IsAuthenticating);
            }
        }

        protected override void OnActivate()
        {
            // TODO: Avatars should not be hardcoded.
            Avatar = "http://www.gravatar.com/avatar/00000000000000000000000000000000?d=mm&s=200";

            if (string.IsNullOrEmpty(DisplayName))
                DisplayName = "Login";

            base.OnActivate();
        }

        public void ShowOpenId()
        {
            ShowOpenIdPopup = true;
        }

        public void CloseOpenId(ICommand switchAccountsCommand)
        {
            if (switchAccountsCommand.CanExecute(null))
                switchAccountsCommand.Execute(null);

            ShowOpenIdPopup = false;
        }

        public void OnTokenReceived(TokenReceivedRoutedEventArgs tokenReceivedEventArgs)
        {
            IsAuthenticating = true;
            ShowOpenIdPopup = false;
            _authenticationService.SignIn(tokenReceivedEventArgs.Token);
        }

        public void Login()
        {
            IsAuthenticating = true;
            _authenticationService.SignIn(Username, Password)
                .ContinueWith((completedTask) =>
                {
                    Password = Username = null;
                    ShowOpenIdPopup = false;
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private void Validate()
        {
            CanLogin = !string.IsNullOrEmpty(Username) & !string.IsNullOrEmpty(Password);
        }
    }
}