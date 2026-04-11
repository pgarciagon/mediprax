using MediPrax.Application.DTOs;
using MediPrax.Core.Enums;

namespace MediPrax.UnitTests;

public class DiagnosisTests
{
    // --- AC: Enum values match KBV spec ---

    [Theory]
    [InlineData(DiagnosisCertainty.G, 0)]
    [InlineData(DiagnosisCertainty.V, 1)]
    [InlineData(DiagnosisCertainty.Z, 2)]
    [InlineData(DiagnosisCertainty.A, 3)]
    public void DiagnosisCertainty_HasCorrectValues(DiagnosisCertainty certainty, int expected)
    {
        Assert.Equal(expected, (int)certainty);
    }

    [Theory]
    [InlineData(DiagnosisLaterality.R, 0)]
    [InlineData(DiagnosisLaterality.L, 1)]
    [InlineData(DiagnosisLaterality.B, 2)]
    public void DiagnosisLaterality_HasCorrectValues(DiagnosisLaterality laterality, int expected)
    {
        Assert.Equal(expected, (int)laterality);
    }

    [Fact]
    public void DiagnosisType_HasExpectedValues()
    {
        Assert.Equal(0, (int)DiagnosisType.Dauerdiagnose);
        Assert.Equal(1, (int)DiagnosisType.Encounterdiagnose);
    }

    [Fact]
    public void DiagnosisStatus_HasExpectedValues()
    {
        Assert.Equal(0, (int)DiagnosisStatus.Active);
        Assert.Equal(1, (int)DiagnosisStatus.Anamnestisch);
        Assert.Equal(2, (int)DiagnosisStatus.Inactive);
    }

    // --- AC: Display text formats correctly ---

    [Fact]
    public void PatientDiagnosisDto_DisplayText_FormatsWithCertainty()
    {
        var dto = new PatientDiagnosisDto
        {
            Icd10Code = "F32.1",
            Icd10Description = "Mittelgradige depressive Episode",
            Certainty = DiagnosisCertainty.G,
            Laterality = null
        };

        Assert.Equal("F32.1 (G) Mittelgradige depressive Episode", dto.DisplayText);
    }

    [Fact]
    public void PatientDiagnosisDto_DisplayText_IncludesLaterality()
    {
        var dto = new PatientDiagnosisDto
        {
            Icd10Code = "G56.0",
            Icd10Description = "Karpaltunnelsyndrom",
            Certainty = DiagnosisCertainty.V,
            Laterality = DiagnosisLaterality.R
        };

        Assert.Equal("G56.0 (V) R Karpaltunnelsyndrom", dto.DisplayText);
    }

    [Fact]
    public void PatientDiagnosisDto_ShortDisplay_FormatsCorrectly()
    {
        var dto = new PatientDiagnosisDto
        {
            Icd10Code = "F32.1",
            Certainty = DiagnosisCertainty.G,
            Laterality = null
        };

        Assert.Equal("F32.1 (G)", dto.ShortDisplay);
    }

    [Fact]
    public void PatientDiagnosisDto_ShortDisplay_WithLaterality()
    {
        var dto = new PatientDiagnosisDto
        {
            Icd10Code = "G56.0",
            Certainty = DiagnosisCertainty.V,
            Laterality = DiagnosisLaterality.L
        };

        Assert.Equal("G56.0 (V) L", dto.ShortDisplay);
    }

    // --- AC: Certainty letters match KVDT field 6003 ---

    [Fact]
    public void DiagnosisCertainty_EnumNames_MatchKbvCodes()
    {
        Assert.Equal("G", DiagnosisCertainty.G.ToString());
        Assert.Equal("V", DiagnosisCertainty.V.ToString());
        Assert.Equal("Z", DiagnosisCertainty.Z.ToString());
        Assert.Equal("A", DiagnosisCertainty.A.ToString());
    }

    [Fact]
    public void DiagnosisLaterality_EnumNames_MatchKbvCodes()
    {
        Assert.Equal("R", DiagnosisLaterality.R.ToString());
        Assert.Equal("L", DiagnosisLaterality.L.ToString());
        Assert.Equal("B", DiagnosisLaterality.B.ToString());
    }
}
