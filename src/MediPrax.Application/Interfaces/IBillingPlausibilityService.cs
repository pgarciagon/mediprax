namespace MediPrax.Application.Interfaces;

public enum PlausibilitySeverity { Error, Warning, Info }

public class PlausibilityWarning
{
    public PlausibilitySeverity Severity { get; init; }
    public string CheckType { get; init; } = string.Empty;
    public string GopCode { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string? PatientName { get; init; }
    public Guid? EncounterId { get; init; }
}

public class QuarterValidationResult
{
    public string Quarter { get; init; } = string.Empty;
    public int TotalPatients { get; init; }
    public int ErrorCount { get; init; }
    public int WarningCount { get; init; }
    public IReadOnlyList<PlausibilityWarning> Warnings { get; init; } = [];
}

public interface IBillingPlausibilityService
{
    /// <summary>
    /// Checks plausibility of all billing items in a single encounter.
    /// </summary>
    Task<IReadOnlyList<PlausibilityWarning>> CheckEncounterAsync(Guid encounterId, CancellationToken ct = default);

    /// <summary>
    /// Validates all billing items in the given quarter across all patients.
    /// </summary>
    Task<QuarterValidationResult> ValidateQuarterAsync(string quarter, CancellationToken ct = default);
}
