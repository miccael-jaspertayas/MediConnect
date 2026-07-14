using System.Xml.Linq;
using MediConnect.Api.Dtos;

namespace MediConnect.Api.Services
{
    public class TriageService
    {
        private readonly List<(string Keyword, int Weight, string Tier)> _rules;

        public TriageService()
        {
            _rules = new List<(string, int, string)>();

            var path = Path.Combine(AppContext.BaseDirectory, "Config", "triage-rules.xml");
            var doc = XDocument.Load(path);

            foreach (var rule in doc.Descendants("Rule"))
            {
                var keyword = rule.Attribute("Keyword")?.Value ?? "";
                var weight = int.Parse(rule.Attribute("Weight")?.Value ?? "0");
                var tier = rule.Attribute("Tier")?.Value ?? "";
                _rules.Add((keyword.ToLower(), weight, tier));
            }
        }

        public TriageResponse Assess(List<string> symptoms)
        {
            int totalScore = 0;
            var matchedTiers = new List<string>();

            foreach (var symptom in symptoms)
            {
                var lowerSymptom = symptom.ToLower();
                var match = _rules.FirstOrDefault(r => lowerSymptom.Contains(r.Keyword));
                if (match.Keyword != null)
                {
                    totalScore += match.Weight;
                    matchedTiers.Add(match.Tier);
                }
            }

            string finalTier = totalScore switch
            {
                >= 8 => "Emergency",
                >= 4 => "Hospital",
                >= 1 => "LocalHealthUnit",
                _ => "NoMatch"
            };

            string explanation = finalTier == "NoMatch"
                ? "No matching symptoms found. Please consult a healthcare provider if symptoms persist."
                : $"Based on {matchedTiers.Count} matched symptom(s) with a total severity score of {totalScore}.";

            return new TriageResponse
            {
                Tier = finalTier,
                Score = totalScore,
                Explanation = explanation
            };
        }
    }
}