namespace MediConnect.Mobile.Models
{
    public class VitalInsight
    {
        public string Label { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public Color BadgeColor { get; set; } = Colors.Gray;
        public Color TextColor { get; set; } = Colors.Black;
    }
}