using System.Net.Http.Json;

namespace MediConnect.Mobile.Services
{
    public class ExternalApiService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<List<string>> SearchSymptomsAsync(string query)
        {
            try
            {
                var url = $"https://clinicaltables.nlm.nih.gov/api/icd10cm/v3/search?terms={Uri.EscapeDataString(query)}";
                var raw = await _httpClient.GetStringAsync(url);
                var parsed = System.Text.Json.JsonDocument.Parse(raw);
                var names = new List<string>();

                var displayArray = parsed.RootElement[3];
                foreach (var item in displayArray.EnumerateArray())
                {
                    names.Add(item[0].GetString() ?? "");
                }

                return names;
            }
            catch
            {
                return GetFallbackSymptoms(query);
            }
        }

        private List<string> GetFallbackSymptoms(string query)
        {
            var fallbackList = new List<string>
            {
                "Chest pain", "Cough", "Headache", "Vomiting", "Nausea",
                "Dizziness", "Shortness of breath (dyspnea)", "Sore throat",
                "Fatigue", "Abdominal pain", "Rash", "Fever", "Diarrhea",
                "Joint pain", "Chills", "Coughing up blood",
                "Whooping cough (pertussis)", "Headache - tension",
                "Headache - cluster", "Vomiting - persistent"
            };

            return fallbackList
                .Where(s => s.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}