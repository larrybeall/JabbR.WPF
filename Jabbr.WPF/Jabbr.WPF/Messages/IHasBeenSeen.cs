using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jabbr.WPF.Messages
{
    public interface IHasBeenSeen
    {
        bool HasBeenSeen { get; set; }
    }
}
