namespace MediPrax.Core.Entities;

/// <summary>
/// An Aktionskette (action chain) — a pre-configured shortcut that executes
/// multiple clinical actions in sequence (diagnosis + billing + template + prescription).
/// Triggered via #shortcut in the encounter form.
/// </summary>
public class ActionChain : BaseEntity
{
    /// <summary>Trigger shortcut, e.g. "dep" (used as #dep in UI).</summary>
    public required string Shortcut { get; set; }

    /// <summary>Display name, e.g. "Depression Erstgespräch".</summary>
    public required string Title { get; set; }

    /// <summary>Optional description of what this chain does.</summary>
    public string? Description { get; set; }

    /// <summary>Category for grouping: "Psychiatrie", "Neurologie", "Allgemein".</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>FK to the user who created this chain.</summary>
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    /// <summary>If true, available to all users. If false, only to the creator.</summary>
    public bool IsGlobal { get; set; } = true;

    /// <summary>Display order in lists.</summary>
    public int SortOrder { get; set; }

    /// <summary>Can be deactivated without deletion.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Ordered steps to execute.</summary>
    public ICollection<ActionChainStep> Steps { get; set; } = [];
}
