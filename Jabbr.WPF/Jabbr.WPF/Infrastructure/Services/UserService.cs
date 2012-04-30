using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using JabbR.Client;
using JabbR.Client.Models;
using Jabbr.WPF.Users;

namespace Jabbr.WPF.Infrastructure.Services
{
    public class UserService : BaseService
    {
        private const string GravatarUrlFormat = "http://www.gravatar.com/avatar/{0}?d=mm&s=75";
        private readonly JabbRClient _client;

        private readonly ServiceLocator _serviceLocator;
        private readonly ConcurrentDictionary<string, UserViewModel> _users;

        public UserService(ServiceLocator serviceLocator, JabbRClient client)
        {
            _serviceLocator = serviceLocator;
            _client = client;
            _users = new ConcurrentDictionary<string, UserViewModel>();

            _client.UserActivityChanged += OnUserActivityChanged;
            _client.NoteChanged += UserNoteChanged;
            _client.UsersInactive += OnUsersInactive;
            _client.GravatarChanged += OnGravatarChanged;
            _client.UsernameChanged += OnUsernameChanged;
        }

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
            toReturn.Gravatar = CreateGravatarUrl(user.Hash);

            toReturn.IsNotifying = true;

            if (!_users.TryAdd(toReturn.Name, toReturn))
            {
                return GetUserViewModel(user.Name);
            }

            return toReturn;
        }

        public void ProcessNoteChanged(User user)
        {
            UserViewModel userVm = GetUserViewModel(user.Name);

            if (userVm == null)
                return;

            userVm.SetNote(user.IsAfk, user.AfkNote, user.Note);
        }

        private string CreateGravatarUrl(string gravatarHash)
        {
            return string.Format(GravatarUrlFormat, gravatarHash ?? "00000000000000000000000000000000");
        }

        private void OnUserActivityChanged(User user)
        {
            UserViewModel userVm = GetUserViewModel(user.Name);
            if (userVm == null)
                return;

            PostOnUi(() => { userVm.IsAway = user.Status == UserStatus.Inactive; });
        }

        private void UserNoteChanged(User user, string room)
        {
            UserViewModel userVm = GetUserViewModel(user.Name);
            if (userVm == null)
                return;

            PostOnUi(() => userVm.SetNote(user.IsAfk, user.AfkNote, user.Note));
        }

        private void OnUsersInactive(IEnumerable<User> users)
        {
            List<User> inactiveUsers = users.ToList();

            PostOnUi(() => inactiveUsers.ForEach(_ => _.Status = UserStatus.Inactive));
        }

        private void OnGravatarChanged(User user, string room)
        {
            UserViewModel userVm = GetUserViewModel(user.Name);
            if (userVm == null)
                return;

            PostOnUi(() => userVm.Gravatar = CreateGravatarUrl(user.Hash));
        }

        private void OnUsernameChanged(string oldUsername, User user, string arg3)
        {
            UserViewModel userVm = GetUserViewModel(oldUsername);

            if (userVm == null)
                return;

            if (!_users.TryRemove(oldUsername, out userVm))
                return;

            _users.TryAdd(user.Name, userVm);

            PostOnUi(() => { userVm.Name = user.Name; });
        }
    }
}