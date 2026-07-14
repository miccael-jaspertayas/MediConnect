namespace MediConnect.Api.Dtos
{
    public class TriageResponse
    {
        public string Tier { get; set; } = string.Empty;
        public int Score { get; set; }
        public string Explanation { get; set; } = string.Empty;
    }
}
