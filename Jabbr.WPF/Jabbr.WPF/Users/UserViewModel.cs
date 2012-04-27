using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Jabbr.WPF.Infrastructure;

namespace Jabbr.WPF.Users
{
    public class UserViewModel : PropertyChangedBase
    {
        private bool _isAway;
        private bool _isAfk;
        private string _name;
        private string _note;
        private string _gravatar;
        private bool _isCurrentUser;

        public bool IsCurrentUser
        {
            get { return _isCurrentUser; }
            set
            {
                if(_isCurrentUser == value)
                    return;

                _isCurrentUser = value;
                NotifyOfPropertyChange(() => IsCurrentUser);
            }
        }

        public virtual bool IsAway
        {
            get { return _isAway; }
            set
            {
                if (_isAway == value)
                    return;

                _isAway = value;
                NotifyOfPropertyChange(() => IsAway);
            }
        }

        public virtual bool IsAfk
        {
            get { return _isAfk; }
            set
            {
                if (_isAfk == value)
                    return;

                _isAfk = value;
                NotifyOfPropertyChange(() => IsAfk);
            }
        }

        public virtual string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;

                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public virtual string Note
        {
            get { return _note; }
            set
            {
                if (_note == value)
                    return;

                _note = value;
                NotifyOfPropertyChange(() => Note);
            }
        }

        public virtual string Gravatar
        {
            get { return _gravatar; }
            set
            {
                if (_gravatar == value)
                    return;

                _gravatar = value;
                NotifyOfPropertyChange(() => Gravatar);
            }
        }

        internal void SetNote(bool isAfk, string afkNote, string note)
        {
            IsAfk = isAfk;

            if (IsAfk)
            {
                if (string.IsNullOrEmpty(afkNote))
                    Note = "AFK";
                else
                    Note = "AFK " + afkNote.Trim();

                IsAway = true;
            }
            else
                Note = note;
        }
    }
}
