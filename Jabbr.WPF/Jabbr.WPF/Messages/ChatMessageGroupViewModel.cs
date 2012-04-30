using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Caliburn.Micro;
using Jabbr.WPF.Users;

namespace Jabbr.WPF.Messages
{
    public class ChatMessageGroupViewModel : MessageViewModel, ICanHaveUnreadMessages
    {
        private readonly IObservableCollection<ChatMessageViewModel> _messages;
        private readonly UserViewModel _user;
        private DateTime _lastMessageDateTime;
        private int _unreadMessageCount;

        public ChatMessageGroupViewModel(ChatMessageViewModel message)
            : base(false)
        {
            IsNotifying = false;

            _messages = new BindableCollection<ChatMessageViewModel> {message};
            _messages.CollectionChanged += MessagesOnCollectionChanged;
            _user = message.User;
            MessageDateTime = message.MessageDateTime;
            MessageId = message.MessageId;
            LastMessageDateTime = message.MessageDateTime;

            IsNotifying = true;
        }

        public IObservableCollection<ChatMessageViewModel> Messages
        {
            get { return _messages; }
        }

        public UserViewModel User
        {
            get { return _user; }
        }

        public DateTime LastMessageDateTime
        {
            get { return _lastMessageDateTime; }
            private set
            {
                if (_lastMessageDateTime == value)
                    return;

                _lastMessageDateTime = value;
                NotifyOfPropertyChange(() => LastMessageDateTime);
            }
        }

        #region ICanHaveUnreadMessages Members

        public int UnreadMessageCount
        {
            get { return _unreadMessageCount; }
            private set
            {
                if (_unreadMessageCount == value)
                    return;

                _unreadMessageCount = value;
                NotifyOfPropertyChange(() => UnreadMessageCount);
                NotifyOfPropertyChange(() => HasUnreadMessages);
            }
        }

        public bool HasUnreadMessages
        {
            get { return _unreadMessageCount > 0; }
        }

        public void SetAllMessagesRead()
        {
            // capture point of time view
            List<ChatMessageViewModel> unseenMessages = _messages.Where(x => !x.HasBeenSeen).ToList();
            foreach (ChatMessageViewModel chatMessageViewModel in unseenMessages)
            {
                chatMessageViewModel.HasBeenSeen = true;
            }
        }

        #endregion

        private void MessagesOnCollectionChanged(object sender,
                                                 NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.Action != NotifyCollectionChangedAction.Add) return;

            int totalNewItems = notifyCollectionChangedEventArgs.NewItems.Count;
            var lastItem = (ChatMessageViewModel) notifyCollectionChangedEventArgs.NewItems[totalNewItems - 1];
            LastMessageDateTime = lastItem.MessageDateTime;
        }

        public bool TryAddMessage(ChatMessageViewModel message)
        {
            if (!ReferenceEquals(message.User, User))
                return false;

            _messages.Add(message);
            UnreadMessageCount = _messages.Count(x => !x.HasBeenSeen);
            return true;
        }
    }
}