using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MyMandiSystem.Converters;

public class DifferenceToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal diff)
        {
            return diff == 0 ? System.Windows.Media.Brushes.DarkGreen : System.Windows.Media.Brushes.Red;
        }
        return System.Windows.Media.Brushes.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
