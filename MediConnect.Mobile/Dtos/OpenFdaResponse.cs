using System.Text.Json.Serialization;

namespace MediConnect.Mobile.Dtos
{
    public class OpenFdaResponse
    {
        [JsonPropertyName("results")]
        public List<OpenFdaResult>? Results { get; set; }
    }

    public class OpenFdaResult
    {
        [JsonPropertyName("indications_and_usage")]
        public List<string>? IndicationsAndUsage { get; set; }

        [JsonPropertyName("purpose")]
        public List<string>? Purpose { get; set; }

        [JsonPropertyName("warnings")]
        public List<string>? Warnings { get; set; }

        [JsonPropertyName("dosage_and_administration")]
        public List<string>? DosageAndAdministration { get; set; }

        [JsonPropertyName("openfda")]
        public OpenFdaMeta? OpenFda { get; set; }
    }

    public class OpenFdaMeta
    {
        [JsonPropertyName("brand_name")]
        public List<string>? BrandName { get; set; }

        [JsonPropertyName("generic_name")]
        public List<string>? GenericName { get; set; }

        [JsonPropertyName("manufacturer_name")]
        public List<string>? ManufacturerName { get; set; }

        [JsonPropertyName("route")]
        public List<string>? Route { get; set; }
    }
}
