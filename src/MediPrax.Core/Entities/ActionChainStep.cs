using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

/// <summary>
/// A single step within an ActionChain. Configuration is stored as JSON
/// with a schema determined by the StepType.
/// </summary>
public class ActionChainStep : BaseEntity
{
    /// <summary>FK to the parent ActionChain.</summary>
    public Guid ActionChainId { get; set; }
    public ActionChain ActionChain { get; set; } = null!;

    /// <summary>What kind of action this step performs.</summary>
    public ActionStepType StepType { get; set; }

    /// <summary>Execution order within the chain.</summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// JSON payload with type-specific configuration.
    /// Examples:
    ///   AddDiagnosis: {"icd10Code":"F32.1","certainty":"G","diagnosisType":"Encounterdiagnose"}
    ///   AddBillingCode: {"gopCode":"21220","quantity":1}
    ///   SetNoteTemplate: {"template":"psych"}
    ///   CreateRecall: {"reason":"Kontrolle","daysFromNow":90}
    /// </summary>
    public required string Configuration { get; set; }
}
