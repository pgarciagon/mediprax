using MediPrax.Application.Catalogs;
using MediPrax.Core.Enums;

namespace MediPrax.UnitTests;

public class PsychometricTestCatalogTests
{
    [Theory]
    [InlineData(PsychometricTestType.PHQ9)]
    [InlineData(PsychometricTestType.GAD7)]
    [InlineData(PsychometricTestType.WHO5)]
    [InlineData(PsychometricTestType.CGI)]
    [InlineData(PsychometricTestType.AUDIT)]
    [InlineData(PsychometricTestType.BDI2)]
    [InlineData(PsychometricTestType.MMSE)]
    [InlineData(PsychometricTestType.MoCA)]
    [InlineData(PsychometricTestType.PANSS)]
    [InlineData(PsychometricTestType.PCL5)]
    [InlineData(PsychometricTestType.PSQI)]
    [InlineData(PsychometricTestType.HAMD)]
    [InlineData(PsychometricTestType.BPRS)]
    [InlineData(PsychometricTestType.ClockDrawing)]
    public void GetDefinition_ReturnsCorrectDefinition_ForEachType(PsychometricTestType type)
    {
        var def = PsychometricTestCatalog.GetDefinition(type);

        Assert.NotNull(def);
        Assert.Equal(type, def.TestType);
        Assert.NotEmpty(def.Name);
        Assert.NotEmpty(def.Abbreviation);
        Assert.NotEmpty(def.Description);
        Assert.NotEmpty(def.Instruction);
        Assert.NotEmpty(def.Items);
        Assert.NotEmpty(def.ScoreRanges);
        Assert.True(def.MaxScore > 0);
    }

    [Fact]
    public void AllDefinitions_ContainsAllTypes()
    {
        var allTypes = Enum.GetValues<PsychometricTestType>();
        var definitions = PsychometricTestCatalog.AllDefinitions;

        Assert.Equal(allTypes.Length, definitions.Count);
        foreach (var type in allTypes)
        {
            Assert.Contains(definitions, d => d.TestType == type);
        }
    }

    [Fact]
    public void PHQ9_Has9Items()
    {
        var def = PsychometricTestCatalog.GetDefinition(PsychometricTestType.PHQ9);
        Assert.Equal(9, def.Items.Count);
        Assert.Equal(27, def.MaxScore);
    }

    [Fact]
    public void GAD7_Has7Items()
    {
        var def = PsychometricTestCatalog.GetDefinition(PsychometricTestType.GAD7);
        Assert.Equal(7, def.Items.Count);
        Assert.Equal(21, def.MaxScore);
    }

    [Fact]
    public void WHO5_Has5Items()
    {
        var def = PsychometricTestCatalog.GetDefinition(PsychometricTestType.WHO5);
        Assert.Equal(5, def.Items.Count);
        Assert.Equal(25, def.MaxScore);
    }

    [Fact]
    public void AUDIT_Has10Items()
    {
        var def = PsychometricTestCatalog.GetDefinition(PsychometricTestType.AUDIT);
        Assert.Equal(10, def.Items.Count);
        Assert.Equal(40, def.MaxScore);
    }

    [Fact]
    public void MMSE_Has11Items()
    {
        var def = PsychometricTestCatalog.GetDefinition(PsychometricTestType.MMSE);
        Assert.Equal(11, def.Items.Count);
        Assert.Equal(30, def.MaxScore);
    }

    [Fact]
    public void CGI_Has2Items()
    {
        var def = PsychometricTestCatalog.GetDefinition(PsychometricTestType.CGI);
        Assert.Equal(2, def.Items.Count);
    }

    [Theory]
    [InlineData(0, "Minimal")]
    [InlineData(4, "Minimal")]
    [InlineData(5, "Leicht")]
    [InlineData(9, "Leicht")]
    [InlineData(10, "Mittelgradig")]
    [InlineData(14, "Mittelgradig")]
    [InlineData(15, "Mittelschwer")]
    [InlineData(19, "Mittelschwer")]
    [InlineData(20, "Schwer")]
    [InlineData(27, "Schwer")]
    public void CalculateInterpretation_ReturnsCorrectInterpretation_ForPHQ9(int score, string expected)
    {
        var result = PsychometricTestCatalog.CalculateInterpretation(PsychometricTestType.PHQ9, score);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, "Minimal")]
    [InlineData(4, "Minimal")]
    [InlineData(5, "Leicht")]
    [InlineData(9, "Leicht")]
    [InlineData(10, "Mittelgradig")]
    [InlineData(14, "Mittelgradig")]
    [InlineData(15, "Schwer")]
    [InlineData(21, "Schwer")]
    public void CalculateInterpretation_ReturnsCorrectInterpretation_ForGAD7(int score, string expected)
    {
        var result = PsychometricTestCatalog.CalculateInterpretation(PsychometricTestType.GAD7, score);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(PsychometricTestType.PHQ9)]
    [InlineData(PsychometricTestType.GAD7)]
    [InlineData(PsychometricTestType.WHO5)]
    [InlineData(PsychometricTestType.AUDIT)]
    [InlineData(PsychometricTestType.MMSE)]
    [InlineData(PsychometricTestType.CGI)]
    public void EachTestDefinition_HasConsistentMaxScore(PsychometricTestType type)
    {
        var def = PsychometricTestCatalog.GetDefinition(type);

        // MaxScore should be reachable by summing max option values across all items
        var calculatedMax = def.Items.Sum(item => item.Options.Max(o => o.Value));
        Assert.Equal(def.MaxScore, calculatedMax);
    }
}
