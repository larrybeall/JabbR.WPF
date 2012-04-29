using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jabbr.WPF.Users;
using System.Threading;
using JabbR.Client.Models;
using System.Collections.Concurrent;
using JabbR.Client;

namespace Jabbr.WPF.Infrastructure.Services
{
    public class UserService : BaseService
    {
        private const string GravatarUrlFormat = "http://www.gravatar.com/avatar/{0}?d=mm&s=75";

        private readonly ServiceLocator _serviceLocator;
        private readonly ConcurrentDictionary<string, UserViewModel> _users;
        private readonly JabbRClient _client;

        public UserService(ServiceLocator serviceLocator, JabbRClient client)
            : base()
        {
            _serviceLocator = serviceLocator;
            _client = client;
            _users = new ConcurrentDictionary<string, UserViewModel>();

            _client.UserActivityChanged += OnUserActivityChanged;
            _client.NoteChanged += UserNoteChanged;
        }

        public event EventHandler<UserJoinedEventArgs> UserJoined;

        public UserViewModel GetUserViewModel(string name)
        {
            UserViewModel user;
            _users.TryGetValue(name, out user);

            return user;
        }

        public UserViewModel GetUserViewModel(User user)
        {
            UserViewModel toReturn = GetUserViewModel(user.Name);

            if (toReturn != null)
                return toReturn;

            toReturn = _serviceLocator.GetViewModel<UserViewModel>();

            toReturn.IsNotifying = false;

            toReturn.Name = user.Name;
            toReturn.IsAway = user.Status == UserStatus.Inactive || user.IsAfk;
            toReturn.Note = (user.IsAfk) ? user.AfkNote ?? user.Note : user.Note;
            toReturn.Gravatar = string.Format(GravatarUrlFormat, user.Hash ?? "00000000000000000000000000000000");

            toReturn.IsNotifying = true;

            if (!_users.TryAdd(toReturn.Name, toReturn))
            {
                return GetUserViewModel(user.Name);
            }

            return toReturn;
        }

        public void ProcessNoteChanged(User user)
        {
            var userVm = GetUserViewModel(user.Name);

            if(userVm == null)
                return;

            userVm.SetNote(user.IsAfk, user.AfkNote, user.Note);
        }

        public void ProcessUserJoined(User user, string room)
        {
            UserViewModel userVm = GetUserViewModel(user);

            PostOnUi(() =>
            {
                var handler = UserJoined;
                if(handler != null)
                    handler(this, new UserJoinedEventArgs(userVm, room));
            });
        }

        public void ProcessUsernameChange(User user, string oldUsername)
        {
            UserViewModel userVm = GetUserViewModel(oldUsername);

            if(userVm == null)
                return;

            if (!_users.TryRemove(oldUsername, out  userVm))
                return;

            _users.TryAdd(user.Name, userVm);

            PostOnUi(() =>
            {
                userVm.Name = user.Name;
            });
        }

        private void OnUserActivityChanged(User user) 
        {
            var userVm = GetUserViewModel(user.Name);
            if (userVm == null)
                return;

            PostOnUi(() =>
            {
                userVm.IsAway = user.Status == UserStatus.Inactive;
            });
        }

        private void UserNoteChanged(User user, string room)
        {
            var userVm = GetUserViewModel(user.Name);
            if(userVm == null)
                return;

            PostOnUi(() => userVm.SetNote(user.IsAfk, user.AfkNote, user.Note));
        }
    }
}
