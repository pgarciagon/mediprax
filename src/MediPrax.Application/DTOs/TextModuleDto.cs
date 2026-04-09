namespace MediPrax.Application.DTOs;

public class TextModuleDto
{
    public Guid Id { get; set; }
    public Guid CreatedById { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public string Shortcut { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsGlobal { get; set; }
    public int UsageCount { get; set; }
}

public class CreateTextModuleDto
{
    public Guid CreatedById { get; set; }
    public string Shortcut { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsGlobal { get; set; }
}
