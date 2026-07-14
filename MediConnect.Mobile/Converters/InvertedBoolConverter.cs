using System.Globalization;

namespace MediConnect.Mobile.Converters
{
    // Flips a bool. Used mainly for Login button so it disables itself while a command is running.
    public class InvertedBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool b && !b;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool b && !b;
        }
    }
}