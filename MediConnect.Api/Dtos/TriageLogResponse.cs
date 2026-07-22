namespace MediConnect.Api.Dtos
{
    public class TriageLogResponse
    {
        public int LogID { get; set; }
        public List<string> Symptoms { get; set; } = new();
        public string Tier { get; set; } = string.Empty;
        public int Score { get; set; }
        public string Explanation { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
