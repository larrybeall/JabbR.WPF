using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Jabbr.WPF.Messages;

namespace Jabbr.WPF.Resources.Converters
{
    public class MessageItemsToUserConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var messages = ((IEnumerable<Object>) value).OfType<ChatMessageViewModel>().ToList();
            var user = messages.First().User;

            return user;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
