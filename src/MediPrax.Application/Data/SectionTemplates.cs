using MediPrax.Core.Enums;

namespace MediPrax.Application.Data;

/// <summary>
/// Shared section templates for ABDTP encounter documentation.
/// Used by both EncounterFormular UI and ActionChainExecutor.
/// </summary>
public static class SectionTemplates
{
    public static Dictionary<EncounterSectionType, string> GetTemplate(string name) => name switch
    {
        "psych" => new()
        {
            [EncounterSectionType.Befund] =
                "Bewusstsein: klar / getrübt\nOrientierung: zeitlich / örtlich / situativ / zur Person — orientiert / desorientiert\nAntrieb: normal / gesteigert / vermindert\nStimmung: euthym / depressiv / gehoben / gereizt\nAffekt: moduliert / eingeengt / flach / inadäquat\nFormales Denken: geordnet / verlangsamt / beschleunigt / umständlich\nInhaltliches Denken: unauffällig / Wahn / Zwangsgedanken\nWahrnehmung: unauffällig / Halluzinationen (akustisch / optisch)\nIch-Störungen: keine / Gedankeneingebung / -entzug / -ausbreitung\nSuizidalität: keine / Gedanken / Absichten / Pläne"
        },
        "neuro" => new()
        {
            [EncounterSectionType.Befund] =
                "Hirnnerven: unauffällig / Auffälligkeiten:\nMotorik: Tonus normal / erhöht / schlaff; Kraft seitengleich / Paresen:\nReflexe: seitengleich / Seitendifferenz; Pathologische Reflexe:\nSensibilität: unauffällig / Hypästhesie / Parästhesien:\nKoordination: Finger-Nase-Versuch / Knie-Hacke-Versuch — unauffällig / Dysmetrie\nGang: sicher / unsicher / Ataxie / Spastik\nSprache: unauffällig / Dysarthrie / Aphasie"
        },
        "kurz" => new()
        {
            [EncounterSectionType.Anamnese] = "Aktueller Zustand:",
            [EncounterSectionType.Therapie] = "Medikation: unverändert / Anpassung:",
            [EncounterSectionType.Procedere] = "Nächster Termin:"
        },
        _ => new()
    };

    /// <summary>Available template names with German labels.</summary>
    public static IReadOnlyList<(string Key, string Label)> Available { get; } =
    [
        ("psych", "Psychiatrisch"),
        ("neuro", "Neurologisch"),
        ("kurz", "Kurzkonsultation")
    ];
}
