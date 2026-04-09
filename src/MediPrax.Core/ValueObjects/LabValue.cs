namespace MediPrax.Core.ValueObjects;

public class LabValue
{
    public string ParameterName { get; set; } = string.Empty;
    public string? ParameterCode { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal? ReferenceRangeLow { get; set; }
    public decimal? ReferenceRangeHigh { get; set; }
    public bool IsAbnormal { get; set; }
    public bool IsCritical { get; set; }
    public string? Category { get; set; }  // "Blutbild", "Schilddrüse", "Niere", etc.
}
