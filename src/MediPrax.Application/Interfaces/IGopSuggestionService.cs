using MediPrax.Core.Enums;

namespace MediPrax.Application.Interfaces;

public class GopSuggestion
{
    public string GopCode { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
    public BillingType BillingType { get; init; }
}

public interface IGopSuggestionService
{
    /// <summary>
    /// Returns suggested GOP codes for the given encounter based on context (ICD codes, duration, prior billings).
    /// </summary>
    Task<IReadOnlyList<GopSuggestion>> GetSuggestionsAsync(Guid encounterId, CancellationToken ct = default);
}
