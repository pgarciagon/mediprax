using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

public class Encounter : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid DoctorId { get; set; }
    public User Doctor { get; set; } = null!;

    public Guid? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    public required DateOnly EncounterDate { get; set; }
    public string? Notes { get; set; }
    public List<string> Icd10Codes { get; set; } = [];
    public int? DurationMinutes { get; set; } // Pflichtdokumentation für Gespräche
    public EncounterStatus Status { get; set; } = EncounterStatus.Open;

    public ICollection<Prescription> Prescriptions { get; set; } = [];
    public ICollection<Document> Documents { get; set; } = [];
    public ICollection<BillingItem> BillingItems { get; set; } = [];
}
