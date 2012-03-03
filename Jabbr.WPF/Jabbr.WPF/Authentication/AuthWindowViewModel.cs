using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JabbR.Client;
using Phoenix.Windows.Controls;
using System.Windows;
using JabbR.Client.Models;

namespace Jabbr.WPF.Authentication
{
    public class AuthWindowViewModel
    {
        public void OnTokenReceived(Window window, TokenReceivedEventArgs args)
        {
            LogOnInfo info;

            var client = JabbrManager.Instance.Client;
            string value = client.Authenticate(args.Token);
            MessageBox.Show(value);
            client.Connect(value).ContinueWith(task =>
                                               {
                                                   info = task.Result;
                                                   System.Diagnostics.Debug.WriteLine("I'm here");
                                               });

        }
    }
}
