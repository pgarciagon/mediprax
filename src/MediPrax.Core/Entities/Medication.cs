using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

/// <summary>
/// Active medication plan entry for a patient.
/// Tracks current medications with dosage schedule.
/// Supports psychiatric-specific features: depot injections, monitoring, titration.
/// </summary>
public class Medication : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid PrescribedById { get; set; }
    public User PrescribedBy { get; set; } = null!;

    public required string Name { get; set; }
    public string? Wirkstoff { get; set; }
    public string? Pzn { get; set; }
    public string? Staerke { get; set; } // z.B. "50mg"
    public string? Dosierung { get; set; } // z.B. "1-0-0-0"
    public string? Einheit { get; set; } // z.B. "Tablette", "Tropfen"
    public string? Hinweis { get; set; } // z.B. "zur Mahlzeit"
    public DateOnly SeitDatum { get; set; }
    public DateOnly? BisDatum { get; set; }
    public bool IsBtm { get; set; }
    public bool IsActive { get; set; } = true;

    // Psychiatric medication management
    public MedicationCategory? Category { get; set; }
    public string? TargetDose { get; set; }
    public string? TitrationSchedule { get; set; }

    // Depot injection tracking
    public bool IsDepot { get; set; }
    public int? DepotIntervalDays { get; set; }
    public DateOnly? LastDepotDate { get; set; }
    public DateOnly? NextDepotDate => LastDepotDate.HasValue && DepotIntervalDays.HasValue
        ? LastDepotDate.Value.AddDays(DepotIntervalDays.Value)
        : null;

    // Monitoring requirements
    public bool RequiresMonitoring { get; set; }
    public string? MonitoringType { get; set; } // "Lithium", "Clozapin", "Valproat", etc.
}
