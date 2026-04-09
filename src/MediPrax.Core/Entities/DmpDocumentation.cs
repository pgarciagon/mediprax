namespace MediPrax.Core.Entities;

public class DmpDocumentation : BaseEntity
{
    public Guid DmpEnrollmentId { get; set; }
    public DmpEnrollment DmpEnrollment { get; set; } = null!;

    public DateOnly DocumentationDate { get; set; }
    public required string Quarter { get; set; }
    public Dictionary<string, string> FormData { get; set; } = [];
    public bool Submitted { get; set; }
    public DateTime? SubmittedAt { get; set; }
}
