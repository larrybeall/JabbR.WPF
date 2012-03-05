using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using JabbR.Client.Models;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

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
            _client.RoomCountChanged += ClientOnRoomCountChanged;
            _client.UserActivityChanged += ClientOnUserActivityChanged;
            _client.UserJoined += ClientOnUserJoined;
            _client.UserLeft += ClientOnUserLeft;
            _client.UserTyping += ClientOnUserTyping;
            _client.UsersInactive += ClientOnUsersInactive;
        }

        private void ClientOnUsersInactive(IEnumerable<User> users)
        {
        }

        private void ClientOnUserTyping(User user, string room)
        {
        }

        private void ClientOnUserLeft(User user, string room)
        {
        }

        private void ClientOnUserJoined(User user, string room)
        {
        }

        private void ClientOnUserActivityChanged(User user)
        {
        }

        private void ClientOnRoomCountChanged(Room room, int count)
        {
        }

        private void ClientOnPrivateMessage(string from, string to, string message)
        {
        }

        private void ClientOnLoggedOut(IEnumerable<string> rooms)
        {
        }

        private void ClientOnKicked(string room)
        {
        }

        private void ClientOnDisconnected()
        {
        }

        private void ClientOnMessageReceived(Message message, string room)
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
            CompleteSignIn(_client.Connect("e14c35c2-5b4a-49f4-be5a-f3a77b325c45"), completAction);
        }

        public void SignInStandard(string username, string password, Action completeAction)
        {
            CompleteSignIn(_client.Connect(username, password), completeAction);
        }

        public void Disconnect()
        {
            _client.Disconnect();
            Thread.Sleep(500);
        }

        private string Authenticate(string token)
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

        private void CompleteSignIn(Task<LogOnInfo> signInTask, Action  completeAction)
        {
            signInTask.ContinueWith((task) =>
                                        {
                                            var loginInfo = task.Result;
                                            System.Diagnostics.Debug.WriteLine("wtf");
                                            var userInfo = _client.GetUserInfo().Result;
                                            System.Diagnostics.Debug.WriteLine("got user info");

                                            foreach (var room in loginInfo.Rooms)
                                            {
                                                _client.JoinRoom(room.Name);
                                            }
                                            InvokeOnUi(completeAction);
                                        });
        }
    }
}
