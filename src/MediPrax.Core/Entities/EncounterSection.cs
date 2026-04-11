using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

public class EncounterSection : BaseEntity
{
    public Guid EncounterId { get; set; }
    public Encounter Encounter { get; set; } = null!;

    public EncounterSectionType SectionType { get; set; }
    public string Content { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public Guid? AuthorId { get; set; }
    public User? Author { get; set; }
}
