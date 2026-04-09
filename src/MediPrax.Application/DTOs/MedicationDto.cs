using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class MedicationDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Wirkstoff { get; set; }
    public string? Pzn { get; set; }
    public string? Staerke { get; set; }
    public string? Dosierung { get; set; }
    public string? Einheit { get; set; }
    public string? Hinweis { get; set; }
    public DateOnly SeitDatum { get; set; }
    public DateOnly? BisDatum { get; set; }
    public bool IsBtm { get; set; }
    public bool IsActive { get; set; }
    public string PrescribedByName { get; set; } = string.Empty;

    // Psychiatric medication management
    public MedicationCategory? Category { get; set; }
    public string? TargetDose { get; set; }
    public bool IsDepot { get; set; }
    public int? DepotIntervalDays { get; set; }
    public DateOnly? LastDepotDate { get; set; }
    public DateOnly? NextDepotDate { get; set; }
    public bool RequiresMonitoring { get; set; }
    public string? MonitoringType { get; set; }
}

public class CreateMedicationDto
{
    public Guid PatientId { get; set; }
    public Guid PrescribedById { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Wirkstoff { get; set; }
    public string? Pzn { get; set; }
    public string? Staerke { get; set; }
    public string? Dosierung { get; set; }
    public string? Einheit { get; set; }
    public string? Hinweis { get; set; }
    public DateOnly SeitDatum { get; set; }
    public DateOnly? BisDatum { get; set; }
    public bool IsBtm { get; set; }

    // Psychiatric medication management
    public MedicationCategory? Category { get; set; }
    public string? TargetDose { get; set; }
    public bool IsDepot { get; set; }
    public int? DepotIntervalDays { get; set; }
    public DateOnly? LastDepotDate { get; set; }
    public bool RequiresMonitoring { get; set; }
    public string? MonitoringType { get; set; }
}
