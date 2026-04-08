namespace MediPrax.Core.ValueObjects;

public class SensoryFindings
{
    public string TouchSensation { get; set; } = "intakt"; // intakt, eingeschränkt, aufgehoben
    public string? TouchDistribution { get; set; }
    public string PainSensation { get; set; } = "intakt";
    public string? PainDistribution { get; set; }
    public string TemperatureSensation { get; set; } = "intakt";
    public string? TemperatureDistribution { get; set; }
    public string VibrationSenseRight { get; set; } = "normal"; // normal or value like "6/8"
    public string VibrationSenseLeft { get; set; } = "normal";
    public string Proprioception { get; set; } = "intakt";
    public string? Comment { get; set; }
}
