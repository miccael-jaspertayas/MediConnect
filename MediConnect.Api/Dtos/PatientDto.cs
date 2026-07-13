namespace MediConnect.Api.Dtos
{
    public class PatientDto
    {
        public int PatientID { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? DOB { get; set; }
        public string? BloodType { get; set; }
        public string? Allergies { get; set; }
        public string? Medications { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
    }
}
