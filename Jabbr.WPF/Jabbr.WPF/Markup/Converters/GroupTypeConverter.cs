using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Jabbr.WPF.Users;

namespace Jabbr.WPF.Markup.Converters
{
    public class GroupTypeConverter : IValueConverter
    {
        private const string OwnersString = "Room Owners";
        private const string OnlineString = "Online";
        private const string AwayString = "Away";

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            GroupType groupType = (GroupType)value;
            string toReturn;

            switch (groupType)
            {
                case GroupType.Owners:
                    toReturn = OwnersString;
                    break;
                case GroupType.Away:
                    toReturn = AwayString;
                    break;
                default:
                    toReturn = OnlineString;
                    break;
            }

            return toReturn;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string groupTypeString = (string) value;
            GroupType toReturn;

            switch (groupTypeString)
            {
                case OwnersString:
                    toReturn = GroupType.Owners;
                    break;
                case AwayString:
                    toReturn = GroupType.Away;
                    break;
                default:
                    toReturn = GroupType.Online;
                    break;
            }

            return toReturn;
        }
    }
}
