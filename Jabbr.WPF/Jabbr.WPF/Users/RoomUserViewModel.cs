using System.ComponentModel;
using Caliburn.Micro;
using System.Windows.Threading;
using System;

namespace Jabbr.WPF.Users
{
    public class RoomUserViewModel : PropertyChangedBase
    {
        private readonly UserViewModel _userViewModel;
        private readonly DispatcherTimer _typingTimer;

        private GroupType _group;
        private bool _isOwner;
        private bool _isTyping;

        public RoomUserViewModel(UserViewModel userViewModel)
        {
            _userViewModel = userViewModel;
            _userViewModel.PropertyChanged += OnUserPropertyChanged;

// ReSharper disable UseObjectOrCollectionInitializer
            _typingTimer = new DispatcherTimer();
// ReSharper restore UseObjectOrCollectionInitializer
            _typingTimer.Interval = TimeSpan.FromSeconds(3);
            _typingTimer.Tick += OnTypingExpiration;

            SetGroup();
        }

        private void OnTypingExpiration(object sender, EventArgs eventArgs)
        {
            _typingTimer.Stop();
            IsTyping = false;
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

        public UserViewModel User
        {
            get { return _userViewModel; }
        }

        private void OnUserPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if(propertyChangedEventArgs.PropertyName == "IsAway")
                SetGroup();
        }

        private void ResetTypingTimer()
        {
            if(_typingTimer.IsEnabled)
                _typingTimer.Stop();

            _typingTimer.Start();
        }

        private void SetGroup()
        {
            if (IsOwner)
            {
                Group = GroupType.Owners;
                return;
            }

            Group = _userViewModel.IsAway ? GroupType.Away : GroupType.Online;
        }
    }
}