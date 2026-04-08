using MediPrax.Application.Services;
using MediPrax.Core.Enums;

namespace MediPrax.UnitTests;

public class TherapyContingentRulesTests
{
    [Theory]
    [InlineData(TherapyType.PsychotherapeutischeSprechstunde, 6, 6, false, 25)]
    [InlineData(TherapyType.Probatorik, 4, 4, false, 50)]
    [InlineData(TherapyType.Akutbehandlung, 24, 24, false, 25)]
    [InlineData(TherapyType.KurzzeittherapieKZT1, 12, 12, false, 50)]
    [InlineData(TherapyType.KurzzeittherapieKZT2, 24, 24, false, 50)]
    [InlineData(TherapyType.LangzeittherapieVT, 60, 80, true, 50)]
    [InlineData(TherapyType.LangzeittherapieTP, 60, 100, true, 50)]
    [InlineData(TherapyType.LangzeittherapieAP, 160, 300, true, 50)]
    [InlineData(TherapyType.LangzeittherapieST, 36, 48, true, 50)]
    [InlineData(TherapyType.NeuropsychologischeTherapie, 60, 120, true, 50)]
    public void GetContingentInfo_ReturnsCorrectData(TherapyType type, int initial, int maxTotal, bool gutachter, int duration)
    {
        var info = TherapyContingentRules.GetContingentInfo(type);

        Assert.Equal(initial, info.InitialSessions);
        Assert.Equal(maxTotal, info.MaxTotal);
        Assert.Equal(gutachter, info.RequiresGutachter);
        Assert.Equal(duration, info.SessionDurationMinutes);
    }

    [Theory]
    [InlineData(TherapyType.LangzeittherapieVT, true)]
    [InlineData(TherapyType.LangzeittherapieTP, true)]
    [InlineData(TherapyType.LangzeittherapieAP, true)]
    [InlineData(TherapyType.LangzeittherapieST, true)]
    [InlineData(TherapyType.NeuropsychologischeTherapie, true)]
    public void RequiresGutachter_ReturnsTrueForLztTypes(TherapyType type, bool expected)
    {
        Assert.Equal(expected, TherapyContingentRules.RequiresGutachter(type));
    }

    [Theory]
    [InlineData(TherapyType.PsychotherapeutischeSprechstunde, false)]
    [InlineData(TherapyType.Probatorik, false)]
    [InlineData(TherapyType.Akutbehandlung, false)]
    [InlineData(TherapyType.KurzzeittherapieKZT1, false)]
    [InlineData(TherapyType.KurzzeittherapieKZT2, false)]
    public void RequiresGutachter_ReturnsFalseForKztTypes(TherapyType type, bool expected)
    {
        Assert.Equal(expected, TherapyContingentRules.RequiresGutachter(type));
    }

    [Theory]
    [InlineData(12, 10)]   // ceil(12 * 0.8) = 10
    [InlineData(24, 20)]   // ceil(24 * 0.8) = 20
    [InlineData(60, 48)]   // ceil(60 * 0.8) = 48
    [InlineData(6, 5)]     // ceil(6 * 0.8) = 5
    public void GetWarningThreshold_Returns80Percent(int approved, int expected)
    {
        Assert.Equal(expected, TherapyContingentRules.GetWarningThreshold(approved));
    }

    [Theory]
    [InlineData(TherapyType.LangzeittherapieVT, false, "35421")]
    [InlineData(TherapyType.LangzeittherapieTP, false, "35431")]
    [InlineData(TherapyType.LangzeittherapieAP, false, "35431")]
    [InlineData(TherapyType.LangzeittherapieST, false, "35441")]
    [InlineData(TherapyType.LangzeittherapieVT, true, "35163")]
    [InlineData(TherapyType.NeuropsychologischeTherapie, false, "30932")]
    public void GetDefaultGop_ReturnsCorrectGop(TherapyType type, bool isGroup, string expectedGop)
    {
        Assert.Equal(expectedGop, TherapyContingentRules.GetDefaultGop(type, isGroup));
    }
}
