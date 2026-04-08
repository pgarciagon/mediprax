namespace MediPrax.Core.ValueObjects;

public class MeningealFindings
{
    public bool NeckStiffness { get; set; }
    public bool KernigSign { get; set; }
    public bool BrudzinskiSign { get; set; }
    public int? LasegueRight { get; set; } // degrees, null = negative
    public int? LasegueLeft { get; set; }
    public string? Comment { get; set; }
}
