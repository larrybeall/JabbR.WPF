namespace Jabbr.WPF.Messages
{
    public interface ICanHaveUnreadMessages
    {
        int UnreadMessageCount { get; }
        bool HasUnreadMessages { get; }
        void SetAllMessagesRead();
    }
}