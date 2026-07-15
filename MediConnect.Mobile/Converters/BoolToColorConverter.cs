using System.Globalization;

namespace MediConnect.Mobile.Converters
{
    // Usage: Converter={StaticResource BoolToColorConverter} ConverterParameter="SuccessColor|ErrorColor"
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var isTrue = value is bool b && b;
            var colors = (parameter as string)?.Split('|');
            if (colors is not { Length: 2 }) return Colors.Black;

            var colorHex = isTrue ? colors[0] : colors[1];
            return Color.FromArgb(colorHex);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
