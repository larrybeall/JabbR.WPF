using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.Windows.Documents;

namespace Jabbr.WPF.Messages
{
    public abstract class MessageViewModel : ViewAware
    {
        private string _richContent;
        private DateTime _messageDateTime;
        private string _messageId;
        private string _rawContent;
        private Guid _messageGroup;

        protected MessageViewModel()
            :base(false)
        {
            
        }

        protected MessageViewModel(bool cacheViews)
            : base(cacheViews)
        {
            
        }

        public Guid MessageGroup
        {
            get { return _messageGroup; }
            set
            {
                if(_messageGroup == value)
                    return;

                _messageGroup = value;
                NotifyOfPropertyChange(() => MessageGroup);
            }
        }

        public string RawContent
        {
            get { return _rawContent; }
            set
            {
                if (_rawContent == value)
                    return;

                _rawContent = value;
                NotifyOfPropertyChange(() => RawContent);
            }
        }

        public string RichContent
        {
            get { return _richContent; }
            set
            {
                if (_richContent == value)
                    return;

                _richContent = value;
                NotifyOfPropertyChange(() => RichContent);
                NotifyOfPropertyChange(() => HasRichContent);
            }
        }

        public bool HasRichContent
        {
            get { return RichContent != null; }
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
