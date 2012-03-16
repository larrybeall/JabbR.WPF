using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Jabbr.WPF.Infrastructure;

namespace Jabbr.WPF.Rooms
{
    public class ChatWindowViewModel : Conductor<RoomViewModel>.Collection.OneActive
    {
        private readonly JabbrManager _jabbrManager;
        private readonly ServiceLocator _serviceLocator;

        public ChatWindowViewModel(JabbrManager jabbrManager, ServiceLocator serviceLocator)
        {
            _jabbrManager = jabbrManager;
            _serviceLocator = serviceLocator;

            Initialize();
        }

        private void Initialize()
        {
            DisplayName = "Chat Window";

            _jabbrManager.JoinedRoom += JabbrManagerOnJoinedRoom;
        }

        private void JabbrManagerOnJoinedRoom(object sender, RoomDetailsEventArgs roomDetailsEventArgs)
        {
            var roomVm = _serviceLocator.GetViewModel<RoomViewModel>();
            roomVm.Initialize(roomDetailsEventArgs);

            ActivateItem(roomVm);
        }
    }
}
