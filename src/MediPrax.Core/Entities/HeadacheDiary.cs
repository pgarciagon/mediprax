namespace MediPrax.Core.Entities;

public class HeadacheDiary : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public DateOnly Date { get; set; }
    public TimeOnly? StartTime { get; set; }
    public decimal? DurationHours { get; set; }
    public int Intensity { get; set; } // VAS 0–10
    public string Type { get; set; } = string.Empty; // Migräne / Spannungskopfschmerz / Cluster / Andere
    public string? Location { get; set; }
    public bool AuraPresent { get; set; }
    public bool Nausea { get; set; }
    public bool Photophobia { get; set; }
    public bool Phonophobia { get; set; }
    public List<string>? Triggers { get; set; }
    public string? MedicationTaken { get; set; }
    public bool? MedicationEffective { get; set; }
}
