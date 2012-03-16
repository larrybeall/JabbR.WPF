using System;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using Jabbr.WPF.Authentication;
using Jabbr.WPF.Infrastructure;
using Jabbr.WPF.Rooms;
using System.Linq;

namespace Jabbr.WPF
{

    public class ShellViewModelOld : Conductor<IScreen>.Collection.OneActive, IShell
    {
        private readonly IWindowManager _windowManager;
        private readonly ServiceLocator _serviceLocator;
        private readonly JabbrManager _jabbrManager;

        public ShellViewModelOld(IWindowManager windowManager, ServiceLocator serviceLocator, JabbrManager jabbrManager)
        {
            _windowManager = windowManager;
            _serviceLocator = serviceLocator;
            _jabbrManager = jabbrManager;

            _jabbrManager.JoinedRoom += JabbrManagerOnJoinedRoom;
            _jabbrManager.LeftRoom += JabbrManagerOnLeftRoom;
            Initialize();
        }

        internal bool SendCommand(string command)
        {
            return SendCommand(command, null);
        }

        internal bool SendCommand(string command, string room)
        {
            if (command.StartsWith("/join"))
            {
                string roomName = command.Replace("/join", string.Empty).Trim();
                var roomVm = GetChatRoom(roomName);
                if (roomVm != null)
                {
                    ActivateItem(roomVm);
                }
                else
                {
                    _jabbrManager.JoinRoom(roomName);
                }

                return true;
            }

            return _jabbrManager.SendCommand(command, room);
        }

        private void JabbrManagerOnLeftRoom(object sender, RoomEventArgs roomEventArgs)
        {
            var room = roomEventArgs.Room;
            var roomVm = GetChatRoom(room);

            if(roomVm != null)
                DeactivateItem(roomVm, true);
        }

        private void JabbrManagerOnJoinedRoom(object sender, RoomDetailsEventArgs roomDetailsEventArgs)
        {
            var room = new ChatRoomViewModel
                           {
                               RoomName = roomDetailsEventArgs.Room.Name
                           };
            Items.Add(room);
        }

        private ChatRoomViewModel GetChatRoom(string roomName)
        {
            if (string.IsNullOrEmpty(roomName))
                return null;

            var rooms = Items.OfType<ChatRoomViewModel>();

            return rooms.FirstOrDefault(r => r.RoomName.Equals(roomName));
        }

        private void Initialize()
        {
            DisplayName = "JabbR.WPF";
            var lobby = _serviceLocator.GetViewModel<LobbyRoomViewModel>();
            lobby.NotifyOfPropertyChange("RoomName");
            ActivateItem(lobby);
        }

        public void OnShellLoaded()
        {
            var viewModel = _serviceLocator.GetViewModel<AuthWindowViewModel>();
            _windowManager.ShowDialog(viewModel);
        }
    }
}
