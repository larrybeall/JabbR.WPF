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
        private IUserViewModel _user;

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

        public IUserViewModel User
        {
            get { return _user; }
            set
            {
                if(_user == value)
                    return;

                _user = value;
                NotifyOfPropertyChange(() => User);
            }
        }
    }
}
