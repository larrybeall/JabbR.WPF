using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Jabbr.WPF.Resources.Converters
{
    public class GreaterThanConverter : ComparableConverter
    {
        protected override bool EvaluateComparisonValue(int comparisonValue)
        {
            return comparisonValue > 0;
        }
    }

    public class LessThanConverter : ComparableConverter
    {
        protected override bool EvaluateComparisonValue(int comparisonValue)
        {
            return comparisonValue < 0;
        }
    }

    public class EqualToConverter : ComparableConverter
    {
        protected override bool EvaluateComparisonValue(int comparisonValue)
        {
            return comparisonValue == 0;
        }
    }

    public abstract class ComparableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            IComparable comparable = value as IComparable;
            if (comparable == null)
                throw new InvalidOperationException("Value is not comparable.");

            int comparisonValue = comparable.CompareTo(parameter);
            return EvaluateComparisonValue(comparisonValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new InvalidOperationException("Cannot convert comparable value back to original value");
        }

        protected abstract bool EvaluateComparisonValue(int comparisonValue);
    }
}
