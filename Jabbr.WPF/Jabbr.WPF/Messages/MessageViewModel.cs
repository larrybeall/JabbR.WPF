using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace Jabbr.WPF.Messages
{
    public abstract class MessageViewModel : PropertyChangedBase
    {
        private string _content;
        private DateTime _messageTime;

        public string Content
        {
            get { return _content; }
            set
            {
                if(_content == value)
                    return;

                _content = value;
                NotifyOfPropertyChange(() => Content);
            }
        }

        public DateTime MessageTime
        {
            get { return _messageTime; }
            set
            {
                if(_messageTime == value)
                    return;

                _messageTime = value;
                NotifyOfPropertyChange(() => MessageTime);
            }
        }
    }
}
