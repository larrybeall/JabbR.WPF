using System;
using System.Collections.Generic;
using Jabbr.WPF.Messages;
using Jabbr.WPF.Rooms;
using Jabbr.WPF.Users;

namespace Jabbr.WPF.Infrastructure.Services
{
    public class RoomEventArgs : EventArgs
    {
        public RoomEventArgs()
        {
        }

        public RoomEventArgs(string room)
        {
            Room = room;
        }

        public string Room { get; set; }
    }

    public class MessageProcessedEventArgs : RoomEventArgs
    {
        public MessageProcessedEventArgs(ChatMessageViewModel message, string room)
        {
            Room = room;
            MessageViewModel = message;
        }

        public ChatMessageViewModel MessageViewModel { get; set; }
    }

    public class UserJoinedEventArgs : RoomEventArgs
    {
        public UserJoinedEventArgs(UserViewModel userViewModel, string room)
        {
            Room = room;
            UserViewModel = userViewModel;
        }

        public UserViewModel UserViewModel { get; set; }
    }

    public class LoginCompleteEventArgs : EventArgs
    {
        public LoginCompleteEventArgs(UserViewModel user, bool hasJoinedRooms)
        {
            User = user;
            HasJoinedRooms = hasJoinedRooms;
        }

        public UserViewModel User { get; private set; }
        public bool HasJoinedRooms { get; private set; }
    }

    public class JoiningRoomEventArgs : EventArgs
    {
        public JoiningRoomEventArgs(RoomViewModel room)
        {
            Room = room;
        }

        public RoomViewModel Room { get; private set; }
    }

    public class RoomsRetrievedEventArgs : EventArgs
    {
        public RoomsRetrievedEventArgs(IEnumerable<RoomViewModel> rooms)
        {
            Rooms = rooms;
        }

        public IEnumerable<RoomViewModel> Rooms { get; private set; }
    }
}