using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JabbrModels = JabbR.Client.Models;

namespace Jabbr.WPF.Infrastructure.Models
{
    public class Room
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public bool Private { get; set; }
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<string> Owners { get; set; }
        public IEnumerable<Message> RecentMessages { get; set; }

        public Room()
        {
        }

        public Room(JabbrModels.Room room)
        {
            Name = room.Name;
            Count = room.Count;
            Private = room.Private;
            Users = room.Users.Select(user => new User(user));
            Owners = room.Owners;
            RecentMessages = room.RecentMessages.Select(message => new Message(message));
        }
    }
}
