using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.Windows;

namespace Jabbr.WPF.Rooms
{
    public class ChatRoomViewModel : RoomViewModel
    {
        public override void SendCommand(string command)
        {
            MessageBox.Show(command);
        }
    }
}
