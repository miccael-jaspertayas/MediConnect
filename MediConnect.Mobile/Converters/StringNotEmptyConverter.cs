using System.Globalization;

namespace MediConnect.Mobile.Converters
{
    // Used to show/hide an element based on whether a bound string has content.
    public class StringNotEmptyConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !string.IsNullOrWhiteSpace(value as string);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException("StringNotEmptyConverter is one-way only.");
        }
    }
}