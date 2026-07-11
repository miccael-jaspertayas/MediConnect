using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediConnect.Api.Models
{
    public class Patient
    {
        [Key]
        public int PatientID { get; private set; }

        [Required]
        public int UserID { get; private set; }

        [ForeignKey(nameof(UserID))]
        public User? User { get; private set; }

        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public DateTime? DOB { get; set; }

        [MaxLength(10)]
        public string? BloodType { get; set; }

        public string? Allergies { get; set; }

        public string? Medications { get; set; }

        [MaxLength(150)]
        public string? EmergencyContactName { get; set; }

        [MaxLength(30)]
        public string? EmergencyContactPhone { get; set; }

        private Patient() { }

        public Patient(int userId)
        {
            UserID = userId;
            Name = string.Empty;
        }
    }
}
