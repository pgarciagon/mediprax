namespace MediPrax.Core.ValueObjects;

public class GaitFindings
{
    public string GaitPattern { get; set; } = "unauffällig"; // unauffällig, spastisch, ataktisch, kleinschrittig, Steppergang, hinkend, breitbasig
    public string ArmSwing { get; set; } = "seitengleich"; // seitengleich, vermindert rechts/links
    public string Stability { get; set; } = "sicher"; // sicher, unsicher, Fallneigung
    public string? Comment { get; set; }
}
