using Jabbr.WPF.Users;

namespace Jabbr.WPF.Messages
{
    public class ChatMessageViewModel : MessageViewModel, IHasBeenSeen
    {
        private bool _hasBeenSeen;
        private string _rawContent;
        private string _richContent;
        private UserViewModel _user;

        public ChatMessageViewModel()
            : base(true)
        {
        }

        public UserViewModel User
        {
            get { return _user; }
            set
            {
                if (_user == value)
                    return;

                _user = value;
                NotifyOfPropertyChange(() => User);
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

        #region IHasBeenSeen Members

        public bool HasBeenSeen
        {
            get { return _hasBeenSeen; }
            set
            {
                if (_hasBeenSeen == value)
                    return;

                _hasBeenSeen = value;
                NotifyOfPropertyChange(() => HasBeenSeen);
            }
        }

        #endregion
    }
}