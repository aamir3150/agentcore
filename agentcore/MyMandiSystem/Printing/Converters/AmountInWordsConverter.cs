using System.Globalization;
using System.Windows.Data;

namespace MyMandiSystem.Printing.Converters;

/// <summary>
/// Converts a decimal amount to words in English.
/// Usage in XAML: Text="{Binding TotalAmount, Converter={StaticResource AmountInWords}}"
/// Output: "Rupees Twelve Thousand Three Hundred Forty-Five Only"
/// </summary>
[ValueConversion(typeof(decimal), typeof(string))]
public sealed class AmountInWordsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not decimal amount) return "";
        if (amount == 0) return "Rupees Zero Only";

        long intPart = (long)Math.Abs(Math.Truncate(amount));
        string words = NumberToWords(intPart);
        string prefix = amount < 0 ? "Minus " : "";
        return $"{prefix}Rupees {words} Only";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();

    #region Number → English Words

    static readonly string[] Ones =
    {
        "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
        "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
        "Seventeen", "Eighteen", "Nineteen"
    };

    static readonly string[] Tens =
    {
        "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
    };

    static string NumberToWords(long n)
    {
        if (n == 0) return "Zero";
        if (n < 0) return "Minus " + NumberToWords(-n);

        string result = "";

        // Pakistani numbering: Crore, Lakh, Thousand, Hundred
        if (n / 10000000 > 0)
        {
            result += NumberToWords(n / 10000000) + " Crore ";
            n %= 10000000;
        }
        if (n / 100000 > 0)
        {
            result += NumberToWords(n / 100000) + " Lakh ";
            n %= 100000;
        }
        if (n / 1000 > 0)
        {
            result += NumberToWords(n / 1000) + " Thousand ";
            n %= 1000;
        }
        if (n / 100 > 0)
        {
            result += NumberToWords(n / 100) + " Hundred ";
            n %= 100;
        }
        if (n > 0)
        {
            if (result != "") result += "and ";
            if (n < 20)
                result += Ones[n];
            else
            {
                result += Tens[n / 10];
                if (n % 10 > 0)
                    result += "-" + Ones[n % 10];
            }
        }

        return result.Trim();
    }

    #endregion
}
