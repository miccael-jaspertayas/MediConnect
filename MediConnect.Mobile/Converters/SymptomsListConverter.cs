using System.Globalization;
using Microsoft.Maui.Controls;

namespace MediConnect.Mobile.Converters
{
    public class SymptomsListConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is List<string> list)
                return string.Join(", ", list);
            return string.Empty;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}