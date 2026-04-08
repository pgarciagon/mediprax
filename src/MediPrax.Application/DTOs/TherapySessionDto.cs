using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class TherapySessionDto
{
    public Guid Id { get; set; }
    public Guid TherapyCaseId { get; set; }
    public int SessionNumber { get; set; }
    public DateOnly SessionDate { get; set; }
    public int DurationMinutes { get; set; }
    public SessionType SessionType { get; set; }
    public bool IsVideoSession { get; set; }
    public string? Notes { get; set; }
    public string? BilledGop { get; set; }
}

public class CreateTherapySessionDto
{
    public Guid TherapyCaseId { get; set; }
    public DateOnly SessionDate { get; set; }
    public int DurationMinutes { get; set; }
    public SessionType SessionType { get; set; }
    public bool IsVideoSession { get; set; }
    public string? Notes { get; set; }
    public string? BilledGop { get; set; }
    public Guid? EncounterId { get; set; }
    public Guid? AppointmentId { get; set; }
}
