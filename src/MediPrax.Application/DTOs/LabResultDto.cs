using MediPrax.Core.Enums;
using MediPrax.Core.ValueObjects;

namespace MediPrax.Application.DTOs;

public class LabResultDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string? OrderedByName { get; set; }
    public string LabName { get; set; } = string.Empty;
    public DateOnly OrderDate { get; set; }
    public DateOnly? ResultDate { get; set; }
    public LabResultStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public bool IsReviewed { get; set; }
    public string? ReviewedByName { get; set; }
    public List<LabValue> Values { get; set; } = [];
    public string? Notes { get; set; }
    public string? ImportSource { get; set; }
    public bool HasAbnormalValues => Values.Any(v => v.IsAbnormal);
    public bool HasCriticalValues => Values.Any(v => v.IsCritical);
}

public class LabResultListItemDto
{
    public Guid Id { get; set; }
    public DateOnly OrderDate { get; set; }
    public DateOnly? ResultDate { get; set; }
    public string LabName { get; set; } = string.Empty;
    public LabResultStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public int ValueCount { get; set; }
    public bool HasAbnormalValues { get; set; }
    public bool HasCriticalValues { get; set; }
}

public class CreateLabResultDto
{
    public Guid PatientId { get; set; }
    public Guid? OrderedById { get; set; }
    public string LabName { get; set; } = string.Empty;
    public DateOnly OrderDate { get; set; }
    public string? Notes { get; set; }
}

public class AddLabValuesDto
{
    public Guid LabResultId { get; set; }
    public DateOnly ResultDate { get; set; }
    public List<LabValue> Values { get; set; } = [];
}
