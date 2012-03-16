using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jabbr.WPF.Infrastructure.Models;
using JabbrModels = JabbR.Client.Models;

namespace Jabbr.WPF.Infrastructure
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

    public class MessageReceivedEventArgs : RoomEventArgs
    {
        public Message Message { get; set; }

        public MessageReceivedEventArgs(JabbrModels.Message message, string room)
        {
            Room = room;
            Message = new Message(message);
        }
    }

    public class UserRoomSpecificEventArgs : RoomEventArgs
    {
        public User User { get; set; }

        public UserRoomSpecificEventArgs(JabbrModels.User user, string room)
        {
            Room = room;
            User = new User(user);
        }
    }

    public class UserEventArgs : EventArgs
    {
        public User User { get; set; }

        public UserEventArgs(JabbrModels.User user)
        {
            User = new User(user);
        }
    }

    public class LoggedInEventArgs : UserEventArgs
    {
        public IEnumerable<Room> Rooms { get; set; }

        public LoggedInEventArgs(JabbrModels.User user, IEnumerable<JabbrModels.Room> rooms)
            :base(user)
        {
            Rooms = rooms.Select(x => new Room(x));
        }
    }

    public class RoomCountEventArgs : EventArgs
    {
        public Room Room { get; set; }
        public int Count { get; set; }

        public RoomCountEventArgs(JabbrModels.Room room, int count)
        {
            Room = new Room(room);
            Count = count;
        }
    }

    public class PrivateMessageEventArgs : EventArgs
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }

        public PrivateMessageEventArgs(string from, string to, string message)
        {
            From = from;
            To = to;
            Message = message;
        }
    }

    public class UsersInactiveEventArgs : EventArgs
    {
        public IEnumerable<User> Users { get; set; }

        public UsersInactiveEventArgs(IEnumerable<JabbrModels.User> users)
        {
            Users = users.Select(user => new User(user));
        }
    }

    public class LoggedOutEventArgs : EventArgs
    {
        public IEnumerable<string> Rooms { get; set; }

        public LoggedOutEventArgs(IEnumerable<string> rooms)
        {
            Rooms = rooms;
        }
    }

    public class RoomDetailsEventArgs : EventArgs
    {
        public Room Room { get; set; }

        public RoomDetailsEventArgs(JabbrModels.Room room)
        {
            Room = new Room(room);
        }
    }

    public class RoomsReceivedEventArgs : EventArgs
    {
        public IEnumerable<Room> Rooms { get; set; }

        public RoomsReceivedEventArgs(IEnumerable<JabbrModels.Room> rooms)
        {
            Rooms = rooms.Select(room => new Room(room));
        }
    }
}
