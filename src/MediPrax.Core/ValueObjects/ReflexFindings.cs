namespace MediPrax.Core.ValueObjects;

public class ReflexFindings
{
    // Scale: 0=absent, 1=abgeschwächt, 2=normal, 3=lebhaft, 4=gesteigert+Kloni
    public int BicepsRight { get; set; } = 2;  // BSR
    public int BicepsLeft { get; set; } = 2;
    public int TricepsRight { get; set; } = 2; // TSR
    public int TricepsLeft { get; set; } = 2;
    public int BrachioradialRight { get; set; } = 2; // RPR
    public int BrachioradialLeft { get; set; } = 2;
    public int PatellarRight { get; set; } = 2; // PSR
    public int PatellarLeft { get; set; } = 2;
    public int AchillesRight { get; set; } = 2; // ASR
    public int AchillesLeft { get; set; } = 2;

    // Pathological reflexes
    public bool BabinskiRight { get; set; }
    public bool BabinskiLeft { get; set; }
    public bool HoffmannTroemnerRight { get; set; }
    public bool HoffmannTroemnerLeft { get; set; }
    public bool GordonRight { get; set; }
    public bool GordonLeft { get; set; }

    public string? Comment { get; set; }
}
