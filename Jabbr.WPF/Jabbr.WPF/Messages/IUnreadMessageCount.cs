using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jabbr.WPF.Messages
{
    public interface ICanHaveUnreadMessages
    {
        int UnreadMessageCount { get; }
        bool HasUnreadMessages { get; }
        void SetAllMessagesRead();
    }
}
