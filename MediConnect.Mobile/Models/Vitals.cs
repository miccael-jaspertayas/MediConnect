using System;

namespace MediConnect.Mobile.Models
{
    public class VitalsModel
    {
        public int VitalID { get; set; }
        public int PatientID { get; set; }
        public DateTime RecordedAt { get; set; }
        public double Temperature { get; set; }
        public int SystolicBP { get; set; }
        public int DiastolicBP { get; set; }
        public int HeartRate { get; set; }
        public double Weight { get; set; }
    }
}