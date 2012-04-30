using System.ComponentModel;
using Caliburn.Micro;

namespace Jabbr.WPF.Users
{
    public class RoomUserViewModel : PropertyChangedBase
    {
        private readonly UserViewModel _userViewModel;

        private GroupType _group;
        private bool _isOwner;
        private bool _isTyping;

        public RoomUserViewModel(UserViewModel userViewModel)
        {
            _userViewModel = userViewModel;
            _userViewModel.PropertyChanged += OnUserPropertyChanged;
            SetGroup();
        }

        public bool IsOwner
        {
            get { return _isOwner; }
            set
            {
                if (_isOwner == value)
                    return;

                _isOwner = value;
                NotifyOfPropertyChange(() => IsOwner);
                SetGroup();
            }
        }

        public bool IsAway
        {
            get { return _userViewModel.IsAway; }
            set
            {
                _userViewModel.IsAway = value;
                SetGroup();
            }
        }

        public bool IsAfk
        {
            get { return _userViewModel.IsAfk; }
            set { _userViewModel.IsAfk = value; }
        }

        public string Name
        {
            get { return _userViewModel.Name; }
            set { _userViewModel.Name = value; }
        }

        public string Note
        {
            get { return _userViewModel.Note; }
            set { _userViewModel.Note = value; }
        }

        public string Gravatar
        {
            get { return _userViewModel.Gravatar; }
            set { _userViewModel.Gravatar = value; }
        }

        public bool IsTyping
        {
            get { return _isTyping; }
            set
            {
                if (value)
                    ResetTypingTimer();

                if (_isTyping == value)
                    return;

                _isTyping = value;
                NotifyOfPropertyChange(() => IsTyping);
            }
        }

        public GroupType Group
        {
            get { return _group; }
            set
            {
                if (_group == value)
                    return;

                _group = value;
                NotifyOfPropertyChange(() => Group);
            }
        }

        internal UserViewModel User
        {
            get { return _userViewModel; }
        }

        private void OnUserPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            NotifyOfPropertyChange(propertyChangedEventArgs.PropertyName);
        }

        private void ResetTypingTimer()
        {
        }

        private void SetGroup()
        {
            if (IsOwner)
            {
                Group = GroupType.Owners;
                return;
            }

            Group = IsAway ? GroupType.Away : GroupType.Online;
        }
    }
}