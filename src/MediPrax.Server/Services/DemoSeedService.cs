using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using MediPrax.Core.ValueObjects;
using MediPrax.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Server.Services;

/// <summary>
/// Seeds realistic demo data for demonstrating all MediPrax features.
/// Only runs in Development if the database has fewer than 5 patients.
/// </summary>
public static class DemoSeedService
{
    public static void Seed(MediPraxDbContext db)
    {
        // Always ensure all doctors exist (even if patients already seeded)
        var drMeier = EnsureUser(db, "Dr. Thomas", "Meier", "meier@neuropsych-bremen.de", UserRole.Arzt);
        var drSchmidt = EnsureUser(db, "Dr. Anna", "Schmidt", "schmidt@neuropsych-bremen.de", UserRole.Arzt);
        var drBauer = EnsureUser(db, "Dr. Michael", "Bauer", "bauer@neuropsych-bremen.de", UserRole.Arzt);
        var drWagner = EnsureUser(db, "Dr. Claudia", "Wagner", "wagner@neuropsych-bremen.de", UserRole.Arzt);
        var drKrause = EnsureUser(db, "Dr. Stefan", "Krause", "krause@neuropsych-bremen.de", UserRole.Arzt);
        var drLehmann = EnsureUser(db, "Dr. Petra", "Lehmann", "lehmann@neuropsych-bremen.de", UserRole.Arzt);
        var drFrank = EnsureUser(db, "Dr. Jan", "Frank", "frank@neuropsych-bremen.de", UserRole.Arzt);
        var drVogt = EnsureUser(db, "Dr. Sabine", "Vogt", "vogt@neuropsych-bremen.de", UserRole.Arzt);

        // Check if demo data was already seeded
        var hasWeber = db.Patients.Any(p => p.LastName == "Weber" && p.FirstName == "Klaus");
        if (hasWeber && db.Appointments.Any()) return; // Already fully seeded
        if (hasWeber)
        {
            // Patients exist but appointments were cleared — re-seed appointments only
            ReSeedAppointments(db);
            return;
        }

        // --- Users (Doctors + MFA) ---
        var mfaKoch = EnsureUser(db, "Sabine", "Koch", "koch@neuropsych-bremen.de", UserRole.MFA);

        // --- Bulk patients (stress test: 300 patients) ---
        if (db.Patients.Count() < 100)
        {
            SeedBulkPatients(db);
        }

        // --- Patients ---
        var weber = CreatePatient(db, "Klaus", "Weber", new DateOnly(1958, 3, 12), "M", InsuranceType.GKV,
            "AOK Bremen", "V1234567890", "A123456789", "Ostertorsteinweg 42", "Bremen", "28203", "0421-3456789");

        var mueller = CreatePatient(db, "Maria", "Müller", new DateOnly(1972, 8, 25), "W", InsuranceType.GKV,
            "TK", "V9876543210", "B987654321", "Schwachhauser Heerstr. 15", "Bremen", "28209", "0421-5678901");

        var hoffmann = CreatePatient(db, "Stefan", "Hoffmann", new DateOnly(1985, 11, 3), "M", InsuranceType.PKV,
            "Debeka", "PKV-2024-001", null, "Am Wall 120", "Bremen", "28195", "0421-1112233");

        var fischer = CreatePatient(db, "Ursula", "Fischer", new DateOnly(1945, 6, 18), "W", InsuranceType.GKV,
            "Barmer", "V5555555555", "C555555555", "Parkallee 88", "Bremen", "28209", "0421-4445566");

        var braun = CreatePatient(db, "Thomas", "Braun", new DateOnly(1990, 1, 30), "M", InsuranceType.GKV,
            "DAK", "V7777777777", "D777777777", "Contrescarpe 60", "Bremen", "28195", "0176-12345678");

        var klein = CreatePatient(db, "Sabrina", "Klein", new DateOnly(1968, 4, 7), "W", InsuranceType.GKV,
            "AOK Bremen", "V3333333333", "E333333333", "Humboldtstr. 35", "Bremen", "28203", "0421-9998877");

        var schulz = CreatePatient(db, "Jürgen", "Schulz", new DateOnly(1952, 12, 1), "M", InsuranceType.GKV,
            "TK", "V4444444444", "F444444444", "Fedelhören 12", "Bremen", "28203", "0421-6667788");

        var lang = CreatePatient(db, "Petra", "Lang", new DateOnly(1980, 7, 22), "W", InsuranceType.PKV,
            "Allianz", "PKV-2024-002", null, "Böttcherstraße 5", "Bremen", "28195", "0151-99887766");

        var richter = CreatePatient(db, "Helmut", "Richter", new DateOnly(1963, 9, 14), "M", InsuranceType.GKV,
            "BKK", "V8888888888", "G888888888", "Martinistr. 22", "Bremen", "28195", "0421-7771122");
        var wolf = CreatePatient(db, "Ingrid", "Wolf", new DateOnly(1975, 2, 28), "W", InsuranceType.GKV,
            "IKK", "V6666666666", "H666666666", "Osterdeich 100", "Bremen", "28205", "0421-3334455");
        var schaefer = CreatePatient(db, "Dieter", "Schäfer", new DateOnly(1950, 5, 20), "M", InsuranceType.GKV,
            "AOK Bremen", "V2222222222", "I222222222", "Wachmannstr. 10", "Bremen", "28209", "0421-5556677");
        var otto = CreatePatient(db, "Monika", "Otto", new DateOnly(1988, 12, 5), "W", InsuranceType.PKV,
            "Signal Iduna", "PKV-2024-003", null, "Lloydstr. 4", "Bremen", "28217", "0176-44556677");
        var seidel = CreatePatient(db, "Frank", "Seidel", new DateOnly(1970, 8, 11), "M", InsuranceType.GKV,
            "TK", "V1111111111", "J111111111", "Bismarckstr. 50", "Bremen", "28203", "0421-8889900");
        var kramer = CreatePatient(db, "Eva", "Krämer", new DateOnly(1982, 3, 19), "W", InsuranceType.GKV,
            "Barmer", "V9999999999", "K999999999", "Findorffstr. 30", "Bremen", "28215", "0151-22334455");

        db.SaveChanges();

        // --- Encounters + Billing for Weber (Depression, recurrent) ---
        var now = DateTime.UtcNow;
        var quarter = $"{now.Year}-Q{(now.Month - 1) / 3 + 1}";

        var encWeber1 = CreateEncounter(db, weber, drMeier, DateOnly.FromDateTime(DateTime.Today.AddDays(-45)),
            ["F33.1"], "Rezidivierende depressive Störung, gegenwärtig mittelgradige Episode.\nStimmung deutlich gedrückt, Antrieb vermindert, Schlafstörungen.\nMedikation: Sertralin 100mg fortführen, Kontrolle in 4 Wochen.", 25);
        AddBilling(db, encWeber1, weber, "21210", "Grundpauschale Psychiatrie", BillingType.EBM, quarter);
        AddBilling(db, encWeber1, weber, "21220", "Psychiatrisches Gespräch", BillingType.EBM, quarter);

        var encWeber2 = CreateEncounter(db, weber, drMeier, DateOnly.FromDateTime(DateTime.Today.AddDays(-14)),
            ["F33.1", "G47.0"], "Kontrolltermin. Stimmung etwas gebessert, schläft besser unter Mirtazapin.\nAntrieb weiterhin reduziert. Sertralin auf 150mg erhöht.", 20);
        AddBilling(db, encWeber2, weber, "21220", "Psychiatrisches Gespräch", BillingType.EBM, quarter);

        // --- Encounters for Müller (MS) ---
        var encMueller1 = CreateEncounter(db, mueller, drSchmidt, DateOnly.FromDateTime(DateTime.Today.AddDays(-90)),
            ["G35.1"], "Multiple Sklerose, schubförmig remittierend.\nEDSS 2.5, kein akuter Schub. Ocrelizumab-Infusion planmäßig erhalten.\nMRT: keine neuen Läsionen.", 30);
        AddBilling(db, encMueller1, mueller, "16210", "Grundpauschale Neurologie", BillingType.EBM, quarter);
        AddBilling(db, encMueller1, mueller, "16220", "Neurologisches Gespräch", BillingType.EBM, quarter);

        var encMueller2 = CreateEncounter(db, mueller, drSchmidt, DateOnly.FromDateTime(DateTime.Today.AddDays(-7)),
            ["G35.1"], "Kontrolltermin. Stabil unter Ocrelizumab. Fatigue weiterhin belastend.\nÜberweisung Ergotherapie empfohlen.", 20);
        AddBilling(db, encMueller2, mueller, "16220", "Neurologisches Gespräch", BillingType.EBM, quarter);

        // --- Encounters for Hoffmann (Anxiety + Psychotherapy) ---
        var encHoffmann1 = CreateEncounter(db, hoffmann, drMeier, DateOnly.FromDateTime(DateTime.Today.AddDays(-60)),
            ["F41.1", "F40.1"], "Generalisierte Angststörung mit sozialer Phobie.\nPsychotherapeutische Sprechstunde: Leidensdruck erheblich, Arbeitsunfähigkeit seit 4 Wochen.\nIndikation für Verhaltenstherapie gestellt.", 50);

        // --- Encounters for Fischer (Parkinson) ---
        var encFischer1 = CreateEncounter(db, fischer, drSchmidt, DateOnly.FromDateTime(DateTime.Today.AddDays(-30)),
            ["G20.0"], "Idiopathisches Parkinson-Syndrom. H&Y Stadium 2.\nTremor rechts zunehmend, Rigor beidseits.\nLevodopa/Carbidopa 100/25mg 3x täglich, gutes Ansprechen.", 25);
        AddBilling(db, encFischer1, fischer, "16211", "Grundpauschale Neurologie ab 60", BillingType.EBM, quarter);
        AddBilling(db, encFischer1, fischer, "16220", "Neurologisches Gespräch", BillingType.EBM, quarter);

        // --- Encounters for Braun (Epilepsy) ---
        var encBraun1 = CreateEncounter(db, braun, drSchmidt, DateOnly.FromDateTime(DateTime.Today.AddDays(-21)),
            ["G40.2", "G40.9"], "Fokale Epilepsie, Anfallsfreiheit seit 3 Monaten unter Levetiracetam.\nEEG: vereinzelt sharp waves temporal links, kein Anfallsmuster.\nLabor: Levetiracetam-Spiegel im Referenzbereich.", 30);
        AddBilling(db, encBraun1, braun, "16210", "Grundpauschale Neurologie", BillingType.EBM, quarter);

        // --- Encounters for Klein (Migraine) ---
        var encKlein1 = CreateEncounter(db, klein, drSchmidt, DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
            ["G43.1", "G43.0"], "Migräne mit Aura, chronifiziert (>15 Tage/Monat).\nProphylaxe mit Topiramat 50mg begonnen. Kopfschmerztagebuch besprochen.\nBotox-Indikation prüfen bei nächstem Termin.", 25);
        AddBilling(db, encKlein1, klein, "16210", "Grundpauschale Neurologie", BillingType.EBM, quarter);
        AddBilling(db, encKlein1, klein, "16220", "Neurologisches Gespräch", BillingType.EBM, quarter);

        // --- Encounters for Schulz (Dementia) ---
        var encSchulz1 = CreateEncounter(db, schulz, drMeier, DateOnly.FromDateTime(DateTime.Today.AddDays(-35)),
            ["F00.1", "G30.1"], "Alzheimer-Demenz, mittelgradige Ausprägung. MMST: 18/30.\nVerhaltensauffälligkeiten: nächtliche Unruhe, Weglauftendenz.\nDonepezil 10mg, Melperon 25mg z.N. Betreuung eingerichtet.", 30);
        AddBilling(db, encSchulz1, schulz, "21211", "Grundpauschale Psychiatrie ab 60", BillingType.EBM, quarter);
        AddBilling(db, encSchulz1, schulz, "21220", "Psychiatrisches Gespräch", BillingType.EBM, quarter);

        db.SaveChanges();

        // --- Medications ---
        AddMedication(db, weber, drMeier, "Sertralin", "Sertralin", "150mg", "1-0-0-0", MedicationCategory.Antidepressivum, -180);
        AddMedication(db, weber, drMeier, "Mirtazapin", "Mirtazapin", "15mg", "0-0-0-1", MedicationCategory.Antidepressivum, -45, hinweis: "zur Nacht, sedierend");

        AddMedication(db, mueller, drSchmidt, "Ocrelizumab", "Ocrelizumab", "300mg", "i.v. alle 6 Monate", null, -180, hinweis: "nächste Infusion Juli 2026");

        AddMedication(db, fischer, drSchmidt, "Levodopa/Carbidopa", "Levodopa", "100/25mg", "1-1-1-0", null, -365);
        AddMedication(db, fischer, drSchmidt, "Pramipexol", "Pramipexol", "0.7mg", "1-0-1-0", null, -90);

        AddMedication(db, braun, drSchmidt, "Levetiracetam", "Levetiracetam", "1000mg", "1-0-1-0", MedicationCategory.Antikonvulsivum, -365, requiresMonitoring: true, monitoringType: "Levetiracetam");

        AddMedication(db, klein, drSchmidt, "Topiramat", "Topiramat", "50mg", "0-0-1-0", MedicationCategory.Antikonvulsivum, -10, hinweis: "Migräneprophylaxe");
        AddMedication(db, klein, drSchmidt, "Sumatriptan", "Sumatriptan", "50mg", "bei Bedarf", null, -180, hinweis: "max. 10 Tbl./Monat");

        AddMedication(db, schulz, drMeier, "Donepezil", "Donepezil", "10mg", "0-0-1-0", null, -270);
        AddMedication(db, schulz, drMeier, "Melperon", "Melperon", "25mg", "0-0-0-1", MedicationCategory.Antipsychotikum, -90, hinweis: "bei nächtlicher Unruhe");

        AddMedication(db, hoffmann, drMeier, "Escitalopram", "Escitalopram", "10mg", "1-0-0-0", MedicationCategory.Antidepressivum, -30);

        db.SaveChanges();

        SeedAppointments(db, weber, mueller, hoffmann, fischer, braun, klein, schulz, lang,
            richter, wolf, schaefer, otto, seidel, kramer,
            drMeier, drSchmidt, drBauer, drWagner, drKrause, drLehmann, drFrank, drVogt);

    }

    private static void SeedAppointments(MediPraxDbContext db, Patient weber, Patient mueller,
        Patient hoffmann, Patient fischer, Patient braun, Patient klein, Patient schulz, Patient lang,
        Patient richter, Patient wolf, Patient schaefer, Patient otto, Patient seidel, Patient kramer,
        User drMeier, User drSchmidt, User drBauer, User drWagner, User drKrause, User drLehmann, User drFrank, User drVogt)
    {
        var today = DateTime.UtcNow.Date;
        var monday = today.AddDays(-(int)today.DayOfWeek + 1);
        var pts = new[] { weber, mueller, hoffmann, fischer, braun, klein, schulz, lang, richter, wolf, schaefer, otto, seidel, kramer };
        var docs = new[] { drMeier, drSchmidt, drBauer, drWagner, drKrause, drLehmann, drFrank, drVogt };
        var notes = new[] {
            "Kontrolltermin", "VT Sitzung", "Medikamentenkontrolle", "EEG-Kontrolle",
            "Befundbesprechung", "Erstgespräch", "Krisenintervention", "Folgetermin",
            "Diagnostik", "Arztbrief", "Überweisung", "Psychotherapie",
            "EMG/NLG", "Doppler", "MRT-Besprechung", "Verlaufskontrolle",
            "Suizidalitätseinschätzung", "Angehörigengespräch", "MMST", "PHQ-9"
        };
        var durations = new[] { 25, 50, 25, 30, 25, 50, 25, 25, 50, 25 };
        var rng = new Random(42); // deterministic

        // Generate dense schedule: each doctor gets 6-8 appointments per day, Mo-Fr
        for (var dayOffset = 0; dayOffset < 5; dayOffset++)
        {
            foreach (var doc in docs)
            {
                // Morning block: 07:30 - 12:30
                var morningSlots = new[] { 7.5, 8.0, 8.5, 9.0, 9.5, 10.0, 10.5, 11.0, 11.5, 12.0 };
                var morningCount = rng.Next(5, 8); // 5-7 morning appointments
                var usedSlots = new HashSet<double>();
                for (var i = 0; i < morningCount; i++)
                {
                    var slot = morningSlots[rng.Next(morningSlots.Length)];
                    if (usedSlots.Contains(slot)) continue;
                    usedSlots.Add(slot);
                    var pt = pts[rng.Next(pts.Length)];
                    var dur = durations[rng.Next(durations.Length)];
                    var note = notes[rng.Next(notes.Length)];
                    AddAppointment(db, pt, doc, monday.AddDays(dayOffset).AddHours(slot), dur, note);
                }

                // Afternoon block: 14:00 - 17:00
                var afternoonSlots = new[] { 14.0, 14.5, 15.0, 15.5, 16.0, 16.5 };
                var afternoonCount = rng.Next(2, 5); // 2-4 afternoon appointments
                usedSlots.Clear();
                for (var i = 0; i < afternoonCount; i++)
                {
                    var slot = afternoonSlots[rng.Next(afternoonSlots.Length)];
                    if (usedSlots.Contains(slot)) continue;
                    usedSlots.Add(slot);
                    var pt = pts[rng.Next(pts.Length)];
                    var dur = durations[rng.Next(durations.Length)];
                    var note = notes[rng.Next(notes.Length)];
                    AddAppointment(db, pt, doc, monday.AddDays(dayOffset).AddHours(slot), dur, note);
                }
            }
        }

        // MONTAG — Dr. Meier
        AddAppointment(db, weber, drMeier, monday.AddHours(8), 25, "Kontrolltermin Depression");
        AddAppointment(db, hoffmann, drMeier, monday.AddHours(8.5), 50, "Psychotherapie VT Sitzung 3");
        AddAppointment(db, schulz, drMeier, monday.AddHours(9.5), 25, "Demenz-Kontrolle mit Ehefrau");
        AddAppointment(db, lang, drMeier, monday.AddHours(10), 50, "Erstgespräch Diagnostik");
        AddAppointment(db, weber, drMeier, monday.AddHours(11), 25, "Medikamentenkontrolle");
        AddAppointment(db, hoffmann, drMeier, monday.AddHours(14), 50, "VT Probatorik");
        AddAppointment(db, schulz, drMeier, monday.AddHours(15), 25, "Angehörigengespräch");
        // MONTAG — Dr. Schmidt
        AddAppointment(db, mueller, drSchmidt, monday.AddHours(8), 30, "MS-Kontrolle + MRT");
        AddAppointment(db, fischer, drSchmidt, monday.AddHours(9), 25, "Parkinson-Kontrolle");
        AddAppointment(db, braun, drSchmidt, monday.AddHours(9.5), 30, "EEG-Kontrolle");
        AddAppointment(db, klein, drSchmidt, monday.AddHours(10.5), 25, "Migräne-Prophylaxe");
        AddAppointment(db, mueller, drSchmidt, monday.AddHours(11), 25, "Fatigue-Besprechung");
        AddAppointment(db, fischer, drSchmidt, monday.AddHours(14), 45, "EMG Kontrolle");
        AddAppointment(db, braun, drSchmidt, monday.AddHours(15), 25, "Levetiracetam-Kontrolle");

        // DIENSTAG — Dr. Meier
        AddAppointment(db, hoffmann, drMeier, monday.AddDays(1).AddHours(8), 50, "VT Sitzung 4");
        AddAppointment(db, weber, drMeier, monday.AddDays(1).AddHours(9), 25, "Folgetermin Depression");
        AddAppointment(db, lang, drMeier, monday.AddDays(1).AddHours(9.5), 50, "Diagnostik Fortsetzung");
        AddAppointment(db, schulz, drMeier, monday.AddDays(1).AddHours(10.5), 25, "MMST-Kontrolle");
        AddAppointment(db, hoffmann, drMeier, monday.AddDays(1).AddHours(11), 25, "Krisenintervention");
        AddAppointment(db, weber, drMeier, monday.AddDays(1).AddHours(14), 25, "Suizidalitätseinschätzung");
        AddAppointment(db, lang, drMeier, monday.AddDays(1).AddHours(15), 25, "Befundbesprechung");
        // DIENSTAG — Dr. Schmidt
        AddAppointment(db, klein, drSchmidt, monday.AddDays(1).AddHours(8), 25, "Kopfschmerz-Kontrolle");
        AddAppointment(db, braun, drSchmidt, monday.AddDays(1).AddHours(8.5), 30, "Anfallskalender-Besprechung");
        AddAppointment(db, mueller, drSchmidt, monday.AddDays(1).AddHours(9.5), 30, "Ocrelizumab Planung");
        AddAppointment(db, fischer, drSchmidt, monday.AddDays(1).AddHours(10.5), 25, "Medikamentenanpassung");
        AddAppointment(db, klein, drSchmidt, monday.AddDays(1).AddHours(11), 45, "Botox-Beratung Migräne");
        AddAppointment(db, braun, drSchmidt, monday.AddDays(1).AddHours(14), 30, "NLG obere Extremität");

        // MITTWOCH — Dr. Meier
        AddAppointment(db, hoffmann, drMeier, monday.AddDays(2).AddHours(8), 50, "VT Sitzung 5");
        AddAppointment(db, schulz, drMeier, monday.AddDays(2).AddHours(9), 25, "Verhaltensauffälligkeiten");
        AddAppointment(db, weber, drMeier, monday.AddDays(2).AddHours(9.5), 25, "Medikamentenkontrolle");
        AddAppointment(db, lang, drMeier, monday.AddDays(2).AddHours(10), 50, "Psychotherapeutische Sprechstunde");
        AddAppointment(db, hoffmann, drMeier, monday.AddDays(2).AddHours(11), 25, "Befundbesprechung");
        AddAppointment(db, schulz, drMeier, monday.AddDays(2).AddHours(14), 25, "Betreuungsgespräch");
        // MITTWOCH — Dr. Schmidt
        AddAppointment(db, fischer, drSchmidt, monday.AddDays(2).AddHours(8), 25, "Tremor-Evaluation");
        AddAppointment(db, mueller, drSchmidt, monday.AddDays(2).AddHours(8.5), 30, "EDSS-Kontrolle");
        AddAppointment(db, braun, drSchmidt, monday.AddDays(2).AddHours(9.5), 30, "EEG langzeit");
        AddAppointment(db, klein, drSchmidt, monday.AddDays(2).AddHours(10.5), 25, "Tagebuch-Besprechung");
        AddAppointment(db, fischer, drSchmidt, monday.AddDays(2).AddHours(11), 25, "Freezing-Evaluation");
        AddAppointment(db, mueller, drSchmidt, monday.AddDays(2).AddHours(14), 25, "Ergotherapie-Überweisung");
        AddAppointment(db, klein, drSchmidt, monday.AddDays(2).AddHours(14.5), 25, "Akuttermin Migräne");

        // DONNERSTAG — Dr. Meier
        AddAppointment(db, hoffmann, drMeier, monday.AddDays(3).AddHours(8), 50, "VT Sitzung 6");
        AddAppointment(db, weber, drMeier, monday.AddDays(3).AddHours(9), 25, "PHQ-9 Kontrolle");
        AddAppointment(db, lang, drMeier, monday.AddDays(3).AddHours(9.5), 25, "Rückmeldung Diagnostik");
        AddAppointment(db, schulz, drMeier, monday.AddDays(3).AddHours(10), 25, "Donepezil-Kontrolle");
        AddAppointment(db, hoffmann, drMeier, monday.AddDays(3).AddHours(11), 25, "Sozialberatung");
        AddAppointment(db, weber, drMeier, monday.AddDays(3).AddHours(14), 25, "Arbeitsunfähigkeit");
        AddAppointment(db, lang, drMeier, monday.AddDays(3).AddHours(14.5), 50, "Psychotherapie Indikation");
        // DONNERSTAG — Dr. Schmidt
        AddAppointment(db, braun, drSchmidt, monday.AddDays(3).AddHours(8), 30, "EEG + Befund");
        AddAppointment(db, fischer, drSchmidt, monday.AddDays(3).AddHours(9), 25, "Pramipexol-Anpassung");
        AddAppointment(db, mueller, drSchmidt, monday.AddDays(3).AddHours(9.5), 30, "Schub-Nachkontrolle");
        AddAppointment(db, klein, drSchmidt, monday.AddDays(3).AddHours(10.5), 25, "Prophylaxe-Evaluation");
        AddAppointment(db, braun, drSchmidt, monday.AddDays(3).AddHours(11), 25, "Führerschein-Gutachten");
        AddAppointment(db, fischer, drSchmidt, monday.AddDays(3).AddHours(14), 45, "Doppler hirnvers. Gefäße");

        // FREITAG — Dr. Meier (halber Tag)
        AddAppointment(db, hoffmann, drMeier, monday.AddDays(4).AddHours(8), 50, "VT Sitzung 7");
        AddAppointment(db, weber, drMeier, monday.AddDays(4).AddHours(9), 25, "Verlaufskontrolle");
        AddAppointment(db, schulz, drMeier, monday.AddDays(4).AddHours(9.5), 25, "Pflegegrad-Dokumentation");
        AddAppointment(db, lang, drMeier, monday.AddDays(4).AddHours(10), 25, "Arztbrief-Besprechung");
        // FREITAG — Dr. Schmidt (halber Tag)
        AddAppointment(db, braun, drSchmidt, monday.AddDays(4).AddHours(8), 30, "Epilepsie Jahres-Kontrolle");
        AddAppointment(db, mueller, drSchmidt, monday.AddDays(4).AddHours(9), 25, "Rezept + Labor");
        AddAppointment(db, klein, drSchmidt, monday.AddDays(4).AddHours(9.5), 25, "Topiramat Nebenwirkungen");
        AddAppointment(db, fischer, drSchmidt, monday.AddDays(4).AddHours(10), 25, "Kontrolle motorisch");

        // SAMSTAG — Notfallsprechstunde (nur Dr. Meier)
        AddAppointment(db, weber, drMeier, monday.AddDays(5).AddHours(9), 15, "Notfall: Krisenintervention");
        AddAppointment(db, braun, drMeier, monday.AddDays(5).AddHours(9.25), 15, "Notfall: Anfall gestern");

        db.SaveChanges();
    }

    private static void SeedDiseaseData(MediPraxDbContext db, Patient weber, Patient mueller,
        Patient hoffmann, Patient fischer, Patient braun, Patient klein, Patient schulz,
        User drMeier, User drSchmidt, Encounter encWeber2)
    {
        // --- Disease-specific data ---

        // Epilepsy: seizure diary for Braun
        db.SeizureDiaries.AddRange(
            new SeizureDiary { PatientId = braun.Id, SeizureDate = DateTime.UtcNow.AddDays(-120), SeizureType = "Fokal", DurationSeconds = 45, Trigger = "Schlafmangel", AuraPresent = true, AuraDescription = "Kribbeln linke Hand", ConsciousnessImpaired = false },
            new SeizureDiary { PatientId = braun.Id, SeizureDate = DateTime.UtcNow.AddDays(-95), SeizureType = "Fokal zu bilateral tonisch-klonisch", DurationSeconds = 120, Trigger = "Alkohol", AuraPresent = true, AuraDescription = "Déjà-vu", ConsciousnessImpaired = true, PostictalState = "Verwirrtheit 15 Min., Müdigkeit", MedicationAtTime = "Levetiracetam 750mg 1-0-1" },
            new SeizureDiary { PatientId = braun.Id, SeizureDate = DateTime.UtcNow.AddDays(-80), SeizureType = "Fokal", DurationSeconds = 30, AuraPresent = false, ConsciousnessImpaired = false, Notes = "Kurzer Anfall, selbstlimitierend" }
        );

        // Headache diary for Klein
        db.HeadacheDiaries.AddRange(
            new HeadacheDiary { PatientId = klein.Id, Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-28)), Type = "Migräne", Intensity = 8, DurationHours = 12, Location = "Einseitig links", AuraPresent = true, Nausea = true, Photophobia = true, Phonophobia = true, MedicationTaken = "Sumatriptan 50mg", MedicationEffective = true },
            new HeadacheDiary { PatientId = klein.Id, Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-21)), Type = "Spannungskopfschmerz", Intensity = 4, DurationHours = 6, Location = "Beidseits", AuraPresent = false, Nausea = false, MedicationTaken = "Ibuprofen 400mg", MedicationEffective = true },
            new HeadacheDiary { PatientId = klein.Id, Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-14)), Type = "Migräne", Intensity = 7, DurationHours = 8, Location = "Einseitig rechts", AuraPresent = true, Nausea = true, Photophobia = true, MedicationTaken = "Sumatriptan 50mg", MedicationEffective = false },
            new HeadacheDiary { PatientId = klein.Id, Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-7)), Type = "Migräne", Intensity = 6, DurationHours = 4, Location = "Einseitig links", AuraPresent = false, Nausea = true, Phonophobia = true, Triggers = ["Stress", "Wetterwechsel"] },
            new HeadacheDiary { PatientId = klein.Id, Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-3)), Type = "Spannungskopfschmerz", Intensity = 3, DurationHours = 3, Location = "Frontal" }
        );

        // MS documentation for Müller
        db.MsDocumentations.AddRange(
            new MsDocumentation { PatientId = mueller.Id, DocumentationDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-365)), EdssScore = 1.5m, IsRelapse = false, CurrentDmt = "Dimethylfumarat" },
            new MsDocumentation { PatientId = mueller.Id, DocumentationDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-270)), EdssScore = 2.0m, IsRelapse = true, RelapseDescription = "Optikusneuritis rechts, Visusminderung auf 0.4", CurrentDmt = "Dimethylfumarat", MriDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-268)), MriFindings = "2 neue T2-Läsionen periventrikulär, 1 Gd-aufnehmend", NewLesions = 2, GadEnhancing = 1 },
            new MsDocumentation { PatientId = mueller.Id, DocumentationDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-180)), EdssScore = 2.5m, IsRelapse = false, CurrentDmt = "Ocrelizumab", Notes = "Therapieeskalation auf Ocrelizumab nach Schub unter DMF" },
            new MsDocumentation { PatientId = mueller.Id, DocumentationDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7)), EdssScore = 2.5m, IsRelapse = false, CurrentDmt = "Ocrelizumab", MriDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)), MriFindings = "Stabile Läsionslast, keine neuen Läsionen, keine Gd-Aufnahme", NewLesions = 0, GadEnhancing = 0 }
        );

        // Parkinson for Fischer
        db.ParkinsonDocumentations.AddRange(
            new ParkinsonDocumentation { PatientId = fischer.Id, DocumentationDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-365)), HoehnYahrStage = 1.5m, Tremor = 1, Rigidity = 1, Bradykinesia = 1, PosturalInstability = 0 },
            new ParkinsonDocumentation { PatientId = fischer.Id, DocumentationDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-180)), HoehnYahrStage = 2, Tremor = 2, Rigidity = 1, Bradykinesia = 2, PosturalInstability = 0, Notes = "Pramipexol hinzugenommen" },
            new ParkinsonDocumentation { PatientId = fischer.Id, DocumentationDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-30)), HoehnYahrStage = 2, Tremor = 2, Rigidity = 2, Bradykinesia = 2, PosturalInstability = 1, OnOffFluctuations = false, Dyskinesia = false, NonMotorSymptoms = ["Obstipation", "REM-Schlafstörung"] }
        );

        // Suicidality for Weber (moderate risk)
        weber.CurrentSuicidalityRisk = SuicidalityRiskLevel.Low;
        weber.SuicidalityRiskUpdatedAt = DateTime.UtcNow.AddDays(-14);

        db.SuicidalityAssessments.Add(new SuicidalityAssessment
        {
            PatientId = weber.Id, EncounterId = encWeber2.Id, AssessedById = drMeier.Id,
            AssessmentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-14)),
            RiskLevel = SuicidalityRiskLevel.Low,
            SuicidalIdeation = true, SuicidalPlans = false, SuicidalIntent = false, PriorAttempts = false,
            RiskFactors = ["Depression (schwer)", "Soziale Isolation", "Arbeitslosigkeit / finanzielle Probleme"],
            ProtectiveFactors = ["Soziales Netzwerk / Familie", "Therapeutische Beziehung", "Distanzierungsfähigkeit"],
            ActionsTaken = ["Engmaschige Terminvereinbarung", "Notfallnummern ausgehändigt"],
            SafetyPlan = "Bei Krisen: Telefonseelsorge 0800-1110111, Notaufnahme Klinikum Bremen-Ost"
        });

        // Psychometric tests for Weber
        db.PsychometricTests.Add(new PsychometricTest
        {
            PatientId = weber.Id, AdministeredById = drMeier.Id,
            TestType = PsychometricTestType.PHQ9, TestDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-45)),
            Status = TestStatus.Completed, TotalScore = 18, Interpretation = "Mittelgradige Depression",
            Responses = [
                new TestResponse { ItemNumber = 0, ItemText = "Wenig Interesse oder Freude an Tätigkeiten", ResponseValue = 3 },
                new TestResponse { ItemNumber = 1, ItemText = "Niedergeschlagenheit, Schwermut, Hoffnungslosigkeit", ResponseValue = 2 },
                new TestResponse { ItemNumber = 2, ItemText = "Schwierigkeiten beim Ein-/Durchschlafen", ResponseValue = 2 },
                new TestResponse { ItemNumber = 3, ItemText = "Müdigkeit oder Gefühl, keine Energie zu haben", ResponseValue = 3 },
                new TestResponse { ItemNumber = 4, ItemText = "Verminderter Appetit oder übermäßiges Essen", ResponseValue = 1 },
                new TestResponse { ItemNumber = 5, ItemText = "Schlechte Meinung von sich selbst", ResponseValue = 2 },
                new TestResponse { ItemNumber = 6, ItemText = "Schwierigkeiten sich zu konzentrieren", ResponseValue = 2 },
                new TestResponse { ItemNumber = 7, ItemText = "Verlangsamung oder Unruhe", ResponseValue = 2 },
                new TestResponse { ItemNumber = 8, ItemText = "Gedanken, sich zu verletzen", ResponseValue = 1 }
            ]
        });

        db.PsychometricTests.Add(new PsychometricTest
        {
            PatientId = weber.Id, AdministeredById = drMeier.Id,
            TestType = PsychometricTestType.PHQ9, TestDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-14)),
            Status = TestStatus.Completed, TotalScore = 12, Interpretation = "Mittelgradige Depression",
            Responses = [
                new TestResponse { ItemNumber = 0, ItemText = "Wenig Interesse oder Freude an Tätigkeiten", ResponseValue = 2 },
                new TestResponse { ItemNumber = 1, ItemText = "Niedergeschlagenheit", ResponseValue = 1 },
                new TestResponse { ItemNumber = 2, ItemText = "Schlafstörungen", ResponseValue = 1 },
                new TestResponse { ItemNumber = 3, ItemText = "Müdigkeit", ResponseValue = 2 },
                new TestResponse { ItemNumber = 4, ItemText = "Appetit", ResponseValue = 1 },
                new TestResponse { ItemNumber = 5, ItemText = "Schlechte Meinung", ResponseValue = 1 },
                new TestResponse { ItemNumber = 6, ItemText = "Konzentration", ResponseValue = 2 },
                new TestResponse { ItemNumber = 7, ItemText = "Verlangsamung", ResponseValue = 1 },
                new TestResponse { ItemNumber = 8, ItemText = "Selbstverletzung", ResponseValue = 1 }
            ]
        });

        // Recalls
        db.Recalls.AddRange(
            new Recall { PatientId = mueller.Id, DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(30)), Reason = "MRT-Kontrolle fällig", Status = RecallStatus.Open },
            new Recall { PatientId = fischer.Id, DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(14)), Reason = "Parkinson-Kontrolle in 2 Wochen", Status = RecallStatus.Open },
            new Recall { PatientId = braun.Id, DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(60)), Reason = "EEG-Kontrolle + Levetiracetam-Spiegel", Status = RecallStatus.Open },
            new Recall { PatientId = klein.Id, DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(21)), Reason = "Topiramat-Kontrolle, Kopfschmerztagebuch mitbringen", Status = RecallStatus.Open },
            new Recall { PatientId = schulz.Id, DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)), Reason = "MMST-Kontrolle", Status = RecallStatus.Open }
        );

        db.SaveChanges();
    }

    private static void SeedBulkPatients(MediPraxDbContext db)
    {
        var firstNamesMale = new[] { "Hans", "Peter", "Karl", "Wolfgang", "Dieter", "Jürgen", "Manfred", "Thomas", "Werner", "Helmut", "Gerhard", "Heinz", "Horst", "Rolf", "Bernd", "Uwe", "Klaus", "Günter", "Michael", "Andreas", "Stefan", "Frank", "Rainer", "Norbert", "Martin", "Detlef", "Axel", "Volker", "Christoph", "Matthias" };
        var firstNamesFemale = new[] { "Ursula", "Ingrid", "Helga", "Renate", "Monika", "Karin", "Brigitte", "Gertrud", "Erika", "Christa", "Gisela", "Hannelore", "Sabine", "Petra", "Andrea", "Birgit", "Claudia", "Heike", "Susanne", "Martina", "Gabriele", "Angelika", "Anja", "Nicole", "Sandra", "Kathrin", "Silke", "Bettina", "Dagmar", "Elke" };
        var lastNames = new[] { "Müller", "Schmidt", "Schneider", "Fischer", "Weber", "Meyer", "Wagner", "Becker", "Schulz", "Hoffmann", "Koch", "Richter", "Bauer", "Klein", "Wolf", "Schröder", "Neumann", "Schwarz", "Zimmermann", "Braun", "Krüger", "Hofmann", "Hartmann", "Lange", "Schmitt", "Werner", "Schmitz", "Krause", "Meier", "Lehmann", "Schmid", "Schulze", "Maier", "Köhler", "Herrmann", "König", "Walter", "Mayer", "Huber", "Kaiser" };
        var streets = new[] { "Hauptstr.", "Bahnhofstr.", "Gartenstr.", "Schulstr.", "Dorfstr.", "Bergstr.", "Kirchstr.", "Waldstr.", "Ringstr.", "Birkenweg", "Am Markt", "Lindenstr.", "Rosenweg", "Schillerstr.", "Goethestr.", "Mozartstr.", "Beethovenstr.", "Lessingstr.", "Am Park", "Friedhofstr." };
        var insurers = new[] { "AOK Bremen", "TK", "Barmer", "DAK", "IKK", "BKK", "KKH", "hkk" };
        var rng = new Random(123);

        for (var i = 0; i < 300; i++)
        {
            var isMale = rng.Next(2) == 0;
            var firstName = isMale ? firstNamesMale[rng.Next(firstNamesMale.Length)] : firstNamesFemale[rng.Next(firstNamesFemale.Length)];
            var lastName = lastNames[rng.Next(lastNames.Length)];

            // Skip if already exists (from named seed data)
            if (db.Patients.Any(p => p.FirstName == firstName && p.LastName == lastName)) continue;

            var year = rng.Next(1940, 2005);
            var month = rng.Next(1, 13);
            var day = rng.Next(1, 28);
            var isPkv = rng.Next(10) == 0; // 10% PKV

            db.Patients.Add(new Patient
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = new DateOnly(year, month, day),
                Gender = isMale ? "M" : "W",
                InsuranceType = isPkv ? InsuranceType.PKV : InsuranceType.GKV,
                InsuranceProvider = isPkv ? "Debeka" : insurers[rng.Next(insurers.Length)],
                InsuranceNumber = $"V{rng.Next(1000000000, 2000000000)}",
                Kvnr = isPkv ? null : $"{(char)('A' + rng.Next(26))}{rng.Next(100000000, 999999999)}",
                Street = $"{streets[rng.Next(streets.Length)]} {rng.Next(1, 200)}",
                City = "Bremen",
                PostalCode = $"28{rng.Next(195, 220):000}",
                Phone = $"0421-{rng.Next(1000000, 9999999)}"
            });
        }
        db.SaveChanges();
    }

    private static User EnsureUser(MediPraxDbContext db, string first, string last, string email, UserRole role)
    {
        var existing = db.Users.FirstOrDefault(u => u.Email == email);
        if (existing is not null) return existing;

        var user = new User
        {
            FirstName = first, LastName = last, Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("mediprax2026"),
            Role = role, IsActive = true
        };
        db.Users.Add(user);
        db.SaveChanges();
        return user;
    }

    private static Patient CreatePatient(MediPraxDbContext db, string first, string last, DateOnly dob,
        string? gender, InsuranceType insurance, string provider, string insuranceNo, string? kvnr,
        string? street, string? city, string? plz, string? phone)
    {
        var patient = new Patient
        {
            FirstName = first, LastName = last, DateOfBirth = dob, Gender = gender,
            InsuranceType = insurance, InsuranceProvider = provider, InsuranceNumber = insuranceNo, Kvnr = kvnr,
            Street = street, City = city, PostalCode = plz, Phone = phone
        };
        db.Patients.Add(patient);
        return patient;
    }

    private static Encounter CreateEncounter(MediPraxDbContext db, Patient patient, User doctor,
        DateOnly date, string[] icds, string notes, int duration)
    {
        var enc = new Encounter
        {
            PatientId = patient.Id, DoctorId = doctor.Id,
            EncounterDate = date, Notes = notes,
            Icd10Codes = icds.ToList(), DurationMinutes = duration,
            Status = EncounterStatus.Completed
        };
        db.Encounters.Add(enc);
        db.SaveChanges(); // Need ID for billing items
        return enc;
    }

    private static void AddBilling(MediPraxDbContext db, Encounter enc, Patient patient,
        string gop, string desc, BillingType type, string quarter)
    {
        db.BillingItems.Add(new BillingItem
        {
            EncounterId = enc.Id, PatientId = patient.Id,
            GopCode = gop, GopDescription = desc, BillingType = type, Quarter = quarter
        });
    }

    private static void AddMedication(MediPraxDbContext db, Patient patient, User doctor,
        string name, string wirkstoff, string staerke, string dosierung,
        MedicationCategory? category, int daysSinceSeit, string? hinweis = null,
        bool requiresMonitoring = false, string? monitoringType = null)
    {
        db.Medications.Add(new Medication
        {
            PatientId = patient.Id, PrescribedById = doctor.Id,
            Name = name, Wirkstoff = wirkstoff, Staerke = staerke, Dosierung = dosierung,
            Category = category, SeitDatum = DateOnly.FromDateTime(DateTime.Today.AddDays(daysSinceSeit)),
            Hinweis = hinweis, IsActive = true,
            RequiresMonitoring = requiresMonitoring, MonitoringType = monitoringType
        });
    }

    private static void ReSeedAppointments(MediPraxDbContext db)
    {
        var w = db.Patients.First(p => p.LastName == "Weber" && p.FirstName == "Klaus");
        var mu = db.Patients.First(p => p.LastName == "Müller" && p.FirstName == "Maria");
        var ho = db.Patients.First(p => p.LastName == "Hoffmann" && p.FirstName == "Stefan");
        var fi = db.Patients.First(p => p.LastName == "Fischer" && p.FirstName == "Ursula");
        var br = db.Patients.First(p => p.LastName == "Braun" && p.FirstName == "Thomas");
        var kl = db.Patients.First(p => p.LastName == "Klein" && p.FirstName == "Sabrina");
        var sc = db.Patients.First(p => p.LastName == "Schulz" && p.FirstName == "Jürgen");
        var la = db.Patients.First(p => p.LastName == "Lang" && p.FirstName == "Petra");
        var ri = db.Patients.FirstOrDefault(p => p.LastName == "Richter") ?? la;
        var wo = db.Patients.FirstOrDefault(p => p.LastName == "Wolf") ?? w;
        var sf = db.Patients.FirstOrDefault(p => p.LastName == "Schäfer") ?? mu;
        var ot = db.Patients.FirstOrDefault(p => p.LastName == "Otto") ?? ho;
        var se = db.Patients.FirstOrDefault(p => p.LastName == "Seidel") ?? br;
        var kr = db.Patients.FirstOrDefault(p => p.LastName == "Krämer") ?? kl;
        var m = db.Users.First(u => u.Email == "meier@neuropsych-bremen.de");
        var s = db.Users.First(u => u.Email == "schmidt@neuropsych-bremen.de");
        var b = db.Users.FirstOrDefault(u => u.Email == "bauer@neuropsych-bremen.de") ?? m;
        var wg = db.Users.FirstOrDefault(u => u.Email == "wagner@neuropsych-bremen.de") ?? s;
        var ks = db.Users.FirstOrDefault(u => u.Email == "krause@neuropsych-bremen.de") ?? m;
        var lh = db.Users.FirstOrDefault(u => u.Email == "lehmann@neuropsych-bremen.de") ?? s;
        var fr = db.Users.FirstOrDefault(u => u.Email == "frank@neuropsych-bremen.de") ?? m;
        var vg = db.Users.FirstOrDefault(u => u.Email == "vogt@neuropsych-bremen.de") ?? s;
        SeedAppointments(db, w, mu, ho, fi, br, kl, sc, la, ri, wo, sf, ot, se, kr, m, s, b, wg, ks, lh, fr, vg);
    }

    private static void AddAppointment(MediPraxDbContext db, Patient patient, User doctor,
        DateTime start, int duration, string? notes)
    {
        db.Appointments.Add(new Appointment
        {
            PatientId = patient.Id, DoctorId = doctor.Id,
            StartTime = DateTime.SpecifyKind(start, DateTimeKind.Utc),
            DurationMinutes = duration, Notes = notes
        });
    }
}
