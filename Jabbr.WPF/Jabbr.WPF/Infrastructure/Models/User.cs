using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JabbrModels = JabbR.Client.Models;

namespace Jabbr.WPF.Infrastructure.Models
{
    public class User
    {
        public string Name { get; set; }
        public string Hash { get; set; }
        public bool Active { get; set; }
        public UserStatus Status { get; set; }
        public string Note { get; set; }
        public string AfkNote { get; set; }
        public bool IsAfk { get; set; }
        public string Flag { get; set; }
        public string Country { get; set; }
        public DateTime LastActivity { get; set; }

        public User()
        {
            
        }

        public User(JabbrModels.User user)
        {
            Name = user.Name;
            Hash = user.Hash;
            Active = user.Active;
            Status = user.Status.Translate();
            Note = user.Note;
            AfkNote = user.AfkNote;
            IsAfk = user.IsAfk;
            Flag = user.Flag;
            Country = user.Country;
            LastActivity = user.LastActivity;
        }
    }
}
