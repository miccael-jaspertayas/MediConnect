using System;
using System.Collections.Generic;
using System.Text;

namespace MediConnect.Mobile.Models
{
    public class MedicationLookupResult
    {
        public bool Found { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string GenericName { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string Usage { get; set; } = string.Empty;
        public string Warnings { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;

        public List<string> UsageLines => ToBulletLines(Usage);
        public List<string> WarningsLines => ToBulletLines(Warnings);
        public List<string> DosageLines => ToBulletLines(Dosage);

        public static MedicationLookupResult NotFound() => new() { Found = false };

        // OpenFDA text often uses ". " or ";" as pseudo-bullet separators,
        // or occasionally real bullet/number prefixes. Split conservatively.
        public static List<string> ToBulletLines(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return new List<string>();

            var cleaned = text.Trim();

            // If it already contains bullet characters, split on those first.
            var parts = cleaned
                .Split(new[] { "•", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToList();

            // Fallback: if no real bullets found and the text is long,
            // split on ". " into sentence-level bullets for readability.
            if (parts.Count <= 1 && cleaned.Length > 140)
            {
                parts = cleaned
                    .Split(new[] { ". " }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim().TrimEnd('.') + ".")
                    .Where(p => p.Length > 3)
                    .ToList();
            }

            return parts.Count > 0 ? parts : new List<string> { cleaned };
        }
    }
}
