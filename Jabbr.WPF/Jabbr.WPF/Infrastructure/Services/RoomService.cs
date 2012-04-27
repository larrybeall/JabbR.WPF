using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JabbR.Client;
using System.Threading.Tasks;
using Jabbr.WPF.Rooms;
using System.Collections.Concurrent;
using JabbR.Client.Models;

namespace Jabbr.WPF.Infrastructure.Services
{
    public class RoomService : BaseService
    {
        private readonly JabbRClient _client;
        private readonly UserService _userService;
        private readonly ServiceLocator _serviceLocator;
        private readonly ConcurrentDictionary<string, RoomViewModel> _roomStore =
            new ConcurrentDictionary<string, RoomViewModel>();

        private DateTime _lastRoomsRetrieve;

        public RoomService(JabbRClient client, ServiceLocator serviceLocator, UserService userService)
            : base()
        {
            _client = client;
            _serviceLocator = serviceLocator;
            _userService = userService;

            _client.Kicked += OnKicked;
            _client.OwnerAdded += OnOwnerAdded;
            _client.OwnerRemoved += OnOwnerRemoved;
            _client.RoomCountChanged += OnRoomCountChanged;
            _client.TopicChanged += OnTopicChanged;
            _client.UserJoined += OnUserJoined;
            _client.UserLeft += OnUserLeft;
            _client.UserTyping += OnUserTyping;
        }

        

        public event EventHandler<JoiningRoomEventArgs> JoiningRoom;
        public event EventHandler<RoomsRetrievedEventArgs> RoomsRetrieved;

        public void JoinRooms(IEnumerable<Room> rooms)
        {
            foreach (var room in rooms)
            {
                JoinRoom(room);
            }
        }

        public Task JoinRoom(Room room)
        {
            var basicRoomVm = GetRoom(room);
            return JoinRoom(basicRoomVm);
        }

        public Task JoinRoom(RoomViewModel roomViewModel)
        {
            roomViewModel.JoinState = JoinState.Joining;
            OnJoiningRoom(roomViewModel);

            return _client.JoinRoom(roomViewModel.RoomName).ContinueWith(task =>
            {
                _client.GetRoomInfo(roomViewModel.RoomName).ContinueWith(details =>
                {
                    var roomInfo = details.Result;
                    var roomVm = GetRoom(roomInfo.Name);
                    roomVm.OnJoined(roomInfo);
                });
            });
        }

        public void LeaveRoom(RoomViewModel roomViewModel)
        {
            _client.LeaveRoom(roomViewModel.RoomName).ContinueWith(
                task => PostOnUi(() => roomViewModel.JoinState = JoinState.NotJoined),
                TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public void GetRooms()
        {
            var timeSinceLastRetrieve = DateTime.Now.Subtract(_lastRoomsRetrieve);
            if (timeSinceLastRetrieve.TotalSeconds <= 30)
            {
                OnRoomsRetrieved();
                return;
            }

            _lastRoomsRetrieve = DateTime.Now;

            _client.GetRooms().ContinueWith(roomsTask =>
            {
                foreach (var room in roomsTask.Result)
                {
                    GetRoom(room);
                }

                OnRoomsRetrieved();
            });
        }

        public RoomViewModel GetRoom(string roomName)
        {
            RoomViewModel room;
            _roomStore.TryGetValue(roomName, out room);

            return room;
        }

        public RoomViewModel GetRoom(Room room, bool isJoining = false)
        {
            RoomViewModel toReturn = GetRoom(room.Name);

            if (toReturn != null)
                return toReturn;

            toReturn = _serviceLocator.GetViewModel<RoomViewModel>();
            toReturn.IsNotifying = false;

            toReturn.Populate(room);

            toReturn.IsNotifying = true;

            _roomStore.TryAdd(room.Name, toReturn);

            return toReturn;
        }

        private void InvokeIfInRoom(string room, Action<RoomViewModel> toInvoke)
        {
            var roomVm = GetRoom(room);
            InvokeIfInRoom(roomVm, toInvoke);
        }

        private void InvokeIfInRoom(Room room, Action<RoomViewModel> toInvoke)
        {
            var roomVm = GetRoom(room.Name);
            InvokeIfInRoom(roomVm, toInvoke);
        }

        private void InvokeIfInRoom(RoomViewModel room, Action<RoomViewModel> toInvoke)
        {
            if(room == null || room.JoinState != JoinState.Joined)
                return;

            toInvoke(room);
        }

        #region event handlers

        private void OnJoiningRoom(RoomViewModel room)
        {
            var handler = JoiningRoom;
            if (handler != null)
                PostOnUi(() => handler(this, new JoiningRoomEventArgs(room)));
        }

        private void OnRoomsRetrieved()
        {
            var rooms = _roomStore.Select(x => x.Value).ToList();

            var handler = RoomsRetrieved;
            if (handler != null)
                PostOnUi(() => handler(this, new RoomsRetrievedEventArgs(rooms)));
        }

        private void OnUserTyping(User user, string room)
        {
            InvokeIfInRoom(room, (roomVm) =>
                {
                    var userVm = _userService.GetUserViewModel(user);
                    if (userVm == null)
                        return;

                    PostOnUi(() => roomVm.SetUserTyping(userVm));
                });
        }

        private void OnUserLeft(User user, string room)
        {
            InvokeIfInRoom(room, (roomVm) =>
                {
                    var userVm = _userService.GetUserViewModel(user.Name);
                    if (userVm == null)
                        return;

                    PostOnUi(() => roomVm.UserLeft(userVm));
                });
        }

        private void OnUserJoined(User user, string room)
        {
            InvokeIfInRoom(room, (roomVm) =>
                {
                    var userVm = _userService.GetUserViewModel(user);
                    PostOnUi(() => roomVm.AddUser(userVm));
                });
        }

        private void OnTopicChanged(Room room)
        {
            InvokeIfInRoom(room, (roomVm) => PostOnUi(() => roomVm.SetTopic(room.Topic)));
        }

        private void OnRoomCountChanged(Room room, int userCount)
        {
            InvokeIfInRoom(room, (roomVm) => PostOnUi(() => roomVm.UserCount = userCount));
        }

        private void OnOwnerRemoved(User user, string room)
        {
            InvokeIfInRoom(room, (roomVm) => PostOnUi(() => roomVm.RemoveOwner(user.Name)));
        }

        private void OnOwnerAdded(User user, string room)
        {
            InvokeIfInRoom(room, (roomVm) => PostOnUi(() => roomVm.RemoveOwner(user.Name)));
        }

        private void OnKicked(string s)
        {
            // TODO need to verify client implementation, this does not seem to provide enough informaton
        } 

        #endregion
    }
}
