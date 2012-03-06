using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JabbR.Client;
using JabbR.Client.Models;

namespace ConsoleTester
{
    class Program
    {
        static void Main(string[] args)
        {
            JabbRClient client = new JabbRClient("http://jabbr.net");
            client.MessageReceived += ClientOnMessageReceived;
            client.Connect("phxtest", "testtest").ContinueWith(task =>
            {
                var logonInfo = task.Result;

                var userinfo = client.GetUserInfo().Result;
                //MessageBox.Show("Signin complete for " + userinfo.Name);

                client.JoinRoom("JabbRWPF");
                System.Diagnostics.Debug.WriteLine("signin complete");
            });

            Console.ReadLine();
        }

        private static void ClientOnMessageReceived(Message arg1, string arg2)
        {
            Console.WriteLine(arg1.Content);
        }
    }
}
