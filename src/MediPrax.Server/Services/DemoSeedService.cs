using MediPrax.Application.Data;
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
        // --- ICD-10 codes (always seed if table is empty) ---
        SeedIcd10Codes(db);

        // Always ensure all doctors exist BEFORE seeding data that requires a doctor
        var drMeier = EnsureUser(db, "Dr. Thomas", "Meier", "meier@neuropsych-bremen.de", UserRole.Arzt);
        var drSchmidt = EnsureUser(db, "Dr. Anna", "Schmidt", "schmidt@neuropsych-bremen.de", UserRole.Arzt);
        var drBauer = EnsureUser(db, "Dr. Michael", "Bauer", "bauer@neuropsych-bremen.de", UserRole.Arzt);
        var drWagner = EnsureUser(db, "Dr. Claudia", "Wagner", "wagner@neuropsych-bremen.de", UserRole.Arzt);
        var drKrause = EnsureUser(db, "Dr. Stefan", "Krause", "krause@neuropsych-bremen.de", UserRole.Arzt);
        var drLehmann = EnsureUser(db, "Dr. Petra", "Lehmann", "lehmann@neuropsych-bremen.de", UserRole.Arzt);
        var drFrank = EnsureUser(db, "Dr. Jan", "Frank", "frank@neuropsych-bremen.de", UserRole.Arzt);
        var drVogt = EnsureUser(db, "Dr. Sabine", "Vogt", "vogt@neuropsych-bremen.de", UserRole.Arzt);

        // --- Medication catalog (always seed if table is empty, needs no doctor) ---
        SeedMedicationCatalog(db);

        // --- Text modules / Textbausteine (always seed if table is empty, needs doctor) ---
        SeedTextModules(db);

        // --- Action chains / Aktionsketten (always seed if table is empty, needs doctor) ---
        SeedActionChains(db);

        // --- Sprechzeiten (M40) ---
        if (!db.DoctorScheduleTemplates.Any())
        {
            var allDocs = new[] { drMeier, drSchmidt, drBauer, drWagner, drKrause, drLehmann, drFrank, drVogt };
            foreach (var doc in allDocs)
            {
                // Mo-Do: 08:00-12:30 + 14:00-17:00
                for (var d = DayOfWeek.Monday; d <= DayOfWeek.Thursday; d++)
                {
                    db.DoctorScheduleTemplates.Add(new DoctorScheduleTemplate { DoctorId = doc.Id, DayOfWeek = d, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(12, 30), SlotDurationMinutes = 25 });
                    db.DoctorScheduleTemplates.Add(new DoctorScheduleTemplate { DoctorId = doc.Id, DayOfWeek = d, StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(17, 0), SlotDurationMinutes = 25 });
                }
                // Fr: 08:00-13:00 (nur vormittags)
                db.DoctorScheduleTemplates.Add(new DoctorScheduleTemplate { DoctorId = doc.Id, DayOfWeek = DayOfWeek.Friday, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(13, 0), SlotDurationMinutes = 25 });
            }

            // Demo absences
            db.DoctorAbsences.Add(new DoctorAbsence { DoctorId = drMeier.Id, StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(21)), EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(35)), AbsenceType = AbsenceType.Urlaub, Reason = "Sommerurlaub" });
            db.DoctorAbsences.Add(new DoctorAbsence { DoctorId = drSchmidt.Id, StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(14)), EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(16)), AbsenceType = AbsenceType.Fortbildung, Reason = "DGN Kongress" });
            db.DoctorAbsences.Add(new DoctorAbsence { DoctorId = drBauer.Id, StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)), EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)), StartTime = new TimeOnly(12, 0), EndTime = new TimeOnly(14, 0), AbsenceType = AbsenceType.Sperrzeit, Reason = "Teambesprechung" });

            db.SaveChanges();
        }

        // --- Bulk patients (stress test) ---
        if (db.Patients.Count() < 100)
            SeedBulkPatients(db);

        // Check if demo data was already seeded
        var hasWeber = db.Patients.Any(p => p.LastName == "Weber" && p.FirstName == "Klaus");
        // Re-seed appointments on every startup so they always cover the next 2 weeks
        if (hasWeber)
        {
            // Clear stale appointments and re-generate with fresh dates
            // Use raw SQL to avoid EF change tracking / concurrency issues
            db.Database.ExecuteSqlRaw("UPDATE encounters SET \"AppointmentId\" = NULL WHERE \"AppointmentId\" IS NOT NULL");
            db.Database.ExecuteSqlRaw("DELETE FROM appointments");
            ReSeedAppointments(db);
            return;
        }

        // --- Users (Doctors + MFA) ---
        var mfaKoch = EnsureUser(db, "Sabine", "Koch", "koch@neuropsych-bremen.de", UserRole.MFA);

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

        // --- Waitlist entries ---
        if (!db.WaitlistEntries.Any())
        {
            db.WaitlistEntries.AddRange(
                new WaitlistEntry { PatientId = richter.Id, PreferredTherapistId = drMeier.Id, RequestDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-92)), Priority = WaitlistPriority.Normal, PreferredTimeSlot = "Vormittags", Notes = "VT gewünscht, Angststörung", Status = WaitlistStatus.Waiting },
                new WaitlistEntry { PatientId = wolf.Id, PreferredTherapistId = drLehmann.Id, RequestDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-75)), Priority = WaitlistPriority.Urgent, PreferredTimeSlot = "Nachmittags", Notes = "Schwere Depression, dringend", Status = WaitlistStatus.Waiting },
                new WaitlistEntry { PatientId = otto.Id, RequestDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-60)), Priority = WaitlistPriority.Normal, Notes = "TP bevorzugt, Burnout", Status = WaitlistStatus.Waiting },
                new WaitlistEntry { PatientId = seidel.Id, PreferredTherapistId = drMeier.Id, RequestDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-45)), Priority = WaitlistPriority.Normal, PreferredTimeSlot = "Vormittags", Notes = "PTBS, Überweisung Hausarzt", Status = WaitlistStatus.Waiting },
                new WaitlistEntry { PatientId = kramer.Id, PreferredTherapistId = drVogt.Id, RequestDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-30)), Priority = WaitlistPriority.Urgent, Notes = "Essstörung, BMI 16", Status = WaitlistStatus.Waiting },
                new WaitlistEntry { PatientId = schaefer.Id, RequestDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-120)), Priority = WaitlistPriority.Normal, PreferredTimeSlot = "Nachmittags", Notes = "Schmerztherapie, chronisch", Status = WaitlistStatus.Offered, OfferedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)) },
                new WaitlistEntry { PatientId = lang.Id, PreferredTherapistId = drMeier.Id, RequestDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-150)), Priority = WaitlistPriority.Normal, Notes = "VT abgeschlossen, jetzt eingeplant", Status = WaitlistStatus.Scheduled }
            );
            db.SaveChanges();
        }

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
        var notePool = new[] {
            "Kontrolltermin", "VT Sitzung", "Medikamentenkontrolle", "EEG-Kontrolle",
            "Befundbesprechung", "Erstgespräch", "Krisenintervention", "Folgetermin",
            "Diagnostik", "Arztbrief", "Überweisung", "Psychotherapie",
            "EMG/NLG", "Doppler", "MRT-Besprechung", "Verlaufskontrolle",
            "Suizidalitätseinschätzung", "Angehörigengespräch", "MMST", "PHQ-9"
        };
        var durations = new[] { 25, 50, 25, 30, 25, 50, 25, 25, 50, 25 };
        var rng = new Random(42); // deterministic

        // Generate dense schedule for 3 weeks: current week + next 2 weeks
        // Each doctor gets 5-8 appointments per day, Mo-Fr
        for (var weekOffset = 0; weekOffset < 3; weekOffset++)
        {
            var weekStart = monday.AddDays(weekOffset * 7);

            for (var dayOffset = 0; dayOffset < 5; dayOffset++)
            {
                foreach (var doc in docs)
                {
                    // Morning block: 07:30 - 12:30
                    var morningSlots = new[] { 7.5, 8.0, 8.5, 9.0, 9.5, 10.0, 10.5, 11.0, 11.5, 12.0 };
                    var morningCount = rng.Next(5, 8);
                    var usedSlots = new HashSet<double>();
                    for (var i = 0; i < morningCount; i++)
                    {
                        var slot = morningSlots[rng.Next(morningSlots.Length)];
                        if (usedSlots.Contains(slot)) continue;
                        usedSlots.Add(slot);
                        var pt = pts[rng.Next(pts.Length)];
                        var dur = durations[rng.Next(durations.Length)];
                        var note = notePool[rng.Next(notePool.Length)];
                        AddAppointment(db, pt, doc, weekStart.AddDays(dayOffset).AddHours(slot), dur, note);
                    }

                    // Afternoon block: 14:00 - 17:00 (Mo-Do only in weeks 2+)
                    if (dayOffset < 4 || weekOffset == 0) // Friday has no afternoon except current week
                    {
                        var afternoonSlots = new[] { 14.0, 14.5, 15.0, 15.5, 16.0, 16.5 };
                        var afternoonCount = rng.Next(2, 5);
                        usedSlots.Clear();
                        for (var i = 0; i < afternoonCount; i++)
                        {
                            var slot = afternoonSlots[rng.Next(afternoonSlots.Length)];
                            if (usedSlots.Contains(slot)) continue;
                            usedSlots.Add(slot);
                            var pt = pts[rng.Next(pts.Length)];
                            var dur = durations[rng.Next(durations.Length)];
                            var note = notePool[rng.Next(notePool.Length)];
                            AddAppointment(db, pt, doc, weekStart.AddDays(dayOffset).AddHours(slot), dur, note);
                        }
                    }
                }
            }
        }

        // ===== WEEK 1 (current): Named appointments for Dr. Meier & Dr. Schmidt =====

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

        // ===== WEEK 2: Named follow-up appointments =====
        var week2 = monday.AddDays(7);

        // MONTAG W2 — Dr. Meier
        AddAppointment(db, hoffmann, drMeier, week2.AddHours(8), 50, "VT Sitzung 8");
        AddAppointment(db, weber, drMeier, week2.AddHours(9), 25, "Depression Verlaufskontrolle");
        AddAppointment(db, schulz, drMeier, week2.AddHours(10), 25, "MMST Wiederholung");
        AddAppointment(db, lang, drMeier, week2.AddHours(10.5), 50, "Psychotherapie probatorische Sitzung 1");
        AddAppointment(db, richter, drMeier, week2.AddHours(14), 50, "Erstgespräch PTBS");
        AddAppointment(db, wolf, drMeier, week2.AddHours(15), 50, "Krisenintervention Depression");
        // MONTAG W2 — Dr. Schmidt
        AddAppointment(db, mueller, drSchmidt, week2.AddHours(8), 30, "Ocrelizumab Infusionsvorbereitung");
        AddAppointment(db, fischer, drSchmidt, week2.AddHours(9), 25, "Parkinson Medikamentenanpassung");
        AddAppointment(db, braun, drSchmidt, week2.AddHours(9.5), 30, "Epilepsie Laborergebnisse");
        AddAppointment(db, klein, drSchmidt, week2.AddHours(10.5), 25, "Topiramat Verträglichkeit");
        AddAppointment(db, schaefer, drSchmidt, week2.AddHours(14), 25, "Schmerztherapie Kontrolle");

        // DIENSTAG W2 — Dr. Meier
        AddAppointment(db, hoffmann, drMeier, week2.AddDays(1).AddHours(8), 50, "VT Sitzung 9 — Exposition vorbereiten");
        AddAppointment(db, weber, drMeier, week2.AddDays(1).AddHours(9), 25, "Sertralin Nebenwirkungen");
        AddAppointment(db, otto, drMeier, week2.AddDays(1).AddHours(9.5), 50, "Erstgespräch Burnout");
        AddAppointment(db, schulz, drMeier, week2.AddDays(1).AddHours(10.5), 25, "Angehörigengespräch Pflegesituation");
        AddAppointment(db, seidel, drMeier, week2.AddDays(1).AddHours(14), 50, "PTBS Diagnostik");
        // DIENSTAG W2 — Dr. Schmidt
        AddAppointment(db, braun, drSchmidt, week2.AddDays(1).AddHours(8), 30, "EEG Kontrolle");
        AddAppointment(db, mueller, drSchmidt, week2.AddDays(1).AddHours(9), 30, "Fatigue Management");
        AddAppointment(db, klein, drSchmidt, week2.AddDays(1).AddHours(10), 25, "Kopfschmerztagebuch Auswertung");
        AddAppointment(db, fischer, drSchmidt, week2.AddDays(1).AddHours(14), 45, "Doppler Kontrolle");

        // MITTWOCH W2 — Dr. Meier
        AddAppointment(db, hoffmann, drMeier, week2.AddDays(2).AddHours(8), 50, "VT Expositionssitzung");
        AddAppointment(db, lang, drMeier, week2.AddDays(2).AddHours(9.5), 50, "Psychotherapie probatorische Sitzung 2");
        AddAppointment(db, kramer, drMeier, week2.AddDays(2).AddHours(14), 50, "Erstgespräch Essstörung");
        // MITTWOCH W2 — Dr. Schmidt
        AddAppointment(db, fischer, drSchmidt, week2.AddDays(2).AddHours(8), 25, "On-Off Monitoring");
        AddAppointment(db, mueller, drSchmidt, week2.AddDays(2).AddHours(9), 30, "MRT Befundbesprechung");
        AddAppointment(db, braun, drSchmidt, week2.AddDays(2).AddHours(10), 25, "Anfallsfreiheit bestätigen");

        // DONNERSTAG W2 — Dr. Meier
        AddAppointment(db, weber, drMeier, week2.AddDays(3).AddHours(8), 25, "PHQ-9 Verlauf");
        AddAppointment(db, schulz, drMeier, week2.AddDays(3).AddHours(9), 25, "Donepezil Verträglichkeit");
        AddAppointment(db, hoffmann, drMeier, week2.AddDays(3).AddHours(9.5), 25, "Soziale Reintegration");
        AddAppointment(db, wolf, drMeier, week2.AddDays(3).AddHours(14), 50, "Depression Folgetermin");
        AddAppointment(db, lang, drMeier, week2.AddDays(3).AddHours(15), 25, "AU-Verlängerung");
        // DONNERSTAG W2 — Dr. Schmidt
        AddAppointment(db, klein, drSchmidt, week2.AddDays(3).AddHours(8), 25, "Botox-Termin Vorbereitung");
        AddAppointment(db, fischer, drSchmidt, week2.AddDays(3).AddHours(9), 25, "Rigor-Evaluation");
        AddAppointment(db, braun, drSchmidt, week2.AddDays(3).AddHours(10), 30, "Fahrtauglichkeit Epilepsie");
        AddAppointment(db, mueller, drSchmidt, week2.AddDays(3).AddHours(14), 25, "Rezept Ocrelizumab");

        // FREITAG W2 — Dr. Meier (halber Tag)
        AddAppointment(db, hoffmann, drMeier, week2.AddDays(4).AddHours(8), 50, "VT Sitzung 10");
        AddAppointment(db, weber, drMeier, week2.AddDays(4).AddHours(9), 25, "Arztbrief für Arbeitgeber");
        AddAppointment(db, schulz, drMeier, week2.AddDays(4).AddHours(10), 25, "Pflegegrad Gutachten");
        // FREITAG W2 — Dr. Schmidt (halber Tag)
        AddAppointment(db, braun, drSchmidt, week2.AddDays(4).AddHours(8), 30, "Levetiracetam Spiegel");
        AddAppointment(db, klein, drSchmidt, week2.AddDays(4).AddHours(9), 25, "Migräne Prophylaxe Evaluation");
        AddAppointment(db, fischer, drSchmidt, week2.AddDays(4).AddHours(10), 25, "Parkinson Jahres-Review");

        // ===== WEEK 3: Named appointments for continuity =====
        var week3 = monday.AddDays(14);

        // MONTAG W3 — Dr. Meier
        AddAppointment(db, hoffmann, drMeier, week3.AddHours(8), 50, "VT Sitzung 11");
        AddAppointment(db, weber, drMeier, week3.AddHours(9), 25, "Medikamentenkontrolle Sertralin 150mg");
        AddAppointment(db, lang, drMeier, week3.AddHours(10), 50, "Psychotherapie KZT Sitzung 1");
        AddAppointment(db, richter, drMeier, week3.AddHours(14), 50, "PTBS Folgetermin");
        AddAppointment(db, otto, drMeier, week3.AddHours(15), 50, "Burnout Diagnostik Fortsetzung");
        // MONTAG W3 — Dr. Schmidt
        AddAppointment(db, mueller, drSchmidt, week3.AddHours(8), 30, "MS Quartalskontrolle");
        AddAppointment(db, fischer, drSchmidt, week3.AddHours(9), 25, "Parkinson Ganganalyse");
        AddAppointment(db, braun, drSchmidt, week3.AddHours(10), 25, "Epilepsie stabil — Rezept");
        AddAppointment(db, klein, drSchmidt, week3.AddHours(14), 45, "Botox Migräne Durchführung");

        // DIENSTAG W3 — Dr. Meier
        AddAppointment(db, hoffmann, drMeier, week3.AddDays(1).AddHours(8), 50, "VT Sitzung 12 — Rückfallprävention");
        AddAppointment(db, schulz, drMeier, week3.AddDays(1).AddHours(9), 25, "Demenz Verlauf");
        AddAppointment(db, seidel, drMeier, week3.AddDays(1).AddHours(10), 50, "PTBS Traumaexposition");
        AddAppointment(db, kramer, drMeier, week3.AddDays(1).AddHours(14), 50, "Essstörung Folgetermin");
        // DIENSTAG W3 — Dr. Schmidt
        AddAppointment(db, braun, drSchmidt, week3.AddDays(1).AddHours(8), 30, "Anfallskalender Review");
        AddAppointment(db, mueller, drSchmidt, week3.AddDays(1).AddHours(9), 25, "Ergotherapie Rückmeldung");
        AddAppointment(db, fischer, drSchmidt, week3.AddDays(1).AddHours(10), 25, "Levodopa Dosisanpassung");

        // MITTWOCH W3 — Dr. Meier
        AddAppointment(db, weber, drMeier, week3.AddDays(2).AddHours(8), 25, "Depression Remissionsprüfung");
        AddAppointment(db, lang, drMeier, week3.AddDays(2).AddHours(9), 50, "KZT Sitzung 2");
        AddAppointment(db, wolf, drMeier, week3.AddDays(2).AddHours(14), 50, "Depression Therapiebeginn VT");
        // MITTWOCH W3 — Dr. Schmidt
        AddAppointment(db, klein, drSchmidt, week3.AddDays(2).AddHours(8), 25, "Botox Nachkontrolle");
        AddAppointment(db, fischer, drSchmidt, week3.AddDays(2).AddHours(9), 25, "Dysarthrie Evaluation");
        AddAppointment(db, mueller, drSchmidt, week3.AddDays(2).AddHours(10), 30, "Kognitive Testung MS");

        // DONNERSTAG W3 — Dr. Meier
        AddAppointment(db, hoffmann, drMeier, week3.AddDays(3).AddHours(8), 50, "VT Sitzung 13");
        AddAppointment(db, schulz, drMeier, week3.AddDays(3).AddHours(9), 25, "Melperon Reduktionsversuch");
        AddAppointment(db, weber, drMeier, week3.AddDays(3).AddHours(10), 25, "Wiedereingliederung planen");
        AddAppointment(db, otto, drMeier, week3.AddDays(3).AddHours(14), 25, "Burnout AU-Bescheinigung");
        // DONNERSTAG W3 — Dr. Schmidt
        AddAppointment(db, braun, drSchmidt, week3.AddDays(3).AddHours(8), 30, "EEG + Blutbild");
        AddAppointment(db, fischer, drSchmidt, week3.AddDays(3).AddHours(9.5), 25, "Pramipexol Nebenwirkungen");
        AddAppointment(db, klein, drSchmidt, week3.AddDays(3).AddHours(10.5), 25, "Kopfschmerzfrequenz Evaluation");

        // FREITAG W3 — Dr. Meier (halber Tag)
        AddAppointment(db, lang, drMeier, week3.AddDays(4).AddHours(8), 50, "KZT Sitzung 3");
        AddAppointment(db, weber, drMeier, week3.AddDays(4).AddHours(9), 25, "Sertralin stabil — Rezept");
        // FREITAG W3 — Dr. Schmidt (halber Tag)
        AddAppointment(db, mueller, drSchmidt, week3.AddDays(4).AddHours(8), 25, "Rezept + Labor");
        AddAppointment(db, braun, drSchmidt, week3.AddDays(4).AddHours(9), 25, "Epilepsie Zusammenfassung");
        AddAppointment(db, fischer, drSchmidt, week3.AddDays(4).AddHours(10), 25, "Parkinson stabil");

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
        // Convert local time (CEST/CET) to UTC for storage
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
        var localTime = DateTime.SpecifyKind(start, DateTimeKind.Unspecified);
        var utcTime = TimeZoneInfo.ConvertTimeToUtc(localTime, tz);

        db.Appointments.Add(new Appointment
        {
            PatientId = patient.Id, DoctorId = doctor.Id,
            StartTime = utcTime,
            DurationMinutes = duration, Notes = notes
        });
    }

    private static void SeedMedicationCatalog(MediPraxDbContext db)
    {
        if (db.MedicationCatalog.Any())
            return;

        try
        {
            db.Database.ExecuteSqlRaw(MedicationCatalogSeedSql);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not seed medication catalog: {ex.Message}");
        }
    }

    private static void SeedActionChains(MediPraxDbContext db)
    {
        if (db.Set<ActionChain>().Any())
            return;

        var doctor = db.Users.FirstOrDefault(u => u.Role == UserRole.Arzt);
        if (doctor is null) return;

        var chains = new (string Shortcut, string Title, string Category, string Desc, (ActionStepType Type, string Config)[] Steps)[]
        {
            ("dep", "Depression Standard", "Psychiatrie",
                "Standardkette für depressive Episode: Diagnose F32.1, GOP 21220, Vorlage psych",
                new[] {
                    (ActionStepType.AddDiagnosis, "{\"icd10Code\":\"F32.1\",\"certainty\":\"Gesichert\",\"diagnosisType\":\"Encounterdiagnose\"}"),
                    (ActionStepType.AddBillingCode, "{\"gopCode\":\"21220\",\"description\":\"Psychiatrisches Gespräch\",\"quantity\":1}"),
                    (ActionStepType.SetNoteTemplate, "{\"template\":\"psych\"}")
                }),
            ("angst", "Angststörung", "Psychiatrie",
                "Generalisierte Angststörung: F41.1, GOP 21220",
                new[] {
                    (ActionStepType.AddDiagnosis, "{\"icd10Code\":\"F41.1\",\"certainty\":\"Gesichert\",\"diagnosisType\":\"Encounterdiagnose\"}"),
                    (ActionStepType.AddBillingCode, "{\"gopCode\":\"21220\",\"description\":\"Psychiatrisches Gespräch\",\"quantity\":1}"),
                    (ActionStepType.SetNoteTemplate, "{\"template\":\"psych\"}")
                }),
            ("epi", "Epilepsie Kontrolle", "Neurologie",
                "Epilepsiekontrolle: G40.9, GOP 16220 + 16311",
                new[] {
                    (ActionStepType.AddDiagnosis, "{\"icd10Code\":\"G40.9\",\"certainty\":\"Gesichert\",\"diagnosisType\":\"Encounterdiagnose\"}"),
                    (ActionStepType.AddBillingCode, "{\"gopCode\":\"16220\",\"description\":\"Neurologisches Gespräch\",\"quantity\":1}"),
                    (ActionStepType.AddBillingCode, "{\"gopCode\":\"16311\",\"description\":\"EEG\",\"quantity\":1}"),
                    (ActionStepType.SetNoteTemplate, "{\"template\":\"neuro\"}")
                }),
            ("erstgespraech", "Psychiatrisches Erstgespräch", "Psychiatrie",
                "Erstgespräch: GOP 21210 + 21220, 50 Min.",
                new[] {
                    (ActionStepType.AddBillingCode, "{\"gopCode\":\"21210\",\"description\":\"Grundpauschale Psychiatrie\",\"quantity\":1}"),
                    (ActionStepType.AddBillingCode, "{\"gopCode\":\"21220\",\"description\":\"Psychiatrisches Gespräch\",\"quantity\":1}"),
                    (ActionStepType.SetNoteTemplate, "{\"template\":\"psych\"}"),
                    (ActionStepType.SetDuration, "{\"durationMinutes\":50}")
                }),
            ("schmerz", "Chronischer Schmerz", "Neurologie",
                "Chronischer Schmerz: R52.2, GOP 16220",
                new[] {
                    (ActionStepType.AddDiagnosis, "{\"icd10Code\":\"R52.2\",\"certainty\":\"Gesichert\",\"diagnosisType\":\"Encounterdiagnose\"}"),
                    (ActionStepType.AddBillingCode, "{\"gopCode\":\"16220\",\"description\":\"Neurologisches Gespräch\",\"quantity\":1}"),
                    (ActionStepType.SetNoteTemplate, "{\"template\":\"neuro\"}")
                }),
            ("demenz", "Demenz Kontrolle", "Psychiatrie",
                "Demenzkontrolle: F00.1, GOP 21220, Wiedervorlage 180 Tage",
                new[] {
                    (ActionStepType.AddDiagnosis, "{\"icd10Code\":\"F00.1\",\"certainty\":\"Gesichert\",\"diagnosisType\":\"Encounterdiagnose\"}"),
                    (ActionStepType.AddBillingCode, "{\"gopCode\":\"21220\",\"description\":\"Psychiatrisches Gespräch\",\"quantity\":1}"),
                    (ActionStepType.SetNoteTemplate, "{\"template\":\"psych\"}"),
                    (ActionStepType.CreateRecall, "{\"reason\":\"Demenz-Kontrolle\",\"daysFromNow\":180}")
                }),
            ("park", "Parkinson Kontrolle", "Neurologie",
                "Parkinson: G20, GOP 16220",
                new[] {
                    (ActionStepType.AddDiagnosis, "{\"icd10Code\":\"G20\",\"certainty\":\"Gesichert\",\"diagnosisType\":\"Encounterdiagnose\"}"),
                    (ActionStepType.AddBillingCode, "{\"gopCode\":\"16220\",\"description\":\"Neurologisches Gespräch\",\"quantity\":1}"),
                    (ActionStepType.SetNoteTemplate, "{\"template\":\"neuro\"}")
                }),
            ("ms", "Multiple Sklerose", "Neurologie",
                "MS-Kontrolle: G35, GOP 16220, Wiedervorlage 90 Tage",
                new[] {
                    (ActionStepType.AddDiagnosis, "{\"icd10Code\":\"G35\",\"certainty\":\"Gesichert\",\"diagnosisType\":\"Encounterdiagnose\"}"),
                    (ActionStepType.AddBillingCode, "{\"gopCode\":\"16220\",\"description\":\"Neurologisches Gespräch\",\"quantity\":1}"),
                    (ActionStepType.SetNoteTemplate, "{\"template\":\"neuro\"}"),
                    (ActionStepType.CreateRecall, "{\"reason\":\"MS-Kontrolle\",\"daysFromNow\":90}")
                }),
        };

        for (int i = 0; i < chains.Length; i++)
        {
            var c = chains[i];
            var chain = new ActionChain
            {
                Shortcut = c.Shortcut,
                Title = c.Title,
                Category = c.Category,
                Description = c.Desc,
                CreatedById = doctor.Id,
                IsGlobal = true,
                IsActive = true,
                SortOrder = i + 1
            };
            db.Set<ActionChain>().Add(chain);
            db.SaveChanges(); // Save to get the chain Id

            for (int j = 0; j < c.Steps.Length; j++)
            {
                var s = c.Steps[j];
                db.Set<ActionChainStep>().Add(new ActionChainStep
                {
                    ActionChainId = chain.Id,
                    StepType = s.Type,
                    SortOrder = j + 1,
                    Configuration = s.Config
                });
            }
            db.SaveChanges();
        }
    }

    private static void SeedTextModules(MediPraxDbContext db)
    {
        if (db.Set<TextModule>().Any())
            return;

        // Need a user for CreatedById — find first doctor
        var doctor = db.Users.FirstOrDefault(u => u.Role == UserRole.Arzt);
        if (doctor is null) return; // Users not yet seeded — will be seeded on next restart

        var modules = new List<TextModule>
        {
            new() { Shortcut = "normpsy", Title = "Normaler psychopathologischer Befund", Category = "Psychiatrie/Befund",
                TargetSection = EncounterSectionType.Befund, IsGlobal = true, CreatedById = doctor.Id,
                Content = "Bewusstsein klar, zur Person, Ort, Zeit und Situation voll orientiert. Aufmerksamkeit und Konzentration unbeeinträchtigt. Auffassung und Mnestik intakt. Formales Denken geordnet, kohärent, keine Perseveration. Inhaltlich kein Wahn, keine Zwänge. Wahrnehmung ungestört, keine Halluzinationen. Ich-Erleben intakt. Affekt ausgeglichen, modulationsfähig, Stimmung euthym. Antrieb und Psychomotorik unauffällig. Keine Suizidalität, keine Fremdgefährdung." },
            new() { Shortcut = "depbefund", Title = "Depressiver Befund", Category = "Psychiatrie/Befund",
                TargetSection = EncounterSectionType.Befund, IsGlobal = true, CreatedById = doctor.Id,
                Content = "Bewusstsein klar, allseits orientiert. Konzentration und Aufmerksamkeit subjektiv reduziert. Formales Denken verlangsamt, grüblerisch, eingeengt auf depressive Thematik. Inhaltlich Insuffizienz- und Schuldgefühle, keine Wahnphänomene. Wahrnehmung ungestört. Affekt deprimiert, Stimmung gedrückt, Schwingungsfähigkeit eingeschränkt. Antrieb vermindert, Psychomotorik verlangsamt. Schlafstörungen (Ein-/Durchschlaf). Appetitminderung. Suizidalität exploriert: aktuell keine akute Suizidalität, Absprachefähigkeit gegeben." },
            new() { Shortcut = "normneuro", Title = "Normaler neurologischer Befund", Category = "Neurologie/Befund",
                TargetSection = EncounterSectionType.Befund, IsGlobal = true, CreatedById = doctor.Id,
                Content = "Hirnnerven: Pupillen isokor, direkte und konsensuelle Lichtreaktion prompt. Augenmotilität frei, kein Nystagmus. Gesichtssensorik und -motorik seitengleich. Gaumensegel hebt seitengleich. Zunge median. Motorik: Muskeltonus und -trophik unauffällig, keine Paresen. MER seitengleich mittellebhaft auslösbar. Pyramidenbahnzeichen negativ. Sensibilität: Oberflächen- und Tiefensensibilität intakt. Koordination: Finger-Nase-Versuch und Knie-Hacke-Versuch zielsicher. Romberg negativ. Stand und Gang sicher." },
            new() { Shortcut = "erstanamnese", Title = "Erstgespräch Anamnese", Category = "Psychiatrie/Anamnese",
                TargetSection = EncounterSectionType.Anamnese, IsGlobal = true, CreatedById = doctor.Id,
                Content = "Vorstellungsgrund: {Patient.Name}, {Patient.Alter} Jahre, stellt sich erstmalig in unserer Praxis vor.\n\nAktuelle Beschwerden:\n\nPsychiatrische Anamnese:\n\nSomatische Anamnese:\n\nFamilienanamnese:\n\nSozialanamnese:\n\nMedikamentenanamnese:\n\nSubstanzanamnese:" },
            new() { Shortcut = "therplan", Title = "Therapieplan Standard", Category = "Allgemein/Therapie",
                TargetSection = EncounterSectionType.Therapie, IsGlobal = true, CreatedById = doctor.Id,
                Content = "1. Medikamentöse Therapie: [Medikament, Dosis, Schema]\n2. Psychotherapie: [Art, Frequenz]\n3. Soziotherapeutische Maßnahmen: [falls indiziert]\n4. Kontrolltermin: [Datum/Intervall]\n5. Laborkontrollen: [falls erforderlich]" },
            new() { Shortcut = "wv", Title = "Wiedervorstellung", Category = "Allgemein/Procedere",
                TargetSection = EncounterSectionType.Procedere, IsGlobal = true, CreatedById = doctor.Id,
                Content = "Wiedervorstellung in [2/4/6/8] Wochen zur Verlaufskontrolle. Bei Verschlechterung umgehende Vorstellung. Medikation wie besprochen [fortführen/anpassen]. Nächster Labortermin: [Datum]." },
            new() { Shortcut = "briefein", Title = "Arztbrief Einleitung", Category = "Allgemein/Arztbrief",
                TargetSection = null, IsGlobal = true, CreatedById = doctor.Id,
                Content = "Wir berichten über unseren gemeinsamen Patienten {Patient.Name}, geb. {Patient.Geburtsdatum}, der sich am {Encounter.Datum} in unserer psychiatrisch-neurologischen Praxis vorstellte." },
            new() { Shortcut = "briefschluss", Title = "Arztbrief Schluss", Category = "Allgemein/Arztbrief",
                TargetSection = null, IsGlobal = true, CreatedById = doctor.Id,
                Content = "Für Rückfragen stehen wir Ihnen gerne zur Verfügung.\n\nMit freundlichen kollegialen Grüßen\n\n{Arzt.Titel} {Arzt.Name}\nFacharzt für Psychiatrie und Neurologie" },
            new() { Shortcut = "diagliste", Title = "Diagnosenliste", Category = "Allgemein/Diagnose",
                TargetSection = EncounterSectionType.Diagnose, IsGlobal = true, CreatedById = doctor.Id,
                Content = "Diagnosen:\n{Diagnosen}\n\nDauerdiagnosen:\n{Dauerdiagnosen}" },
            new() { Shortcut = "medliste", Title = "Aktuelle Medikation", Category = "Allgemein/Therapie",
                TargetSection = EncounterSectionType.Therapie, IsGlobal = true, CreatedById = doctor.Id,
                Content = "Aktuelle Medikation:\n{Medikation}" },
            new() { Shortcut = "kurz", Title = "Kurzkonsultation", Category = "Psychiatrie/Kurzkonsultation",
                TargetSection = EncounterSectionType.Befund, IsGlobal = true, CreatedById = doctor.Id,
                Content = "Kurzkonsultation am {Datum}. Patient berichtet [Befinden]. Psychopathologisch [Befund]. Medikation wird [beibehalten/angepasst]. Nächster Termin in [X] Wochen." },
            new() { Shortcut = "ksanamnese", Title = "Kopfschmerz-Anamnese", Category = "Neurologie/Anamnese",
                TargetSection = EncounterSectionType.Anamnese, IsGlobal = true, CreatedById = doctor.Id,
                Content = "Kopfschmerzcharakter: [dumpf-drückend/pulsierend/stechend]\nLokalisation: [frontal/temporal/okzipital/hemikraniell]\nIntensität (VAS 0-10):\nDauer: [Stunden/Tage]\nFrequenz: [pro Woche/Monat]\nBegleitsymptome: [Übelkeit/Erbrechen/Phono-/Photophobie/Aura]\nAuslöser: [Stress/Schlafmangel/Alkohol/Menstruation]\nBisherige Therapie:\nVorbehandlung:" },
        };

        db.Set<TextModule>().AddRange(modules);
        db.SaveChanges();
    }

    private static void SeedIcd10Codes(MediPraxDbContext db)
    {
        if (db.Set<Icd10Code>().Any())
            return;

        var entries = Icd10Catalog.AllEntries;
        foreach (var entry in entries)
        {
            db.Set<Icd10Code>().Add(new Icd10Code
            {
                Code = entry.Code,
                Description = entry.Description,
                Category = GetIcd10Category(entry.Code),
                IsActive = true
            });
        }
        db.SaveChanges();
    }

    private static string GetIcd10Category(string code)
    {
        if (code.StartsWith("F0")) return "Organische Störungen";
        if (code.StartsWith("F1")) return "Störungen durch psychotrope Substanzen";
        if (code.StartsWith("F2")) return "Schizophrenie und wahnhafte Störungen";
        if (code.StartsWith("F3")) return "Affektive Störungen";
        if (code.StartsWith("F4")) return "Neurotische Störungen";
        if (code.StartsWith("F5")) return "Verhaltensauffälligkeiten";
        if (code.StartsWith("F6")) return "Persönlichkeitsstörungen";
        if (code.StartsWith("F9")) return "ADHS und Verhaltensstörungen";
        if (code.StartsWith("G")) return "Neurologie";
        if (code.StartsWith("I")) return "Schlaganfall";
        if (code.StartsWith("R")) return "Symptome";
        if (code.StartsWith("Z")) return "Faktoren";
        return "Sonstige";
    }

    /// <summary>
    /// Embedded SQL for seeding the medication catalog (279 rows).
    /// This is extracted from migration 20260413174153_AddMedicationCatalog
    /// and embedded here so it works in Docker where migration .cs files are not available.
    /// </summary>
    private const string MedicationCatalogSeedSql = """
INSERT INTO medication_catalog ("Id","Pzn","Handelsname","Wirkstoff","AtcCode","Staerke","Darreichungsform","Packungsgroesse","NormPackungsgroesse","Hersteller","IsBtm","IsTRezeptPflichtig","IsVerschreibungspflichtig","Category","IsActive","DataSource","LastUpdated")
VALUES
-- ═══ ANTIDEPRESSIVA (SSRI) ═══
('a0000001-0001-0001-0001-000000000001','10010001','Sertralin HEXAL 50mg','Sertralin','N06AB06','50 mg','Filmtabletten','20 Stk','N1','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000002','10010002','Sertralin HEXAL 50mg','Sertralin','N06AB06','50 mg','Filmtabletten','50 Stk','N2','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000003','10010003','Sertralin HEXAL 50mg','Sertralin','N06AB06','50 mg','Filmtabletten','100 Stk','N3','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000004','10010004','Sertralin HEXAL 100mg','Sertralin','N06AB06','100 mg','Filmtabletten','20 Stk','N1','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000005','10010005','Sertralin HEXAL 100mg','Sertralin','N06AB06','100 mg','Filmtabletten','50 Stk','N2','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000006','10010006','Sertralin HEXAL 100mg','Sertralin','N06AB06','100 mg','Filmtabletten','100 Stk','N3','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000007','10010007','Cipralex 10mg','Escitalopram','N06AB10','10 mg','Filmtabletten','20 Stk','N1','Lundbeck',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000008','10010008','Cipralex 10mg','Escitalopram','N06AB10','10 mg','Filmtabletten','50 Stk','N2','Lundbeck',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000009','10010009','Cipralex 10mg','Escitalopram','N06AB10','10 mg','Filmtabletten','100 Stk','N3','Lundbeck',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000010','10010010','Cipralex 20mg','Escitalopram','N06AB10','20 mg','Filmtabletten','20 Stk','N1','Lundbeck',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000011','10010011','Cipralex 20mg','Escitalopram','N06AB10','20 mg','Filmtabletten','50 Stk','N2','Lundbeck',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000012','10010012','Escitalopram-ratiopharm 10mg','Escitalopram','N06AB10','10 mg','Filmtabletten','20 Stk','N1','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000013','10010013','Escitalopram-ratiopharm 20mg','Escitalopram','N06AB10','20 mg','Filmtabletten','20 Stk','N1','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000014','10010014','Citalopram HEXAL 20mg','Citalopram','N06AB04','20 mg','Filmtabletten','20 Stk','N1','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000015','10010015','Citalopram HEXAL 20mg','Citalopram','N06AB04','20 mg','Filmtabletten','50 Stk','N2','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000016','10010016','Citalopram HEXAL 20mg','Citalopram','N06AB04','20 mg','Filmtabletten','100 Stk','N3','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000017','10010017','Citalopram HEXAL 40mg','Citalopram','N06AB04','40 mg','Filmtabletten','20 Stk','N1','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000018','10010018','Citalopram HEXAL 40mg','Citalopram','N06AB04','40 mg','Filmtabletten','50 Stk','N2','HEXAL',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000019','10010019','Fluoxetin-ratiopharm 20mg','Fluoxetin','N06AB03','20 mg','Kapseln','20 Stk','N1','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000020','10010020','Fluoxetin-ratiopharm 20mg','Fluoxetin','N06AB03','20 mg','Kapseln','50 Stk','N2','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000021','10010021','Fluoxetin-ratiopharm 20mg','Fluoxetin','N06AB03','20 mg','Kapseln','100 Stk','N3','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000022','10010022','Paroxetin-1A Pharma 20mg','Paroxetin','N06AB05','20 mg','Filmtabletten','20 Stk','N1','1A Pharma',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000023','10010023','Paroxetin-1A Pharma 20mg','Paroxetin','N06AB05','20 mg','Filmtabletten','50 Stk','N2','1A Pharma',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0001-0001-0001-000000000024','10010024','Fluvoxamin-neuraxpharm 100mg','Fluvoxamin','N06AB08','100 mg','Filmtabletten','20 Stk','N1','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),

-- ═══ ANTIDEPRESSIVA (SNRI) ═══
('a0000001-0002-0001-0001-000000000001','10020001','Venlafaxin-ratiopharm 75mg','Venlafaxin','N06AX16','75 mg','Retardkapseln','30 Stk','N1','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0002-0001-0001-000000000002','10020002','Venlafaxin-ratiopharm 75mg','Venlafaxin','N06AX16','75 mg','Retardkapseln','100 Stk','N3','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0002-0001-0001-000000000003','10020003','Venlafaxin-ratiopharm 150mg','Venlafaxin','N06AX16','150 mg','Retardkapseln','30 Stk','N1','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0002-0001-0001-000000000004','10020004','Venlafaxin-ratiopharm 150mg','Venlafaxin','N06AX16','150 mg','Retardkapseln','100 Stk','N3','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0002-0001-0001-000000000005','10020005','Cymbalta 30mg','Duloxetin','N06AX21','30 mg','Kapseln','28 Stk','N1','Lilly',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0002-0001-0001-000000000006','10020006','Cymbalta 60mg','Duloxetin','N06AX21','60 mg','Kapseln','28 Stk','N1','Lilly',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0002-0001-0001-000000000007','10020007','Cymbalta 60mg','Duloxetin','N06AX21','60 mg','Kapseln','98 Stk','N3','Lilly',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0002-0001-0001-000000000008','10020008','Duloxetin Zentiva 30mg','Duloxetin','N06AX21','30 mg','Kapseln','28 Stk','N1','Zentiva',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0002-0001-0001-000000000009','10020009','Duloxetin Zentiva 60mg','Duloxetin','N06AX21','60 mg','Kapseln','28 Stk','N1','Zentiva',false,false,true,'Antidepressivum',true,'SEED',NOW()),

-- ═══ ANTIDEPRESSIVA (Trizyklisch) ═══
('a0000001-0003-0001-0001-000000000001','10030001','Amitriptylin-neuraxpharm 25mg','Amitriptylin','N06AA09','25 mg','Filmtabletten','20 Stk','N1','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0003-0001-0001-000000000002','10030002','Amitriptylin-neuraxpharm 25mg','Amitriptylin','N06AA09','25 mg','Filmtabletten','50 Stk','N2','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0003-0001-0001-000000000003','10030003','Amitriptylin-neuraxpharm 25mg','Amitriptylin','N06AA09','25 mg','Filmtabletten','100 Stk','N3','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0003-0001-0001-000000000004','10030004','Amitriptylin-neuraxpharm 75mg','Amitriptylin','N06AA09','75 mg','Retardtabletten','20 Stk','N1','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0003-0001-0001-000000000005','10030005','Amitriptylin-neuraxpharm 75mg','Amitriptylin','N06AA09','75 mg','Retardtabletten','50 Stk','N2','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0003-0001-0001-000000000006','10030006','Clomipramin-neuraxpharm 25mg','Clomipramin','N06AA04','25 mg','Filmtabletten','20 Stk','N1','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0003-0001-0001-000000000007','10030007','Clomipramin-neuraxpharm 75mg','Clomipramin','N06AA04','75 mg','Retardtabletten','20 Stk','N1','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0003-0001-0001-000000000008','10030008','Clomipramin-neuraxpharm 75mg','Clomipramin','N06AA04','75 mg','Retardtabletten','50 Stk','N2','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0003-0001-0001-000000000009','10030009','Doxepin-ratiopharm 25mg','Doxepin','N06AA12','25 mg','Filmtabletten','20 Stk','N1','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0003-0001-0001-000000000010','10030010','Doxepin-ratiopharm 50mg','Doxepin','N06AA12','50 mg','Filmtabletten','20 Stk','N1','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0003-0001-0001-000000000011','10030011','Doxepin-ratiopharm 50mg','Doxepin','N06AA12','50 mg','Filmtabletten','50 Stk','N2','ratiopharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0003-0001-0001-000000000012','10030012','Trimipramin-neuraxpharm 25mg','Trimipramin','N06AA06','25 mg','Filmtabletten','20 Stk','N1','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0003-0001-0001-000000000013','10030013','Trimipramin-neuraxpharm 100mg','Trimipramin','N06AA06','100 mg','Filmtabletten','20 Stk','N1','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),

-- ═══ ANTIDEPRESSIVA (Andere) ═══
('a0000001-0004-0001-0001-000000000001','10040001','Mirtazapin Sandoz 15mg','Mirtazapin','N06AX11','15 mg','Schmelztabletten','30 Stk','N1','Sandoz',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0004-0001-0001-000000000002','10040002','Mirtazapin Sandoz 15mg','Mirtazapin','N06AX11','15 mg','Schmelztabletten','96 Stk','N3','Sandoz',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0004-0001-0001-000000000003','10040003','Mirtazapin Sandoz 30mg','Mirtazapin','N06AX11','30 mg','Schmelztabletten','30 Stk','N1','Sandoz',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0004-0001-0001-000000000004','10040004','Mirtazapin Sandoz 30mg','Mirtazapin','N06AX11','30 mg','Schmelztabletten','96 Stk','N3','Sandoz',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0004-0001-0001-000000000005','10040005','Mirtazapin Sandoz 45mg','Mirtazapin','N06AX11','45 mg','Schmelztabletten','30 Stk','N1','Sandoz',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0004-0001-0001-000000000006','10040006','Elontril 150mg','Bupropion','N06AX12','150 mg','Retardtabletten','30 Stk','N1','GlaxoSmithKline',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0004-0001-0001-000000000007','10040007','Elontril 300mg','Bupropion','N06AX12','300 mg','Retardtabletten','30 Stk','N1','GlaxoSmithKline',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0004-0001-0001-000000000008','10040008','Elontril 300mg','Bupropion','N06AX12','300 mg','Retardtabletten','90 Stk','N3','GlaxoSmithKline',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0004-0001-0001-000000000009','10040009','Valdoxan 25mg','Agomelatin','N06AX22','25 mg','Filmtabletten','28 Stk','N1','Servier',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0004-0001-0001-000000000010','10040010','Valdoxan 25mg','Agomelatin','N06AX22','25 mg','Filmtabletten','98 Stk','N3','Servier',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0004-0001-0001-000000000011','10040011','Trazodon-neuraxpharm 100mg','Trazodon','N06AX05','100 mg','Tabletten','20 Stk','N1','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),
('a0000001-0004-0001-0001-000000000012','10040012','Trazodon-neuraxpharm 100mg','Trazodon','N06AX05','100 mg','Tabletten','50 Stk','N2','neuraxpharm',false,false,true,'Antidepressivum',true,'SEED',NOW()),

-- ═══ ANTIPSYCHOTIKA (Atypisch) ═══
('a0000002-0001-0001-0001-000000000001','20010001','Risperidon-ratiopharm 1mg','Risperidon','N05AX08','1 mg','Filmtabletten','20 Stk','N1','ratiopharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000002','20010002','Risperidon-ratiopharm 2mg','Risperidon','N05AX08','2 mg','Filmtabletten','20 Stk','N1','ratiopharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000003','20010003','Risperidon-ratiopharm 2mg','Risperidon','N05AX08','2 mg','Filmtabletten','50 Stk','N2','ratiopharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000004','20010004','Risperidon-ratiopharm 4mg','Risperidon','N05AX08','4 mg','Filmtabletten','20 Stk','N1','ratiopharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000005','20010005','Zyprexa 5mg','Olanzapin','N05AH03','5 mg','Filmtabletten','28 Stk','N1','Lilly',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000006','20010006','Zyprexa 10mg','Olanzapin','N05AH03','10 mg','Filmtabletten','28 Stk','N1','Lilly',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000007','20010007','Zyprexa 10mg','Olanzapin','N05AH03','10 mg','Filmtabletten','70 Stk','N3','Lilly',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000008','20010008','Olanzapin AbZ 15mg','Olanzapin','N05AH03','15 mg','Filmtabletten','28 Stk','N1','AbZ',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000009','20010009','Seroquel 25mg','Quetiapin','N05AH04','25 mg','Filmtabletten','20 Stk','N1','AstraZeneca',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000010','20010010','Seroquel 100mg','Quetiapin','N05AH04','100 mg','Filmtabletten','20 Stk','N1','AstraZeneca',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000011','20010011','Seroquel 200mg','Quetiapin','N05AH04','200 mg','Filmtabletten','20 Stk','N1','AstraZeneca',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000012','20010012','Seroquel Prolong 300mg','Quetiapin','N05AH04','300 mg','Retardtabletten','30 Stk','N1','AstraZeneca',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000013','20010013','Quetiapin-ratiopharm 25mg','Quetiapin','N05AH04','25 mg','Filmtabletten','20 Stk','N1','ratiopharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000014','20010014','Quetiapin-ratiopharm 100mg','Quetiapin','N05AH04','100 mg','Filmtabletten','100 Stk','N3','ratiopharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000015','20010015','Abilify 10mg','Aripiprazol','N05AX12','10 mg','Tabletten','28 Stk','N1','Otsuka',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000016','20010016','Abilify 15mg','Aripiprazol','N05AX12','15 mg','Tabletten','28 Stk','N1','Otsuka',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000017','20010017','Abilify 30mg','Aripiprazol','N05AX12','30 mg','Tabletten','28 Stk','N1','Otsuka',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000018','20010018','Aripiprazol AbZ 10mg','Aripiprazol','N05AX12','10 mg','Tabletten','28 Stk','N1','AbZ',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000019','20010019','Leponex 25mg','Clozapin','N05AH02','25 mg','Tabletten','50 Stk','N2','Novartis',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000020','20010020','Leponex 100mg','Clozapin','N05AH02','100 mg','Tabletten','50 Stk','N2','Novartis',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000021','20010021','Clozapin-neuraxpharm 25mg','Clozapin','N05AH02','25 mg','Tabletten','50 Stk','N2','neuraxpharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000022','20010022','Clozapin-neuraxpharm 100mg','Clozapin','N05AH02','100 mg','Tabletten','50 Stk','N2','neuraxpharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000023','20010023','Zeldox 40mg','Ziprasidon','N05AE04','40 mg','Kapseln','30 Stk','N1','Pfizer',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000024','20010024','Zeldox 80mg','Ziprasidon','N05AE04','80 mg','Kapseln','30 Stk','N1','Pfizer',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000025','20010025','Latuda 37mg','Lurasidon','N05AE05','37 mg','Filmtabletten','28 Stk','N1','Aziende Chimiche',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000026','20010026','Latuda 74mg','Lurasidon','N05AE05','74 mg','Filmtabletten','28 Stk','N1','Aziende Chimiche',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000027','20010027','Reagila 1.5mg','Cariprazin','N05AX15','1.5 mg','Kapseln','28 Stk','N1','Gedeon Richter',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0001-0001-0001-000000000028','20010028','Reagila 3mg','Cariprazin','N05AX15','3 mg','Kapseln','28 Stk','N1','Gedeon Richter',false,false,true,'Antipsychotikum',true,'SEED',NOW()),

-- ═══ ANTIPSYCHOTIKA (Typisch) ═══
('a0000002-0002-0001-0001-000000000001','20020001','Haldol 1mg','Haloperidol','N05AD01','1 mg','Tabletten','50 Stk','N2','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0002-0001-0001-000000000002','20020002','Haldol 5mg','Haloperidol','N05AD01','5 mg','Tabletten','50 Stk','N2','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0002-0001-0001-000000000003','20020003','Haldol Tropfen 2mg/ml','Haloperidol','N05AD01','2 mg/ml','Tropfen','30 ml','N1','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0002-0001-0001-000000000004','20020004','Melperon-ratiopharm 25mg','Melperon','N05AD03','25 mg','Filmtabletten','20 Stk','N1','ratiopharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0002-0001-0001-000000000005','20020005','Melperon-ratiopharm 100mg','Melperon','N05AD03','100 mg','Filmtabletten','50 Stk','N2','ratiopharm',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0002-0001-0001-000000000006','20020006','Atosil 25mg','Promethazin','N05AA02','25 mg','Filmtabletten','20 Stk','N1','Bayer',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0002-0001-0001-000000000007','20020007','Atosil 25mg','Promethazin','N05AA02','25 mg','Filmtabletten','50 Stk','N2','Bayer',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0002-0001-0001-000000000008','20020008','Dipiperon 40mg','Pipamperon','N05AD05','40 mg','Tabletten','20 Stk','N1','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0002-0001-0001-000000000009','20020009','Dipiperon 40mg','Pipamperon','N05AD05','40 mg','Tabletten','50 Stk','N2','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0002-0001-0001-000000000010','20020010','Fluanxol 0.5mg','Flupentixol','N05AF01','0.5 mg','Filmtabletten','20 Stk','N1','Lundbeck',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0002-0001-0001-000000000011','20020011','Fluanxol 0.5mg','Flupentixol','N05AF01','0.5 mg','Filmtabletten','50 Stk','N2','Lundbeck',false,false,true,'Antipsychotikum',true,'SEED',NOW()),

-- ═══ ANTIPSYCHOTIKA (Depot) ═══
('a0000002-0003-0001-0001-000000000001','20030001','Xeplion 75mg','Paliperidon','N05AX13','75 mg','Depot-Injektion','1 Stk','N1','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0003-0001-0001-000000000002','20030002','Xeplion 150mg','Paliperidon','N05AX13','150 mg','Depot-Injektion','1 Stk','N1','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0003-0001-0001-000000000003','20030003','Trevicta 263mg','Paliperidon','N05AX13','263 mg','Depot-Injektion','1 Stk','N1','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0003-0001-0001-000000000004','20030004','Abilify Maintena 400mg','Aripiprazol','N05AX12','400 mg','Depot-Injektion','1 Stk','N1','Otsuka',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0003-0001-0001-000000000005','20030005','Risperdal Consta 25mg','Risperidon','N05AX08','25 mg','Depot-Injektion','1 Stk','N1','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0003-0001-0001-000000000006','20030006','Risperdal Consta 37.5mg','Risperidon','N05AX08','37.5 mg','Depot-Injektion','1 Stk','N1','Janssen',false,false,true,'Antipsychotikum',true,'SEED',NOW()),
('a0000002-0003-0001-0001-000000000007','20030007','Fluanxol Depot 20mg/ml','Flupentixol','N05AF01','20 mg/ml','Depot-Injektion','1 ml','N1','Lundbeck',false,false,true,'Antipsychotikum',true,'SEED',NOW()),

-- ═══ STIMMUNGSSTABILISIERER ═══
('a0000003-0001-0001-0001-000000000001','30010001','Quilonum retard 450mg','Lithium','N05AN01','450 mg','Retardtabletten','50 Stk','N2','GlaxoSmithKline',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
('a0000003-0001-0001-0001-000000000002','30010002','Quilonum retard 450mg','Lithium','N05AN01','450 mg','Retardtabletten','100 Stk','N3','GlaxoSmithKline',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
('a0000003-0001-0001-0001-000000000003','30010003','Hypnorex retard 400mg','Lithium','N05AN01','400 mg','Retardtabletten','50 Stk','N2','Sanofi',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
('a0000003-0001-0001-0001-000000000004','30010004','Ergenyl chrono 300mg','Valproat','N03AG01','300 mg','Retardtabletten','50 Stk','N2','Sanofi',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
('a0000003-0001-0001-0001-000000000005','30010005','Ergenyl chrono 300mg','Valproat','N03AG01','300 mg','Retardtabletten','100 Stk','N3','Sanofi',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
('a0000003-0001-0001-0001-000000000006','30010006','Ergenyl chrono 500mg','Valproat','N03AG01','500 mg','Retardtabletten','50 Stk','N2','Sanofi',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
('a0000003-0001-0001-0001-000000000007','30010007','Valproat-ratiopharm 600mg','Valproat','N03AG01','600 mg','Retardtabletten','50 Stk','N2','ratiopharm',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
('a0000003-0001-0001-0001-000000000008','30010008','Tegretal 200mg','Carbamazepin','N03AF01','200 mg','Retardtabletten','50 Stk','N2','Novartis',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
('a0000003-0001-0001-0001-000000000009','30010009','Tegretal 400mg','Carbamazepin','N03AF01','400 mg','Retardtabletten','50 Stk','N2','Novartis',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
('a0000003-0001-0001-0001-000000000010','30010010','Carbamazepin-ratiopharm 200mg','Carbamazepin','N03AF01','200 mg','Retardtabletten','50 Stk','N2','ratiopharm',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
('a0000003-0001-0001-0001-000000000011','30010011','Lamictal 25mg','Lamotrigin','N03AX09','25 mg','Tabletten','42 Stk','N1','GlaxoSmithKline',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
('a0000003-0001-0001-0001-000000000012','30010012','Lamictal 50mg','Lamotrigin','N03AX09','50 mg','Tabletten','42 Stk','N1','GlaxoSmithKline',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
('a0000003-0001-0001-0001-000000000013','30010013','Lamictal 100mg','Lamotrigin','N03AX09','100 mg','Tabletten','50 Stk','N2','GlaxoSmithKline',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
('a0000003-0001-0001-0001-000000000014','30010014','Lamictal 200mg','Lamotrigin','N03AX09','200 mg','Tabletten','50 Stk','N2','GlaxoSmithKline',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
('a0000003-0001-0001-0001-000000000015','30010015','Lamotrigin HEXAL 100mg','Lamotrigin','N03AX09','100 mg','Tabletten','100 Stk','N3','HEXAL',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),
('a0000003-0001-0001-0001-000000000016','30010016','Lamotrigin HEXAL 200mg','Lamotrigin','N03AX09','200 mg','Tabletten','100 Stk','N3','HEXAL',false,false,true,'Stimmungsstabilisierer',true,'SEED',NOW()),

-- ═══ ANXIOLYTIKA & HYPNOTIKA ═══
('a0000004-0001-0001-0001-000000000001','40010001','Tavor 0.5mg','Lorazepam','N05BA06','0.5 mg','Tabletten','20 Stk','N1','Pfizer',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
('a0000004-0001-0001-0001-000000000002','40010002','Tavor 1mg','Lorazepam','N05BA06','1 mg','Tabletten','20 Stk','N1','Pfizer',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
('a0000004-0001-0001-0001-000000000003','40010003','Tavor 1mg','Lorazepam','N05BA06','1 mg','Tabletten','50 Stk','N2','Pfizer',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
('a0000004-0001-0001-0001-000000000004','40010004','Tavor 2.5mg','Lorazepam','N05BA06','2.5 mg','Tabletten','20 Stk','N1','Pfizer',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
('a0000004-0001-0001-0001-000000000005','40010005','Lorazepam-neuraxpharm 1mg','Lorazepam','N05BA06','1 mg','Tabletten','20 Stk','N1','neuraxpharm',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
('a0000004-0001-0001-0001-000000000006','40010006','Valium 5mg','Diazepam','N05BA01','5 mg','Tabletten','20 Stk','N1','Roche',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
('a0000004-0001-0001-0001-000000000007','40010007','Valium 10mg','Diazepam','N05BA01','10 mg','Tabletten','20 Stk','N1','Roche',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
('a0000004-0001-0001-0001-000000000008','40010008','Diazepam-ratiopharm 5mg','Diazepam','N05BA01','5 mg','Tabletten','20 Stk','N1','ratiopharm',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
('a0000004-0001-0001-0001-000000000009','40010009','Adumbran 10mg','Oxazepam','N05BA04','10 mg','Tabletten','20 Stk','N1','Pfizer',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
('a0000004-0001-0001-0001-000000000010','40010010','Adumbran 50mg','Oxazepam','N05BA04','50 mg','Tabletten','20 Stk','N1','Pfizer',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
('a0000004-0001-0001-0001-000000000011','40010011','Oxazepam-ratiopharm 10mg','Oxazepam','N05BA04','10 mg','Tabletten','20 Stk','N1','ratiopharm',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
('a0000004-0001-0001-0001-000000000012','40010012','Lexotanil 6mg','Bromazepam','N05BA08','6 mg','Tabletten','20 Stk','N1','Roche',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
('a0000004-0001-0001-0001-000000000013','40010013','Lexotanil 6mg','Bromazepam','N05BA08','6 mg','Tabletten','50 Stk','N2','Roche',true,false,true,'Anxiolytikum',true,'SEED',NOW()),
('a0000004-0001-0001-0001-000000000014','40010014','Buspiron-neuraxpharm 5mg','Buspiron','N05BE01','5 mg','Tabletten','20 Stk','N1','neuraxpharm',false,false,true,'Anxiolytikum',true,'SEED',NOW()),
('a0000004-0001-0001-0001-000000000015','40010015','Buspiron-neuraxpharm 10mg','Buspiron','N05BE01','10 mg','Tabletten','20 Stk','N1','neuraxpharm',false,false,true,'Anxiolytikum',true,'SEED',NOW()),

-- Hypnotika
('a0000004-0002-0001-0001-000000000001','40020001','Stilnox 10mg','Zolpidem','N05CF02','10 mg','Filmtabletten','10 Stk','N1','Sanofi',true,false,true,'Hypnotikum',true,'SEED',NOW()),
('a0000004-0002-0001-0001-000000000002','40020002','Stilnox 10mg','Zolpidem','N05CF02','10 mg','Filmtabletten','20 Stk','N2','Sanofi',true,false,true,'Hypnotikum',true,'SEED',NOW()),
('a0000004-0002-0001-0001-000000000003','40020003','Zolpidem-ratiopharm 10mg','Zolpidem','N05CF02','10 mg','Filmtabletten','10 Stk','N1','ratiopharm',true,false,true,'Hypnotikum',true,'SEED',NOW()),
('a0000004-0002-0001-0001-000000000004','40020004','Ximovan 7.5mg','Zopiclon','N05CF01','7.5 mg','Filmtabletten','10 Stk','N1','Sanofi',true,false,true,'Hypnotikum',true,'SEED',NOW()),
('a0000004-0002-0001-0001-000000000005','40020005','Ximovan 7.5mg','Zopiclon','N05CF01','7.5 mg','Filmtabletten','20 Stk','N2','Sanofi',true,false,true,'Hypnotikum',true,'SEED',NOW()),
('a0000004-0002-0001-0001-000000000006','40020006','Zopiclon-ratiopharm 7.5mg','Zopiclon','N05CF01','7.5 mg','Filmtabletten','10 Stk','N1','ratiopharm',true,false,true,'Hypnotikum',true,'SEED',NOW()),
('a0000004-0002-0001-0001-000000000007','40020007','Chloralhydrat Rectiole 600mg','Chloralhydrat','N05CC01','600 mg','Rektallösung','5 Stk','N1','Pohl-Boskamp',false,false,true,'Hypnotikum',true,'SEED',NOW()),

-- ═══ ANTIKONVULSIVA ═══
('a0000005-0001-0001-0001-000000000001','50010001','Keppra 250mg','Levetiracetam','N03AX14','250 mg','Filmtabletten','30 Stk','N1','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000002','50010002','Keppra 500mg','Levetiracetam','N03AX14','500 mg','Filmtabletten','30 Stk','N1','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000003','50010003','Keppra 500mg','Levetiracetam','N03AX14','500 mg','Filmtabletten','100 Stk','N3','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000004','50010004','Keppra 1000mg','Levetiracetam','N03AX14','1000 mg','Filmtabletten','30 Stk','N1','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000005','50010005','Keppra 1000mg','Levetiracetam','N03AX14','1000 mg','Filmtabletten','100 Stk','N3','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000006','50010006','Levetiracetam HEXAL 500mg','Levetiracetam','N03AX14','500 mg','Filmtabletten','100 Stk','N3','HEXAL',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000007','50010007','Levetiracetam HEXAL 1000mg','Levetiracetam','N03AX14','1000 mg','Filmtabletten','100 Stk','N3','HEXAL',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000008','50010008','Vimpat 50mg','Lacosamid','N03AX18','50 mg','Filmtabletten','14 Stk','N1','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000009','50010009','Vimpat 100mg','Lacosamid','N03AX18','100 mg','Filmtabletten','14 Stk','N1','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000010','50010010','Vimpat 100mg','Lacosamid','N03AX18','100 mg','Filmtabletten','56 Stk','N2','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000011','50010011','Vimpat 200mg','Lacosamid','N03AX18','200 mg','Filmtabletten','56 Stk','N2','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000012','50010012','Topamax 25mg','Topiramat','N03AX11','25 mg','Filmtabletten','28 Stk','N1','Janssen',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000013','50010013','Topamax 50mg','Topiramat','N03AX11','50 mg','Filmtabletten','28 Stk','N1','Janssen',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000014','50010014','Topamax 100mg','Topiramat','N03AX11','100 mg','Filmtabletten','28 Stk','N1','Janssen',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000015','50010015','Topiramat HEXAL 100mg','Topiramat','N03AX11','100 mg','Filmtabletten','100 Stk','N3','HEXAL',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000016','50010016','Neurontin 300mg','Gabapentin','N03AX12','300 mg','Kapseln','50 Stk','N1','Pfizer',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000017','50010017','Neurontin 300mg','Gabapentin','N03AX12','300 mg','Kapseln','100 Stk','N2','Pfizer',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000018','50010018','Neurontin 600mg','Gabapentin','N03AX12','600 mg','Filmtabletten','50 Stk','N1','Pfizer',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000019','50010019','Gabapentin-ratiopharm 300mg','Gabapentin','N03AX12','300 mg','Kapseln','100 Stk','N2','ratiopharm',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000020','50010020','Lyrica 75mg','Pregabalin','N03AX16','75 mg','Kapseln','14 Stk','N1','Pfizer',true,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000021','50010021','Lyrica 75mg','Pregabalin','N03AX16','75 mg','Kapseln','56 Stk','N2','Pfizer',true,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000022','50010022','Lyrica 150mg','Pregabalin','N03AX16','150 mg','Kapseln','56 Stk','N2','Pfizer',true,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000023','50010023','Lyrica 300mg','Pregabalin','N03AX16','300 mg','Kapseln','56 Stk','N2','Pfizer',true,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000024','50010024','Pregabalin-ratiopharm 75mg','Pregabalin','N03AX16','75 mg','Kapseln','56 Stk','N2','ratiopharm',true,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000025','50010025','Pregabalin-ratiopharm 150mg','Pregabalin','N03AX16','150 mg','Kapseln','56 Stk','N2','ratiopharm',true,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000026','50010026','Trileptal 300mg','Oxcarbazepin','N03AF02','300 mg','Filmtabletten','50 Stk','N2','Novartis',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000027','50010027','Trileptal 600mg','Oxcarbazepin','N03AF02','600 mg','Filmtabletten','50 Stk','N2','Novartis',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000028','50010028','Oxcarbazepin-ratiopharm 300mg','Oxcarbazepin','N03AF02','300 mg','Filmtabletten','50 Stk','N2','ratiopharm',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000029','50010029','Phenhydan 100mg','Phenytoin','N03AB02','100 mg','Tabletten','50 Stk','N2','Desitin',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000030','50010030','Briviact 25mg','Brivaracetam','N03AX23','25 mg','Filmtabletten','14 Stk','N1','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000031','50010031','Briviact 50mg','Brivaracetam','N03AX23','50 mg','Filmtabletten','14 Stk','N1','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000032','50010032','Briviact 50mg','Brivaracetam','N03AX23','50 mg','Filmtabletten','56 Stk','N2','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000033','50010033','Briviact 100mg','Brivaracetam','N03AX23','100 mg','Filmtabletten','56 Stk','N2','UCB',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000034','50010034','Zonegran 100mg','Zonisamid','N03AX15','100 mg','Kapseln','28 Stk','N1','Eisai',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000035','50010035','Fycompa 2mg','Perampanel','N03AX22','2 mg','Filmtabletten','7 Stk','N1','Eisai',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000036','50010036','Fycompa 4mg','Perampanel','N03AX22','4 mg','Filmtabletten','28 Stk','N1','Eisai',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000037','50010037','Luminal 100mg','Phenobarbital','N03AA02','100 mg','Tabletten','50 Stk','N2','Desitin',true,false,true,'Antikonvulsivum',true,'SEED',NOW()),
('a0000005-0001-0001-0001-000000000038','50010038','Petnidan 250mg','Ethosuximid','N03AD01','250 mg','Kapseln','50 Stk','N2','Desitin',false,false,true,'Antikonvulsivum',true,'SEED',NOW()),

-- ═══ PARKINSON-MEDIKAMENTE ═══
('a0000006-0001-0001-0001-000000000001','60010001','Madopar 125 (100/25)','Levodopa/Benserazid','N04BA02','100/25 mg','Kapseln','30 Stk','N1','Roche',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000002','60010002','Madopar 125 (100/25)','Levodopa/Benserazid','N04BA02','100/25 mg','Kapseln','100 Stk','N3','Roche',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000003','60010003','Madopar 250 (200/50)','Levodopa/Benserazid','N04BA02','200/50 mg','Tabletten','30 Stk','N1','Roche',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000004','60010004','Madopar 250 (200/50)','Levodopa/Benserazid','N04BA02','200/50 mg','Tabletten','100 Stk','N3','Roche',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000005','60010005','Nacom 100/25','Levodopa/Carbidopa','N04BA02','100/25 mg','Tabletten','30 Stk','N1','MSD',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000006','60010006','Nacom 100/25','Levodopa/Carbidopa','N04BA02','100/25 mg','Tabletten','100 Stk','N3','MSD',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000007','60010007','Nacom 200/50','Levodopa/Carbidopa','N04BA02','200/50 mg','Tabletten','100 Stk','N3','MSD',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000008','60010008','Sifrol 0.18mg','Pramipexol','N04BC05','0.18 mg','Tabletten','30 Stk','N1','Boehringer',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000009','60010009','Sifrol 0.18mg','Pramipexol','N04BC05','0.18 mg','Tabletten','100 Stk','N3','Boehringer',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000010','60010010','Sifrol 0.7mg','Pramipexol','N04BC05','0.7 mg','Tabletten','30 Stk','N1','Boehringer',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000011','60010011','Pramipexol-ratiopharm 0.18mg','Pramipexol','N04BC05','0.18 mg','Tabletten','30 Stk','N1','ratiopharm',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000012','60010012','Requip 2mg','Ropinirol','N04BC04','2 mg','Filmtabletten','28 Stk','N1','GlaxoSmithKline',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000013','60010013','Requip-Modutab 4mg','Ropinirol','N04BC04','4 mg','Retardtabletten','28 Stk','N1','GlaxoSmithKline',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000014','60010014','Requip-Modutab 8mg','Ropinirol','N04BC04','8 mg','Retardtabletten','28 Stk','N1','GlaxoSmithKline',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000015','60010015','Neupro 2mg/24h','Rotigotin','N04BC09','2 mg/24h','Transd. Pflaster','28 Stk','N1','UCB',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000016','60010016','Neupro 4mg/24h','Rotigotin','N04BC09','4 mg/24h','Transd. Pflaster','28 Stk','N1','UCB',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000017','60010017','Neupro 6mg/24h','Rotigotin','N04BC09','6 mg/24h','Transd. Pflaster','28 Stk','N1','UCB',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000018','60010018','Neupro 8mg/24h','Rotigotin','N04BC09','8 mg/24h','Transd. Pflaster','28 Stk','N1','UCB',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000019','60010019','Azilect 1mg','Rasagilin','N04BD02','1 mg','Tabletten','28 Stk','N1','Teva',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000020','60010020','Rasagilin-ratiopharm 1mg','Rasagilin','N04BD02','1 mg','Tabletten','28 Stk','N1','ratiopharm',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000021','60010021','Selegilin-ratiopharm 5mg','Selegilin','N04BD01','5 mg','Tabletten','30 Stk','N1','ratiopharm',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000022','60010022','Xadago 50mg','Safinamid','N04BD03','50 mg','Filmtabletten','28 Stk','N1','Zambon',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000023','60010023','Xadago 100mg','Safinamid','N04BD03','100 mg','Filmtabletten','28 Stk','N1','Zambon',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000024','60010024','Comtess 200mg','Entacapon','N04BX02','200 mg','Filmtabletten','30 Stk','N1','Orion',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000025','60010025','Comtess 200mg','Entacapon','N04BX02','200 mg','Filmtabletten','100 Stk','N3','Orion',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000026','60010026','Ongentys 50mg','Opicapon','N04BX04','50 mg','Kapseln','30 Stk','N1','Bial',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000027','60010027','PK-Merz 100mg','Amantadin','N04BB01','100 mg','Filmtabletten','30 Stk','N1','Merz',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000028','60010028','PK-Merz 100mg','Amantadin','N04BB01','100 mg','Filmtabletten','100 Stk','N3','Merz',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),
('a0000006-0001-0001-0001-000000000029','60010029','Amantadin-neuraxpharm 200mg','Amantadin','N04BB01','200 mg','Retardtabletten','30 Stk','N1','neuraxpharm',false,false,true,'Parkinsonmittel',true,'SEED',NOW()),

-- ═══ MS-THERAPEUTIKA ═══
('a0000007-0001-0001-0001-000000000001','70010001','Tecfidera 120mg','Dimethylfumarat','L04AX07','120 mg','Kapseln','14 Stk','N1','Biogen',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
('a0000007-0001-0001-0001-000000000002','70010002','Tecfidera 240mg','Dimethylfumarat','L04AX07','240 mg','Kapseln','56 Stk','N2','Biogen',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
('a0000007-0001-0001-0001-000000000003','70010003','Tecfidera 240mg','Dimethylfumarat','L04AX07','240 mg','Kapseln','168 Stk','N3','Biogen',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
('a0000007-0001-0001-0001-000000000004','70010004','Gilenya 0.5mg','Fingolimod','L04AA27','0.5 mg','Kapseln','28 Stk','N1','Novartis',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
('a0000007-0001-0001-0001-000000000005','70010005','Gilenya 0.5mg','Fingolimod','L04AA27','0.5 mg','Kapseln','98 Stk','N3','Novartis',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
('a0000007-0001-0001-0001-000000000006','70010006','Aubagio 14mg','Teriflunomid','L04AA31','14 mg','Filmtabletten','28 Stk','N1','Sanofi',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
('a0000007-0001-0001-0001-000000000007','70010007','Aubagio 14mg','Teriflunomid','L04AA31','14 mg','Filmtabletten','84 Stk','N3','Sanofi',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
('a0000007-0001-0001-0001-000000000008','70010008','Ocrevus 300mg','Ocrelizumab','L04AA36','300 mg','Inf.-Lösung','1 Stk','N1','Roche',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
('a0000007-0001-0001-0001-000000000009','70010009','Tysabri 300mg','Natalizumab','L04AA23','300 mg','Inf.-Lösung','1 Stk','N1','Biogen',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
('a0000007-0001-0001-0001-000000000010','70010010','Lemtrada 12mg','Alemtuzumab','L04AA34','12 mg','Inf.-Lösung','1 Stk','N1','Sanofi',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
('a0000007-0001-0001-0001-000000000011','70010011','Copaxone 20mg','Glatirameracetat','L03AX13','20 mg','Inj.-Lösung','28 Stk','N1','Teva',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
('a0000007-0001-0001-0001-000000000012','70010012','Copaxone 40mg','Glatirameracetat','L03AX13','40 mg','Inj.-Lösung','12 Stk','N1','Teva',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
('a0000007-0001-0001-0001-000000000013','70010013','Mavenclad 10mg','Cladribin','L04AA40','10 mg','Tabletten','1 Stk','N1','Merck',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
('a0000007-0001-0001-0001-000000000014','70010014','Kesimpta 20mg','Ofatumumab','L04AA52','20 mg','Inj.-Lösung','1 Stk','N1','Novartis',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
('a0000007-0001-0001-0001-000000000015','70010015','Zeposia 0.92mg','Ozanimod','L04AA38','0.92 mg','Kapseln','28 Stk','N1','Bristol-MS',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
('a0000007-0001-0001-0001-000000000016','70010016','Mayzent 2mg','Siponimod','L04AA42','2 mg','Filmtabletten','28 Stk','N1','Novartis',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
('a0000007-0001-0001-0001-000000000017','70010017','Ponvory 20mg','Ponesimod','L04AA50','20 mg','Filmtabletten','28 Stk','N1','Janssen',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
('a0000007-0001-0001-0001-000000000018','70010018','Vumerity 231mg','Diroximelfumarat','L04AX09','231 mg','Kapseln','56 Stk','N2','Biogen',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),
('a0000007-0001-0001-0001-000000000019','70010019','Betaferon 250mcg','Interferon beta-1b','L03AB08','250 mcg','Inj.-Lösung','15 Stk','N1','Bayer',false,false,true,'MsTherapeutikum',true,'SEED',NOW()),

-- ═══ SCHMERZ / MIGRÄNE ═══
('a0000008-0001-0001-0001-000000000001','80010001','Imigran 50mg','Sumatriptan','N02CC01','50 mg','Filmtabletten','2 Stk','N1','GlaxoSmithKline',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000002','80010002','Imigran 50mg','Sumatriptan','N02CC01','50 mg','Filmtabletten','6 Stk','N2','GlaxoSmithKline',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000003','80010003','Imigran 100mg','Sumatriptan','N02CC01','100 mg','Filmtabletten','2 Stk','N1','GlaxoSmithKline',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000004','80010004','Imigran 100mg','Sumatriptan','N02CC01','100 mg','Filmtabletten','6 Stk','N2','GlaxoSmithKline',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000005','80010005','Sumatriptan-1A Pharma 50mg','Sumatriptan','N02CC01','50 mg','Filmtabletten','6 Stk','N2','1A Pharma',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000006','80010006','Sumatriptan-1A Pharma 100mg','Sumatriptan','N02CC01','100 mg','Filmtabletten','6 Stk','N2','1A Pharma',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000007','80010007','Maxalt 10mg','Rizatriptan','N02CC04','10 mg','Schmelztabletten','3 Stk','N1','MSD',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000008','80010008','Maxalt 10mg','Rizatriptan','N02CC04','10 mg','Schmelztabletten','6 Stk','N2','MSD',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000009','80010009','Rizatriptan HEXAL 10mg','Rizatriptan','N02CC04','10 mg','Schmelztabletten','6 Stk','N2','HEXAL',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000010','80010010','AscoTop 2.5mg','Zolmitriptan','N02CC03','2.5 mg','Schmelztabletten','3 Stk','N1','AstraZeneca',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000011','80010011','AscoTop 2.5mg','Zolmitriptan','N02CC03','2.5 mg','Schmelztabletten','6 Stk','N2','AstraZeneca',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000012','80010012','Dociton 40mg','Propranolol','C07AA05','40 mg','Filmtabletten','50 Stk','N2','MEDA',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000013','80010013','Dociton 80mg','Propranolol','C07AA05','80 mg','Filmtabletten','50 Stk','N2','MEDA',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000014','80010014','Propranolol-ratiopharm 40mg','Propranolol','C07AA05','40 mg','Filmtabletten','50 Stk','N2','ratiopharm',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000015','80010015','Flunarizin-ratiopharm 5mg','Flunarizin','N07CA03','5 mg','Kapseln','20 Stk','N1','ratiopharm',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000016','80010016','Flunarizin-ratiopharm 5mg','Flunarizin','N07CA03','5 mg','Kapseln','50 Stk','N2','ratiopharm',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000017','80010017','Aimovig 70mg','Erenumab','N02CD01','70 mg','Inj.-Lösung','1 Stk','N1','Novartis',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000018','80010018','Aimovig 140mg','Erenumab','N02CD01','140 mg','Inj.-Lösung','1 Stk','N1','Novartis',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000019','80010019','Ajovy 225mg','Fremanezumab','N02CD03','225 mg','Inj.-Lösung','1 Stk','N1','Teva',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000020','80010020','Ajovy 225mg','Fremanezumab','N02CD03','225 mg','Inj.-Lösung','3 Stk','N2','Teva',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000021','80010021','Emgality 120mg','Galcanezumab','N02CD02','120 mg','Inj.-Lösung','1 Stk','N1','Lilly',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000022','80010022','Emgality 120mg','Galcanezumab','N02CD02','120 mg','Inj.-Lösung','3 Stk','N2','Lilly',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),
('a0000008-0001-0001-0001-000000000023','80010023','Vydura 75mg','Rimegepant','N02CD06','75 mg','Schmelztabletten','8 Stk','N1','Pfizer',false,false,true,'Migraenetherapeutikum',true,'SEED',NOW()),

-- ═══ STIMULANZIEN ═══
('a0000009-0001-0001-0001-000000000001','90010001','Medikinet retard 10mg','Methylphenidat','N06BA04','10 mg','Retardkapseln','30 Stk','N1','Medice',true,false,true,'Stimulans',true,'SEED',NOW()),
('a0000009-0001-0001-0001-000000000002','90010002','Medikinet retard 20mg','Methylphenidat','N06BA04','20 mg','Retardkapseln','30 Stk','N1','Medice',true,false,true,'Stimulans',true,'SEED',NOW()),
('a0000009-0001-0001-0001-000000000003','90010003','Concerta 36mg','Methylphenidat','N06BA04','36 mg','Retardtabletten','30 Stk','N1','Janssen',true,false,true,'Stimulans',true,'SEED',NOW()),
('a0000009-0001-0001-0001-000000000004','90010004','Concerta 54mg','Methylphenidat','N06BA04','54 mg','Retardtabletten','30 Stk','N1','Janssen',true,false,true,'Stimulans',true,'SEED',NOW()),
('a0000009-0001-0001-0001-000000000005','90010005','Ritalin adult 10mg','Methylphenidat','N06BA04','10 mg','Tabletten','30 Stk','N1','Novartis',true,false,true,'Stimulans',true,'SEED',NOW()),
('a0000009-0001-0001-0001-000000000006','90010006','Ritalin adult 20mg','Methylphenidat','N06BA04','20 mg','Retardkapseln','30 Stk','N1','Novartis',true,false,true,'Stimulans',true,'SEED',NOW()),
('a0000009-0001-0001-0001-000000000007','90010007','Methylphenidat HEXAL 10mg','Methylphenidat','N06BA04','10 mg','Tabletten','50 Stk','N2','HEXAL',true,false,true,'Stimulans',true,'SEED',NOW()),
('a0000009-0001-0001-0001-000000000008','90010008','Elvanse 30mg','Lisdexamfetamin','N06BA12','30 mg','Kapseln','30 Stk','N1','Takeda',true,false,true,'Stimulans',true,'SEED',NOW()),
('a0000009-0001-0001-0001-000000000009','90010009','Elvanse 50mg','Lisdexamfetamin','N06BA12','50 mg','Kapseln','30 Stk','N1','Takeda',true,false,true,'Stimulans',true,'SEED',NOW()),
('a0000009-0001-0001-0001-000000000010','90010010','Elvanse 70mg','Lisdexamfetamin','N06BA12','70 mg','Kapseln','30 Stk','N1','Takeda',true,false,true,'Stimulans',true,'SEED',NOW()),
('a0000009-0001-0001-0001-000000000011','90010011','Strattera 10mg','Atomoxetin','N06BA09','10 mg','Kapseln','7 Stk','N1','Lilly',false,false,true,'Stimulans',true,'SEED',NOW()),
('a0000009-0001-0001-0001-000000000012','90010012','Strattera 25mg','Atomoxetin','N06BA09','25 mg','Kapseln','28 Stk','N1','Lilly',false,false,true,'Stimulans',true,'SEED',NOW()),
('a0000009-0001-0001-0001-000000000013','90010013','Strattera 40mg','Atomoxetin','N06BA09','40 mg','Kapseln','28 Stk','N1','Lilly',false,false,true,'Stimulans',true,'SEED',NOW()),
('a0000009-0001-0001-0001-000000000014','90010014','Strattera 60mg','Atomoxetin','N06BA09','60 mg','Kapseln','28 Stk','N1','Lilly',false,false,true,'Stimulans',true,'SEED',NOW()),

-- ═══ SONSTIGE ═══
('a0000010-0001-0001-0001-000000000001','99010001','Circadin 2mg','Melatonin','N05CH01','2 mg','Retardtabletten','21 Stk','N1','Neurim',false,false,true,'Sonstiges',true,'SEED',NOW()),
('a0000010-0001-0001-0001-000000000002','99010002','Akineton 2mg','Biperiden','N04AA02','2 mg','Tabletten','30 Stk','N1','Desma',false,false,true,'Sonstiges',true,'SEED',NOW()),
('a0000010-0001-0001-0001-000000000003','99010003','Akineton 4mg','Biperiden','N04AA02','4 mg','Retardtabletten','30 Stk','N1','Desma',false,false,true,'Sonstiges',true,'SEED',NOW()),
('a0000010-0001-0001-0001-000000000004','99010004','Artane 2mg','Trihexyphenidyl','N04AA01','2 mg','Tabletten','30 Stk','N1','Teofarma',false,false,true,'Sonstiges',true,'SEED',NOW()),
('a0000010-0001-0001-0001-000000000005','99010005','Artane 5mg','Trihexyphenidyl','N04AA01','5 mg','Tabletten','30 Stk','N1','Teofarma',false,false,true,'Sonstiges',true,'SEED',NOW()),
('a0000010-0001-0001-0001-000000000006','99010006','Adepend 50mg','Naltrexon','N07BB04','50 mg','Filmtabletten','28 Stk','N1','Desitin',false,false,true,'Sonstiges',true,'SEED',NOW()),
('a0000010-0001-0001-0001-000000000007','99010007','Campral 333mg','Acamprosat','N07BB03','333 mg','Filmtabletten','84 Stk','N1','Merck',false,false,true,'Sonstiges',true,'SEED',NOW()),
('a0000010-0001-0001-0001-000000000008','99010008','Antabus 200mg','Disulfiram','N07BB01','200 mg','Tabletten','50 Stk','N2','Actavis',false,false,true,'Sonstiges',true,'SEED',NOW()),

-- ═══ SUBSTITUTIONSMITTEL (BtM) ═══
('a0000010-0002-0001-0001-000000000001','99020001','L-Polamidon 5mg/ml','Levomethadon','N07BC05','5 mg/ml','Lösung','100 ml','N2','Sanofi',true,false,true,'Substitutionsmittel',true,'SEED',NOW()),
('a0000010-0002-0001-0001-000000000002','99020002','Methaddict 5mg','Methadon','N07BC02','5 mg','Tabletten','50 Stk','N2','Hexal Medical',true,false,true,'Substitutionsmittel',true,'SEED',NOW()),
('a0000010-0002-0001-0001-000000000003','99020003','Methaddict 10mg','Methadon','N07BC02','10 mg','Tabletten','50 Stk','N2','Hexal Medical',true,false,true,'Substitutionsmittel',true,'SEED',NOW()),
('a0000010-0002-0001-0001-000000000004','99020004','Subutex 2mg','Buprenorphin','N07BC01','2 mg','Sublingualtabletten','7 Stk','N1','Indivior',true,false,true,'Substitutionsmittel',true,'SEED',NOW()),
('a0000010-0002-0001-0001-000000000005','99020005','Subutex 8mg','Buprenorphin','N07BC01','8 mg','Sublingualtabletten','7 Stk','N1','Indivior',true,false,true,'Substitutionsmittel',true,'SEED',NOW()),
('a0000010-0002-0001-0001-000000000006','99020006','Suboxone 8/2mg','Buprenorphin/Naloxon','N07BC51','8/2 mg','Sublingualtabletten','7 Stk','N1','Indivior',true,false,true,'Substitutionsmittel',true,'SEED',NOW())

ON CONFLICT ("Pzn") DO NOTHING;
""";
}
