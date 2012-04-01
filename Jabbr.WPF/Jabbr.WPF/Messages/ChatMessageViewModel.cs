using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jabbr.WPF.Infrastructure.Models;
using Jabbr.WPF.Infrastructure;
using Caliburn.Micro;
using Jabbr.WPF.Users;
using System.Windows.Documents;

namespace Jabbr.WPF.Messages
{
    public class ChatMessageViewModel : PropertyChangedBase
    {
        private InlineCollection _content;
        private DateTime _messageDateTime;
        private string _messageId;
        private bool _hasBeenSeen;
        private string _username;
        private string _gravatarHash;

        public string RawContent { get; set; }

        public InlineCollection Content
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

        public DateTime MessageDateTime
        {
            get { return _messageDateTime; }
            set
            {
                if(_messageDateTime == value)
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
                if(_messageId == value)
                    return;

                _messageId = value;
                NotifyOfPropertyChange(() => MessageId);
            }
        }

        public bool HasBeenSeen
        {
            get { return _hasBeenSeen; }
            set
            {
                if(_hasBeenSeen == value)
                    return;

                _hasBeenSeen = value;
                NotifyOfPropertyChange(() => HasBeenSeen);
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                if(_username == value)
                    return;

                _username = value;
                NotifyOfPropertyChange(() => Username);
            }
        }

        public string GravatarHash
        {
            get { return _gravatarHash; }
            set
            {
                if(_gravatarHash == value)
                    return;

                _gravatarHash = value;
                NotifyOfPropertyChange(() => GravatarHash);
            }
        }
    }
}
