using System.Globalization;

namespace MediConnect.Mobile.Converters
{
    public class TierToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (value as string) switch
            {
                "Emergency" => Colors.Red,
                "Hospital" => Colors.Orange,
                "LocalHealthUnit" => Colors.Green,
                _ => Colors.Gray
            };
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}