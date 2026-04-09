using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class CreateArztbriefDto
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? EncounterId { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public string? RecipientAddress { get; set; }
    public string Subject { get; set; } = "Arztbrief";
    public string Body { get; set; } = string.Empty;
    public List<string> Diagnoses { get; set; } = [];
}

public class ArztbriefDetailDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? EncounterId { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public string? RecipientAddress { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public List<string> Diagnoses { get; set; } = [];
    public ArztbriefStatus Status { get; set; }
    public bool HasPdf { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ArztbriefListItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? RecipientName { get; set; }
    public ArztbriefStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool HasPdf { get; set; }

    public string StatusDisplay => Status switch
    {
        ArztbriefStatus.Entwurf => "Entwurf",
        ArztbriefStatus.Freigegeben => "Freigegeben",
        ArztbriefStatus.Versendet => "Versendet",
        _ => Status.ToString()
    };

    public string StatusColor => Status switch
    {
        ArztbriefStatus.Entwurf => "warning",
        ArztbriefStatus.Freigegeben => "success",
        ArztbriefStatus.Versendet => "info",
        _ => "secondary"
    };
}
