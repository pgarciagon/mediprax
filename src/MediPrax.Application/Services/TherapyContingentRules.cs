using MediPrax.Core.Enums;

namespace MediPrax.Application.Services;

public record ContingentInfo(
    int InitialSessions,
    int? MaxExtension,
    int MaxTotal,
    bool RequiresGutachter,
    int SessionDurationMinutes,
    string Description
);

public static class TherapyContingentRules
{
    private static readonly Dictionary<TherapyType, ContingentInfo> Contingents = new()
    {
        [TherapyType.PsychotherapeutischeSprechstunde] = new(6, null, 6, false, 25, "Psychotherapeutische Sprechstunde (max. 6x25 Min.)"),
        [TherapyType.Probatorik] = new(4, null, 4, false, 50, "Probatorische Sitzungen (max. 4 Sitzungen)"),
        [TherapyType.Akutbehandlung] = new(24, null, 24, false, 25, "Akutbehandlung (max. 24x25 Min.)"),
        [TherapyType.KurzzeittherapieKZT1] = new(12, null, 12, false, 50, "Kurzzeittherapie KZT1 (12 Sitzungen)"),
        [TherapyType.KurzzeittherapieKZT2] = new(24, null, 24, false, 50, "Kurzzeittherapie KZT2 (24 Sitzungen)"),
        [TherapyType.LangzeittherapieVT] = new(60, 20, 80, true, 50, "Langzeittherapie Verhaltenstherapie (bis 60/80 Sitzungen)"),
        [TherapyType.LangzeittherapieTP] = new(60, 40, 100, true, 50, "Langzeittherapie Tiefenpsychologisch (bis 60/100 Sitzungen)"),
        [TherapyType.LangzeittherapieAP] = new(160, 140, 300, true, 50, "Langzeittherapie Analytisch (bis 160/300 Sitzungen)"),
        [TherapyType.LangzeittherapieST] = new(36, 12, 48, true, 50, "Langzeittherapie Systemisch (bis 36/48 Sitzungen)"),
        [TherapyType.NeuropsychologischeTherapie] = new(60, 60, 120, true, 50, "Neuropsychologische Therapie (bis 60/120 Sitzungen)"),
    };

    public static ContingentInfo GetContingentInfo(TherapyType type) =>
        Contingents.TryGetValue(type, out var info) ? info : throw new ArgumentException($"Unknown therapy type: {type}");

    public static bool RequiresGutachter(TherapyType type) =>
        GetContingentInfo(type).RequiresGutachter;

    public static string GetDefaultGop(TherapyType type, bool isGroup) => type switch
    {
        TherapyType.PsychotherapeutischeSprechstunde => "35150",
        TherapyType.Probatorik => "35150",
        TherapyType.Akutbehandlung => "35152",
        TherapyType.KurzzeittherapieKZT1 or TherapyType.KurzzeittherapieKZT2 => isGroup ? "35163" : "35421",
        TherapyType.LangzeittherapieVT => isGroup ? "35163" : "35421",
        TherapyType.LangzeittherapieTP => isGroup ? "35173" : "35431",
        TherapyType.LangzeittherapieAP => isGroup ? "35183" : "35431",
        TherapyType.LangzeittherapieST => isGroup ? "35193" : "35441",
        TherapyType.NeuropsychologischeTherapie => "30932",
        _ => "35150"
    };

    public static int GetWarningThreshold(int approvedSessions) =>
        (int)Math.Ceiling(approvedSessions * 0.8);
}
