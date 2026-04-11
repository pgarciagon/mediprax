# Cross-Cutting Concerns

#### Navigation Updates

Add new navigation items to `MainLayout.razor`:

```
Startseite
Patienten
Termine
  Wochenansicht
  Wartezimmer
  Warteliste (M25)
Dokumentation
  Uebersicht
  Psychopath. Befund (M17)
  Neurologischer Befund (M20)
  Suizidalitaet (M27)
  Krankheitsspezifisch (M26)
    Anfallskalender
    Kopfschmerz-Tagebuch
    MS-Dokumentation
    Parkinson
Testung (M18)
Therapie (M19)
  Therapiefaelle
  PTV-Formulare
  Kontingente
Medikation
  Medikationsplan
  Depot-Uebersicht (M21)
  Interaktionen (M21)
  BtM-Verwaltung (M35)
Labor (M22)
Geraete (M28)
Formulare
  Rezept
  AU
  Ueberweisung
  Krankenhauseinweisung (M24)
  Heilmittelverordnung (M24)
  Soziotherapie (M24)
  PsychKG-Zeugnis (M32)
Dokumente
Abrechnung
  Uebersicht
  Quartalsvalidierung (M23)
  Privatrechnungen (M33)
DMP (M31)
Berichte
Telematik
Verwaltung
  Benutzer
  Sprechzeiten (M40)
  Textbausteine (M30)
  Geraete-Konfiguration (M28)
  DATEV-Export (M37)
  Audit-Log
  Datenimport
Konto
```

#### Database Migrations

Each milestone generates its own EF Core migration:

```bash
dotnet ef migrations add AddPsychopathologicalFinding    # M17
dotnet ef migrations add AddPsychometricTests             # M18
dotnet ef migrations add AddTherapyCaseManagement         # M19
dotnet ef migrations add AddNeurologicalExamination       # M20
dotnet ef migrations add EnhanceMedicationManagement      # M21
dotnet ef migrations add AddLabResults                    # M22
dotnet ef migrations add AddAppointmentEnhancements       # M25
dotnet ef migrations add AddDiseaseDocumentation          # M26
dotnet ef migrations add AddSuicidalityAssessment         # M27
dotnet ef migrations add AddTextModules                   # M30
dotnet ef migrations add AddDmpDepression                 # M31
dotnet ef migrations add AddPatientGuardianFields         # M32
dotnet ef migrations add AddPrivateInvoice                # M33
dotnet ef migrations add AddBtmPrescription               # M35
dotnet ef migrations add AddDoctorScheduleAndAbsence      # M40
```

#### Test Coverage Requirements

Each milestone must include:
- **Unit tests:** Catalog searches, business rules (contingents, plausibility, interactions)
- **Integration tests:** Service CRUD operations with test database
- Maintain existing test patterns (xUnit, TestDbContextFactory, IDisposable)

#### Arztbrief Integration

The Arztbrief generator (`ArztbriefDocument.cs`) should be extended to optionally include:
- Psychopathological finding narrative (M17)
- Psychometric test results summary (M18)
- Neurological examination narrative (M20)
- Active medication plan with BMP format (M21)
- Lab results summary (M22)
- Therapy progress report (M39)

---

### Dependency Graph

```
M17 (AMDP) ---------------------------------------- standalone
M18 (Psychometric Tests) -------------------------- standalone
M19 (PTV / Therapy Cases) ------------------------- standalone
M20 (Neuro Exam) ---------------------------------- standalone

M21 (Advanced Medication) ------------------------- standalone (enhances existing)
M22 (Lab Integration) --------- depends on -------- M21 (monitoring alerts reference medication)
M23 (Billing) ----------------- depends on -------- M19 (therapy contingent billing linkage)

M24 (KBV Forms) ----------------------------------- standalone
M25 (Appointments) ------------ depends on -------- M19 (session tracking links to TherapyCase)
M26 (Disease Docs) -------------------------------- standalone
M27 (Suicidality) ------------- depends on -------- M17 (references AMDP suicidality section)

M28 (GDT) ----------------------------------------- standalone
M29 (Video) ------------------- depends on -------- M25 (appointment enhancements)
M30 (Text Modules) ------------ depends on -------- M17, M20 (default templates use findings)

M31 (DMP) -------------------- depends on --------- M18 (PHQ-9 integration)
M32 (PsychKG) ---------------- depends on --------- M17 (uses AMDP findings)
M33 (GOAe Invoicing) -------------------------------- standalone
M34 (eAU) ------------------------------------------ standalone (enhances existing)
M35 (BtM) -------------------- depends on --------- M21 (medication category system)
M36 (Mahnwesen) -------------- depends on --------- M33 (PrivateInvoice entity)
M37 (DATEV) ------------------- depends on -------- M33 (invoice data for export)
M38 (Umsatzstatistik) -------- depends on --------- M10, M33 (billing + invoice data)
M39 (Therapiebericht) -------- depends on --------- M19 (TherapyCase data)
M40 (Sprechzeiten) --------------------------------- standalone
M41 (Terminvergabe) ---------- depends on ---------- M40 (availability data)
M42 (Diagnosevorschlaege) ---- depends on --------- M17, M20, M27 (structured findings)
```

### Recommended Implementation Order

```
Phase 1 (P0 -- Parallel):     M17 + M18 + M19 + M20
Phase 2 (P1 -- Sequential):   M21 -> M22 -> M23, M24 (parallel with M23), M40 -> M41, M42
Phase 3 (P2 -- Mixed):        M25 -> M26 + M27 (parallel) -> M28, M39
Phase 4 (P3 -- As needed):    M30, M29, M31, M32, M33 -> M36 -> M37, M34, M35, M38
```

