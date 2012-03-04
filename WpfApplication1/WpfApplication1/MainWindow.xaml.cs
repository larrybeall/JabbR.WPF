using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JabbR.Client;
using JabbR.Client.Models;
using System.Threading;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool _alreadyAttemptedFails = false;

        private void Connect()
        {
            SynchronizationContext context = SynchronizationContext.Current;
            JabbRClient client = new JabbRClient("http://jabbr.net");
            client.MessageReceived += ClientOnMessageReceived;
            client.Connect("e14c35c2-5b4a-49f4-be5a-f3a77b325c45").ContinueWith(task =>
                                                           {
                                                               var logonInfo = task.Result;

                                                               var userinfo = client.GetUserInfo().Result;
                                                               MessageBox.Show("Signin complete for " + userinfo.Name);

                                                               //client.JoinRoom("test");
                                                           });
        }

        private void ClientOnMessageReceived(Message message, string s)
        {
            MessageBox.Show(message.Content);
        }

        private void Works_Click(object sender, RoutedEventArgs e)
        {
            if (_alreadyAttemptedFails)
                MessageBox.Show("Already tried fails.  Must close application and try again!");

            Connect();
        }

        private void Fails_Click(object sender, RoutedEventArgs e)
        {
            _alreadyAttemptedFails = true;
            Window window = new Window();
            window.ShowDialog();
            Connect();
        }
    }
}
