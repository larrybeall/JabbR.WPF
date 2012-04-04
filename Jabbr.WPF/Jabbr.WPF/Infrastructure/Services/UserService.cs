using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jabbr.WPF.Users;
using Jabbr.WPF.Infrastructure.Models;
using System.Threading;
using JabbrModels = JabbR.Client.Models;

namespace Jabbr.WPF.Infrastructure.Services
{
    public class UserService : BaseService
    {
        private const string GravatarUrlFormat = "http://www.gravatar.com/avatar/{0}?d=mm&s=75";

        private readonly static object SyncRoot = new object();

        private readonly ServiceLocator _serviceLocator;
        private readonly List<UserViewModel> _users;  

        public UserService(ServiceLocator serviceLocator)
            : base()
        {
            _serviceLocator = serviceLocator;
            _users = new List<UserViewModel>();
        }

        public event EventHandler<UserJoinedEventArgs> UserJoined;

        public UserViewModel GetUserViewModel(string name)
        {
            return _users.SingleOrDefault(x => x.Name == name);
        }

        public UserViewModel GetUserViewModel(JabbrModels.User user)
        {
            return GetUserViewModel(new User(user));
        }

        public UserViewModel GetUserViewModel(User user)
        {
            UserViewModel toReturn = GetUserViewModel(user.Name);

            if (toReturn != null)
                return toReturn;

            lock (SyncRoot)
            {
                // need to perform lookup a second time after we have acquired a lock
                // as the value could have changed
                toReturn = GetUserViewModel(user.Name);
                if (toReturn != null)
                    return toReturn;

                toReturn = _serviceLocator.GetViewModel<UserViewModel>();

                toReturn.IsNotifying = false;

                toReturn.Name = user.Name;
                toReturn.IsAway = user.Status == UserStatus.Inactive || user.IsAfk;
                toReturn.Note = (user.IsAfk) ? user.AfkNote ?? user.Note : user.Note;
                toReturn.Gravatar = string.Format(GravatarUrlFormat, user.Hash ?? "00000000000000000000000000000000");

                toReturn.IsNotifying = true;

                _users.Add(toReturn);
            }

            return toReturn;
        }

        public void ProcessNoteChanged(JabbrModels.User user)
        {
            var userVm = GetUserViewModel(user.Name);

            if(userVm == null)
                return;

            userVm.SetNote(user.IsAfk, user.AfkNote, user.Note);
        }

        public void ProcessUserJoined(JabbrModels.User user, string room)
        {
            UserViewModel userVm = GetUserViewModel(user);

            PostOnUi(() =>
            {
                var handler = UserJoined;
                if(handler != null)
                    handler(this, new UserJoinedEventArgs(userVm, room));
            });
        }

        public void ProcessUsernameChange(JabbrModels.User user, string oldUsername)
        {
            UserViewModel userVm = GetUserViewModel(oldUsername);

            if(userVm == null)
                return;

            PostOnUi(() =>
            {
                userVm.Name = user.Name;
            });
        }

        public void ProcessUserActivityChanged(JabbrModels.User user)
        {
            var userVm = GetUserViewModel(user.Name);
            if(userVm == null)
                return;

            PostOnUi(() =>
            {
                userVm.IsAway = user.Status == JabbrModels.UserStatus.Inactive;
            });
        }
    }
}
