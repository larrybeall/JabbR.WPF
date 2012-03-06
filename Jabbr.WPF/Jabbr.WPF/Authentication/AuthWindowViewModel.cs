using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JabbR.Client;
using Phoenix.Windows.Controls;
using System.Windows;
using JabbR.Client.Models;
using Jabbr.WPF.Infrastructure;
using Caliburn.Micro;

namespace Jabbr.WPF.Authentication
{
    public class AuthWindowViewModel : PropertyChangedBase
    {
        private readonly JabbrManager _jabbrManager;
        private string _username;
        private string _password;

        public AuthWindowViewModel(JabbrManager jabbrManager)
        {
            _jabbrManager = jabbrManager;
        }

        public string Username
        {
            get { return _username; }
            set
            {
                if(_username == value)
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

        public void OnTokenReceived(Window window, EventArgs args)
        {
            _jabbrManager.SignIn(string.Empty, window.Close);
        }

        public void SignInStandard(Window window)
        {
            _jabbrManager.SignInStandard(Username, Password, window.Close);
        }
    }
}
