using MediPrax.Application.Data;
using MediPrax.Application.DTOs;
using MediPrax.Core.Enums;

namespace MediPrax.UnitTests;

public class ActionChainTests
{
    [Theory]
    [InlineData(ActionStepType.AddDiagnosis, "Diagnose hinzufügen")]
    [InlineData(ActionStepType.AddBillingCode, "Abrechnungsziffer")]
    [InlineData(ActionStepType.SetNoteTemplate, "Befundvorlage")]
    [InlineData(ActionStepType.CreatePrescription, "Rezept erstellen")]
    [InlineData(ActionStepType.SetEncounterType, "Kontaktart setzen")]
    [InlineData(ActionStepType.SetDuration, "Dauer setzen")]
    [InlineData(ActionStepType.CreateReferral, "Überweisung")]
    [InlineData(ActionStepType.CreateRecall, "Wiedervorlage")]
    public void StepTypeDisplay_ReturnsGermanLabel(ActionStepType type, string expected)
    {
        var step = new ActionChainStepDto { StepType = type };
        Assert.Equal(expected, step.StepTypeDisplay);
    }

    [Fact]
    public void ActionChainResult_Success_WhenNoFailures()
    {
        var result = new ActionChainResult(
            Success: true,
            StepsExecuted: 3,
            StepsFailed: 0,
            Results: [
                new ActionStepResult(ActionStepType.AddDiagnosis, true, "F32.1", null),
                new ActionStepResult(ActionStepType.AddBillingCode, true, "21220", null),
                new ActionStepResult(ActionStepType.SetNoteTemplate, true, "psych", null)
            ],
            Warnings: []
        );

        Assert.True(result.Success);
        Assert.Equal(3, result.StepsExecuted);
        Assert.Equal(0, result.StepsFailed);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void ActionChainResult_Failure_WhenStepsFail()
    {
        var result = new ActionChainResult(
            Success: false,
            StepsExecuted: 2,
            StepsFailed: 1,
            Results: [
                new ActionStepResult(ActionStepType.AddDiagnosis, true, "F32.1", null),
                new ActionStepResult(ActionStepType.AddBillingCode, true, "21220", null),
                new ActionStepResult(ActionStepType.SetNoteTemplate, false, null, "Template not found")
            ],
            Warnings: ["SetNoteTemplate: Template not found"]
        );

        Assert.False(result.Success);
        Assert.Equal(1, result.StepsFailed);
        Assert.Single(result.Warnings);
    }

    [Fact]
    public void SectionTemplates_Psych_ReturnsBefund()
    {
        var template = SectionTemplates.GetTemplate("psych");

        Assert.Single(template);
        Assert.True(template.ContainsKey(EncounterSectionType.Befund));
        Assert.Contains("Bewusstsein", template[EncounterSectionType.Befund]);
        Assert.Contains("Suizidalität", template[EncounterSectionType.Befund]);
    }

    [Fact]
    public void SectionTemplates_Neuro_ReturnsBefund()
    {
        var template = SectionTemplates.GetTemplate("neuro");

        Assert.Single(template);
        Assert.True(template.ContainsKey(EncounterSectionType.Befund));
        Assert.Contains("Hirnnerven", template[EncounterSectionType.Befund]);
        Assert.Contains("Gang", template[EncounterSectionType.Befund]);
    }

    [Fact]
    public void SectionTemplates_Kurz_ReturnsMultipleSections()
    {
        var template = SectionTemplates.GetTemplate("kurz");

        Assert.Equal(3, template.Count);
        Assert.True(template.ContainsKey(EncounterSectionType.Anamnese));
        Assert.True(template.ContainsKey(EncounterSectionType.Therapie));
        Assert.True(template.ContainsKey(EncounterSectionType.Procedere));
    }

    [Fact]
    public void SectionTemplates_Unknown_ReturnsEmpty()
    {
        var template = SectionTemplates.GetTemplate("nonexistent");
        Assert.Empty(template);
    }

    [Fact]
    public void SectionTemplates_Available_ContainsAllTemplates()
    {
        Assert.Equal(3, SectionTemplates.Available.Count);
        Assert.Contains(SectionTemplates.Available, t => t.Key == "psych");
        Assert.Contains(SectionTemplates.Available, t => t.Key == "neuro");
        Assert.Contains(SectionTemplates.Available, t => t.Key == "kurz");
    }

    [Fact]
    public void CreateActionChainDto_Defaults()
    {
        var dto = new CreateActionChainDto();

        Assert.True(dto.IsGlobal);
        Assert.True(dto.IsActive);
        Assert.Empty(dto.Steps);
    }
}
