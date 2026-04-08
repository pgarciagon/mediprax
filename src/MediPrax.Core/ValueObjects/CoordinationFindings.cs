namespace MediPrax.Core.ValueObjects;

public class CoordinationFindings
{
    public string FingerNoseTest { get; set; } = "zielsicher"; // zielsicher, Dysmetrie, Intentionstremor
    public string HeelShinTest { get; set; } = "zielsicher";
    public string RombergTest { get; set; } = "sicher"; // sicher, unsicher, pathologisch
    public string TandemGait { get; set; } = "sicher"; // sicher, unsicher, nicht möglich
    public string Diadochokinesis { get; set; } = "unauffällig"; // unauffällig, Dysdiadochokinese
    public string? Comment { get; set; }
}
