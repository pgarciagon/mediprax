namespace MediPrax.Core.Entities;

public class MsDocumentation : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public DateOnly DocumentationDate { get; set; }
    public decimal EdssScore { get; set; } // EDSS 0.0–10.0
    public bool IsRelapse { get; set; }
    public string? RelapseDescription { get; set; }
    public DateOnly? MriDate { get; set; }
    public string? MriFindings { get; set; }
    public int? NewLesions { get; set; }
    public int? GadEnhancing { get; set; }
    public string? CurrentDmt { get; set; }
    public string? Notes { get; set; }
}
