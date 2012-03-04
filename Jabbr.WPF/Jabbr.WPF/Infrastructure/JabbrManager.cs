using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using JabbR.Client.Models;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Jabbr.WPF.Infrastructure
{
    public class JabbrManager
    {
        private static volatile JabbrManager _instance;
        private readonly static object SyncRoot = new object();

        private readonly JabbR.Client.JabbRClient _client;
        private readonly string _url;
        private readonly SynchronizationContext _uiContext;

        private JabbrManager()
        {
            _uiContext = SynchronizationContext.Current;
            _url = "http://jabbr.net";
            _client = new JabbR.Client.JabbRClient(_url);
            _client.MessageReceived += ClientOnMessageReceived;
            _client.Disconnected += ClientOnDisconnected;
            _client.Kicked += ClientOnKicked;
            _client.LoggedOut += ClientOnLoggedOut;
            _client.PrivateMessage += ClientOnPrivateMessage;
            //_client.RoomCountChanged += ClientOnRoomCountChanged;
            _client.UserActivityChanged += ClientOnUserActivityChanged;
            _client.UserJoined += ClientOnUserJoined;
            _client.UserLeft += ClientOnUserLeft;
            _client.UserTyping += ClientOnUserTyping;
            _client.UsersInactive += ClientOnUsersInactive;
        }

        private void ClientOnUsersInactive(IEnumerable<User> enumerable)
        {
        }

        private void ClientOnUserTyping(User user, string s)
        {
        }

        private void ClientOnUserLeft(User user, string s)
        {
        }

        private void ClientOnUserJoined(User user, string s)
        {
        }

        private void ClientOnUserActivityChanged(User user)
        {
        }

        private void ClientOnRoomCountChanged(Room r, int i)
        {
        }

        private void ClientOnPrivateMessage(string s, string s1, string arg3)
        {
        }

        private void ClientOnLoggedOut(IEnumerable<string> enumerable)
        {
        }

        private void ClientOnKicked(string s)
        {
        }

        private void ClientOnDisconnected()
        {
        }

        private void ClientOnMessageReceived(Message message, string s)
        {
        }

        public static JabbrManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new JabbrManager();
                    }
                }

                return _instance;
            }
        }

        public string Username { get; private set; }

        public JabbR.Client.JabbRClient Client
        {
            get { return _client; }
        }

        public void SignIn(string token, Action completAction)
        {
            System.Diagnostics.Debug.WriteLine("start sign in.");
            //string userId = Authenticate(token);
            System.Diagnostics.Debug.WriteLine("got user id");
            _client.Connect("e14c35c2-5b4a-49f4-be5a-f3a77b325c45").ContinueWith(task =>
                                                     {
                                                         var loginInfo = task.Result;
                                                         System.Diagnostics.Debug.WriteLine("wtf");
                                                         var userInfo = _client.GetUserInfo().Result;
                                                         System.Diagnostics.Debug.WriteLine("got user info");

                                                         foreach (var room in loginInfo.Rooms)
                                                         {
                                                             _client.JoinRoom(room.Name);
                                                         }
                                                         InvokeOnUi(completAction);
                                                     });
        }

        public string Authenticate(string token)
        {
            CookieContainer cookieContainer = new CookieContainer();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url + "/Auth/Login.ashx");
            request.CookieContainer = cookieContainer;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            string data = "token=" + token;
            byte[] postBytes = Encoding.Default.GetBytes(data);

            request.ContentLength = postBytes.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(postBytes, 0, postBytes.Length);
            dataStream.Close();
            var response = request.GetResponse();
            response.Close();

            CookieCollection cookies = cookieContainer.GetCookies(new Uri(_url));
            string cookieValue = cookies[0].Value;

            JObject jsonObject = JObject.Parse(cookieValue);

            return (string)jsonObject["userId"];
        }

        private void InvokeOnUi(Action toInvoke)
        {
            if (_uiContext == null)
            {
                toInvoke();
                return;
            }

            _uiContext.Post(_ => toInvoke(), null);
        }
    }
}
