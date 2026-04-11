namespace MediPrax.Core.Enums;

/// <summary>
/// Types of actions that can be executed as part of an ActionChain (Aktionskette).
/// Each step type has a corresponding JSON configuration schema.
/// </summary>
public enum ActionStepType
{
    AddDiagnosis = 0,
    AddBillingCode = 1,
    SetNoteTemplate = 2,
    CreatePrescription = 3,
    SetEncounterType = 4,
    SetDuration = 5,
    CreateReferral = 6,
    CreateRecall = 7
}
