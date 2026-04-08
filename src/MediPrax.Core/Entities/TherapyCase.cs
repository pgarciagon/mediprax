using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

public class TherapyCase : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid TherapistId { get; set; }
    public User Therapist { get; set; } = null!;

    public TherapyType TherapyType { get; set; }
    public TherapyCaseStatus Status { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateOnly? InsuranceApprovalDate { get; set; }
    public string? InsuranceApprovalRef { get; set; }
    public int ApprovedSessions { get; set; }
    public int CompletedSessions { get; set; }
    public int SessionDurationMinutes { get; set; } // 25 or 50
    public bool IsGroupTherapy { get; set; }
    public GutachterStatus? GutachterStatus { get; set; }
    public List<string> Diagnoses { get; set; } = [];
    public string? Notes { get; set; }

    public ICollection<TherapySession> Sessions { get; set; } = [];
    public ICollection<PtvForm> PtvForms { get; set; } = [];
}
