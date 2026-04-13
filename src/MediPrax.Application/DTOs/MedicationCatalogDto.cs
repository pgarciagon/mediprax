using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class MedicationCatalogEntryDto
{
    public Guid Id { get; set; }
    public string Pzn { get; set; } = string.Empty;
    public string Handelsname { get; set; } = string.Empty;
    public string Wirkstoff { get; set; } = string.Empty;
    public string? WirkstoffAsk { get; set; }
    public string? AtcCode { get; set; }
    public string? Staerke { get; set; }
    public string? Darreichungsform { get; set; }
    public string? Packungsgroesse { get; set; }
    public string? NormPackungsgroesse { get; set; }
    public string? Hersteller { get; set; }
    public bool IsBtm { get; set; }
    public bool IsTRezeptPflichtig { get; set; }
    public bool IsVerschreibungspflichtig { get; set; }
    public decimal? Festbetrag { get; set; }
    public decimal? Avp { get; set; }
    public MedicationCategory? Category { get; set; }
    public bool IsActive { get; set; }
    public string DataSource { get; set; } = string.Empty;
}

public class CatalogStatisticsDto
{
    public int TotalEntries { get; set; }
    public int ActiveEntries { get; set; }
    public Dictionary<string, int> ByCategory { get; set; } = new();
    public int BtmCount { get; set; }
    public int TRezeptCount { get; set; }
}
