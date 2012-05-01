using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JabbR.Client;
using JabbR.Client.Models;
using Jabbr.WPF.Rooms;
using Jabbr.WPF.Users;

namespace Jabbr.WPF.Infrastructure.Services
{
    public class RoomService : BaseService
    {
        private readonly JabbRClient _client;

        private readonly ConcurrentDictionary<string, RoomViewModel> _roomStore =
            new ConcurrentDictionary<string, RoomViewModel>();

        private readonly ServiceLocator _serviceLocator;
        private readonly UserService _userService;

        private DateTime _lastRoomsRetrieve;

        public RoomService(JabbRClient client, ServiceLocator serviceLocator, UserService userService)
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
            foreach (Room room in rooms)
            {
                JoinRoom(room);
            }
        }

        public Task JoinRoom(Room room)
        {
            RoomViewModel basicRoomVm = GetRoom(room);
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
                    Room roomInfo = details.Result;
                    RoomViewModel roomVm = GetRoom(roomInfo.Name);
                    PostOnUi(() => roomVm.OnJoined(roomInfo));
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
            TimeSpan timeSinceLastRetrieve = DateTime.Now.Subtract(_lastRoomsRetrieve);
            if (timeSinceLastRetrieve.TotalSeconds <= 30)
            {
                OnRoomsRetrieved();
                return;
            }

            _lastRoomsRetrieve = DateTime.Now;

            _client.GetRooms().ContinueWith(roomsTask =>
            {
                foreach (Room room in roomsTask.Result)
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

        public void SetTyping(string room)
        {
            _client.SetTyping(room);
        }

        private void InvokeIfInRoom(string room, Action<RoomViewModel> toInvoke)
        {
            RoomViewModel roomVm = GetRoom(room);
            InvokeIfInRoom(roomVm, toInvoke);
        }

        private void InvokeIfInRoom(Room room, Action<RoomViewModel> toInvoke)
        {
            RoomViewModel roomVm = GetRoom(room.Name);
            InvokeIfInRoom(roomVm, toInvoke);
        }

        private void InvokeIfInRoom(RoomViewModel room, Action<RoomViewModel> toInvoke)
        {
            if (room == null || room.JoinState != JoinState.Joined)
                return;

            toInvoke(room);
        }

        #region event handlers

        private void OnJoiningRoom(RoomViewModel room)
        {
            EventHandler<JoiningRoomEventArgs> handler = JoiningRoom;
            if (handler != null)
                PostOnUi(() => handler(this, new JoiningRoomEventArgs(room)));
        }

        private void OnRoomsRetrieved()
        {
            List<RoomViewModel> rooms = _roomStore.Select(x => x.Value).ToList();

            EventHandler<RoomsRetrievedEventArgs> handler = RoomsRetrieved;
            if (handler != null)
                PostOnUi(() => handler(this, new RoomsRetrievedEventArgs(rooms)));
        }

        private void OnUserTyping(User user, string room)
        {
            InvokeIfInRoom(room, roomVm =>
            {
                UserViewModel userVm = _userService.GetUserViewModel(user);
                if (userVm == null || userVm.IsCurrentUser)
                    return;

                PostOnUi(() => roomVm.SetUserTyping(userVm));
            });
        }

        private void OnUserLeft(User user, string room)
        {
            InvokeIfInRoom(room, roomVm =>
            {
                UserViewModel userVm = _userService.GetUserViewModel(user.Name);
                if (userVm == null)
                    return;

                PostOnUi(() => roomVm.UserLeft(userVm));
            });
        }

        private void OnUserJoined(User user, string room)
        {
            InvokeIfInRoom(room, roomVm =>
            {
                UserViewModel userVm = _userService.GetUserViewModel(user);
                PostOnUi(() => roomVm.AddUser(userVm));
            });
        }

        private void OnTopicChanged(Room room)
        {
            InvokeIfInRoom(room, roomVm => PostOnUi(() => roomVm.SetTopic(room.Topic)));
        }

        private void OnRoomCountChanged(Room room, int userCount)
        {
            InvokeIfInRoom(room, roomVm => PostOnUi(() => roomVm.UserCount = userCount));
        }

        private void OnOwnerRemoved(User user, string room)
        {
            InvokeIfInRoom(room, roomVm => PostOnUi(() => roomVm.RemoveOwner(user.Name)));
        }

        private void OnOwnerAdded(User user, string room)
        {
            InvokeIfInRoom(room, roomVm => PostOnUi(() => roomVm.AddOwner(user.Name)));
        }

        private void OnKicked(string room)
        {
            InvokeIfInRoom(room, roomVm => PostOnUi(roomVm.Kicked));
        }

        #endregion
    }
}