using System.Globalization;

namespace MediConnect.Mobile.Converters
{
    // Converts a UTC DateTime into the device's local time before formatting for display.
    public class UtcToLocalTimeConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not DateTime dt) return string.Empty;

            var utc = dt.Kind == DateTimeKind.Utc ? dt : DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            var local = utc.ToLocalTime();

            var format = parameter as string ?? "MMM d, yyyy - h:mm tt";
            return local.ToString(format, culture);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}