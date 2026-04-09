namespace MediPrax.Application.DTOs;

public class MsDocumentationDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateOnly DocumentationDate { get; set; }
    public decimal EdssScore { get; set; }
    public bool IsRelapse { get; set; }
    public string? RelapseDescription { get; set; }
    public DateOnly? MriDate { get; set; }
    public string? MriFindings { get; set; }
    public int? NewLesions { get; set; }
    public int? GadEnhancing { get; set; }
    public string? CurrentDmt { get; set; }
    public string? Notes { get; set; }

    public string EdssDisplay => EdssScore switch
    {
        0 => "0.0 — Normal",
        <= 3.5m => $"{EdssScore:F1} — Leichte Behinderung",
        <= 5.5m => $"{EdssScore:F1} — Mäßige Behinderung",
        <= 7.5m => $"{EdssScore:F1} — Schwere Behinderung",
        _ => $"{EdssScore:F1} — Sehr schwer"
    };
}

public class CreateMsDocumentationDto
{
    public Guid PatientId { get; set; }
    public DateOnly DocumentationDate { get; set; }
    public decimal EdssScore { get; set; }
    public bool IsRelapse { get; set; }
    public string? RelapseDescription { get; set; }
    public DateOnly? MriDate { get; set; }
    public string? MriFindings { get; set; }
    public int? NewLesions { get; set; }
    public int? GadEnhancing { get; set; }
    public string? CurrentDmt { get; set; }
    public string? Notes { get; set; }
}
