using System;
using System.Collections.Generic;
using System.Text;

namespace MediConnect.Mobile.Models
{
    public class MedicalRecord
    {
        public int RecordID { get; set; }
        public int PatientID { get; set; }

        public DateTime VisitDate { get; set; }

        public string HospitalName { get; set; } = string.Empty;

        public string DoctorName { get; set; } = string.Empty;

        public string Diagnosis { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;
    }
}
