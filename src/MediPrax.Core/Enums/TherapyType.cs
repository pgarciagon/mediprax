namespace MediPrax.Core.Enums;

public enum TherapyType
{
    PsychotherapeutischeSprechstunde,  // Max 6x25min
    Probatorik,                        // Max 4-6 sessions
    Akutbehandlung,                    // Max 24x25min or 12x50min
    KurzzeittherapieKZT1,             // 12 sessions
    KurzzeittherapieKZT2,             // 12 more sessions (24 total)
    LangzeittherapieVT,               // Verhaltenstherapie up to 60/80
    LangzeittherapieTP,               // Tiefenpsychologisch up to 60/100
    LangzeittherapieAP,               // Analytisch up to 160/300
    LangzeittherapieST,               // Systemisch up to 36/48
    NeuropsychologischeTherapie        // up to 60/120
}
