using MediPrax.Application.Services;

namespace MediPrax.UnitTests;

public class GdtParserTests
{
    private const string SampleGdt = "01380006310\r\n013810000013\r\n0143000TestPatient123\r\n0143101Mustermann\r\n0103102Max\r\n015310301011990\r\n015620101042026\r\n0216200EEG Befund: Normales Alpha-EEG\r\n0186200Keine fokalen Auffälligkeiten\r\n0146302EEG-Device\r\n";

    [Fact]
    public void IsValidGdt_ReturnsFalse_ForEmptyContent()
    {
        Assert.False(GdtParser.IsValidGdt(""));
        Assert.False(GdtParser.IsValidGdt(null!));
    }

    [Fact]
    public void IsValidGdt_ReturnsFalse_ForTooFewLines()
    {
        Assert.False(GdtParser.IsValidGdt("one\ntwo\n"));
    }

    [Fact]
    public void IsValidGdt_ReturnsTrue_ForValidContent()
    {
        Assert.True(GdtParser.IsValidGdt(SampleGdt));
    }

    [Fact]
    public void Parse_ExtractsPatientName()
    {
        var result = GdtParser.Parse(SampleGdt);
        Assert.Contains("Mustermann", result.PatientName);
    }

    [Fact]
    public void Parse_ExtractsResultLines()
    {
        var result = GdtParser.Parse(SampleGdt);
        Assert.Equal(2, result.ResultLines.Count);
        Assert.Contains("EEG Befund", result.ResultLines[0]);
    }

    [Fact]
    public void Parse_ExtractsDeviceName()
    {
        var result = GdtParser.Parse(SampleGdt);
        Assert.Equal("EEG-Device", result.DeviceName);
    }

    [Fact]
    public void Parse_PreservesRawContent()
    {
        var result = GdtParser.Parse(SampleGdt);
        Assert.Equal(SampleGdt, result.RawContent);
    }
}
