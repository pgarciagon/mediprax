using MediPrax.Core.Enums;

namespace MediPrax.Application.Catalogs;

public record TestItem(int Number, string Text, IReadOnlyList<AnswerOption> Options);
public record AnswerOption(string Text, int Value);
public record ScoreRange(int Min, int Max, string Label, string Description);

public class TestDefinition
{
    public PsychometricTestType TestType { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Abbreviation { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Instruction { get; init; } = string.Empty;
    public IReadOnlyList<TestItem> Items { get; init; } = [];
    public IReadOnlyList<ScoreRange> ScoreRanges { get; init; } = [];
    public int MaxScore { get; init; }
    public string? SuggestedGop { get; init; }
}

public static class PsychometricTestCatalog
{
    private static readonly Dictionary<PsychometricTestType, TestDefinition> _definitions;

    static PsychometricTestCatalog()
    {
        _definitions = new[]
        {
            BuildPhq9(),
            BuildGad7(),
            BuildWho5(),
            BuildAudit(),
            BuildMmse(),
            BuildCgi(),
            BuildBdi2(),
            BuildMoCA(),
            BuildPanss(),
            BuildPcl5(),
            BuildPsqi(),
            BuildHamd(),
            BuildBprs(),
            BuildClockDrawing()
        }.ToDictionary(d => d.TestType);
    }

    public static TestDefinition GetDefinition(PsychometricTestType type)
        => _definitions.TryGetValue(type, out var def)
            ? def
            : throw new ArgumentOutOfRangeException(nameof(type), $"No definition for {type}");

    public static IReadOnlyList<TestDefinition> AllDefinitions => _definitions.Values.ToList().AsReadOnly();

    public static string CalculateInterpretation(PsychometricTestType type, int score)
    {
        var def = GetDefinition(type);
        var range = def.ScoreRanges.FirstOrDefault(r => score >= r.Min && score <= r.Max);
        return range?.Label ?? "Nicht klassifizierbar";
    }

    // ─────────────────────────────────────────────────────────────────
    // PHQ-9: Patient Health Questionnaire — Depression (9 items, 0-27)
    // ─────────────────────────────────────────────────────────────────
    private static TestDefinition BuildPhq9()
    {
        var options = new AnswerOption[]
        {
            new("Überhaupt nicht", 0),
            new("An einzelnen Tagen", 1),
            new("An mehr als der Hälfte der Tage", 2),
            new("Beinahe jeden Tag", 3)
        };

        return new TestDefinition
        {
            TestType = PsychometricTestType.PHQ9,
            Name = "Patient Health Questionnaire-9",
            Abbreviation = "PHQ-9",
            Description = "Screening-Fragebogen zur Erfassung des Schweregrades einer depressiven Störung",
            Instruction = "Wie oft haben Sie in den letzten 2 Wochen unter den folgenden Beschwerden gelitten?",
            MaxScore = 27,
            SuggestedGop = "35600",
            Items =
            [
                new(1, "Wenig Interesse oder Freude an Ihren Tätigkeiten", options),
                new(2, "Niedergeschlagenheit, Schwermut oder Hoffnungslosigkeit", options),
                new(3, "Schwierigkeiten ein- oder durchzuschlafen oder vermehrter Schlaf", options),
                new(4, "Müdigkeit oder Gefühl, keine Energie zu haben", options),
                new(5, "Verminderter Appetit oder übermäßiges Bedürfnis zu essen", options),
                new(6, "Schlechte Meinung von sich selbst", options),
                new(7, "Schwierigkeiten, sich auf etwas zu konzentrieren", options),
                new(8, "Verlangsamung oder Unruhe in Bewegung/Sprache", options),
                new(9, "Gedanken, dass Sie lieber tot wären oder sich Leid zufügen möchten", options)
            ],
            ScoreRanges =
            [
                new(0, 4, "Minimal", "Keine oder minimale depressive Symptomatik"),
                new(5, 9, "Leicht", "Leichte depressive Symptomatik"),
                new(10, 14, "Mittelgradig", "Mittelgradige depressive Symptomatik"),
                new(15, 19, "Mittelschwer", "Mittelschwere depressive Symptomatik"),
                new(20, 27, "Schwer", "Schwere depressive Symptomatik")
            ]
        };
    }

    // ─────────────────────────────────────────────────────────────────
    // GAD-7: Generalized Anxiety Disorder (7 items, 0-21)
    // ─────────────────────────────────────────────────────────────────
    private static TestDefinition BuildGad7()
    {
        var options = new AnswerOption[]
        {
            new("Überhaupt nicht", 0),
            new("An einzelnen Tagen", 1),
            new("An mehr als der Hälfte der Tage", 2),
            new("Beinahe jeden Tag", 3)
        };

        return new TestDefinition
        {
            TestType = PsychometricTestType.GAD7,
            Name = "Generalized Anxiety Disorder-7",
            Abbreviation = "GAD-7",
            Description = "Screening-Fragebogen zur Erfassung generalisierter Angststörungen",
            Instruction = "Wie oft haben Sie in den letzten 2 Wochen unter den folgenden Beschwerden gelitten?",
            MaxScore = 21,
            SuggestedGop = "35600",
            Items =
            [
                new(1, "Nervosität, Ängstlichkeit oder Anspannung", options),
                new(2, "Nicht in der Lage, Sorgen zu stoppen oder zu kontrollieren", options),
                new(3, "Übermäßige Sorgen bezüglich verschiedener Angelegenheiten", options),
                new(4, "Schwierigkeiten zu entspannen", options),
                new(5, "Rastlosigkeit, so dass Stillsitzen schwerfällt", options),
                new(6, "Schnelle Verärgerung oder Gereiztheit", options),
                new(7, "Gefühl der Angst, als würde etwas Schlimmes passieren", options)
            ],
            ScoreRanges =
            [
                new(0, 4, "Minimal", "Keine oder minimale Angstsymptomatik"),
                new(5, 9, "Leicht", "Leichte Angstsymptomatik"),
                new(10, 14, "Mittelgradig", "Mittelgradige Angstsymptomatik"),
                new(15, 21, "Schwer", "Schwere Angstsymptomatik")
            ]
        };
    }

    // ─────────────────────────────────────────────────────────────────
    // WHO-5: WHO-5 Wellbeing Index (5 items, 0-25, ×4 for percentage)
    // ─────────────────────────────────────────────────────────────────
    private static TestDefinition BuildWho5()
    {
        var options = new AnswerOption[]
        {
            new("Zu keinem Zeitpunkt", 0),
            new("Ab und zu", 1),
            new("Weniger als die Hälfte der Zeit", 2),
            new("Etwas mehr als die Hälfte der Zeit", 3),
            new("Meistens", 4),
            new("Die ganze Zeit", 5)
        };

        return new TestDefinition
        {
            TestType = PsychometricTestType.WHO5,
            Name = "WHO-5 Wohlbefindens-Index",
            Abbreviation = "WHO-5",
            Description = "Kurzfragebogen zur Erfassung des aktuellen Wohlbefindens",
            Instruction = "In den letzten zwei Wochen...",
            MaxScore = 25,
            SuggestedGop = "35600",
            Items =
            [
                new(1, "...war ich froh und guter Laune", options),
                new(2, "...habe ich mich ruhig und entspannt gefühlt", options),
                new(3, "...habe ich mich energisch und aktiv gefühlt", options),
                new(4, "...habe ich mich beim Aufwachen frisch und ausgeruht gefühlt", options),
                new(5, "...war mein Alltag voller Dinge, die mich interessieren", options)
            ],
            ScoreRanges =
            [
                new(0, 8, "Schwere Depression wahrscheinlich", "Rohwert 0-8 (0-32%): Screening auf Depression dringend empfohlen"),
                new(9, 12, "Niedrig", "Rohwert 9-12 (36-48%): Niedriges Wohlbefinden, weiteres Screening empfohlen"),
                new(13, 25, "Gut", "Rohwert 13-25 (52-100%): Gutes Wohlbefinden")
            ]
        };
    }

    // ─────────────────────────────────────────────────────────────────
    // AUDIT: Alcohol Use Disorders Identification Test (10 items, 0-40)
    // ─────────────────────────────────────────────────────────────────
    private static TestDefinition BuildAudit()
    {
        var frequencyOptions = new AnswerOption[]
        {
            new("Nie", 0),
            new("Einmal im Monat oder seltener", 1),
            new("2- bis 4-mal im Monat", 2),
            new("2- bis 3-mal pro Woche", 3),
            new("4-mal oder öfter pro Woche", 4)
        };

        var amountOptions = new AnswerOption[]
        {
            new("1 oder 2", 0),
            new("3 oder 4", 1),
            new("5 oder 6", 2),
            new("7 bis 9", 3),
            new("10 oder mehr", 4)
        };

        var frequencyDetailOptions = new AnswerOption[]
        {
            new("Nie", 0),
            new("Seltener als einmal im Monat", 1),
            new("Einmal im Monat", 2),
            new("Einmal pro Woche", 3),
            new("Täglich oder fast täglich", 4)
        };

        var yesNoOptions = new AnswerOption[]
        {
            new("Nein", 0),
            new("Ja, aber nicht im letzten Jahr", 2),
            new("Ja, im letzten Jahr", 4)
        };

        return new TestDefinition
        {
            TestType = PsychometricTestType.AUDIT,
            Name = "Alcohol Use Disorders Identification Test",
            Abbreviation = "AUDIT",
            Description = "WHO-Fragebogen zur Identifikation von Alkoholkonsumstörungen",
            Instruction = "Bitte beantworten Sie die folgenden Fragen zu Ihrem Alkoholkonsum. Beantworten Sie die Fragen so ehrlich wie möglich.",
            MaxScore = 40,
            SuggestedGop = "35600",
            Items =
            [
                new(1, "Wie oft trinken Sie Alkohol?", frequencyOptions),
                new(2, "Wenn Sie Alkohol trinken, wie viele Gläser trinken Sie dann üblicherweise an einem Tag? (Ein Glas entspricht 0,33l Bier, 0,25l Wein/Sekt, oder einem einfachen Schnaps)", amountOptions),
                new(3, "Wie oft trinken Sie 6 oder mehr Gläser bei einer Gelegenheit?", frequencyDetailOptions),
                new(4, "Wie oft konnten Sie in den letzten 12 Monaten nicht mehr aufhören zu trinken, nachdem Sie einmal angefangen hatten?", frequencyDetailOptions),
                new(5, "Wie oft konnten Sie in den letzten 12 Monaten Ihren Verpflichtungen nicht mehr nachkommen, weil Sie getrunken hatten?", frequencyDetailOptions),
                new(6, "Wie oft brauchten Sie in den letzten 12 Monaten morgens ein alkoholisches Getränk, um in Gang zu kommen?", frequencyDetailOptions),
                new(7, "Wie oft hatten Sie in den letzten 12 Monaten Schuldgefühle oder ein schlechtes Gewissen, weil Sie getrunken hatten?", frequencyDetailOptions),
                new(8, "Wie oft waren Sie in den letzten 12 Monaten nicht in der Lage, sich an Ereignisse der letzten Nacht zu erinnern, weil Sie getrunken hatten?", frequencyDetailOptions),
                new(9, "Haben Sie sich oder hat sich eine andere Person schon einmal verletzt, weil Sie getrunken hatten?", yesNoOptions),
                new(10, "Hat ein Verwandter, Freund, Arzt oder eine andere Person Ihr Trinkverhalten angesprochen oder Ihnen geraten, Ihren Alkoholkonsum zu verringern?", yesNoOptions)
            ],
            ScoreRanges =
            [
                new(0, 7, "Risikoarmer Konsum", "Kein Handlungsbedarf"),
                new(8, 15, "Riskanter Konsum", "Kurzintervention empfohlen"),
                new(16, 19, "Schädlicher Konsum", "Kurzintervention und weitergehende Diagnostik empfohlen"),
                new(20, 40, "Wahrscheinliche Abhängigkeit", "Überweisung an Suchtberatung/Fachklinik empfohlen")
            ]
        };
    }

    // ─────────────────────────────────────────────────────────────────
    // MMSE: Mini Mental State Examination (11 sections, 0-30)
    // ─────────────────────────────────────────────────────────────────
    private static TestDefinition BuildMmse()
    {
        // Each section is scored 0 to max points for that section.
        // We model each section as one item with options 0..max.

        static AnswerOption[] PointsRange(int max)
            => Enumerable.Range(0, max + 1)
                .Select(i => new AnswerOption($"{i} Punkt(e)", i))
                .ToArray();

        return new TestDefinition
        {
            TestType = PsychometricTestType.MMSE,
            Name = "Mini-Mental-Status-Test",
            Abbreviation = "MMSE",
            Description = "Screening-Instrument zur Erfassung kognitiver Störungen",
            Instruction = "Bitte bewerten Sie die folgenden kognitiven Bereiche des Patienten. Vergeben Sie die erreichte Punktzahl pro Abschnitt.",
            MaxScore = 30,
            SuggestedGop = "35600",
            Items =
            [
                new(1, "Zeitliche Orientierung: Jahr, Jahreszeit, Datum, Wochentag, Monat (je 1 Punkt)", PointsRange(5)),
                new(2, "Örtliche Orientierung: Staat, Land, Stadt, Klinik/Praxis, Stockwerk (je 1 Punkt)", PointsRange(5)),
                new(3, "Aufnahmefähigkeit: 3 Wörter nachsprechen — z.B. Auto, Blume, Kerze (je 1 Punkt)", PointsRange(3)),
                new(4, "Aufmerksamkeit und Rechnen: Von 100 in 7er-Schritten rückwärts zählen (5 Subtraktionen, je 1 Punkt)", PointsRange(5)),
                new(5, "Erinnerungsfähigkeit: Die 3 Wörter aus Aufgabe 3 erinnern (je 1 Punkt)", PointsRange(3)),
                new(6, "Benennen: Zwei Gegenstände benennen — Bleistift, Armbanduhr (je 1 Punkt)", PointsRange(2)),
                new(7, "Nachsprechen: 'Kein Wenn und Aber' (1 Punkt)", PointsRange(1)),
                new(8, "Dreiteiliger Befehl: Nehmen Sie das Blatt, falten Sie es in der Mitte, legen Sie es auf den Boden (je 1 Punkt)", PointsRange(3)),
                new(9, "Lesen und Befolgen: 'Schließen Sie die Augen' — Patient liest und führt aus (1 Punkt)", PointsRange(1)),
                new(10, "Schreiben: Einen vollständigen Satz schreiben (1 Punkt)", PointsRange(1)),
                new(11, "Abzeichnen: Zwei sich überschneidende Fünfecke abzeichnen (1 Punkt)", PointsRange(1))
            ],
            ScoreRanges =
            [
                new(27, 30, "Normal", "Keine kognitive Beeinträchtigung"),
                new(20, 26, "Leichte kognitive Beeinträchtigung", "Weitere Diagnostik empfohlen"),
                new(10, 19, "Mittelschwere Demenz", "Therapie und Betreuungsplanung empfohlen"),
                new(0, 9, "Schwere Demenz", "Umfassende Versorgungsplanung erforderlich")
            ]
        };
    }

    // ─────────────────────────────────────────────────────────────────
    // CGI: Clinical Global Impression (2 items)
    // ─────────────────────────────────────────────────────────────────
    private static TestDefinition BuildCgi()
    {
        var severityOptions = new AnswerOption[]
        {
            new("Nicht beurteilbar", 0),
            new("1 — Patient ist überhaupt nicht krank", 1),
            new("2 — Patient ist ein Grenzfall psychiatrischer Erkrankung", 2),
            new("3 — Patient ist nur leicht krank", 3),
            new("4 — Patient ist mäßig krank", 4),
            new("5 — Patient ist deutlich krank", 5),
            new("6 — Patient ist schwer krank", 6),
            new("7 — Patient gehört zu den extrem schwer Kranken", 7)
        };

        var improvementOptions = new AnswerOption[]
        {
            new("Nicht beurteilbar", 0),
            new("1 — Zustand ist sehr viel besser", 1),
            new("2 — Zustand ist viel besser", 2),
            new("3 — Zustand ist nur wenig besser", 3),
            new("4 — Zustand ist unverändert", 4),
            new("5 — Zustand ist etwas schlechter", 5),
            new("6 — Zustand ist viel schlechter", 6),
            new("7 — Zustand ist sehr viel schlechter", 7)
        };

        return new TestDefinition
        {
            TestType = PsychometricTestType.CGI,
            Name = "Clinical Global Impression",
            Abbreviation = "CGI",
            Description = "Globale klinische Einschätzung des Schweregrades und der Veränderung",
            Instruction = "Bitte bewerten Sie den aktuellen Schweregrad der Erkrankung (CGI-S) und die Veränderung seit Behandlungsbeginn (CGI-I).",
            MaxScore = 14,
            SuggestedGop = "35600",
            Items =
            [
                new(1, "CGI-S (Schweregrad): Wie schwer ist der Patient zum jetzigen Zeitpunkt erkrankt?", severityOptions),
                new(2, "CGI-I (Verbesserung): Wie sehr hat sich der Zustand des Patienten seit Behandlungsbeginn verändert?", improvementOptions)
            ],
            ScoreRanges =
            [
                new(0, 4, "Leicht/Gebessert", "Schweregrad gering und/oder Verbesserung erkennbar"),
                new(5, 8, "Mittelgradig", "Mittelgradige Ausprägung"),
                new(9, 14, "Schwer/Verschlechtert", "Schwere Ausprägung und/oder Verschlechterung")
            ]
        };
    }

    // ─────────────────────────────────────────────────────────────────
    // Stub definitions for remaining test types
    // ─────────────────────────────────────────────────────────────────

    private static TestDefinition BuildBdi2()
    {
        var options = new AnswerOption[]
        {
            new("0", 0), new("1", 1), new("2", 2), new("3", 3)
        };

        return new TestDefinition
        {
            TestType = PsychometricTestType.BDI2,
            Name = "Beck-Depressions-Inventar II",
            Abbreviation = "BDI-II",
            Description = "Selbstbeurteilungsbogen zur Erfassung der Schwere depressiver Symptome",
            Instruction = "Bitte lesen Sie jede Gruppe von Aussagen sorgfältig durch und wählen Sie die Aussage, die am besten beschreibt, wie Sie sich in den letzten zwei Wochen gefühlt haben.",
            MaxScore = 63,
            SuggestedGop = "35600",
            Items = Enumerable.Range(1, 21).Select(i => new TestItem(i, $"BDI-II Item {i}", options)).ToList(),
            ScoreRanges =
            [
                new(0, 8, "Keine Depression", "Minimal"),
                new(9, 13, "Minimale Depression", "Leichte depressive Symptome"),
                new(14, 19, "Leichte Depression", "Leichte depressive Störung"),
                new(20, 28, "Mittelgradige Depression", "Mittelgradige depressive Störung"),
                new(29, 63, "Schwere Depression", "Schwere depressive Störung")
            ]
        };
    }

    private static TestDefinition BuildMoCA()
    {
        static AnswerOption[] PointsRange(int max)
            => Enumerable.Range(0, max + 1).Select(i => new AnswerOption($"{i} Punkt(e)", i)).ToArray();

        return new TestDefinition
        {
            TestType = PsychometricTestType.MoCA,
            Name = "Montreal Cognitive Assessment",
            Abbreviation = "MoCA",
            Description = "Screening-Instrument für leichte kognitive Beeinträchtigungen",
            Instruction = "Bitte bewerten Sie die folgenden kognitiven Bereiche des Patienten.",
            MaxScore = 30,
            SuggestedGop = "35600",
            Items =
            [
                new(1, "Visuell-räumlich / Exekutiv (Trail, Würfel, Uhr)", PointsRange(5)),
                new(2, "Benennen (Löwe, Nashorn, Kamel)", PointsRange(3)),
                new(3, "Aufmerksamkeit (Zahlenspanne, Buchstabenaufgabe, Serielle Subtraktion)", PointsRange(6)),
                new(4, "Sprache (Satz nachsprechen, Wortflüssigkeit)", PointsRange(3)),
                new(5, "Abstraktion (Gemeinsamkeiten finden)", PointsRange(2)),
                new(6, "Verzögertes Erinnern (5 Wörter)", PointsRange(5)),
                new(7, "Orientierung (Datum, Monat, Jahr, Wochentag, Ort, Stadt)", PointsRange(6))
            ],
            ScoreRanges =
            [
                new(26, 30, "Normal", "Keine kognitive Beeinträchtigung"),
                new(18, 25, "Leichte kognitive Beeinträchtigung", "Weitere Diagnostik empfohlen"),
                new(10, 17, "Moderate kognitive Beeinträchtigung", "Umfassende neuropsychologische Untersuchung empfohlen"),
                new(0, 9, "Schwere kognitive Beeinträchtigung", "Dringend weitere Abklärung erforderlich")
            ]
        };
    }

    private static TestDefinition BuildPanss()
    {
        var options = new AnswerOption[]
        {
            new("1 — Nicht vorhanden", 1),
            new("2 — Minimal", 2),
            new("3 — Leicht", 3),
            new("4 — Mäßig", 4),
            new("5 — Mäßig schwer", 5),
            new("6 — Schwer", 6),
            new("7 — Extrem", 7)
        };

        return new TestDefinition
        {
            TestType = PsychometricTestType.PANSS,
            Name = "Positive and Negative Syndrome Scale",
            Abbreviation = "PANSS",
            Description = "Standardisierte Beurteilungsskala für Positiv- und Negativsymptomatik bei Schizophrenie",
            Instruction = "Bitte bewerten Sie jeden der folgenden Symptombereiche auf einer Skala von 1 (nicht vorhanden) bis 7 (extrem).",
            MaxScore = 210,
            SuggestedGop = "35600",
            Items = Enumerable.Range(1, 30).Select(i => new TestItem(i, $"PANSS Item {i}", options)).ToList(),
            ScoreRanges =
            [
                new(30, 58, "Leicht", "Leichte Symptomausprägung"),
                new(59, 95, "Mittelgradig", "Mittelgradige Symptomausprägung"),
                new(96, 150, "Schwer", "Schwere Symptomausprägung"),
                new(151, 210, "Sehr schwer", "Sehr schwere Symptomausprägung")
            ]
        };
    }

    private static TestDefinition BuildPcl5()
    {
        var options = new AnswerOption[]
        {
            new("Überhaupt nicht", 0),
            new("Ein wenig", 1),
            new("Mäßig", 2),
            new("Ziemlich", 3),
            new("Äußerst", 4)
        };

        return new TestDefinition
        {
            TestType = PsychometricTestType.PCL5,
            Name = "PTSD Checklist for DSM-5",
            Abbreviation = "PCL-5",
            Description = "Selbstbeurteilungsfragebogen zur Erfassung posttraumatischer Belastungssymptome nach DSM-5",
            Instruction = "Wie sehr waren Sie im letzten Monat durch die folgenden Probleme belastet?",
            MaxScore = 80,
            SuggestedGop = "35600",
            Items = Enumerable.Range(1, 20).Select(i => new TestItem(i, $"PCL-5 Item {i}", options)).ToList(),
            ScoreRanges =
            [
                new(0, 30, "Unterhalb Cut-off", "PTBS-Diagnose unwahrscheinlich"),
                new(31, 80, "Oberhalb Cut-off", "Verdacht auf PTBS, klinisches Interview empfohlen")
            ]
        };
    }

    private static TestDefinition BuildPsqi()
    {
        static AnswerOption[] PointsRange(int max)
            => Enumerable.Range(0, max + 1).Select(i => new AnswerOption($"{i}", i)).ToArray();

        return new TestDefinition
        {
            TestType = PsychometricTestType.PSQI,
            Name = "Pittsburgh Schlafqualitätsindex",
            Abbreviation = "PSQI",
            Description = "Fragebogen zur Beurteilung der Schlafqualität im letzten Monat",
            Instruction = "Die folgenden Fragen beziehen sich auf Ihre üblichen Schlafgewohnheiten im letzten Monat.",
            MaxScore = 21,
            SuggestedGop = "35600",
            Items = Enumerable.Range(1, 7).Select(i => new TestItem(i, $"PSQI Komponente {i}", PointsRange(3))).ToList(),
            ScoreRanges =
            [
                new(0, 5, "Gute Schlafqualität", "Keine Schlafstörung"),
                new(6, 21, "Schlechte Schlafqualität", "Hinweise auf eine klinisch relevante Schlafstörung")
            ]
        };
    }

    private static TestDefinition BuildHamd()
    {
        static AnswerOption[] PointsRange(int max)
            => Enumerable.Range(0, max + 1).Select(i => new AnswerOption($"{i}", i)).ToArray();

        return new TestDefinition
        {
            TestType = PsychometricTestType.HAMD,
            Name = "Hamilton Depressionsskala",
            Abbreviation = "HAMD-17",
            Description = "Fremdbeurteilungsskala zur Erfassung der Schwere depressiver Symptome",
            Instruction = "Bitte bewerten Sie die folgenden Symptome anhand des klinischen Interviews.",
            MaxScore = 52,
            SuggestedGop = "35600",
            Items = Enumerable.Range(1, 17).Select(i => new TestItem(i, $"HAMD Item {i}", PointsRange(4))).ToList(),
            ScoreRanges =
            [
                new(0, 7, "Normal/Remission", "Keine depressive Symptomatik"),
                new(8, 13, "Leicht", "Leichte depressive Symptomatik"),
                new(14, 18, "Mittelgradig", "Mittelgradige depressive Symptomatik"),
                new(19, 22, "Schwer", "Schwere depressive Symptomatik"),
                new(23, 52, "Sehr schwer", "Sehr schwere depressive Symptomatik")
            ]
        };
    }

    private static TestDefinition BuildBprs()
    {
        var options = new AnswerOption[]
        {
            new("1 — Nicht vorhanden", 1),
            new("2 — Sehr gering", 2),
            new("3 — Gering", 3),
            new("4 — Mäßig", 4),
            new("5 — Mäßig schwer", 5),
            new("6 — Schwer", 6),
            new("7 — Extrem schwer", 7)
        };

        return new TestDefinition
        {
            TestType = PsychometricTestType.BPRS,
            Name = "Brief Psychiatric Rating Scale",
            Abbreviation = "BPRS",
            Description = "Kurzskala zur psychiatrischen Beurteilung",
            Instruction = "Bitte bewerten Sie die folgenden Symptome auf einer Skala von 1 (nicht vorhanden) bis 7 (extrem schwer).",
            MaxScore = 126,
            SuggestedGop = "35600",
            Items = Enumerable.Range(1, 18).Select(i => new TestItem(i, $"BPRS Item {i}", options)).ToList(),
            ScoreRanges =
            [
                new(18, 31, "Leicht", "Minimale Symptomausprägung"),
                new(32, 52, "Mittelgradig", "Mittelgradige Symptomausprägung"),
                new(53, 126, "Schwer", "Schwere Symptomausprägung")
            ]
        };
    }

    private static TestDefinition BuildClockDrawing()
    {
        static AnswerOption[] PointsRange(int max)
            => Enumerable.Range(0, max + 1).Select(i => new AnswerOption($"{i} Punkt(e)", i)).ToArray();

        return new TestDefinition
        {
            TestType = PsychometricTestType.ClockDrawing,
            Name = "Uhrentest",
            Abbreviation = "CDT",
            Description = "Screening-Test zur Erfassung visuell-räumlicher und exekutiver Funktionen",
            Instruction = "Bitte zeichnen Sie eine Uhr mit allen Zahlen und stellen Sie die Zeiger auf 11:10 Uhr.",
            MaxScore = 6,
            SuggestedGop = "35600",
            Items =
            [
                new(1, "Uhrentest-Bewertung (Shulman-Scoring)", PointsRange(6))
            ],
            ScoreRanges =
            [
                new(5, 6, "Normal", "Keine visuell-räumliche Beeinträchtigung"),
                new(3, 4, "Leichte Beeinträchtigung", "Leichte visuell-räumliche oder exekutive Störung"),
                new(1, 2, "Schwere Beeinträchtigung", "Ausgeprägte visuell-räumliche oder exekutive Störung"),
                new(0, 0, "Nicht durchführbar", "Test konnte nicht durchgeführt werden")
            ]
        };
    }
}
