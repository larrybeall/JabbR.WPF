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
        private const string OwnersGroup = "Owners";
        private const string OnlineGroup = "Online";
        private const string AwayGroup = "Away";

        private readonly JabbrManager _jabbrManager;

        private bool _isOwner;
        private bool _isAway;
        private bool _isAfk;
        private string _name;
        private string _note;
        private string _gravatar;
        private string _group;

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

                SetGroup();
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

                SetGroup();
            }
        }

        public bool IsAfk
        {
            get { return _isAfk; }
            set
            {
                if(_isAfk == value)
                    return;

                _isAfk = value;
                NotifyOfPropertyChange(() => IsAfk);
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

        public string Note
        {
            get { return _note; }
            set
            {
                if(_note == value)
                    return;

                _note = value;
                NotifyOfPropertyChange(() => Note);
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

        public string Group
        {
            get { return _group; }
            set
            {
                if(_group == value)
                    return;

                _group = value;
                NotifyOfPropertyChange(() => Group);
            }
        }

        private void SetGroup()
        {
            if (IsOwner)
            {
                Group = OwnersGroup;
                return;
            }

            Group = IsAway ? AwayGroup : OnlineGroup;
        }

        internal void Initialize(User user, bool isOwner)
        {
            _jabbrManager.UserActivityChanged += JabbrManagerOnUserActivityChanged;

            IsOwner = isOwner;
            Name = user.Name;
            IsAway = user.Status == UserStatus.Inactive || user.IsAfk;
            Note = (user.IsAfk) ? user.AfkNote ?? user.Note : user.Note;
            Gravatar = string.Format(GravatarUrlFormat, user.Hash ?? "00000000000000000000000000000000");
        }

        internal void SetNote(User user)
        {
            IsAfk = user.IsAfk;

            if (IsAfk)
            {
                if (string.IsNullOrEmpty(user.AfkNote))
                    Note = "AFK";
                else
                    Note = "AFK " + user.AfkNote.Trim();
                
                IsAway = true;
            }
            else
                Note = user.Note;
        }

        private void JabbrManagerOnUserActivityChanged(object sender, UserEventArgs userEventArgs)
        {
            string userName = userEventArgs.User.Name;
            if(!userName.Equals(Name))
                return;

            IsAway = userEventArgs.User.Status == UserStatus.Inactive;
        }
    }
}
