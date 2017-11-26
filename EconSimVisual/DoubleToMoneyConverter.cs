using System;
using System.Globalization;
using System.Windows.Data;
using EconSimVisual.Extensions;

namespace EconSimVisual
{
    public class DoubleToMoneyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double?) value)?.FormatMoney();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
