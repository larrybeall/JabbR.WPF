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
    public class ChatMessageViewModel : MessageViewModel
    {
        private bool _hasBeenSeen;
        private string _username;
        private string _gravatarHash;

        public ChatMessageViewModel()
            :base(true)
        {
            
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
