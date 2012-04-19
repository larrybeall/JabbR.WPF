using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Jabbr.WPF.Infrastructure;
using Jabbr.WPF.Infrastructure.Services;

namespace Jabbr.WPF.Rooms
{
    public class ChatWindowViewModel : Conductor<RoomViewModel>.Collection.OneActive
    {
        private readonly RoomService _roomService;
        private readonly ServiceLocator _serviceLocator;

        public ChatWindowViewModel(RoomService roomService, ServiceLocator serviceLocator)
        {
            _roomService = roomService;
            _serviceLocator = serviceLocator;

            Initialize();
        }

        private void Initialize()
        {
            DisplayName = "Chat Window";

            _roomService.JoinedRoom += RoomServiceOnJoinedRoom;
        }

        private void RoomServiceOnJoinedRoom(object sender, JoinedRoomEventArgs joinedRoomEventArgs)
        {
            ActivateItem(joinedRoomEventArgs.Room);
        }
    }
}
