using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace Jabbr.WPF.Messages
{
    //public class MessageCollectionViewModel
    //{
    //    private IObservableCollection<MessageViewModel> _messages;

    //    public MessageCollectionViewModel()
    //    {
    //        _messages = new BindableCollection<MessageViewModel>();
    //    }

    //    public IObservableCollection<MessageViewModel> Messages
    //    {
    //        get { return _messages; }
    //    }

    //    public int UnreadMessageCount
    //    {
    //        get { return _messages.OfType<ChatMessageViewModel>().Count(x => !x.HasBeenSeen); }
    //    }

    //    public bool IsNotifying
    //    {
    //        get { return _messages.IsNotifying; }
    //        set { _messages.IsNotifying = value; }
    //    }

    //    public void AddNewMessage(MessageViewModel message)
    //    {
    //        Guid messageGroup = Guid.NewGuid();
    //        ChatMessageViewModel chatMessage = message as ChatMessageViewModel;
    //        int msgCount = Messages.Count;

    //        if (msgCount > 0 && chatMessage != null)
    //        {
    //            var lastMessage = Messages[msgCount - 1] as ChatMessageViewModel;
    //            if (lastMessage != null && ReferenceEquals(lastMessage.User, chatMessage.User))
    //            {
    //                messageGroup = lastMessage.MessageGroup;
    //            }
    //        }

    //        message.MessageGroup = messageGroup;

    //        Messages.Add(message);
    //    }

    //    public void AddPreviousMessage(MessageViewModel message)
    //    {
    //        Guid messageGroup = Guid.NewGuid();
    //        ChatMessageViewModel chatMessage = message as ChatMessageViewModel;

    //        if(Messages.Count > 0 && chatMessage != null)
    //        {
    //            var firstMessage = Messages[0] as ChatMessageViewModel;
    //            if (firstMessage != null && ReferenceEquals(firstMessage.User, chatMessage.User))
    //            {
    //                messageGroup = firstMessage.MessageGroup;
    //            }
    //        }

    //        message.MessageGroup = messageGroup;

    //        Messages.Insert(0, message);
    //    }

    //    public void AddPreviousMessages(IEnumerable<MessageViewModel> messages)
    //    {
    //        var orderedMessages = messages.OrderByDescending(x => x.MessageDateTime);

    //        foreach (var messageViewModel in orderedMessages)
    //        {
    //            AddPreviousMessage(messageViewModel);
    //        }
    //    }

    //    public void AddNewMessages(IEnumerable<MessageViewModel> messages)
    //    {
    //        var orderedMessages = messages.OrderBy(x => x.MessageDateTime);

    //        foreach (var messageViewModel in orderedMessages)
    //        {
    //            AddNewMessage(messageViewModel);
    //        }
    //    }

    //    public void SetAllMessagesAsSeen()
    //    {
    //        // capture collection reference incase another thread updates this collection
    //        var messages = Messages.OfType<ChatMessageViewModel>().Where(x => !x.HasBeenSeen).ToList();
    //        foreach (var chatMessageViewModel in messages)
    //        {
    //            chatMessageViewModel.HasBeenSeen = true;
    //        }
    //    }
    //}
}
