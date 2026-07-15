
using System;
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

        // Links this data back Patient model
        [ForeignKey("PatientID")]
        public Patient? Patient { get; set; }

        [Required]
        public DateTime RecordedAt { get; set; }

        
        public double? Temperature { get; set; }

        public int? SystolicBP { get; set; }

        public int? DiastolicBP { get; set; }

        public int? HeartRate { get; set; }

        public double? Weight { get; set; }
    }
}