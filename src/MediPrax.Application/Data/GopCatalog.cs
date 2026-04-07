using MediPrax.Core.Enums;

namespace MediPrax.Application.Data;

public static class GopCatalog
{
    public record GopEntry(
        string Code,
        string Description,
        BillingType Type,
        decimal Punktzahl,
        decimal? Steigerungsfaktor = null // nur GOÄ
    );

    public static IReadOnlyList<GopEntry> Search(string term, BillingType? type = null, int maxResults = 15)
    {
        if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            return [];

        var lower = term.ToLower();
        var query = Entries.AsEnumerable();

        if (type.HasValue)
            query = query.Where(e => e.Type == type.Value);

        return query
            .Where(e => e.Code.ToLower().Contains(lower) || e.Description.ToLower().Contains(lower))
            .Take(maxResults)
            .ToList();
    }

    public static GopEntry? GetByCode(string code)
        => Entries.FirstOrDefault(e => e.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

    public static decimal CalculateGoaeAmount(string code, decimal steigerungsfaktor)
    {
        var entry = GetByCode(code);
        if (entry is null || entry.Type != BillingType.GOA) return 0;
        // GOÄ: Punktzahl × Punktwert (5,82873 Cent) × Steigerungsfaktor
        return Math.Round(entry.Punktzahl * 0.0582873m * steigerungsfaktor, 2);
    }

    // Punktwert EBM 2026: ca. 4,03 Cent (angenähert)
    public static decimal CalculateEbmAmount(string code)
    {
        var entry = GetByCode(code);
        if (entry is null || entry.Type != BillingType.EBM) return 0;
        return Math.Round(entry.Punktzahl * 0.0403m, 2);
    }

    private static readonly GopEntry[] Entries =
    [
        // ========== EBM — Kapitel 16: Neurologie ==========
        new("16210", "Grundpauschale Neurologie (bis 59 J.)", BillingType.EBM, 157),
        new("16211", "Grundpauschale Neurologie (ab 60 J.)", BillingType.EBM, 179),
        new("16220", "Neurologisches Gespräch (mind. 10 Min.)", BillingType.EBM, 248),
        new("16221", "Zuschlag Gespräch (weitere 10 Min.)", BillingType.EBM, 248),
        new("16230", "Neurologische Untersuchung", BillingType.EBM, 175),
        new("16231", "Erweiterte neurologische Untersuchung", BillingType.EBM, 269),
        new("16233", "EEG", BillingType.EBM, 352),
        new("16310", "NLG / EMG", BillingType.EBM, 281),
        new("16311", "Evozierte Potentiale (VEP, AEP, SEP)", BillingType.EBM, 315),
        new("16320", "Doppler-/Duplexsonographie hirnversorgender Gefäße", BillingType.EBM, 340),
        new("16322", "Transkranielle Dopplersonographie", BillingType.EBM, 280),

        // ========== EBM — Kapitel 21: Psychiatrie ==========
        new("21210", "Grundpauschale Psychiatrie (bis 59 J.)", BillingType.EBM, 157),
        new("21211", "Grundpauschale Psychiatrie (ab 60 J.)", BillingType.EBM, 179),
        new("21213", "Zusatzpauschale psychiatrische Behandlung", BillingType.EBM, 114),
        new("21214", "Zuschlag bei schwerer psychischer Störung", BillingType.EBM, 88),
        new("21215", "Zuschlag Krisenintervention", BillingType.EBM, 235),
        new("21220", "Psychiatrisches Gespräch (mind. 10 Min.)", BillingType.EBM, 248),
        new("21221", "Zuschlag Gespräch (weitere 10 Min.)", BillingType.EBM, 248),
        new("21230", "Psychiatrische Untersuchung", BillingType.EBM, 175),
        new("21235", "Testverfahren (standardisiert)", BillingType.EBM, 196),

        // ========== EBM — Allgemeine GOPs ==========
        new("01100", "Unvorhergesehene Inanspruchnahme (außerhalb Sprechstunde)", BillingType.EBM, 125),
        new("01101", "Unvorhergesehene Inanspruchnahme (Notfall)", BillingType.EBM, 261),
        new("01321", "Grundpauschale hausärztliche Versorgung", BillingType.EBM, 115),
        new("01430", "Verwaltungskomplex", BillingType.EBM, 16),
        new("01435", "Telefonische Beratung", BillingType.EBM, 45),
        new("01820", "Rezept (Wiederholungsrezept)", BillingType.EBM, 11),
        new("40120", "Überweisung / Konsilbericht", BillingType.EBM, 49),

        // ========== GOÄ — Beratung/Untersuchung ==========
        new("1", "Beratung", BillingType.GOA, 80),
        new("3", "Eingehende Beratung (mind. 10 Min.)", BillingType.GOA, 150),
        new("4", "Erhebung Fremdanamnese", BillingType.GOA, 220),
        new("5", "Symptombezogene Untersuchung", BillingType.GOA, 80),
        new("6", "Vollständige körperliche Untersuchung", BillingType.GOA, 150),
        new("7", "Untersuchung eines Organsystems", BillingType.GOA, 120),
        new("8", "Ganzkörperstatus", BillingType.GOA, 250),
        new("15", "Einleitung/Koordination flankierender Maßnahmen", BillingType.GOA, 120),

        // ========== GOÄ — Neurologie ==========
        new("800", "Neurologische Untersuchung", BillingType.GOA, 200),
        new("801", "Eingehende neurologische Untersuchung", BillingType.GOA, 350),
        new("804", "EEG", BillingType.GOA, 450),
        new("825", "NLG (Nervenleitgeschwindigkeit)", BillingType.GOA, 200),
        new("826", "EMG (Elektromyographie)", BillingType.GOA, 250),
        new("827", "Evozierte Potentiale", BillingType.GOA, 300),
        new("829", "Doppler hirnversorgender Gefäße", BillingType.GOA, 350),

        // ========== GOÄ — Psychiatrie ==========
        new("855", "Erhebung des psychiatrischen Status", BillingType.GOA, 250),
        new("857", "Psychiatrische Behandlung (mind. 20 Min.)", BillingType.GOA, 400),
        new("860", "Erhebung einer biographischen Anamnese", BillingType.GOA, 500),
        new("861", "Tiefenpsychologisch fundierte Psychotherapie", BillingType.GOA, 650),
        new("870", "Verhaltenstherapie (Einzelsitzung)", BillingType.GOA, 650),
        new("886", "Psychiatrische Notfallbehandlung", BillingType.GOA, 400),
    ];
}
