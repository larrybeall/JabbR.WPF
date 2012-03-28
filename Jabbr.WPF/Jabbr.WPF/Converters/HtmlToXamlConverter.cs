using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Documents;

namespace Jabbr.WPF.Converters
{
    public class HtmlToXamlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string htmlString = value as string;
            if(string.IsNullOrEmpty(htmlString))
                return null;

            string xamlString = HtmlToXamlConversion.HtmlToXamlConverter.ConvertHtmlToXaml(htmlString, false);
            var parsedXaml = XamlReader.Parse(xamlString);
            var xamlLines = ((Paragraph) ((Section) parsedXaml).Blocks.FirstBlock).Inlines;

            return xamlLines;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
