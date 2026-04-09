namespace MediPrax.Application.Services;

/// <summary>
/// Psychiatric-specific rules for video consultations per KBV regulations.
/// </summary>
public static class VideoConsultationRules
{
    /// <summary>
    /// First psychotherapeutic Sprechstunde must be in-person.
    /// </summary>
    public static bool IsFirstSprechstundeAllowedAsVideo(int completedSprechstunden)
        => completedSprechstunden > 0;

    /// <summary>
    /// At least 50 minutes of Sprechstunde and Probatorik must be in-person
    /// before video sessions are allowed.
    /// </summary>
    public static bool HasMinimumInPersonTime(int inPersonMinutes)
        => inPersonMinutes >= 50;

    /// <summary>
    /// GOPs that have video consultation variants (add suffix "V").
    /// </summary>
    public static readonly string[] VideoEligibleGops =
    [
        "35100", // Psychotherapeutische Sprechstunde
        "35110", // Psychotherapeutische Akutbehandlung
        "35150", // Tiefenpsychologisch fundiert Einzeltherapie
        "35151", // Tiefenpsychologisch fundiert Gruppentherapie
        "35140", // Verhaltenstherapie Einzeltherapie
        "35141", // Verhaltenstherapie Gruppentherapie
        "21220", // Psychiatrisches Gespräch
        "16220", // Neurologisches Gespräch
    ];

    /// <summary>
    /// Checks if a GOP code is eligible for video consultation.
    /// </summary>
    public static bool IsVideoEligible(string gopCode)
        => VideoEligibleGops.Contains(gopCode, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Returns the video variant GOP code (appends "V" suffix) if eligible.
    /// </summary>
    public static string? GetVideoVariantCode(string gopCode)
        => IsVideoEligible(gopCode) ? $"{gopCode}V" : null;

    /// <summary>
    /// Validates video consultation ratio for a therapy case.
    /// Recommended: at least 25% of sessions should be in-person.
    /// </summary>
    public static bool IsVideoRatioAcceptable(int videoSessions, int totalSessions)
    {
        if (totalSessions == 0) return true;
        var videoRatio = (double)videoSessions / totalSessions;
        return videoRatio <= 0.75;
    }
}
