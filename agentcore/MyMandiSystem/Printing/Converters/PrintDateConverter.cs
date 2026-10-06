using System.Globalization;
using System.Windows.Data;

namespace MyMandiSystem.Printing.Converters;

/// <summary>
/// Ensures a consistent date format across all printed documents.
/// Uses explicit culture so output is identical on every PC regardless of regional settings.
/// Output format: 06-Oct-2026
/// </summary>
[ValueConversion(typeof(DateTime), typeof(string))]
public sealed class PrintDateConverter : IValueConverter
{
    private static readonly CultureInfo PrintCulture = CultureInfo.InvariantCulture;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime dt)
            return dt.ToString("dd-MMM-yyyy", PrintCulture);
        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
