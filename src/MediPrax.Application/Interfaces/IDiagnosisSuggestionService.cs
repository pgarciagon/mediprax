using MediPrax.Application.Catalogs;

namespace MediPrax.Application.Interfaces;

/// <summary>
/// Service that provides ICD-10 diagnosis suggestions based on structured findings
/// (AMDP psychopathological findings, neurological examination, suicidality assessment)
/// linked to an encounter.
/// </summary>
public interface IDiagnosisSuggestionService
{
    /// <summary>
    /// Loads all structured findings for the given encounter and returns
    /// rule-based ICD-10 diagnosis suggestions sorted by confidence.
    /// </summary>
    Task<IReadOnlyList<DiagnosisSuggestion>> GetSuggestionsForEncounterAsync(
        Guid encounterId, Guid patientId, CancellationToken ct = default);
}
