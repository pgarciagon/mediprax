using MediPrax.Core.Enums;

namespace MediPrax.Core.Entities;

/// <summary>
/// Persistent medication catalog entry (drug database).
/// Not a patient entity — no soft delete, no BaseEntity.
/// Seeded with ~300 psychiatry/neurology medications.
/// </summary>
public class MedicationCatalogEntry
{
    public Guid Id { get; set; }

    /// <summary>Pharmazentralnummer (8 digits).</summary>
    public required string Pzn { get; set; }

    /// <summary>Trade name (Handelsname).</summary>
    public required string Handelsname { get; set; }

    /// <summary>Active substance (Wirkstoff).</summary>
    public required string Wirkstoff { get; set; }

    /// <summary>BfArM substance identifier.</summary>
    public string? WirkstoffAsk { get; set; }

    /// <summary>ATC classification code.</summary>
    public string? AtcCode { get; set; }

    /// <summary>Strength, e.g. "50 mg".</summary>
    public string? Staerke { get; set; }

    /// <summary>Dosage form (Darreichungsform), e.g. Filmtabletten.</summary>
    public string? Darreichungsform { get; set; }

    /// <summary>Package size, e.g. "100 Stk".</summary>
    public string? Packungsgroesse { get; set; }

    /// <summary>Norm package size: N1, N2, N3.</summary>
    public string? NormPackungsgroesse { get; set; }

    /// <summary>Manufacturer (Hersteller).</summary>
    public string? Hersteller { get; set; }

    /// <summary>Controlled substance (Betäubungsmittel).</summary>
    public bool IsBtm { get; set; }

    /// <summary>Requires T-Rezept (e.g. Lenalidomid).</summary>
    public bool IsTRezeptPflichtig { get; set; }

    /// <summary>Prescription-only (verschreibungspflichtig).</summary>
    public bool IsVerschreibungspflichtig { get; set; } = true;

    /// <summary>Fixed reimbursement amount (Festbetrag).</summary>
    public decimal? Festbetrag { get; set; }

    /// <summary>Pharmacy retail price (Apothekenverkaufspreis).</summary>
    public decimal? Avp { get; set; }

    /// <summary>Medication category for psychiatry/neurology.</summary>
    public MedicationCategory? Category { get; set; }

    /// <summary>Whether this entry is active in the catalog.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Data source identifier: SEED, FHIR, MANUAL.</summary>
    public string DataSource { get; set; } = "SEED";

    /// <summary>Last update timestamp.</summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
