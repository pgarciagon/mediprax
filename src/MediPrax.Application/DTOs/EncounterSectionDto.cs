using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class EncounterSectionDto
{
    public Guid Id { get; set; }
    public Guid EncounterId { get; set; }
    public EncounterSectionType SectionType { get; set; }
    public string Content { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public Guid? AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateEncounterSectionDto
{
    public EncounterSectionType SectionType { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? AuthorId { get; set; }
}

public class UpdateEncounterSectionDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
}
