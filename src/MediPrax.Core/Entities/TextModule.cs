namespace MediPrax.Core.Entities;

public class TextModule : BaseEntity
{
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    public required string Shortcut { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsGlobal { get; set; }
    public int UsageCount { get; set; }
}
