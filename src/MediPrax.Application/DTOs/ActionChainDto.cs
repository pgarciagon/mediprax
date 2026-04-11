using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class ActionChainDto
{
    public Guid Id { get; set; }
    public string Shortcut { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public Guid CreatedById { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public bool IsGlobal { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int StepCount { get; set; }
    public List<ActionChainStepDto> Steps { get; set; } = [];
}

public class ActionChainStepDto
{
    public Guid Id { get; set; }
    public ActionStepType StepType { get; set; }
    public int SortOrder { get; set; }
    public string Configuration { get; set; } = string.Empty;

    public string StepTypeDisplay => StepType switch
    {
        ActionStepType.AddDiagnosis => "Diagnose hinzufügen",
        ActionStepType.AddBillingCode => "Abrechnungsziffer",
        ActionStepType.SetNoteTemplate => "Befundvorlage",
        ActionStepType.CreatePrescription => "Rezept erstellen",
        ActionStepType.SetEncounterType => "Kontaktart setzen",
        ActionStepType.SetDuration => "Dauer setzen",
        ActionStepType.CreateReferral => "Überweisung",
        ActionStepType.CreateRecall => "Wiedervorlage",
        _ => StepType.ToString()
    };
}

public class CreateActionChainDto
{
    public string Shortcut { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public Guid CreatedById { get; set; }
    public bool IsGlobal { get; set; } = true;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public List<CreateActionChainStepDto> Steps { get; set; } = [];
}

public class CreateActionChainStepDto
{
    public ActionStepType StepType { get; set; }
    public int SortOrder { get; set; }
    public string Configuration { get; set; } = string.Empty;
}

public class UpdateActionChainDto : CreateActionChainDto
{
    public Guid Id { get; set; }
}

/// <summary>Result of executing an action chain.</summary>
public record ActionChainResult(
    bool Success,
    int StepsExecuted,
    int StepsFailed,
    List<ActionStepResult> Results,
    List<string> Warnings
);

/// <summary>Result of a single step within a chain execution.</summary>
public record ActionStepResult(
    ActionStepType StepType,
    bool Success,
    string? Detail,
    string? ErrorMessage
);
