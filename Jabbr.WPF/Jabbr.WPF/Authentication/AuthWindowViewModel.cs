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

namespace Jabbr.WPF.Authentication
{
    public class AuthWindowViewModel
    {
        public string Token { get; set; }

        public void OnTokenReceived(Window window, EventArgs args)
        {
            window.Close();
        }

    }
}
