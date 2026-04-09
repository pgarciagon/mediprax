namespace MediPrax.Application.DTOs;

public class SeizureDiaryDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateTime SeizureDate { get; set; }
    public string SeizureType { get; set; } = string.Empty;
    public int? DurationSeconds { get; set; }
    public string? Trigger { get; set; }
    public bool AuraPresent { get; set; }
    public string? AuraDescription { get; set; }
    public bool ConsciousnessImpaired { get; set; }
    public string? PostictalState { get; set; }
    public string? MedicationAtTime { get; set; }
    public string? Notes { get; set; }

    public string DurationDisplay => DurationSeconds.HasValue
        ? DurationSeconds.Value >= 60
            ? $"{DurationSeconds.Value / 60} Min. {DurationSeconds.Value % 60} Sek."
            : $"{DurationSeconds.Value} Sek."
        : "—";
}

public class CreateSeizureDiaryDto
{
    public Guid PatientId { get; set; }
    public DateTime SeizureDate { get; set; }
    public string SeizureType { get; set; } = string.Empty;
    public int? DurationSeconds { get; set; }
    public string? Trigger { get; set; }
    public bool AuraPresent { get; set; }
    public string? AuraDescription { get; set; }
    public bool ConsciousnessImpaired { get; set; }
    public string? PostictalState { get; set; }
    public string? MedicationAtTime { get; set; }
    public string? Notes { get; set; }
}
