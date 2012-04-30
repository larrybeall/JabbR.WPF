using System;
using System.Globalization;
using System.Windows.Data;
using Jabbr.WPF.Users;

namespace Jabbr.WPF.Markup.Converters
{
    public class GroupTypeConverter : IValueConverter
    {
        private const string OwnersString = "Room Owners";
        private const string OnlineString = "Online";
        private const string AwayString = "Away";

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var groupType = (GroupType) value;
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var groupTypeString = (string) value;
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

        #endregion
    }
}