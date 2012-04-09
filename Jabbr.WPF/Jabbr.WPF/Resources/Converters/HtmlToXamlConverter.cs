using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Documents;

namespace Jabbr.WPF.Resources.Converters
{
    public class HtmlToXamlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string htmlString = value as string;
            if(string.IsNullOrEmpty(htmlString))
                return null;

            return HtmlToXamlConversion.HtmlToXamlConverter.ConvertHtmlToXaml(htmlString, false);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
