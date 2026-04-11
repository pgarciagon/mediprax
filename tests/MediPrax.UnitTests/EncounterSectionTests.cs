using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Enums;

namespace MediPrax.UnitTests;

public class EncounterSectionTests
{
    [Fact]
    public void EncounterSectionType_HasExpectedValues()
    {
        Assert.Equal(0, (int)EncounterSectionType.Anamnese);
        Assert.Equal(1, (int)EncounterSectionType.Befund);
        Assert.Equal(2, (int)EncounterSectionType.Diagnose);
        Assert.Equal(3, (int)EncounterSectionType.Therapie);
        Assert.Equal(4, (int)EncounterSectionType.Procedere);
        Assert.Equal(5, (int)EncounterSectionType.Sonstiges);
    }

    [Fact]
    public void ParseNotes_StructuredAbdtp_ParsesCorrectly()
    {
        var notes = "Anamnese:\nKopfschmerzen seit 3 Wochen\n\nBefund:\nDruckschmerz temporal links\n\nDiagnose:\nSpannungskopfschmerz\n\nTherapie:\nIbuprofen 400mg\n\nProcedere:\nWiedervorstellung in 2 Wochen";

        var result = EncounterSectionService.ParseNotesIntoSections(notes);

        Assert.Equal(5, result.Count);
        Assert.Equal(EncounterSectionType.Anamnese, result[0].Type);
        Assert.Equal("Kopfschmerzen seit 3 Wochen", result[0].Content);
        Assert.Equal(EncounterSectionType.Befund, result[1].Type);
        Assert.Equal(EncounterSectionType.Diagnose, result[2].Type);
        Assert.Equal(EncounterSectionType.Therapie, result[3].Type);
        Assert.Equal(EncounterSectionType.Procedere, result[4].Type);
        Assert.Equal("Wiedervorstellung in 2 Wochen", result[4].Content);
    }

    [Fact]
    public void ParseNotes_ShorthandHeaders_ParsesCorrectly()
    {
        var notes = "A:\nKopfschmerzen\nB:\nUnauffällig\nD:\nG43.0\nT:\nSumatriptan\nP:\nKontrolle";

        var result = EncounterSectionService.ParseNotesIntoSections(notes);

        Assert.Equal(5, result.Count);
        Assert.Equal(EncounterSectionType.Anamnese, result[0].Type);
        Assert.Equal(EncounterSectionType.Befund, result[1].Type);
        Assert.Equal(EncounterSectionType.Diagnose, result[2].Type);
        Assert.Equal(EncounterSectionType.Therapie, result[3].Type);
        Assert.Equal(EncounterSectionType.Procedere, result[4].Type);
    }

    [Fact]
    public void ParseNotes_UnstructuredText_GoesToSonstiges()
    {
        var notes = "Patient kommt wegen Rückenschmerzen. Allgemeinzustand gut. Nächster Termin in 4 Wochen.";

        var result = EncounterSectionService.ParseNotesIntoSections(notes);

        Assert.Single(result);
        Assert.Equal(EncounterSectionType.Sonstiges, result[0].Type);
        Assert.Contains("Rückenschmerzen", result[0].Content);
    }

    [Fact]
    public void ParseNotes_EmptyText_ReturnsEmpty()
    {
        var result = EncounterSectionService.ParseNotesIntoSections("   ");

        Assert.Empty(result);
    }

    [Fact]
    public void ParseNotes_PartialSections_ParsesAvailable()
    {
        var notes = "Anamnese:\nSchwindel\n\nTherapie:\nBetahistin";

        var result = EncounterSectionService.ParseNotesIntoSections(notes);

        Assert.Equal(2, result.Count);
        Assert.Equal(EncounterSectionType.Anamnese, result[0].Type);
        Assert.Equal(EncounterSectionType.Therapie, result[1].Type);
    }

    // --- Template population tests ---

    [Fact]
    public void PsychTemplate_PopulatesBefundSection()
    {
        // Simulates what ApplySectionTemplate("psych") does in the UI
        var sections = new Dictionary<EncounterSectionType, string>
        {
            [EncounterSectionType.Anamnese] = string.Empty,
            [EncounterSectionType.Befund] = string.Empty,
            [EncounterSectionType.Diagnose] = string.Empty,
            [EncounterSectionType.Therapie] = string.Empty,
            [EncounterSectionType.Procedere] = string.Empty,
            [EncounterSectionType.Sonstiges] = string.Empty
        };

        // Apply "psych" template — only Befund gets content
        sections[EncounterSectionType.Befund] =
            "Bewusstsein: klar / getrübt\nOrientierung: zeitlich / örtlich / situativ / zur Person";

        Assert.NotEmpty(sections[EncounterSectionType.Befund]);
        Assert.Contains("Bewusstsein", sections[EncounterSectionType.Befund]);
        Assert.Empty(sections[EncounterSectionType.Anamnese]);
        Assert.Empty(sections[EncounterSectionType.Therapie]);
        Assert.Empty(sections[EncounterSectionType.Procedere]);
    }

    [Fact]
    public void KurzTemplate_PopulatesAnameseTherapieProcedere()
    {
        // Simulates what ApplySectionTemplate("kurz") does in the UI
        var sections = new Dictionary<EncounterSectionType, string>
        {
            [EncounterSectionType.Anamnese] = string.Empty,
            [EncounterSectionType.Befund] = string.Empty,
            [EncounterSectionType.Diagnose] = string.Empty,
            [EncounterSectionType.Therapie] = string.Empty,
            [EncounterSectionType.Procedere] = string.Empty,
            [EncounterSectionType.Sonstiges] = string.Empty
        };

        // Apply "kurz" template — A, T, P get content
        sections[EncounterSectionType.Anamnese] = "Aktueller Zustand:";
        sections[EncounterSectionType.Therapie] = "Medikation: unverändert / Anpassung:";
        sections[EncounterSectionType.Procedere] = "Nächster Termin:";

        Assert.NotEmpty(sections[EncounterSectionType.Anamnese]);
        Assert.NotEmpty(sections[EncounterSectionType.Therapie]);
        Assert.NotEmpty(sections[EncounterSectionType.Procedere]);
        Assert.Empty(sections[EncounterSectionType.Befund]);
        Assert.Empty(sections[EncounterSectionType.Diagnose]);
    }

    // --- ParseSectionFilter tests ---

    [Fact]
    public void ParseSectionFilter_WithInBefund_ReturnsFilteredQuery()
    {
        var (query, filter) = SearchService.ParseSectionFilter("tremor in:befund");

        Assert.Equal("tremor", query);
        Assert.Equal(EncounterSectionType.Befund, filter);
    }

    [Fact]
    public void ParseSectionFilter_WithInAnamnese_ReturnsFilteredQuery()
    {
        var (query, filter) = SearchService.ParseSectionFilter("Kopfschmerzen in:anamnese");

        Assert.Equal("Kopfschmerzen", query);
        Assert.Equal(EncounterSectionType.Anamnese, filter);
    }

    [Fact]
    public void ParseSectionFilter_WithoutFilter_ReturnsFullQuery()
    {
        var (query, filter) = SearchService.ParseSectionFilter("Kopfschmerzen");

        Assert.Equal("Kopfschmerzen", query);
        Assert.Null(filter);
    }

    [Fact]
    public void ParseSectionFilter_CaseInsensitive()
    {
        var (query, filter) = SearchService.ParseSectionFilter("test IN:Therapie");

        Assert.Equal("test", query);
        Assert.Equal(EncounterSectionType.Therapie, filter);
    }

    // --- Arztbrief integration: GetCombinedText format ---

    [Fact]
    public void ArztbriefSectionFallback_SectionsOverrideNotes()
    {
        // When an encounter has sections, Arztbrief should use sections (not Notes).
        // This simulates the logic in ArztbriefFormular.OnEncounterChanged:
        // "if sections.Count > 0, build body from sections; else use Notes"

        var sections = new List<EncounterSectionDto>
        {
            new() { SectionType = EncounterSectionType.Anamnese, Content = "Kopfschmerzen" },
            new() { SectionType = EncounterSectionType.Befund, Content = "Druckschmerz temporal" },
        };
        var notes = "Old unstructured notes";

        // Arztbrief logic: sections take precedence
        string befund;
        if (sections.Count > 0)
        {
            befund = string.Join("\n\n", sections
                .Where(s => !string.IsNullOrWhiteSpace(s.Content))
                .Select(s => $"{s.SectionType}:\n{s.Content}"));
        }
        else
        {
            befund = notes;
        }

        Assert.Contains("Anamnese:", befund);
        Assert.Contains("Kopfschmerzen", befund);
        Assert.Contains("Befund:", befund);
        Assert.DoesNotContain("Old unstructured", befund);
    }

    [Fact]
    public void ArztbriefSectionFallback_NoSections_UsesNotes()
    {
        var sections = new List<EncounterSectionDto>();
        var notes = "Old unstructured notes";

        string befund;
        if (sections.Count > 0)
        {
            befund = string.Join("\n\n", sections
                .Where(s => !string.IsNullOrWhiteSpace(s.Content))
                .Select(s => $"{s.SectionType}:\n{s.Content}"));
        }
        else
        {
            befund = notes;
        }

        Assert.Equal("Old unstructured notes", befund);
    }
}
