using MediPrax.Application.Catalogs;
using MediPrax.Application.Services;
using MediPrax.Core.ValueObjects;

namespace MediPrax.UnitTests;

public class PsychopathFindingTextGeneratorTests
{
    [Fact]
    public void Generate_AllZero_ContainsBewusstseinKlar()
    {
        var findings = CreateAllZeroFindings();
        var text = PsychopathFindingTextGenerator.Generate(findings);

        Assert.Contains("Bewusstsein klar", text);
        Assert.Contains("allseits orientiert", text);
        Assert.Contains("Formales Denken geordnet", text);
        Assert.Contains("Kein Wahn", text, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("keine Sinnestäuschungen", text, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("keine Ich-Störungen", text, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("keine Suizidalität", text, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Generate_DepressionPattern_GeneratesCorrectText()
    {
        var findings = CreateAllZeroFindings();

        // Set depression-typical findings
        SetSeverity(findings, "AFF-5", 2);  // deprimiert deutlich
        SetSeverity(findings, "AFF-6", 1);  // hoffnungslos leicht
        SetSeverity(findings, "ANT-1", 2);  // antriebsarm deutlich
        SetSeverity(findings, "SOM-1", 1);  // Schlafstörungen
        SetSeverity(findings, "SUI-1", 1);  // Lebensmüdigkeit

        var text = PsychopathFindingTextGenerator.Generate(findings);

        Assert.Contains("Bewusstsein klar", text);
        Assert.Contains("deprimiert", text);
        Assert.Contains("hoffnungslos", text);
        Assert.Contains("antriebsgemindert", text);
        Assert.Contains("Schlafstörungen", text);
        Assert.Contains("Lebensmüdigkeit", text);
    }

    [Fact]
    public void Generate_PsychosisPattern_GeneratesCorrectText()
    {
        var findings = CreateAllZeroFindings();

        // Set psychosis-typical findings
        SetSeverity(findings, "INH-8", 2);  // Verfolgungswahn
        SetSeverity(findings, "SIN-2", 2);  // Stimmenhören
        SetSeverity(findings, "ICH-5", 1);  // Gedankeneingebung
        SetSeverity(findings, "AFF-7", 1);  // ängstlich

        var text = PsychopathFindingTextGenerator.Generate(findings);

        Assert.Contains("Verfolgungswahn", text);
        Assert.Contains("Stimmenhören", text);
        Assert.Contains("Gedankeneingebung", text);
        Assert.Contains("ängstlich", text);
    }

    [Fact]
    public void Generate_EmptyFindings_ReturnsDefaultText()
    {
        var text = PsychopathFindingTextGenerator.Generate([]);
        Assert.Equal("Keine Befunde erhoben.", text);
    }

    [Fact]
    public void Generate_NullFindings_ReturnsDefaultText()
    {
        var text = PsychopathFindingTextGenerator.Generate(null!);
        Assert.Equal("Keine Befunde erhoben.", text);
    }

    [Fact]
    public void Generate_IncludesSuicidalitySection_WhenPresent()
    {
        var findings = CreateAllZeroFindings();
        SetSeverity(findings, "SUI-1", 1);  // Lebensmüdigkeit
        SetSeverity(findings, "SUI-2", 1);  // Suizidgedanken

        var text = PsychopathFindingTextGenerator.Generate(findings);

        Assert.Contains("Lebensmüdigkeit", text);
        Assert.Contains("Suizidgedanken", text);
    }

    [Fact]
    public void Generate_SeverityLabels_AreCorrect()
    {
        var findings = CreateAllZeroFindings();
        SetSeverity(findings, "AFF-5", 1);  // leichte deprimiert

        var text = PsychopathFindingTextGenerator.Generate(findings);
        Assert.Contains("leichte", text, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("deprimiert", text, StringComparison.OrdinalIgnoreCase);

        SetSeverity(findings, "AFF-5", 2);
        text = PsychopathFindingTextGenerator.Generate(findings);
        Assert.Contains("deutliche", text, StringComparison.OrdinalIgnoreCase);

        SetSeverity(findings, "AFF-5", 3);
        text = PsychopathFindingTextGenerator.Generate(findings);
        Assert.Contains("schwere", text, StringComparison.OrdinalIgnoreCase);
    }

    private static List<SymptomFinding> CreateAllZeroFindings()
    {
        return AmdpCatalog.AllSymptoms.Select(s => new SymptomFinding
        {
            CategoryCode = s.CategoryCode,
            SymptomCode = s.SymptomCode,
            SymptomName = s.Name,
            Severity = 0
        }).ToList();
    }

    private static void SetSeverity(List<SymptomFinding> findings, string symptomCode, int severity)
    {
        var finding = findings.First(f => f.SymptomCode == symptomCode);
        finding.Severity = severity;
    }
}
