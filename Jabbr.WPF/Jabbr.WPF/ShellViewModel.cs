using System;

namespace Jabbr.WPF {
    using System.ComponentModel.Composition;
    using System.Windows;
using Caliburn.Micro;
    using Jabbr.WPF.Authentication;
using Jabbr.WPF.Infrastructure;
    using Jabbr.WPF.Rooms;

    [Export(typeof(IShell))]
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IShell
    {
        private readonly IWindowManager _windowManager;
        private readonly ServiceLocator _serviceLocator;
        private readonly JabbrManager _jabbrManager;

        public ShellViewModel(IWindowManager windowManager, ServiceLocator serviceLocator, JabbrManager jabbrManager)
        {
            _windowManager = windowManager;
            _serviceLocator = serviceLocator;
            _jabbrManager = jabbrManager;

            _jabbrManager.JoinedRoom += JabbrManagerOnJoinedRoom;
            Initialize();
        }

        private void JabbrManagerOnJoinedRoom(object sender, RoomDetailsEventArgs roomDetailsEventArgs)
        {
            var room = new ChatRoomViewModel
                           {
                               RoomName = roomDetailsEventArgs.Room.Name
                           };
            Items.Add(room);
        }

        private void Initialize()
        {
            DisplayName = "JabbR.WPF";
            var lobby = _serviceLocator.GetViewModel<LobbyRoomViewModel>();
            ActivateItem(lobby);
        }

        public void OnShellLoaded()
        {
            var viewModel = _serviceLocator.GetViewModel<AuthWindowViewModel>();
            _windowManager.ShowDialog(viewModel);
        }
    }
}
