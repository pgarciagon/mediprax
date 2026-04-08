namespace MediPrax.Core.ValueObjects;

public class SymptomFinding
{
    public string CategoryCode { get; set; } = string.Empty;
    public string SymptomCode { get; set; } = string.Empty;
    public string SymptomName { get; set; } = string.Empty;
    public int Severity { get; set; } // 0=not present, 1=mild, 2=moderate, 3=severe
    public string? Comment { get; set; }
}
