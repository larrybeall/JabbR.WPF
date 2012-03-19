using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Jabbr.WPF.Infrastructure.Models;
using Jabbr.WPF.Infrastructure;

namespace Jabbr.WPF.Users
{
    public class UserViewModel : PropertyChangedBase
    {
        private const string GravatarUrlFormat = "http://www.gravatar.com/avatar/{0}?d=mm&s=75";

        private readonly JabbrManager _jabbrManager;

        private bool _isOwner;
        private bool _isAway;
        private string _name;
        private string _notes;
        private string _gravatar;

        public UserViewModel(JabbrManager jabbrManager)
        {
            _jabbrManager = jabbrManager;
        }

        public bool IsOwner
        {
            get { return _isOwner; }
            set
            {
                if(_isOwner == value)
                    return;

                _isOwner = value;
                NotifyOfPropertyChange(() => IsOwner);
            }
        }

        public bool IsAway
        {
            get { return _isAway; }
            set
            {
                if(_isAway == value)
                    return;
                
                _isAway = value;
                NotifyOfPropertyChange(() => IsAway);
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if(_name == value)
                    return;

                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public string Notes
        {
            get { return _notes; }
            set
            {
                if(_notes == value)
                    return;

                _notes = value;
                NotifyOfPropertyChange(() => Notes);
            }
        }

        public string Gravatar
        {
            get { return _gravatar; }
            set
            {
                if(_gravatar == value)
                    return;
                
                _gravatar = value;
                NotifyOfPropertyChange(() => Gravatar);
            }
        }

        internal void Initialize(User user, bool isOwner)
        {
            _jabbrManager.UserActivityChanged += JabbrManagerOnUserActivityChanged;

            IsOwner = isOwner;
            Name = user.Name;
            IsAway = user.Status == UserStatus.Inactive || user.IsAfk;
            Notes = (user.IsAfk) ? user.AfkNote ?? user.Note : user.Note;
            Gravatar = string.Format(GravatarUrlFormat, user.Hash ?? "00000000000000000000000000000000");
        }

        private void JabbrManagerOnUserActivityChanged(object sender, UserEventArgs userEventArgs)
        {
            string userName = userEventArgs.User.Name;
            if(!userName.Equals(Name))
                return;

            IsAway = userEventArgs.User.IsAfk || userEventArgs.User.Status == UserStatus.Inactive;
        }
    }
}
