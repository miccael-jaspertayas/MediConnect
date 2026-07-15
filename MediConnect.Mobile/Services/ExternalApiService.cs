using MediConnect.Mobile.Dtos;
using MediConnect.Mobile.Models;
using System.Net.Http.Json;
using System.Web;

namespace MediConnect.Mobile.Services
{
    public class ExternalApiService
    {
        private readonly HttpClient _http = new();

        public async Task<MedicationLookupResult> LookupMedicationAsync(string drugName)
        {
            if (string.IsNullOrWhiteSpace(drugName))
                return MedicationLookupResult.NotFound();

            var term = drugName.Trim();

            // Try an exact brand-name match first
            var exact = await QueryAsync($"openfda.brand_name.exact:\"{Escape(term)}\"");
            if (exact is not null) return exact;

            // Fall back to a looser OR search across brand + generic name,
            // in case the patient typed the generic name or the exact brand string doesn't match OpenFDA's casing/format.
            var loose = await QueryAsync(
                $"openfda.brand_name:\"{Escape(term)}\"+openfda.generic_name:\"{Escape(term)}\"");
            if (loose is not null) return loose;

            return MedicationLookupResult.NotFound();
        }

        private async Task<MedicationLookupResult?> QueryAsync(string searchExpression)
        {
            try
            {
                var encoded = HttpUtility.UrlEncode(searchExpression);
                var url = $"https://api.fda.gov/drug/label.json?search={encoded}&limit=1";

                var response = await _http.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;

                var data = await response.Content.ReadFromJsonAsync<OpenFdaResponse>();
                var result = data?.Results?.FirstOrDefault();
                if (result is null) return null;

                var usage = result.IndicationsAndUsage?.FirstOrDefault()
                            ?? result.Purpose?.FirstOrDefault();

                // If there's no usage/purpose text at all, treat it as not
                // useful to the patient even though OpenFDA returned a record.
                if (string.IsNullOrWhiteSpace(usage)) return null;

                return new MedicationLookupResult
                {
                    Found = true,
                    BrandName = result.OpenFda?.BrandName?.FirstOrDefault() ?? "Unknown",
                    GenericName = result.OpenFda?.GenericName?.FirstOrDefault() ?? "Unknown",
                    Manufacturer = result.OpenFda?.ManufacturerName?.FirstOrDefault() ?? "Unknown",
                    Route = result.OpenFda?.Route?.FirstOrDefault() ?? "Unknown",
                    Usage = usage,
                    Warnings = result.Warnings?.FirstOrDefault() ?? string.Empty,
                    Dosage = result.DosageAndAdministration?.FirstOrDefault() ?? string.Empty
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string Escape(string input) => input.Replace("\"", "");

        public async Task<List<string>> SearchSymptomsAsync(string query)
        {
            try
            {
                var url = $"https://clinicaltables.nlm.nih.gov/api/conditions/v3/search?terms={Uri.EscapeDataString(query)}";
                var raw = await _http.GetStringAsync(url);
                var parsed = System.Text.Json.JsonDocument.Parse(raw);
                var rootArray = parsed.RootElement.EnumerateArray().ToList();

                var names = new List<string>();

                if (rootArray.Count > 3 && rootArray[3].ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    foreach (var item in rootArray[3].EnumerateArray())
                    {
                        // Default df=consumer_name returns each item as a single-element array, e.g. ["Fever"]
                        var inner = item.EnumerateArray().ToList();
                        if (inner.Count > 0 && !string.IsNullOrWhiteSpace(inner[0].GetString()))
                            names.Add(inner[0].GetString()!);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[Triage] NIH returned {names.Count} results for '{query}'");
                return names.Where(n => !string.IsNullOrWhiteSpace(n)).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Triage] NIH call failed: {ex.Message} — using fallback");
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