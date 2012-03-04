using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jabbr.WPF.Infrastructure
{
    public class SignInCompleteEventArgs : EventArgs
    {
        public string Username { get; set; }
    }
}
