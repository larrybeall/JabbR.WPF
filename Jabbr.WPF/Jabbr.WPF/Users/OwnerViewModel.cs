using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jabbr.WPF.Infrastructure;

namespace Jabbr.WPF.Users
{
    public class OwnerViewModel : IUserViewModel
    {
        private readonly IUserViewModel _userViewModel;

        public OwnerViewModel(IUserViewModel userViewModel)
        {
            _userViewModel = userViewModel;
        }

        public bool IsOwner
        {
            get { return true; }
        }

        public bool IsAway
        {
            get { return _userViewModel.IsAway; }
            set { _userViewModel.IsAway = value; }
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

        public GroupType Group
        {
            get { return GroupType.Owners; }
            set { /* intentionally blank */ }
        }
    }
}
