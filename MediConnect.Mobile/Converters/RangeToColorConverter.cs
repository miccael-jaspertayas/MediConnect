using System.Globalization;

namespace MediConnect.Mobile.Converters
{
    public class RangeToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var selected = value is int i && parameter is string p && int.Parse(p) == i;
            return selected ? Color.FromArgb("#1C6F6F") : Color.FromArgb("#F4F6FA");
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class RangeToTextColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var selected = value is int i && parameter is string p && int.Parse(p) == i;
            return selected ? Colors.White : Color.FromArgb("#1C6F6F");
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}