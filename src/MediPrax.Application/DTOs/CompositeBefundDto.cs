namespace MediPrax.Application.DTOs;

/// <summary>
/// Composes the full Befund from three sources:
/// 1. Psychopathological finding (AMDP) narrative
/// 2. Neurological examination narrative
/// 3. Free-text Befund section (ABDTP)
/// Composition at read-time — no data duplication.
/// </summary>
public class CompositeBefundDto
{
    public string? PsychopathNarrative { get; set; }
    public Guid? PsychopathFindingId { get; set; }
    public DateOnly? PsychopathAssessmentDate { get; set; }

    public string? NeuroNarrative { get; set; }
    public Guid? NeuroExamId { get; set; }
    public DateOnly? NeuroExamDate { get; set; }

    public string? FreitextBefund { get; set; }

    /// <summary>
    /// Full composed text: AMDP + Neuro + Freitext, separated by double newlines.
    /// Used for Arztbrief export and combined display.
    /// </summary>
    public string ComposedText
    {
        get
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(PsychopathNarrative))
                parts.Add($"Psychopathologischer Befund: {PsychopathNarrative}");

            if (!string.IsNullOrWhiteSpace(NeuroNarrative))
                parts.Add($"Neurologischer Befund: {NeuroNarrative}");

            if (!string.IsNullOrWhiteSpace(FreitextBefund))
                parts.Add(FreitextBefund);

            return string.Join("\n\n", parts);
        }
    }

    /// <summary>True if either structured finding (AMDP or Neuro) is present.</summary>
    public bool HasStructuredFindings =>
        PsychopathFindingId.HasValue || NeuroExamId.HasValue;
}
