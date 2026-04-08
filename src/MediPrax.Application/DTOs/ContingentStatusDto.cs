namespace MediPrax.Application.DTOs;

public class ContingentStatusDto
{
    public int ApprovedSessions { get; set; }
    public int CompletedSessions { get; set; }
    public int RemainingSessions { get; set; }
    public int MaxTotal { get; set; }
    public bool RequiresGutachter { get; set; }
    public string WarningLevel { get; set; } = "None"; // None, Yellow, Red
    public string? WarningMessage { get; set; }
    public bool CanExtend { get; set; }
    public int? ExtensionPossible { get; set; }
}
