namespace MediConnect.Api.Dtos
{
    public class TriageRequest
    {
        public int PatientID { get; set; }
        public List<string> Symptoms { get; set; } = new();
    }
}