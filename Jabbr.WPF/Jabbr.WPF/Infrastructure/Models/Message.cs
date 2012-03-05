using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JabbrModels = JabbR.Client.Models;

namespace Jabbr.WPF.Infrastructure.Models
{
    public class Message
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public DateTimeOffset When { get; set; }
        public User User { get; set; }

        public Message()
        {
            
        }

        public Message(JabbrModels.Message message)
        {
            Id = message.Id;
            Content = message.Content;
            When = message.When;
            User = new User(message.User);
        }
    }
}
