using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class EauDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateOnly Von { get; set; }
    public DateOnly Bis { get; set; }
    public string Diagnose { get; set; } = string.Empty;
    public string? IcdCode { get; set; }
    public bool IsErstbescheinigung { get; set; } = true;
    public EauStatus Status { get; set; }
    public string? TransmissionId { get; set; }
    public DateTime? TransmittedAt { get; set; }
    public string? ErrorMessage { get; set; }

    public string StatusDisplay => Status switch
    {
        EauStatus.Draft => "Entwurf",
        EauStatus.Transmitted => "Übermittelt",
        EauStatus.Confirmed => "Bestätigt",
        EauStatus.Cancelled => "Storniert",
        _ => Status.ToString()
    };
}

public class CreateEauDto
{
    public Guid PatientId { get; set; }
    public DateOnly Von { get; set; }
    public DateOnly Bis { get; set; }
    public string Diagnose { get; set; } = string.Empty;
    public string? IcdCode { get; set; }
    public bool IsErstbescheinigung { get; set; } = true;
}
