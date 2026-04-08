using MediPrax.Application.Catalogs;
using MediPrax.Core.ValueObjects;

namespace MediPrax.Application.Services;

public static class PsychopathFindingTextGenerator
{
    private static readonly Dictionary<string, string> CategoryNormalText = new()
    {
        ["BEW"] = "Bewusstsein klar",
        ["ORI"] = "allseits orientiert",
        ["AUF"] = "Auffassung und Konzentration ungestört, Gedächtnis intakt",
        ["FDS"] = "Formales Denken geordnet",
        ["INH"] = "Kein Wahn",
        ["SIN"] = "keine Sinnestäuschungen",
        ["ICH"] = "keine Ich-Störungen",
        ["AFF"] = "Stimmung ausgeglichen, Affekt moduliert",
        ["ANT"] = "Antrieb und Psychomotorik unauffällig",
        ["CIR"] = "kein Morgentief, kein Abendtief",
        ["SUI"] = "keine Suizidalität",
        ["SOM"] = "keine somatischen Auffälligkeiten",
    };

    private static readonly Dictionary<int, string> SeverityLabels = new()
    {
        [1] = "leichte",
        [2] = "deutliche",
        [3] = "schwere",
    };

    public static string Generate(List<SymptomFinding> findings)
    {
        if (findings is null || findings.Count == 0)
            return "Keine Befunde erhoben.";

        var grouped = findings
            .GroupBy(f => f.CategoryCode)
            .OrderBy(g => GetCategoryOrder(g.Key))
            .ToList();

        var parts = new List<string>();

        // Ensure all categories are represented
        foreach (var category in AmdpCatalog.Categories)
        {
            var group = grouped.FirstOrDefault(g => g.Key == category.Code);
            if (group is null)
            {
                // Category not in findings at all — treat as normal
                if (CategoryNormalText.TryGetValue(category.Code, out var normalText))
                    parts.Add(normalText);
                continue;
            }

            var positiveFindings = group.Where(f => f.Severity > 0).ToList();

            if (positiveFindings.Count == 0)
            {
                // All symptoms in this category are 0 — use normal text
                if (CategoryNormalText.TryGetValue(category.Code, out var normalText))
                    parts.Add(normalText);
            }
            else
            {
                // Generate text for positive findings
                var symptomTexts = positiveFindings
                    .Select(f => FormatSymptom(f))
                    .Where(t => !string.IsNullOrEmpty(t))
                    .ToList();

                if (symptomTexts.Count > 0)
                    parts.Add(string.Join(", ", symptomTexts));
            }
        }

        if (parts.Count == 0)
            return "Psychopathologischer Befund unauffällig.";

        // Join with periods for major sections, commas within
        return FormatNarrative(parts);
    }

    private static string FormatSymptom(SymptomFinding finding)
    {
        var catalog = AmdpCatalog.AllSymptoms
            .FirstOrDefault(s => s.SymptomCode == finding.SymptomCode);

        if (catalog is null)
            return finding.SymptomName;

        var template = catalog.NarrativeTemplate;

        if (template.Contains("{Severity}"))
        {
            if (SeverityLabels.TryGetValue(finding.Severity, out var label))
                return template.Replace("{Severity}", label);
            return template.Replace("{Severity} ", "");
        }

        // For templates without severity placeholder, return as-is
        return template;
    }

    private static string FormatNarrative(List<string> parts)
    {
        if (parts.Count == 0) return string.Empty;

        // Group related parts with commas, separate major sections with periods
        var sentences = new List<string>();
        var currentSentence = new List<string>();

        for (int i = 0; i < parts.Count; i++)
        {
            currentSentence.Add(parts[i]);

            // Start a new sentence at certain logical breaks
            bool isBreakPoint = i == parts.Count - 1
                || IsNewSection(parts[i], i < parts.Count - 1 ? parts[i + 1] : null);

            if (isBreakPoint && currentSentence.Count > 0)
            {
                var sentence = string.Join(", ", currentSentence);
                // Capitalize first letter
                if (sentence.Length > 0)
                    sentence = char.ToUpper(sentence[0]) + sentence[1..];
                sentences.Add(sentence);
                currentSentence = [];
            }
        }

        return string.Join(". ", sentences) + ".";
    }

    private static bool IsNewSection(string current, string? next)
    {
        if (next is null) return true;

        // Break after consciousness/orientation block
        if (current.Contains("orientiert") || current.Contains("desorientiert")) return true;
        // Break after cognitive block
        if (current.Contains("Gedächtnis") || current.Contains("Konzentration")) return true;
        // Break after thought content
        if (current.Contains("Wahn") || current.Contains("Sinnestäuschung")) return true;
        // Break before affect section
        if (next.Contains("Stimmung") || next.Contains("deprimiert") || next.Contains("ängstlich")) return true;
        // Break before drive/suicidality
        if (next.Contains("Antrieb") || next.Contains("antrieb") || next.Contains("Suizid") || next.Contains("Lebensmüdigkeit")) return true;
        // Break before somatic
        if (next.Contains("Schlaf") || next.Contains("Appetit") || next.Contains("somatisch")) return true;

        return false;
    }

    private static int GetCategoryOrder(string code) => code switch
    {
        "BEW" => 0,
        "ORI" => 1,
        "AUF" => 2,
        "FDS" => 3,
        "INH" => 4,
        "SIN" => 5,
        "ICH" => 6,
        "AFF" => 7,
        "ANT" => 8,
        "CIR" => 9,
        "SUI" => 10,
        "SOM" => 11,
        _ => 99,
    };
}
