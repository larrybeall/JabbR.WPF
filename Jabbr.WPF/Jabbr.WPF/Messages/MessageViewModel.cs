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
        private Inline[] _richContent;
        private DateTime _messageDateTime;
        private string _messageId;
        private string _rawContent;

        public MessageViewModel()
            :base(false)
        {
            
        }

        public MessageViewModel(bool cacheViews)
            : base(cacheViews)
        {
            
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

        public Inline[] RichContent
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
