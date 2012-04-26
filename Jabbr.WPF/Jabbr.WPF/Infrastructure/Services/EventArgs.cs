using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jabbr.WPF.Messages;
using Jabbr.WPF.Users;
using Jabbr.WPF.Rooms;

namespace Jabbr.WPF.Infrastructure.Services
{
    public class RoomEventArgs : EventArgs
    {
        public string Room { get; set; }

        public RoomEventArgs()
        {
        }

        public RoomEventArgs(string room)
        {
            Room = room;
        }
    }

    public class MessageProcessedEventArgs : RoomEventArgs
    {
        public ChatMessageViewModel MessageViewModel { get; set; }

        public MessageProcessedEventArgs(ChatMessageViewModel message, string room)
        {
            Room = room;
            MessageViewModel = message;
        }
    }

    public class UserJoinedEventArgs : RoomEventArgs
    {
        public UserViewModel UserViewModel { get; set; }

        public UserJoinedEventArgs(UserViewModel userViewModel, string room)
        {
            Room = room;
            UserViewModel = userViewModel;
        }
    }

    public class LoginCompleteEventArgs : EventArgs
    {
        public UserViewModel User { get; private set; }
        public bool HasJoinedRooms { get; private set; }

        public LoginCompleteEventArgs(UserViewModel user, bool hasJoinedRooms)
        {
            User = user;
            HasJoinedRooms = hasJoinedRooms;
        }
    }

    public class JoiningRoomEventArgs : EventArgs
    {
        public RoomViewModel Room { get; private set; }

        public JoiningRoomEventArgs(RoomViewModel room)
        {
            Room = room;
        }
    }

    public class RoomsRetrievedEventArgs : EventArgs
    {
        public IEnumerable<RoomViewModel> Rooms { get; private set; }
 
        public RoomsRetrievedEventArgs(IEnumerable<RoomViewModel> rooms)
        {
            Rooms = rooms;
        }
    }
}
