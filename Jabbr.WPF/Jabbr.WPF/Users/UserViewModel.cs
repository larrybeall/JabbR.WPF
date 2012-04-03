using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Jabbr.WPF.Infrastructure.Models;
using Jabbr.WPF.Infrastructure;

namespace Jabbr.WPF.Users
{
    public class UserViewModel : UserViewModelBase
    {
        public override bool IsOwner
        {
            get { return false; }
        }
    }
}
