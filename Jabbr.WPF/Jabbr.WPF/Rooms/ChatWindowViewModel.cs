using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Jabbr.WPF.Infrastructure;
using Jabbr.WPF.Infrastructure.Services;
using JabbR.Client;

namespace Jabbr.WPF.Rooms
{
    public class ChatWindowViewModel : Conductor<RoomViewModel>.Collection.OneActive
    {
        private readonly RoomService _roomService;
        private readonly ServiceLocator _serviceLocator;
        private readonly JabbRClient _client;
        private string _sendText;

        public ChatWindowViewModel(RoomService roomService, ServiceLocator serviceLocator, JabbRClient client)
        {
            _roomService = roomService;
            _serviceLocator = serviceLocator;
            _client = client;

            Initialize();
        }

        public string SendText
        {
            get { return _sendText; }
            set
            {
                if(_sendText == value)
                    return;

                _sendText = value;
                NotifyOfPropertyChange(() => SendText);
            }
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

        public void Send()
        {
            if(string.IsNullOrEmpty(SendText))
                return;

            _client.Send(SendText, ActiveItem.DisplayName);
            SendText = null;
        }
    }
}
