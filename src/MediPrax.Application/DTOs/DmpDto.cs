using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class DmpEnrollmentDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string DmpType { get; set; } = string.Empty;
    public DateOnly EnrollmentDate { get; set; }
    public DateOnly? DisenrollmentDate { get; set; }
    public DmpStatus Status { get; set; }

    public string StatusDisplay => Status switch
    {
        DmpStatus.Active => "Aktiv",
        DmpStatus.Paused => "Pausiert",
        DmpStatus.Disenrolled => "Ausgeschrieben",
        _ => Status.ToString()
    };
}

public class DmpDocumentationDto
{
    public Guid Id { get; set; }
    public Guid DmpEnrollmentId { get; set; }
    public DateOnly DocumentationDate { get; set; }
    public string Quarter { get; set; } = string.Empty;
    public Dictionary<string, string> FormData { get; set; } = [];
    public bool Submitted { get; set; }
    public DateTime? SubmittedAt { get; set; }
}

public class CreateDmpEnrollmentDto
{
    public Guid PatientId { get; set; }
    public string DmpType { get; set; } = string.Empty;
    public DateOnly EnrollmentDate { get; set; }
}

public class CreateDmpDocumentationDto
{
    public Guid DmpEnrollmentId { get; set; }
    public DateOnly DocumentationDate { get; set; }
    public string Quarter { get; set; } = string.Empty;
    public Dictionary<string, string> FormData { get; set; } = [];
}
