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
        private readonly ConcurrentDictionary<string, RoomViewModel> _joinedRooms =
            new ConcurrentDictionary<string, RoomViewModel>();

        public RoomService(JabbRClient client, ServiceLocator serviceLocator)
            : base()
        {
            _client = client;
            _serviceLocator = serviceLocator;
        }

        public event EventHandler<JoinedRoomEventArgs> JoinedRoom; 

        public void JoinRooms(IEnumerable<Room> rooms)
        {
            foreach (var room in rooms)
            {
                JoinRoom(room.Name);
            }
        }

        public Task JoinRoom(string roomName)
        {
            return _client.JoinRoom(roomName).ContinueWith(task =>
            {
                _client.GetRoomInfo(roomName).ContinueWith(details =>
                {
                    var roomInfo = details.Result;
                    var roomVm = GetRoom(roomInfo);
                    OnJoinedRoom(roomVm);
                });
            });
        }

        //public Task GetRooms()
        //{
            
        //}

        public RoomViewModel GetRoom(string roomName)
        {
            RoomViewModel room;
            _joinedRooms.TryGetValue(roomName, out room);

            return room;
        }

        public RoomViewModel GetRoom(Room room)
        {
            RoomViewModel toReturn = GetRoom(room.Name);

            if (toReturn != null)
                return toReturn;

            toReturn = _serviceLocator.GetViewModel<RoomViewModel>();
            toReturn.IsNotifying = false;

            toReturn.Initialize(room);

            toReturn.IsNotifying = true;

            _joinedRooms.TryAdd(room.Name, toReturn);

            return toReturn;
        }

        private void OnJoinedRoom(RoomViewModel room)
        {
            var handler = JoinedRoom;
            if(handler != null)
                PostOnUi(() => handler(this, new JoinedRoomEventArgs(room)));
        }
    }
}
