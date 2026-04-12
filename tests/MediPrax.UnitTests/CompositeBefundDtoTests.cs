using MediPrax.Application.DTOs;

namespace MediPrax.UnitTests;

public class CompositeBefundDtoTests
{
    [Fact]
    public void ComposedText_AllParts_JoinsWithDoubleNewline()
    {
        var dto = new CompositeBefundDto
        {
            PsychopathNarrative = "Bewusstsein klar, allseits orientiert",
            PsychopathFindingId = Guid.NewGuid(),
            NeuroNarrative = "Hirnnerven o.p.B., Kraft 5/5",
            NeuroExamId = Guid.NewGuid(),
            FreitextBefund = "Zusaetzlich: RR 130/80"
        };

        var text = dto.ComposedText;

        Assert.Contains("Psychopathologischer Befund: Bewusstsein klar", text);
        Assert.Contains("Neurologischer Befund: Hirnnerven o.p.B.", text);
        Assert.Contains("Zusaetzlich: RR 130/80", text);
        // Three parts separated by double newlines → Split produces 3 elements
        Assert.Equal(3, text.Split("\n\n").Length);
    }

    [Fact]
    public void ComposedText_OnlyFreitext_NoHeaders()
    {
        var dto = new CompositeBefundDto { FreitextBefund = "RR 130/80" };

        Assert.Equal("RR 130/80", dto.ComposedText);
        Assert.DoesNotContain("Psychopathologischer Befund:", dto.ComposedText);
        Assert.DoesNotContain("Neurologischer Befund:", dto.ComposedText);
    }

    [Fact]
    public void ComposedText_OnlyPsychopath_ReturnsPrefixed()
    {
        var dto = new CompositeBefundDto
        {
            PsychopathNarrative = "Bewusstsein klar",
            PsychopathFindingId = Guid.NewGuid()
        };

        Assert.StartsWith("Psychopathologischer Befund: Bewusstsein klar", dto.ComposedText);
    }

    [Fact]
    public void ComposedText_Empty_ReturnsEmpty()
    {
        var dto = new CompositeBefundDto();
        Assert.Equal(string.Empty, dto.ComposedText);
    }

    [Fact]
    public void ComposedText_WhitespaceOnly_ReturnsEmpty()
    {
        var dto = new CompositeBefundDto
        {
            PsychopathNarrative = "   ",
            FreitextBefund = "  \n  "
        };
        Assert.Equal(string.Empty, dto.ComposedText);
    }

    [Fact]
    public void HasStructuredFindings_WithPsychopathId_ReturnsTrue()
    {
        var dto = new CompositeBefundDto { PsychopathFindingId = Guid.NewGuid() };
        Assert.True(dto.HasStructuredFindings);
    }

    [Fact]
    public void HasStructuredFindings_WithNeuroId_ReturnsTrue()
    {
        var dto = new CompositeBefundDto { NeuroExamId = Guid.NewGuid() };
        Assert.True(dto.HasStructuredFindings);
    }

    [Fact]
    public void HasStructuredFindings_NeitherPresent_ReturnsFalse()
    {
        var dto = new CompositeBefundDto { FreitextBefund = "Some text" };
        Assert.False(dto.HasStructuredFindings);
    }

    [Fact]
    public void ComposedText_PsychopathAndNeuro_NoFreitext_HasTwoParts()
    {
        var dto = new CompositeBefundDto
        {
            PsychopathNarrative = "AMDP text",
            PsychopathFindingId = Guid.NewGuid(),
            NeuroNarrative = "Neuro text",
            NeuroExamId = Guid.NewGuid()
        };

        var parts = dto.ComposedText.Split("\n\n");
        Assert.Equal(2, parts.Length);
        Assert.StartsWith("Psychopathologischer Befund:", parts[0]);
        Assert.StartsWith("Neurologischer Befund:", parts[1]);
    }
}
