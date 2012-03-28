using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jabbr.WPF.Infrastructure.Models;
using Jabbr.WPF.Infrastructure;

namespace Jabbr.WPF.Messages
{
    public class ChatMessageViewModel : MessageViewModel
    {
        private bool _seen;

        public bool Seen
        {
            get { return _seen; }
            set
            {
                if(_seen == value)
                    return;

                _seen = value;
                NotifyOfPropertyChange(() => Seen);
            }
        }

        internal void Initialize(Message message, bool isRoomVisible)
        {
            Content = message.Content;
            Seen = isRoomVisible;
            MessageTime = message.When.LocalDateTime;
        }
    }
}
