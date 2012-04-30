using System;
using Caliburn.Micro;

namespace Jabbr.WPF.Messages
{
    public abstract class MessageViewModel : ViewAware
    {
        private DateTime _messageDateTime;
        private string _messageId;

        protected MessageViewModel()
            : base(false)
        {
        }

        protected MessageViewModel(bool cacheViews)
            : base(cacheViews)
        {
        }

        public DateTime MessageDateTime
        {
            get { return _messageDateTime; }
            set
            {
                if (_messageDateTime == value)
                    return;

                _messageDateTime = value;
                NotifyOfPropertyChange(() => MessageDateTime);
            }
        }

        public string MessageId
        {
            get { return _messageId; }
            set
            {
                if (_messageId == value)
                    return;

                _messageId = value;
                NotifyOfPropertyChange(() => MessageId);
            }
        }
    }
}