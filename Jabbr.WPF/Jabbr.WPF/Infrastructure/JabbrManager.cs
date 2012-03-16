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

        public JabbrManager()
        {
            _uiContext = SynchronizationContext.Current;
            _url = "http://jabbr.net";
            _client = new JabbRClient(_url);
            SubscribeToEvents();
        }

        public event EventHandler<UsersInactiveEventArgs> UsersInactive;
        public event EventHandler<UserRoomSpecificEventArgs> UserTyping;
        public event EventHandler<UserRoomSpecificEventArgs> UserLeftRoom;
        public event EventHandler<UserRoomSpecificEventArgs> UserJoinedRoom;
        public event EventHandler<UserEventArgs> UserActivityChanged;
        public event EventHandler<RoomCountEventArgs> RoomCountChanged;
        public event EventHandler<PrivateMessageEventArgs> PrivateMessageReceived;
        public event EventHandler<LoggedOutEventArgs> LoggedOut;
        public event EventHandler<RoomEventArgs> Kicked;
        public event EventHandler<EventArgs> Disconnected;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<RoomDetailsEventArgs> JoinedRoom;
        public event EventHandler<RoomsReceivedEventArgs> RoomsReceived;
        public event EventHandler<LoggedInEventArgs> LoggedIn;
        public event EventHandler<RoomEventArgs> LeftRoom;


        private void OnLeftRoom(string room)
        {
            var handler = LeftRoom;
            if(handler != null)
                InvokeOnUi(() => handler(this, new RoomEventArgs(room)));
        }

        private void OnLoggedIn(User user, IEnumerable<Room> rooms)
        {
            IsLoggedIn = true;
            Username = user.Name;

            var handler = LoggedIn;
            if(handler != null)
                InvokeOnUi(() => handler(this, new LoggedInEventArgs(user, rooms)));
        }

        private void OnJoinedRoom(Room roomDetails)
        {
            var handler = JoinedRoom;
            if(handler != null)
                InvokeOnUi(() => handler(this, new RoomDetailsEventArgs(roomDetails)));
        }

        private void OnRoomsReceived(IEnumerable<Room> rooms)
        {
            var handler = RoomsReceived;
            if (handler != null)
                InvokeOnUi(() => handler(this, new RoomsReceivedEventArgs(rooms)));
        }

        private void ClientOnUsersInactive(IEnumerable<User> users)
        {
            var handler = UsersInactive;
            if (handler != null)
                InvokeOnUi(() => handler(this, new UsersInactiveEventArgs(users)));
        }

        private void ClientOnUserTyping(User user, string room)
        {
            var handler = UserTyping;
            if (handler != null)
                InvokeOnUi(() => handler(this, new UserRoomSpecificEventArgs(user, room)));
        }

        private void ClientOnUserLeft(User user, string room)
        {
            var handler = UserLeftRoom;
            if(handler != null)
                InvokeOnUi(() => handler(this, new UserRoomSpecificEventArgs(user, room)));
        }

        private void ClientOnUserJoined(User user, string room)
        {
            var handler = UserJoinedRoom;
            if(handler != null)
                InvokeOnUi(() => handler(this, new UserRoomSpecificEventArgs(user, room)));
        }

        private void ClientOnUserActivityChanged(User user)
        {
            var handler = UserActivityChanged;
            if(handler != null)
                InvokeOnUi(() => handler(this, new UserEventArgs(user)));
        }

        private void ClientOnRoomCountChanged(Room room, int count)
        {
            var handler = RoomCountChanged;
            if(handler != null)
                InvokeOnUi(() => handler(this, new RoomCountEventArgs(room, count)));
        }

        private void ClientOnPrivateMessage(string from, string to, string message)
        {
            var handler = PrivateMessageReceived;
            if(handler != null)
                InvokeOnUi(() => handler(this, new PrivateMessageEventArgs(from, to, message)));
        }

        private void ClientOnLoggedOut(IEnumerable<string> rooms)
        {
            IsLoggedIn = false;
            Username = null;

            var handler = LoggedOut;
            if(handler != null)
                InvokeOnUi(() => handler(this, new LoggedOutEventArgs(rooms)));
        }

        private void ClientOnKicked(string room)
        {
            var handler = Kicked;
            if(handler != null)
                InvokeOnUi(() => handler(this, new RoomEventArgs(room)));
        }

        private void ClientOnDisconnected()
        {
            var handler = Disconnected;
            if (handler != null)
                InvokeOnUi(() => handler(this, EventArgs.Empty));
        }

        private void ClientOnMessageReceived(Message message, string room)
        {
            var handler = MessageReceived;
            if (handler != null)
                InvokeOnUi(() => handler(this, new MessageReceivedEventArgs(message, room)));
        }
        
        public string Username { get; private set; }
        public bool IsLoggedIn { get; private set; }

        public JabbR.Client.JabbRClient Client
        {
            get { return _client; }
        }

        public void SignIn(string token, Action completAction)
        {
            System.Diagnostics.Debug.WriteLine("start sign in.");
            string userId = Authenticate(token);
            System.Diagnostics.Debug.WriteLine("got user id");
            CompleteSignIn(_client.Connect("e14c35c2-5b4a-49f4-be5a-f3a77b325c45"), completAction);
        }

        public void SignInStandard(string username, string password, Action completeAction)
        {
            CompleteSignIn(_client.Connect(username, password), completeAction);
        }

        public bool SendCommand(string command, string room)
        {
            if (command.StartsWith("/join"))
            {
                string toJoin = command.Replace("/join", string.Empty).Trim();
                JoinRoom(toJoin);
                return true;
            }

            if (command.StartsWith("/leave"))
            {
                string toLeave = command.Replace("/leave", string.Empty).Trim();
                LeaveRoom(toLeave);
                return true;
            }

            return _client.Send(command, room).Result;
        }

        public void JoinRoom(string room)
        {
            _client.JoinRoom(room).ContinueWith(_ =>
            {
                _client.GetRoomInfo(room).ContinueWith(details =>
                {
                    var roomInfo = details.Result;
                    OnJoinedRoom(roomInfo);
                });
            });
        }

        public void LeaveRoom(string room)
        {
            _client.LeaveRoom(room).ContinueWith(t => OnLeftRoom(room));
        }

        public void Disconnect()
        {
            _client.Disconnect();
            Thread.Sleep(500);
        }

        private void SubscribeToEvents()
        {
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

        private void UnsubcribeFromEvents()
        {
            _client.MessageReceived -= ClientOnMessageReceived;
            _client.Disconnected -= ClientOnDisconnected;
            _client.Kicked -= ClientOnKicked;
            _client.LoggedOut -= ClientOnLoggedOut;
            _client.PrivateMessage -= ClientOnPrivateMessage;
            _client.RoomCountChanged -= ClientOnRoomCountChanged;
            _client.UserActivityChanged -= ClientOnUserActivityChanged;
            _client.UserJoined -= ClientOnUserJoined;
            _client.UserLeft -= ClientOnUserLeft;
            _client.UserTyping -= ClientOnUserTyping;
            _client.UsersInactive -= ClientOnUsersInactive;
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
                    var userInfo = _client.GetUserInfo().Result;

                    _client.GetRooms().ContinueWith((rooms) =>
                        {
                            var availableRooms = rooms.Result;
                            OnRoomsReceived(availableRooms);
                        });

                    foreach (var room in loginInfo.Rooms)
                    {
                        string roomName = room.Name;
                        JoinRoom(roomName);
                    }
                    OnLoggedIn(userInfo, loginInfo.Rooms);
                    InvokeOnUi(completeAction);
                });
        }
    }
}
