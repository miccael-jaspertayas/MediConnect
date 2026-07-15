using System.Globalization;

namespace MediConnect.Mobile.Converters
{
    // true (hidden) -> closed eye emoji, false (visible) -> open eye emoji
    public class BoolToEyeIconConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var isHidden = value is bool b && b;
            return isHidden ? "👁‍🗨" : "👁";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
