using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

public class Prescription : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid DoctorId { get; set; }
    public User Doctor { get; set; } = null!;

    public Guid? EncounterId { get; set; }
    public Encounter? Encounter { get; set; }

    public required string MedicationName { get; set; }
    public string? MedicationPzn { get; set; } // Pharmazentralnummer
    public string? Dosage { get; set; }
    public bool IsBtm { get; set; } // Betäubungsmittel
    public string? ERezeptId { get; set; }
    public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Draft;
}
