namespace MediPrax.Core.ValueObjects;

public class MotorFindings
{
    // Strength per region (MRC grade 0-5)
    public int StrengthRightArm { get; set; } = 5;
    public int StrengthLeftArm { get; set; } = 5;
    public int StrengthRightLeg { get; set; } = 5;
    public int StrengthLeftLeg { get; set; } = 5;

    // Tone per region
    public string ToneRightArm { get; set; } = "normal"; // normal, spastisch, rigide, schlaff
    public string ToneLeftArm { get; set; } = "normal";
    public string ToneRightLeg { get; set; } = "normal";
    public string ToneLeftLeg { get; set; } = "normal";

    // Bulk
    public string BulkRightArm { get; set; } = "normal"; // normal, Atrophie
    public string BulkLeftArm { get; set; } = "normal";
    public string BulkRightLeg { get; set; } = "normal";
    public string BulkLeftLeg { get; set; } = "normal";

    // Fasciculations
    public bool FasciculationsPresent { get; set; }
    public string? FasciculationsLocation { get; set; }

    public string? Comment { get; set; }
}
