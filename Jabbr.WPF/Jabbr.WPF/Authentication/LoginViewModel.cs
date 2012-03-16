using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace Jabbr.WPF.Authentication
{
    public class LoginViewModel : Screen
    {
        private string _username;
        private string _password;
        private string _avatar;
        private bool _showOpenIdPopup;

        public string Username
        {
            get { return _username; }
            set
            {
                if (_username == value)
                    return;

                _username = value;
                NotifyOfPropertyChange(() => Username);
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
            }
        }

        public string Avatar
        {
            get { return _avatar; }
            set
            {
                if(_avatar == value)
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
                if(_showOpenIdPopup == value)
                    return;

                _showOpenIdPopup = value;
                NotifyOfPropertyChange(() => ShowOpenIdPopup);
            }
        }

        protected override void OnActivate()
        {
            Avatar = "http://www.gravatar.com/avatar/00000000000000000000000000000000?d=mm&s=200";

            if (string.IsNullOrEmpty(DisplayName))
                DisplayName = "Login";

            base.OnActivate();
        }

        public void ShowOpenId()
        {
            ShowOpenIdPopup = true;
        }

        public void CloseOpenId()
        {
            ShowOpenIdPopup = false;
        }
    }
}
