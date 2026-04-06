namespace MediPrax.Application.Data;

public static class Icd10Catalog
{
    public record Icd10Entry(string Code, string Description);

    public static IReadOnlyList<Icd10Entry> Search(string term, int maxResults = 15)
    {
        if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            return [];

        var lower = term.ToLower();
        return Entries
            .Where(e => e.Code.ToLower().Contains(lower) || e.Description.ToLower().Contains(lower))
            .Take(maxResults)
            .ToList();
    }

    public static string? GetDescription(string code)
        => Entries.FirstOrDefault(e => e.Code.Equals(code, StringComparison.OrdinalIgnoreCase))?.Description;

    // Psychiatrie & Neurologie — häufigste ICD-10-GM Codes
    private static readonly Icd10Entry[] Entries =
    [
        // Organische Störungen (F00-F09)
        new("F00.0", "Demenz bei Alzheimer-Krankheit, mit frühem Beginn"),
        new("F00.1", "Demenz bei Alzheimer-Krankheit, mit spätem Beginn"),
        new("F01.0", "Vaskuläre Demenz mit akutem Beginn"),
        new("F01.1", "Multiinfarkt-Demenz"),
        new("F05.0", "Delir ohne Demenz"),
        new("F06.7", "Leichte kognitive Störung"),

        // Störungen durch psychotrope Substanzen (F10-F19)
        new("F10.0", "Psychische Störungen durch Alkohol — Akute Intoxikation"),
        new("F10.1", "Psychische Störungen durch Alkohol — Schädlicher Gebrauch"),
        new("F10.2", "Psychische Störungen durch Alkohol — Abhängigkeitssyndrom"),
        new("F11.2", "Psychische Störungen durch Opioide — Abhängigkeitssyndrom"),
        new("F13.2", "Psychische Störungen durch Sedativa — Abhängigkeitssyndrom"),
        new("F17.1", "Psychische Störungen durch Tabak — Schädlicher Gebrauch"),

        // Schizophrenie (F20-F29)
        new("F20.0", "Paranoide Schizophrenie"),
        new("F20.1", "Hebephrene Schizophrenie"),
        new("F22.0", "Wahnhafte Störung"),
        new("F23.0", "Akute polymorphe psychotische Störung ohne Schizophrenie"),
        new("F25.0", "Schizoaffektive Störung, gegenwärtig manisch"),
        new("F25.1", "Schizoaffektive Störung, gegenwärtig depressiv"),

        // Affektive Störungen (F30-F39)
        new("F31.0", "Bipolare affektive Störung, gegenwärtig hypomanisch"),
        new("F31.3", "Bipolare affektive Störung, gegenwärtig leichte depressive Episode"),
        new("F31.4", "Bipolare affektive Störung, gegenwärtig schwere depressive Episode"),
        new("F32.0", "Leichte depressive Episode"),
        new("F32.1", "Mittelgradige depressive Episode"),
        new("F32.2", "Schwere depressive Episode ohne psychotische Symptome"),
        new("F32.3", "Schwere depressive Episode mit psychotischen Symptomen"),
        new("F33.0", "Rezidivierende depressive Störung, leichte Episode"),
        new("F33.1", "Rezidivierende depressive Störung, mittelgradige Episode"),
        new("F33.2", "Rezidivierende depressive Störung, schwere Episode"),
        new("F34.1", "Dysthymia"),

        // Neurotische Störungen (F40-F48)
        new("F40.0", "Agoraphobie"),
        new("F40.1", "Soziale Phobien"),
        new("F41.0", "Panikstörung"),
        new("F41.1", "Generalisierte Angststörung"),
        new("F41.2", "Angst und depressive Störung, gemischt"),
        new("F42.0", "Vorwiegend Zwangsgedanken"),
        new("F42.1", "Vorwiegend Zwangshandlungen"),
        new("F43.0", "Akute Belastungsreaktion"),
        new("F43.1", "Posttraumatische Belastungsstörung"),
        new("F43.2", "Anpassungsstörungen"),
        new("F44.0", "Dissoziative Amnesie"),
        new("F45.0", "Somatisierungsstörung"),
        new("F45.2", "Hypochondrische Störung"),
        new("F48.0", "Neurasthenie"),

        // Verhaltensauffälligkeiten (F50-F59)
        new("F50.0", "Anorexia nervosa"),
        new("F50.2", "Bulimia nervosa"),
        new("F51.0", "Nichtorganische Insomnie"),

        // Persönlichkeitsstörungen (F60-F69)
        new("F60.2", "Dissoziale Persönlichkeitsstörung"),
        new("F60.3", "Emotional instabile Persönlichkeitsstörung"),
        new("F60.5", "Anankastische Persönlichkeitsstörung"),
        new("F60.6", "Ängstliche (vermeidende) Persönlichkeitsstörung"),

        // ADHS
        new("F90.0", "Einfache Aktivitäts- und Aufmerksamkeitsstörung"),
        new("F90.1", "Hyperkinetische Störung des Sozialverhaltens"),

        // Neurologie (G00-G99)
        new("G20.0", "Primäres Parkinson-Syndrom mit keiner oder geringer Beeinträchtigung"),
        new("G20.1", "Primäres Parkinson-Syndrom mit mäßiger bis schwerer Beeinträchtigung"),
        new("G25.0", "Essentieller Tremor"),
        new("G30.0", "Alzheimer-Krankheit mit frühem Beginn"),
        new("G30.1", "Alzheimer-Krankheit mit spätem Beginn"),
        new("G35.0", "Erstmanifestation einer Multiplen Sklerose"),
        new("G35.1", "Multiple Sklerose mit vorherrschend schubförmigem Verlauf"),
        new("G40.0", "Lokalisationsbezogene Epilepsie, idiopathisch"),
        new("G40.2", "Lokalisationsbezogene Epilepsie, symptomatisch"),
        new("G43.0", "Migräne ohne Aura"),
        new("G43.1", "Migräne mit Aura"),
        new("G44.2", "Spannungskopfschmerz"),
        new("G47.3", "Schlafapnoe"),
        new("G50.0", "Trigeminusneuralgie"),
        new("G51.0", "Fazialisparese"),
        new("G62.1", "Alkoholpolyneuropathie"),
        new("G62.9", "Polyneuropathie, nicht näher bezeichnet"),
        new("G80.0", "Spastische tetraplegische Zerebralparese"),
        new("G93.3", "Chronisches Müdigkeitssyndrom"),

        // Schlaganfall (I60-I69)
        new("I63.0", "Hirninfarkt durch Thrombose präzerebraler Arterien"),
        new("I63.3", "Hirninfarkt durch Thrombose zerebraler Arterien"),
        new("I63.9", "Hirninfarkt, nicht näher bezeichnet"),
        new("I67.2", "Zerebrale Atherosklerose"),

        // Symptome (R-Codes)
        new("R41.3", "Sonstige Amnesie"),
        new("R41.8", "Sonstige Symptome, kognitive Funktionen betreffend"),
        new("R45.1", "Ruhelosigkeit und Erregung"),
        new("R51", "Kopfschmerz"),
        new("R56.0", "Fieberkrämpfe"),

        // Z-Codes (Faktoren)
        new("Z73.0", "Ausgebranntsein (Burn-out)"),
        new("Z76.5", "Malingering (Simulation)"),
    ];
}
