
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediConnect.Api.Models
{
    public class Vitals
    {
        [Key]
        public int VitalID { get; set; }

        [Required]
        public int PatientID { get; set; }

        // Links this data back to Jasper's Patient model
        [ForeignKey("PatientID")]
        public Patient Patient { get; set; }

        [Required]
        public DateTime RecordedAt { get; set; }

        [Required]
        public double Temperature { get; set; }

        [Required]
        public int SystolicBP { get; set; }

        [Required]
        public int DiastolicBP { get; set; }

        [Required]
        public int HeartRate { get; set; }

        [Required]
        public double Weight { get; set; }
    }
}