namespace MediPrax.Application.DTOs;

public class MonitoringAlertDto
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string MedicationName { get; set; } = string.Empty;
    public string MonitoringType { get; set; } = string.Empty;
    public string Parameter { get; set; } = string.Empty;
    public int IntervalDays { get; set; }
    public DateOnly? LastCheckDate { get; set; }
    public bool IsOverdue { get; set; }
    public string Message { get; set; } = string.Empty;
}
