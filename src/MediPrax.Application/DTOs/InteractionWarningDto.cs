namespace MediPrax.Application.DTOs;

public class InteractionWarningDto
{
    public string Substance1 { get; set; } = string.Empty;
    public string Substance2 { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // Critical, Warning, Info
    public string InteractionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
