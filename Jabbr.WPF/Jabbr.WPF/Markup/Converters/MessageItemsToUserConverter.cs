using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Jabbr.WPF.Messages;
using Jabbr.WPF.Users;

namespace Jabbr.WPF.Markup.Converters
{
    public class MessageItemsToUserConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<ChatMessageViewModel> messages = ((IEnumerable<Object>) value).OfType<ChatMessageViewModel>().ToList();
            UserViewModel user = messages.First().User;

            return user;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}