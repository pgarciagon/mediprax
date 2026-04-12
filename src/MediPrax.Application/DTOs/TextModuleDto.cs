using MediPrax.Core.Enums;

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
    public EncounterSectionType? TargetSection { get; set; }
    public DateTime? LastUsedAt { get; set; }

    public string TargetSectionDisplay => TargetSection switch
    {
        EncounterSectionType.Anamnese => "Anamnese",
        EncounterSectionType.Befund => "Befund",
        EncounterSectionType.Diagnose => "Diagnose",
        EncounterSectionType.Therapie => "Therapie",
        EncounterSectionType.Procedere => "Procedere",
        EncounterSectionType.Sonstiges => "Sonstiges",
        _ => "Alle Sektionen"
    };
}

public class CreateTextModuleDto
{
    public Guid CreatedById { get; set; }
    public string Shortcut { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsGlobal { get; set; }
    public EncounterSectionType? TargetSection { get; set; }
}

public class TextModuleExportDto
{
    public string Version { get; set; } = "1.0";
    public string ExportedAt { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");
    public List<TextModuleExportItem> Modules { get; set; } = [];
}

public class TextModuleExportItem
{
    public string Shortcut { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? TargetSection { get; set; }
    public bool IsGlobal { get; set; }
}
