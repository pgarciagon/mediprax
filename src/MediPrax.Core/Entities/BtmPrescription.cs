namespace MediPrax.Core.Entities;

public class BtmPrescription : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid PrescribedById { get; set; }
    public User PrescribedBy { get; set; } = null!;

    public DateOnly PrescriptionDate { get; set; }
    public required string MedicationName { get; set; }
    public string? Pzn { get; set; }
    public required string Substance { get; set; }
    public string Amount { get; set; } = string.Empty;
    public string? Dosierung { get; set; }
    public required string BtmRecipeNumber { get; set; }
    public string? PrescriberBtmNumber { get; set; }
    public bool IsEBtm { get; set; }
    public string? Notes { get; set; }
}
