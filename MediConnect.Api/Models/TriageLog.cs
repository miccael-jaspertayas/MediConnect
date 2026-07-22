using System.ComponentModel.DataAnnotations;

namespace MediConnect.Api.Models
{
    public class TriageLog
    {
        [Key]
        public int LogID { get; set; }

        public int PatientID { get; set; }
        public Patient? Patient { get; set; }

        public string Symptoms { get; set; } = string.Empty;
        public string Tier { get; set; } = string.Empty;
        public int Score { get; set; }
        public string Explanation { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}