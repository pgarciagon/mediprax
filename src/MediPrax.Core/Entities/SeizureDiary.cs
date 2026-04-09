namespace MediPrax.Core.Entities;

public class SeizureDiary : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public DateTime SeizureDate { get; set; }
    public string SeizureType { get; set; } = string.Empty; // Fokal / Generalisiert / Unklassifiziert
    public int? DurationSeconds { get; set; }
    public string? Trigger { get; set; }
    public bool AuraPresent { get; set; }
    public string? AuraDescription { get; set; }
    public bool ConsciousnessImpaired { get; set; }
    public string? PostictalState { get; set; }
    public string? MedicationAtTime { get; set; }
    public string? Notes { get; set; }
}
