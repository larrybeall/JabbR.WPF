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
        private readonly ServiceLocator _serviceLocator;
        private readonly ConcurrentDictionary<string, RoomViewModel> _roomStore =
            new ConcurrentDictionary<string, RoomViewModel>();

        private DateTime _lastRoomsRetrieve;

        public RoomService(JabbRClient client, ServiceLocator serviceLocator)
            : base()
        {
            _client = client;
            _serviceLocator = serviceLocator;
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

        private void OnJoiningRoom(RoomViewModel room)
        {
            var handler = JoiningRoom;
            if(handler != null)
                PostOnUi(() => handler(this, new JoiningRoomEventArgs(room)));
        }

        private void OnRoomsRetrieved()
        {
            var rooms = _roomStore.Select(x => x.Value).ToList();

            var handler = RoomsRetrieved;
            if(handler != null)
                PostOnUi(() => handler(this, new RoomsRetrievedEventArgs(rooms)));
        }
    }
}
