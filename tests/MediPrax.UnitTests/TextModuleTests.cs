using MediPrax.Application.DTOs;
using MediPrax.Core.Enums;

namespace MediPrax.UnitTests;

public class TextModuleTests
{
    [Fact]
    public void TextModuleDto_TargetSectionDisplay_ReturnsGermanLabel()
    {
        var dto = new TextModuleDto { TargetSection = EncounterSectionType.Befund };
        Assert.Equal("Befund", dto.TargetSectionDisplay);
    }

    [Fact]
    public void TextModuleDto_TargetSectionDisplay_NullReturnsAlleSektionen()
    {
        var dto = new TextModuleDto { TargetSection = null };
        Assert.Equal("Alle Sektionen", dto.TargetSectionDisplay);
    }

    [Theory]
    [InlineData(EncounterSectionType.Anamnese, "Anamnese")]
    [InlineData(EncounterSectionType.Befund, "Befund")]
    [InlineData(EncounterSectionType.Diagnose, "Diagnose")]
    [InlineData(EncounterSectionType.Therapie, "Therapie")]
    [InlineData(EncounterSectionType.Procedere, "Procedere")]
    [InlineData(EncounterSectionType.Sonstiges, "Sonstiges")]
    public void TextModuleDto_TargetSectionDisplay_AllValues(EncounterSectionType section, string expected)
    {
        var dto = new TextModuleDto { TargetSection = section };
        Assert.Equal(expected, dto.TargetSectionDisplay);
    }

    [Fact]
    public void TextModuleExportDto_DefaultValues()
    {
        var export = new TextModuleExportDto();
        Assert.Equal("1.0", export.Version);
        Assert.NotNull(export.Modules);
        Assert.Empty(export.Modules);
        Assert.Equal(DateTime.Today.ToString("yyyy-MM-dd"), export.ExportedAt);
    }

    [Fact]
    public void TextModuleExportItem_Serialization()
    {
        var item = new TextModuleExportItem
        {
            Shortcut = "test",
            Title = "Test Module",
            Content = "Hello {Patient.Name}",
            Category = "Psychiatrie/Befund",
            TargetSection = "Befund",
            IsGlobal = true
        };

        Assert.Equal("test", item.Shortcut);
        Assert.Equal("Befund", item.TargetSection);
        Assert.True(item.IsGlobal);
    }

    [Fact]
    public void CreateTextModuleDto_DefaultValues()
    {
        var dto = new CreateTextModuleDto();
        Assert.Equal(string.Empty, dto.Shortcut);
        Assert.Equal(string.Empty, dto.Title);
        Assert.Equal(string.Empty, dto.Content);
        Assert.Equal(string.Empty, dto.Category);
        Assert.False(dto.IsGlobal);
        Assert.Null(dto.TargetSection);
    }

    [Fact]
    public void CreateTextModuleDto_WithTargetSection()
    {
        var dto = new CreateTextModuleDto
        {
            TargetSection = EncounterSectionType.Therapie
        };
        Assert.Equal(EncounterSectionType.Therapie, dto.TargetSection);
    }
}
