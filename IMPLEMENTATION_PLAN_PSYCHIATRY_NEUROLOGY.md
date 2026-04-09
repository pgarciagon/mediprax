# Implementation Plan: Psychiatry & Neurology Features

> Comprehensive plan to bring mediprax to competitive parity with leading German PVS
> (ELEFANT, psyprax, EPIKUR, SMARTY, tomedo) for psychiatric and neurological practices.
>
> Generated: 2026-04-08

---

## Table of Contents

1. [Overview & Prioritization](#1-overview--prioritization)
2. [Milestone 17: Structured Psychopathological Assessment (AMDP)](#milestone-17-structured-psychopathological-assessment-amdp)
3. [Milestone 18: Psychometric Testing & Assessment Tools](#milestone-18-psychometric-testing--assessment-tools)
4. [Milestone 19: PTV Forms & Therapy Approval Management](#milestone-19-ptv-forms--therapy-approval-management)
5. [Milestone 20: Structured Neurological Examination](#milestone-20-structured-neurological-examination)
6. [Milestone 21: Advanced Medication Management (Psychiatry)](#milestone-21-advanced-medication-management-psychiatry)
7. [Milestone 22: Laboratory Integration & Monitoring](#milestone-22-laboratory-integration--monitoring)
8. [Milestone 23: Billing Enhancements (Plausibility, GOÄ Invoicing, GOP Engine)](#milestone-23-billing-enhancements)
9. [Milestone 24: Missing KBV Forms (Muster 2, 7, 13, 14, 26)](#milestone-24-missing-kbv-forms)
10. [Milestone 25: Appointment Enhancements (Recurring, Waitlist, Session Tracking)](#milestone-25-appointment-enhancements)
11. [Milestone 26: Disease-Specific Documentation Modules](#milestone-26-disease-specific-documentation-modules)
12. [Milestone 27: Suicidality Assessment & Safety Planning](#milestone-27-suicidality-assessment--safety-planning)
13. [Milestone 28: GDT Device Interface (EEG, EMG, Evoked Potentials)](#milestone-28-gdt-device-interface)
14. [Milestone 29: Videosprechstunde Integration](#milestone-29-videosprechstunde-integration)
15. [Milestone 30: Text Modules & Documentation Automation](#milestone-30-text-modules--documentation-automation)
16. [Milestone 31: DMP Depression](#milestone-31-dmp-depression)
17. [Milestone 32: Involuntary Commitment & Legal Documentation](#milestone-32-involuntary-commitment--legal-documentation)
18. [Milestone 33: Private Billing (GOÄ Invoicing & Mahnwesen)](#milestone-33-private-billing-goä-invoicing)
19. [Milestone 34: eAU Electronic Transmission](#milestone-34-eau-electronic-transmission)
20. [Milestone 35: BtM Management & E-BtM-Rezept](#milestone-35-btm-management)
21. [Cross-Cutting Concerns](#cross-cutting-concerns)
22. [Dependency Graph](#dependency-graph)
23. [Timeline Estimate](#timeline-estimate)

---

## 1. Overview & Prioritization

### Priority Matrix

| Priority | Milestones | Rationale |
|----------|-----------|-----------|
| **P0 — Must Have** | 17, 18, 19, 20 | Core psychiatric/neurological documentation. Cannot compete without these. |
| **P1 — High** | 21, 22, 23, 24 | Patient safety (medication monitoring), revenue protection (billing), daily-use forms. |
| **P2 — Medium** | 25, 26, 27, 28 | Major workflow improvements, specialty documentation, device integration. |
| **P3 — Nice to Have** | 29, 30, 31, 32, 33, 34, 35 | Competitive differentiators, niche requirements. |

### Architecture Principles (Carried Forward)

All new features follow the established mediprax patterns:

- **Layers:** Core (Entity) → Application (Service + DTOs + Interface) → Infrastructure (EF Config) → Server (Blazor Page + DI Registration)
- **Entities:** Inherit `BaseEntity`, soft-delete via global query filters
- **Services:** Inject `DbContext`, use `DbSet<T>` properties, return DTOs, async with `CancellationToken`
- **DTOs:** `XxxDto`, `CreateXxxDto`, `UpdateXxxDto`, `XxxListItemDto`
- **Pages:** `@rendermode InteractiveServer`, `@attribute [Authorize(Policy = "...")]`, loading/error pattern
- **German UI, English code**
- **Audit trail** on all significant operations

---

## Milestone 17: Structured Psychopathological Assessment (AMDP)

> **Priority:** P0 | **Estimated Effort:** Large
> **Why:** The AMDP-based psychopathological finding is the #1 most-used documentation tool in German psychiatry. Every competitor has this.

### 17.1 Data Model

#### New Entity: `PsychopathologicalFinding`

```
MediPrax.Core/Entities/PsychopathologicalFinding.cs
```

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK (BaseEntity) |
| `EncounterId` | `Guid` | FK → Encounter |
| `PatientId` | `Guid` | FK → Patient |
| `AssessedById` | `Guid` | FK → User (doctor) |
| `AssessmentDate` | `DateOnly` | Date of assessment |
| `Findings` | `List<SymptomFinding>` | JSONB — structured findings |
| `NarrativeText` | `string?` | Auto-generated or manual free text |
| `Notes` | `string?` | Additional clinical notes |

#### Value Object: `SymptomFinding` (stored as JSONB)

| Property | Type | Description |
|----------|------|-------------|
| `CategoryCode` | `string` | AMDP category (e.g., "BEW" for consciousness) |
| `SymptomCode` | `string` | AMDP symptom code (e.g., "BEW-1") |
| `SymptomName` | `string` | German name (e.g., "Bewusstseinslage") |
| `Severity` | `int` | 0 = not present, 1 = mild, 2 = moderate, 3 = severe |
| `Comment` | `string?` | Free text for this symptom |

### 17.2 AMDP Symptom Catalog

Create a static catalog class (like the existing `Icd10Catalog` and `GopCatalog`):

```
MediPrax.Application/Catalogs/AmdpCatalog.cs
```

**Categories to implement** (per AMDP 9th edition):

| Code | Category (German) | # Symptoms |
|------|-------------------|------------|
| BEW | Bewusstseinsstörungen | 5 |
| ORI | Orientierungsstörungen | 4 |
| AUF | Aufmerksamkeits- und Gedächtnisstörungen | 6 |
| FDS | Formale Denkstörungen | 14 |
| BEF | Befürchtungen und Zwänge | 5 |
| WAH | Wahn | 10 |
| SIN | Sinnestäuschungen | 8 |
| ICH | Ich-Störungen | 4 |
| AFF | Störungen der Affektivität | 14 |
| ANT | Antriebs- und psychomotorische Störungen | 10 |
| CIR | Circadiane Besonderheiten | 2 |
| AND | Andere Störungen | 4 |
| SUI | Suizidalität | 4 |
| SOM | Somatische Befunde | ~40 |

**Total: ~130 items** — hardcoded as `IReadOnlyList<AmdpSymptom>` with `Search(string term)` method.

### 17.3 Narrative Text Generator ("Befundomat")

```
MediPrax.Application/Services/PsychopathFindingTextGenerator.cs
```

Takes a `List<SymptomFinding>` and generates grammatically correct German narrative text.

**Logic:**
- Group findings by category
- For severity 0: skip or mention "unauffällig" for the category
- For severity 1–3: generate sentence with severity qualifier
- Example output: *"Bewusstsein klar, allseits orientiert. Formales Denken: leichte Verlangsamung, keine Denkstörungen. Stimmung deutlich gedrückt, Affekt eingeengt. Antrieb vermindert. Keine Suizidalität."*

Template-based approach with per-symptom sentence fragments stored in the catalog.

### 17.4 Service Layer

```
MediPrax.Application/Interfaces/IPsychopathFindingService.cs
MediPrax.Application/Services/PsychopathFindingService.cs
```

| Method | Description |
|--------|-------------|
| `GetByEncounterAsync(Guid encounterId)` | Get finding for encounter |
| `GetByPatientAsync(Guid patientId)` | Get all findings for longitudinal view |
| `CreateAsync(CreatePsychopathFindingDto)` | Create new assessment |
| `UpdateAsync(UpdatePsychopathFindingDto)` | Update existing assessment |
| `GenerateNarrativeAsync(List<SymptomFinding>)` | Generate text from structured input |
| `CompareAsync(Guid findingId1, Guid findingId2)` | Compare two findings side-by-side |

### 17.5 Blazor UI

```
MediPrax.Server/Components/Pages/Dokumentation/PsychopathBefund.razor
```

**Layout:**
1. **Category accordion** — Each AMDP category is a collapsible section
2. **Symptom rows** — Each symptom shows: Name | Severity selector (0/1/2/3 radio buttons) | Comment field
3. **Live narrative preview** — Right panel or bottom section showing auto-generated text updating in real-time
4. **Manual override** — Allow editing the generated narrative text directly
5. **Longitudinal view** — Side-by-side comparison with previous finding (color-coded changes: improved=green, worsened=red, unchanged=grey)
6. **Quick presets** — "Unauffällig" button sets all to 0; "Depression typisch" preset; "Psychose typisch" preset

**Integration:**
- Accessible from `EncounterFormular.razor` via "Psychopathologischer Befund" button
- Finding is linked to the Encounter
- Auto-saved on each change (debounced)
- PDF inclusion in Arztbrief generator

### 17.6 Database Migration

- Table: `psychopathological_findings`
- JSONB column for `Findings`
- Indexes: `EncounterId` (unique), `PatientId + AssessmentDate`

### 17.7 Tests

- `PsychopathFindingServiceTests` — CRUD operations
- `AmdpCatalogTests` — Catalog search, completeness
- `PsychopathFindingTextGeneratorTests` — Narrative generation for various symptom combinations

### 17.8 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/PsychopathologicalFinding.cs` |
| Create | `MediPrax.Core/ValueObjects/SymptomFinding.cs` |
| Create | `MediPrax.Application/Catalogs/AmdpCatalog.cs` |
| Create | `MediPrax.Application/DTOs/PsychopathFindingDto.cs` |
| Create | `MediPrax.Application/Interfaces/IPsychopathFindingService.cs` |
| Create | `MediPrax.Application/Services/PsychopathFindingService.cs` |
| Create | `MediPrax.Application/Services/PsychopathFindingTextGenerator.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/PsychopathologicalFindingConfiguration.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/MediPraxDbContext.cs` — Add DbSet + query filter |
| Create | `MediPrax.Server/Components/Pages/Dokumentation/PsychopathBefund.razor` |
| Modify | `MediPrax.Server/Components/Pages/Dokumentation/EncounterFormular.razor` — Add link |
| Modify | `MediPrax.Server/Program.cs` — Register service |
| Modify | `MediPrax.Reporting/Arztbrief/ArztbriefDocument.cs` — Include finding in PDF |
| Create | `tests/MediPrax.IntegrationTests/PsychopathFindingServiceTests.cs` |
| Create | `tests/MediPrax.UnitTests/AmdpCatalogTests.cs` |
| Create | `tests/MediPrax.UnitTests/PsychopathFindingTextGeneratorTests.cs` |

---

## Milestone 18: Psychometric Testing & Assessment Tools

> **Priority:** P0 | **Estimated Effort:** Large
> **Why:** Standardized screening instruments (PHQ-9, GAD-7, etc.) are expected by every psychiatrist. They enable outcome measurement, therapy evaluation, and billing (GOP 35600).

### 18.1 Data Model

#### Entity: `PsychometricTest`

```
MediPrax.Core/Entities/PsychometricTest.cs
```

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `PatientId` | `Guid` | FK → Patient |
| `EncounterId` | `Guid?` | FK → Encounter (optional) |
| `AdministeredById` | `Guid` | FK → User |
| `TestType` | `PsychometricTestType` | Enum: PHQ9, GAD7, BDI2, etc. |
| `TestDate` | `DateOnly` | Date administered |
| `Responses` | `List<TestResponse>` | JSONB — individual item responses |
| `TotalScore` | `int` | Calculated total score |
| `Interpretation` | `string` | Auto-generated interpretation text |
| `Notes` | `string?` | Clinician notes |
| `Status` | `TestStatus` | Enum: InProgress, Completed |

#### Enum: `PsychometricTestType`

```csharp
public enum PsychometricTestType
{
    PHQ9,       // Patient Health Questionnaire (Depression)
    GAD7,       // Generalized Anxiety Disorder
    BDI2,       // Beck Depression Inventory II
    HAMD,       // Hamilton Depression Scale
    PANSS,      // Positive and Negative Syndrome Scale
    BPRS,       // Brief Psychiatric Rating Scale
    CGI,        // Clinical Global Impression
    WHO5,       // WHO-5 Wellbeing Index
    AUDIT,      // Alcohol Use Disorders Identification Test
    PCL5,       // PTSD Checklist
    PSQI,       // Pittsburgh Sleep Quality Index
    MMSE,       // Mini Mental State (also neuro)
    MoCA,       // Montreal Cognitive Assessment (also neuro)
    ClockDrawing // Clock Drawing Test (also neuro)
}
```

#### Value Object: `TestResponse` (JSONB)

| Property | Type | Description |
|----------|------|-------------|
| `ItemNumber` | `int` | Question number (1-based) |
| `ItemText` | `string` | Question text (German) |
| `ResponseValue` | `int` | Selected answer value |
| `ResponseText` | `string?` | Selected answer label |

### 18.2 Test Definition Catalog

```
MediPrax.Application/Catalogs/PsychometricTestCatalog.cs
```

Each test definition includes:

| Property | Description |
|----------|-------------|
| `TestType` | Enum value |
| `Name` | Full German name |
| `Abbreviation` | Short name |
| `Description` | What it measures |
| `Items` | List of questions with answer options |
| `ScoreRanges` | Interpretation thresholds |
| `MaxScore` | Maximum possible score |
| `ScoringMethod` | Sum, average, or custom |
| `Source` | Citation / reference |

**PHQ-9 Example:**

```csharp
new TestDefinition
{
    TestType = PsychometricTestType.PHQ9,
    Name = "Patient Health Questionnaire – 9",
    Items = [
        new("Wenig Interesse oder Freude an Ihren Tätigkeiten", [
            ("Überhaupt nicht", 0),
            ("An einzelnen Tagen", 1),
            ("An mehr als der Hälfte der Tage", 2),
            ("Beinahe jeden Tag", 3)
        ]),
        // ... 8 more items
    ],
    ScoreRanges = [
        (0, 4, "Minimal", "Keine depressive Störung"),
        (5, 9, "Leicht", "Leichte depressive Symptomatik"),
        (10, 14, "Mittelgradig", "Mittelgradige depressive Störung"),
        (15, 19, "Mittelschwer", "Mittelschwere depressive Störung"),
        (20, 27, "Schwer", "Schwere depressive Störung")
    ],
    MaxScore = 27,
    SuggestedGop = "35600"
}
```

**Tests to implement in order of priority:**
1. PHQ-9 (depression — most commonly used)
2. GAD-7 (anxiety)
3. WHO-5 (general wellbeing)
4. CGI (clinical global impression)
5. AUDIT (alcohol screening)
6. BDI-II (depression — more detailed)
7. MMSE (cognitive screening — neurology)
8. MoCA (cognitive assessment — neurology)
9. PANSS (schizophrenia)
10. PCL-5 (PTSD)
11. PSQI (sleep)
12. HAMD (Hamilton depression — clinician-rated)
13. BPRS (brief psychiatric rating)
14. Clock Drawing Test

### 18.3 Service Layer

```
MediPrax.Application/Interfaces/IPsychometricTestService.cs
MediPrax.Application/Services/PsychometricTestService.cs
```

| Method | Description |
|--------|-------------|
| `GetByIdAsync(Guid id)` | Get test with full responses |
| `GetByPatientAsync(Guid patientId, PsychometricTestType? type)` | All tests for patient, optionally filtered by type |
| `GetScoreHistoryAsync(Guid patientId, PsychometricTestType type)` | Score over time for charts |
| `CreateAsync(CreatePsychometricTestDto)` | Start a new test |
| `SubmitResponsesAsync(Guid testId, List<TestResponse>)` | Submit answers, calculate score |
| `DeleteAsync(Guid id)` | Soft delete |

### 18.4 Blazor UI

#### Test Administration Page

```
MediPrax.Server/Components/Pages/Testung/TestDurchfuehrung.razor
```

**Layout:**
- Step-by-step wizard: one question per screen (tablet-friendly for patient self-administration)
- Alternative: all questions on one page (for clinician administration)
- Progress bar showing completion
- Response buttons with clear German labels
- Real-time score calculation

#### Test Results / History Page

```
MediPrax.Server/Components/Pages/Testung/TestErgebnisse.razor
```

**Layout:**
- Score history chart per test type (line chart using Blazor charting library or simple SVG)
- Color-coded severity bands on chart
- Table of all tests with: Date, Type, Score, Interpretation
- Click to view full test details
- PDF export of test results

#### Integration Points

- `PatientDetail.razor` — New "Testung" section showing recent scores
- `EncounterFormular.razor` — Button "Test durchführen" linking to test
- Billing suggestion: auto-prompt to add GOP 35600 when test is completed

### 18.5 Score Visualization

Use inline SVG for score-over-time charts (no external JS dependency):

```razor
<svg width="600" height="200" class="score-chart">
    <!-- Score bands (background colors for severity zones) -->
    <rect y="0" height="30" fill="#fee" /> <!-- Severe -->
    <rect y="30" height="37" fill="#fec" /> <!-- Moderate -->
    <rect y="67" height="37" fill="#ffc" /> <!-- Mild -->
    <rect y="104" height="96" fill="#efe" /> <!-- Minimal -->

    <!-- Score line with data points -->
    @foreach (var point in scoreHistory)
    {
        <circle cx="@point.X" cy="@point.Y" r="4" fill="#333" />
    }
    <polyline points="@string.Join(" ", scoreHistory.Select(p => $"{p.X},{p.Y}"))"
              fill="none" stroke="#333" stroke-width="2" />
</svg>
```

### 18.6 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/PsychometricTest.cs` |
| Create | `MediPrax.Core/Enums/PsychometricTestType.cs` |
| Create | `MediPrax.Core/Enums/TestStatus.cs` |
| Create | `MediPrax.Core/ValueObjects/TestResponse.cs` |
| Create | `MediPrax.Application/Catalogs/PsychometricTestCatalog.cs` |
| Create | `MediPrax.Application/DTOs/PsychometricTestDto.cs` |
| Create | `MediPrax.Application/Interfaces/IPsychometricTestService.cs` |
| Create | `MediPrax.Application/Services/PsychometricTestService.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/PsychometricTestConfiguration.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/MediPraxDbContext.cs` |
| Create | `MediPrax.Server/Components/Pages/Testung/TestDurchfuehrung.razor` |
| Create | `MediPrax.Server/Components/Pages/Testung/TestErgebnisse.razor` |
| Create | `MediPrax.Server/Components/Pages/Testung/TestAuswahl.razor` |
| Modify | `MediPrax.Server/Components/Pages/Patienten/PatientDetail.razor` — Add Testung section |
| Modify | `MediPrax.Server/Components/Pages/Dokumentation/EncounterFormular.razor` — Add test link |
| Modify | `MediPrax.Server/Components/Layout/MainLayout.razor` — Add navigation item |
| Modify | `MediPrax.Server/Program.cs` |
| Create | `tests/MediPrax.IntegrationTests/PsychometricTestServiceTests.cs` |
| Create | `tests/MediPrax.UnitTests/PsychometricTestCatalogTests.cs` |

---

## Milestone 19: PTV Forms & Therapy Approval Management

> **Priority:** P0 | **Estimated Effort:** X-Large
> **Why:** Any practice doing Richtlinien-Psychotherapie (behavioral therapy, depth psychology, analytical, systemic) MUST manage PTV forms and therapy approvals. This is a dealbreaker feature.

### 19.1 Data Model

#### Entity: `TherapyCase` (Behandlungsfall Psychotherapie)

```
MediPrax.Core/Entities/TherapyCase.cs
```

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `PatientId` | `Guid` | FK → Patient |
| `TherapistId` | `Guid` | FK → User |
| `TherapyType` | `TherapyType` | Enum (see below) |
| `Status` | `TherapyCaseStatus` | Enum (see below) |
| `StartDate` | `DateOnly` | Therapy start date |
| `EndDate` | `DateOnly?` | Therapy end date |
| `InsuranceApprovalDate` | `DateOnly?` | When KK approved |
| `InsuranceApprovalRef` | `string?` | KK reference number |
| `ApprovedSessions` | `int` | Total approved session count |
| `CompletedSessions` | `int` | Sessions already held |
| `SessionDurationMinutes` | `int` | 25 or 50 minutes |
| `IsGroupTherapy` | `bool` | Einzel vs. Gruppe |
| `GutachterStatus` | `GutachterStatus?` | Expert review status |
| `Diagnoses` | `List<string>` | JSONB — ICD-10 codes |
| `Notes` | `string?` | Internal notes |

#### Enum: `TherapyType`

```csharp
public enum TherapyType
{
    PsychotherapeutischeSprechstunde,    // Max 6×25min
    Probatorik,                          // Max 4-6 sessions
    Akutbehandlung,                      // Max 24×25min or 12×50min
    KurzzeittherapieKZT1,               // 12 sessions (25 or 50 min)
    KurzzeittherapieKZT2,               // 12 more sessions
    LangzeittherapieVT,                  // Verhaltenstherapie (up to 60/80)
    LangzeittherapieTP,                  // Tiefenpsychologisch (up to 60/100)
    LangzeittherapieAP,                  // Analytisch (up to 160/300)
    LangzeittherapieST,                  // Systemisch (up to 36/48)
    NeuropsychologischeTherapie          // up to 60/120
}
```

#### Enum: `TherapyCaseStatus`

```csharp
public enum TherapyCaseStatus
{
    SprechstundePhase,
    ProbatorikPhase,
    AntragGestellt,         // Application submitted
    InGutachterverfahren,   // Waiting for expert review
    Bewilligt,              // Approved
    Abgelehnt,              // Rejected
    InBehandlung,           // Active treatment
    VerlaengerungBeantragt, // Extension requested
    Abgeschlossen,          // Completed
    Abgebrochen             // Terminated early
}
```

#### Enum: `GutachterStatus`

```csharp
public enum GutachterStatus
{
    NichtErforderlich,  // KZT doesn't need Gutachter
    Eingereicht,        // Submitted
    Bewilligt,          // Approved
    Abgelehnt,          // Rejected
    NachbesserungAngefordert // Revision requested
}
```

#### Entity: `TherapySession` (links to Appointment/Encounter)

```
MediPrax.Core/Entities/TherapySession.cs
```

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `TherapyCaseId` | `Guid` | FK → TherapyCase |
| `EncounterId` | `Guid?` | FK → Encounter |
| `AppointmentId` | `Guid?` | FK → Appointment |
| `SessionNumber` | `int` | Sequential number within therapy |
| `SessionDate` | `DateOnly` | Date of session |
| `DurationMinutes` | `int` | 25 or 50 |
| `SessionType` | `SessionType` | Enum: Sprechstunde, Probatorik, Regular, Akut |
| `IsVideoSession` | `bool` | Videosprechstunde? |
| `Notes` | `string?` | Brief session notes |
| `BilledGop` | `string?` | Which GOP was billed |

#### Entity: `PtvForm`

```
MediPrax.Core/Entities/PtvForm.cs
```

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `TherapyCaseId` | `Guid` | FK → TherapyCase |
| `FormType` | `PtvFormType` | Enum: PTV1, PTV2, PTV3, etc. |
| `CreatedDate` | `DateOnly` | Date generated |
| `FormData` | `Dictionary<string, string>` | JSONB — form field values |
| `PdfData` | `byte[]?` | Generated PDF |
| `Status` | `PtvFormStatus` | Draft, Finalized, Sent |

### 19.2 Therapy Contingent Rules (Business Logic)

```
MediPrax.Application/Services/TherapyContingentRules.cs
```

Encode KBV therapy contingent rules:

| Therapy Type | Initial Sessions | Extension | Max Total | Gutachter Required |
|-------------|-----------------|-----------|-----------|-------------------|
| Sprechstunde | 6×25min | — | 6 | No |
| Probatorik (VT/TP/ST) | 4 | — | 4 | No |
| Probatorik (AP) | 4 | +4 | 8 | No |
| Akutbehandlung | 24×25min | — | 24 | No |
| KZT1 | 12 | — | 12 | No |
| KZT2 | 12 | — | 24 total | No |
| LZT-VT | 60 | — | 80 | Yes |
| LZT-TP | 60 | — | 100 | Yes |
| LZT-AP (Einzel) | 160 | — | 300 | Yes |
| LZT-ST | 36 | — | 48 | Yes |

**Warnings:**
- `WarningThreshold` at 80% of approved sessions (e.g., session 10 of 12)
- `ExtensionSuggestion` when approaching limit
- `BlockBilling` when sessions exceed approved contingent

### 19.3 PTV Form Generation (PDF)

```
MediPrax.Reporting/PtvForms/
├── Ptv1Document.cs       // Antrag auf Psychotherapie
├── Ptv2Document.cs       // Information Therapeut → KK
├── Ptv3Document.cs       // Bericht an Gutachter
├── Ptv10Document.cs      // Akutbehandlung
├── Ptv11Document.cs      // Probatorische Sitzungen
├── Ptv12Document.cs      // Akutbehandlung Anzeige
└── PtvFormDataPopulator.cs // Auto-fill from patient + therapy case
```

Each PTV form auto-populates:
- Patient demographics from `Patient` entity
- Insurance data (KK name, number, KVNR)
- Diagnoses from `TherapyCase.Diagnoses`
- Therapist information from `User`
- Therapy type and session counts
- Date fields

### 19.4 Service Layer

```
MediPrax.Application/Interfaces/ITherapyCaseService.cs
MediPrax.Application/Services/TherapyCaseService.cs
```

| Method | Description |
|--------|-------------|
| `GetByIdAsync(Guid id)` | Full therapy case with sessions |
| `GetByPatientAsync(Guid patientId)` | All therapy cases for patient |
| `GetActiveByTherapistAsync(Guid therapistId)` | Active cases for therapist |
| `CreateAsync(CreateTherapyCaseDto)` | Create new therapy case |
| `UpdateStatusAsync(Guid id, TherapyCaseStatus)` | Update case status |
| `AddSessionAsync(Guid caseId, CreateTherapySessionDto)` | Log a session |
| `GetContingentStatusAsync(Guid caseId)` | Remaining sessions, warnings |
| `GeneratePtvFormAsync(Guid caseId, PtvFormType)` | Generate PTV PDF |
| `GetPtvFormsAsync(Guid caseId)` | List all PTV forms |
| `RequestExtensionAsync(Guid caseId, int additionalSessions)` | Start extension workflow |

### 19.5 Blazor UI

#### Therapy Case Overview

```
MediPrax.Server/Components/Pages/Therapie/TherapieFaelle.razor
```

- List of all active therapy cases
- Filter by therapist, status, therapy type
- Color-coded session countdown (green > yellow > red)
- Quick actions: Add session, Generate PTV form

#### Therapy Case Detail

```
MediPrax.Server/Components/Pages/Therapie/TherapieFallDetail.razor
```

- **Header:** Patient name, therapy type, status, progress bar (sessions used/approved)
- **Session list:** Numbered sessions with dates and notes
- **PTV forms section:** Generated forms with download links
- **Contingent alert:** Warning banner when approaching limit
- **Actions:** Change status, add session, generate form, request extension

#### PTV Form Editor

```
MediPrax.Server/Components/Pages/Therapie/PtvFormEditor.razor
```

- Form-based editor for PTV data
- Auto-populated fields (editable)
- Preview → Finalize → Generate PDF workflow
- Print button

### 19.6 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/TherapyCase.cs` |
| Create | `MediPrax.Core/Entities/TherapySession.cs` |
| Create | `MediPrax.Core/Entities/PtvForm.cs` |
| Create | `MediPrax.Core/Enums/TherapyType.cs` |
| Create | `MediPrax.Core/Enums/TherapyCaseStatus.cs` |
| Create | `MediPrax.Core/Enums/GutachterStatus.cs` |
| Create | `MediPrax.Core/Enums/PtvFormType.cs` |
| Create | `MediPrax.Core/Enums/PtvFormStatus.cs` |
| Create | `MediPrax.Core/Enums/SessionType.cs` |
| Create | `MediPrax.Application/DTOs/TherapyCaseDto.cs` |
| Create | `MediPrax.Application/DTOs/TherapySessionDto.cs` |
| Create | `MediPrax.Application/DTOs/PtvFormDto.cs` |
| Create | `MediPrax.Application/DTOs/ContingentStatusDto.cs` |
| Create | `MediPrax.Application/Interfaces/ITherapyCaseService.cs` |
| Create | `MediPrax.Application/Services/TherapyCaseService.cs` |
| Create | `MediPrax.Application/Services/TherapyContingentRules.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/TherapyCaseConfiguration.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/TherapySessionConfiguration.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/PtvFormConfiguration.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/MediPraxDbContext.cs` |
| Create | `MediPrax.Reporting/PtvForms/Ptv1Document.cs` |
| Create | `MediPrax.Reporting/PtvForms/Ptv2Document.cs` |
| Create | `MediPrax.Reporting/PtvForms/Ptv3Document.cs` |
| Create | `MediPrax.Reporting/PtvForms/Ptv10Document.cs` |
| Create | `MediPrax.Reporting/PtvForms/Ptv11Document.cs` |
| Create | `MediPrax.Reporting/PtvForms/PtvFormDataPopulator.cs` |
| Create | `MediPrax.Server/Components/Pages/Therapie/TherapieFaelle.razor` |
| Create | `MediPrax.Server/Components/Pages/Therapie/TherapieFallDetail.razor` |
| Create | `MediPrax.Server/Components/Pages/Therapie/PtvFormEditor.razor` |
| Modify | `MediPrax.Server/Components/Layout/MainLayout.razor` — Add "Therapie" nav |
| Modify | `MediPrax.Server/Program.cs` |
| Create | `tests/MediPrax.IntegrationTests/TherapyCaseServiceTests.cs` |
| Create | `tests/MediPrax.UnitTests/TherapyContingentRulesTests.cs` |

---

## Milestone 20: Structured Neurological Examination

> **Priority:** P0 | **Estimated Effort:** Medium
> **Why:** Every neurological practice needs structured exam documentation. The current basic templates are insufficient for systematic neurological status recording.

### 20.1 Data Model

#### Entity: `NeurologicalExamination`

```
MediPrax.Core/Entities/NeurologicalExamination.cs
```

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `EncounterId` | `Guid` | FK → Encounter |
| `PatientId` | `Guid` | FK → Patient |
| `ExaminedById` | `Guid` | FK → User |
| `ExamDate` | `DateOnly` | Date of exam |
| `CranialNerves` | `CranialNerveFindings` | JSONB |
| `MotorSystem` | `MotorFindings` | JSONB |
| `Reflexes` | `ReflexFindings` | JSONB |
| `SensorySystem` | `SensoryFindings` | JSONB |
| `Coordination` | `CoordinationFindings` | JSONB |
| `Gait` | `GaitFindings` | JSONB |
| `MeningealSigns` | `MeningealFindings` | JSONB |
| `NarrativeText` | `string?` | Auto-generated or manual |
| `Notes` | `string?` | Additional notes |

#### Value Objects (all JSONB):

**`CranialNerveFindings`:**
- `I_Olfactory` (string: "normal" / "eingeschränkt" / "aufgehoben" + comment)
- `II_Visual` (Visus, Gesichtsfeld, Pupillen, Funduskopie)
- `III_IV_VI_Oculomotor` (Augenbewegungen, Doppelbilder, Nystagmus)
- `V_Trigeminal` (Sensibilität, Masseter, Kornealreflex)
- `VII_Facial` (Stirnrunzeln, Mundwinkel, Geschmack)
- `VIII_Vestibulocochlear` (Gehör, Schwindel, Weber/Rinne)
- `IX_X_Glossopharyngeal_Vagus` (Gaumensegel, Schlucken, Würgereflex)
- `XI_Accessory` (Kopfdrehung, Schulterhebung)
- `XII_Hypoglossal` (Zungenbeweglichkeit, Atrophie, Fibrillationen)

**`MotorFindings`:**
- Per body region (RArm, LArm, RLeg, LLeg):
  - `Strength` (MRC grade 0–5)
  - `Tone` (normal / spastisch / rigide / schlaff)
  - `Bulk` (normal / Atrophie)
  - `Fasciculations` (bool)

**`ReflexFindings`:**
- Per reflex (BSR, TSR, RPR, PSR, ASR):
  - `Right` (0 = absent, 1 = abgeschwächt, 2 = normal, 3 = lebhaft, 4 = gesteigert+Kloni)
  - `Left` (same scale)
- Pathological: `BabinskiRight`, `BabinskiLeft`, `HoffmannTroemer`, etc.

**`SensoryFindings`:**
- `Touch`, `Pain`, `Temperature`, `Vibration`, `Proprioception`
- Each: (normal / eingeschränkt / aufgehoben) + distribution pattern

**`CoordinationFindings`:**
- `FingerNose` (normal / Dysmetrie / Intentionstremor)
- `HeelShin` (normal / Dysmetrie)
- `Romberg` (normal / unsicher / pathologisch)
- `TandemGait` (normal / unsicher / nicht möglich)
- `Diadochokinese` (normal / Dysdiadochokinese)

**`GaitFindings`:**
- `GaitPattern` (normal / spastisch / ataktisch / kleinschrittig / Steppergang / etc.)
- `ArmSwing` (normal / vermindert)
- `Stability` (normal / unsicher / Fallneigung)

**`MeningealFindings`:**
- `NeckStiffness` (bool)
- `Kernig` (bool)
- `Brudzinski` (bool)
- `Lasegue` (Right: degrees / Left: degrees)

### 20.2 Narrative Text Generator

```
MediPrax.Application/Services/NeuroExamTextGenerator.cs
```

Generates standard neurological report text from structured findings:

*"Hirnnerven: Pupillen isokor, prompt reagibel. Augenbewegungen frei, kein Nystagmus. Mimische Muskulatur seitengleich innerviert. Zunge gerade herausgestreckt. Motorik: Kraftgrade allseits 5/5, Tonus unauffällig. Muskeleigenreflexe seitengleich mittellebhaft auslösbar, keine pathologischen Reflexe. Sensibilität für alle Qualitäten intakt. Koordination: Finger-Nase-Versuch zielsicher, Stand nach Romberg sicher. Gangbild unauffällig. Meningismus: nicht vorhanden."*

### 20.3 Blazor UI

```
MediPrax.Server/Components/Pages/Dokumentation/NeurologischerBefund.razor
```

**Layout:**
1. **Tabbed sections:** Hirnnerven | Motorik | Reflexe | Sensibilität | Koordination | Gang | Meningismus
2. **Reflex diagram:** Visual body diagram with clickable reflex points (HTML/SVG)
3. **Motor strength grid:** Body region × MRC grade table
4. **"Normalbefund" button:** Sets all to normal with one click (most common workflow)
5. **Live narrative preview** at bottom
6. **Side-by-side comparison** with previous exam

### 20.4 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/NeurologicalExamination.cs` |
| Create | `MediPrax.Core/ValueObjects/CranialNerveFindings.cs` |
| Create | `MediPrax.Core/ValueObjects/MotorFindings.cs` |
| Create | `MediPrax.Core/ValueObjects/ReflexFindings.cs` |
| Create | `MediPrax.Core/ValueObjects/SensoryFindings.cs` |
| Create | `MediPrax.Core/ValueObjects/CoordinationFindings.cs` |
| Create | `MediPrax.Core/ValueObjects/GaitFindings.cs` |
| Create | `MediPrax.Core/ValueObjects/MeningealFindings.cs` |
| Create | `MediPrax.Application/DTOs/NeurologicalExaminationDto.cs` |
| Create | `MediPrax.Application/Interfaces/INeurologicalExamService.cs` |
| Create | `MediPrax.Application/Services/NeurologicalExamService.cs` |
| Create | `MediPrax.Application/Services/NeuroExamTextGenerator.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/NeurologicalExaminationConfiguration.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/MediPraxDbContext.cs` |
| Create | `MediPrax.Server/Components/Pages/Dokumentation/NeurologischerBefund.razor` |
| Modify | `MediPrax.Server/Components/Pages/Dokumentation/EncounterFormular.razor` |
| Modify | `MediPrax.Server/Program.cs` |
| Modify | `MediPrax.Reporting/Arztbrief/ArztbriefDocument.cs` |
| Create | `tests/MediPrax.IntegrationTests/NeurologicalExamServiceTests.cs` |
| Create | `tests/MediPrax.UnitTests/NeuroExamTextGeneratorTests.cs` |

---

## Milestone 21: Advanced Medication Management (Psychiatry)

> **Priority:** P1 | **Estimated Effort:** Large
> **Why:** Psychiatric polypharmacy is complex. Drug interactions, monitoring requirements, and depot management are patient safety concerns.

### 21.1 Enhancements to Existing `Medication` Entity

Add new properties to `MediPrax.Core/Entities/Medication.cs`:

| Property | Type | Description |
|----------|------|-------------|
| `Category` | `MedicationCategory` | Enum: Antidepressant, Antipsychotic, etc. |
| `TargetDose` | `string?` | Target dosage (for titration) |
| `TitrationSchedule` | `string?` | Free text titration plan |
| `IsDepot` | `bool` | Depot/injection medication |
| `DepotIntervalDays` | `int?` | Injection interval (e.g., 28 days) |
| `LastDepotDate` | `DateOnly?` | Last injection date |
| `NextDepotDate` | `DateOnly?` | Computed: Last + Interval |
| `RequiresMonitoring` | `bool` | Triggers lab monitoring |
| `MonitoringType` | `string?` | E.g., "Lithium", "Clozapine", "Metabolic" |

### 21.2 Enum: `MedicationCategory`

```csharp
public enum MedicationCategory
{
    Antidepressivum,
    Antipsychotikum,
    Stimmungsstabilisierer,
    Anxiolytikum,
    Hypnotikum,
    Stimulans,           // ADHD
    Antikonvulsivum,     // Antiepileptika
    Substitutionsmittel, // Opioid substitution
    Sonstiges
}
```

### 21.3 Drug Interaction Service

```
MediPrax.Application/Services/DrugInteractionService.cs
```

**Approach:** Embedded rule-based system (not external API — keeps it offline-capable):

```
MediPrax.Application/Catalogs/DrugInteractionCatalog.cs
```

| Interaction Type | Priority | Examples |
|-----------------|----------|---------|
| QTc prolongation | Critical | Haloperidol + Citalopram |
| Serotonin syndrome | Critical | SSRI + MAO inhibitor |
| CYP2D6 inhibition | Warning | Fluoxetine + Codeine |
| CYP3A4 induction | Warning | Carbamazepine + Oral contraceptives |
| Sedation potentiation | Info | Benzodiazepine + Opiate |
| Anticholinergic load | Warning | Amitriptyline + Biperiden |

**Method:** `CheckInteractionsAsync(Guid patientId) → List<InteractionWarning>`
- Checks all active medications for the patient
- Returns severity-sorted list of interaction warnings
- Displayed on `PatientDetail.razor` medication section

### 21.4 Monitoring Schedule Service

```
MediPrax.Application/Services/MedicationMonitoringService.cs
```

**Monitoring templates per medication:**

| Medication | Monitoring | Frequency |
|-----------|-----------|-----------|
| Lithium | Serum level, TSH, Creatinine, GFR | Every 3 months |
| Clozapine | WBC, Neutrophils | Weekly (first 18 weeks), then monthly |
| Valproate | Serum level, Liver function, Blood count | Every 3-6 months |
| Atypical antipsychotics | Weight, Glucose, HbA1c, Lipids | Every 3-6 months |
| Carbamazepine | Serum level, Blood count, Sodium, Liver | Every 3-6 months |

**Method:** `GetOverdueMonitoringAsync(Guid? patientId) → List<MonitoringAlert>`
- Checks medication start dates and last monitoring dates
- Generates alerts: "Lithiumspiegel überfällig (letzte Kontrolle: 15.01.2026)"
- Dashboard integration + patient detail alerts

### 21.5 Depot Medication Tracking

- **Calendar view** of upcoming depot injections across all patients
- **Dashboard widget:** "Anstehende Depot-Injektionen diese Woche"
- **Recall integration:** Auto-create `Recall` when depot is due

### 21.6 Bundeseinheitlicher Medikationsplan (BMP)

```
MediPrax.Reporting/Medikationsplan/BmpDocument.cs
```

Generate BMP per KBV specification:
- Standard A4 format with structured medication table
- QR code containing medication data (per BMP specification 2.6+)
- Columns: Wirkstoff | Handelsname | Stärke | Form | Morgens | Mittags | Abends | Nachts | Hinweis
- Auto-generated from active medications

### 21.7 Files to Create/Modify

| Action | File |
|--------|------|
| Modify | `MediPrax.Core/Entities/Medication.cs` — Add new properties |
| Create | `MediPrax.Core/Enums/MedicationCategory.cs` |
| Create | `MediPrax.Application/Catalogs/DrugInteractionCatalog.cs` |
| Create | `MediPrax.Application/Services/DrugInteractionService.cs` |
| Create | `MediPrax.Application/Services/MedicationMonitoringService.cs` |
| Create | `MediPrax.Application/DTOs/InteractionWarningDto.cs` |
| Create | `MediPrax.Application/DTOs/MonitoringAlertDto.cs` |
| Modify | `MediPrax.Application/Services/MedicationService.cs` — Integrate checks |
| Create | `MediPrax.Reporting/Medikationsplan/BmpDocument.cs` |
| Create | `MediPrax.Reporting/Medikationsplan/BmpModel.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/Configurations/MedicationConfiguration.cs` |
| Modify | `MediPrax.Server/Components/Pages/Patienten/PatientDetail.razor` — Show warnings |
| Create | `MediPrax.Server/Components/Pages/Medikation/DepotUebersicht.razor` |
| Modify | `MediPrax.Server/Program.cs` |
| Create | `tests/MediPrax.UnitTests/DrugInteractionCatalogTests.cs` |
| Create | `tests/MediPrax.IntegrationTests/MedicationMonitoringServiceTests.cs` |

---

## Milestone 22: Laboratory Integration & Monitoring

> **Priority:** P1 | **Estimated Effort:** Medium-Large
> **Why:** Psychiatric medication monitoring requires regular lab tests. Without lab integration, results must be manually transcribed, risking errors and missed critical values.

### 22.1 Data Model

#### Entity: `LabResult`

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `PatientId` | `Guid` | FK → Patient |
| `OrderedById` | `Guid?` | FK → User |
| `LabName` | `string` | Laboratory name |
| `OrderDate` | `DateOnly` | When ordered |
| `ResultDate` | `DateOnly?` | When results arrived |
| `Status` | `LabResultStatus` | Ordered, Received, Reviewed |
| `ReviewedById` | `Guid?` | FK → User who reviewed |
| `ReviewedAt` | `DateTime?` | When reviewed |
| `Values` | `List<LabValue>` | JSONB — individual lab values |
| `Notes` | `string?` | Clinician notes |
| `ImportSource` | `string?` | "LDT" or "Manual" |

#### Value Object: `LabValue` (JSONB)

| Property | Type | Description |
|----------|------|-------------|
| `ParameterName` | `string` | E.g., "Lithium", "TSH", "Leukozyten" |
| `ParameterCode` | `string?` | Lab code |
| `Value` | `string` | Result value |
| `Unit` | `string` | Unit (e.g., "mmol/l", "µg/l") |
| `ReferenceRangeLow` | `decimal?` | Normal range lower bound |
| `ReferenceRangeHigh` | `decimal?` | Normal range upper bound |
| `IsAbnormal` | `bool` | Outside reference range |
| `IsCritical` | `bool` | Critical value requiring immediate attention |
| `Category` | `string?` | E.g., "Blutbild", "Schilddrüse", "Niere" |

### 22.2 LDT Import (Labordatentransfer)

```
MediPrax.Application/Services/LdtImportService.cs
```

- Parse LDT 3.0 file format (fixed-width text file, standard from KBV/DKG)
- Map patient by name + DOB or KVNR
- Map lab parameters to LabValue
- Flag abnormal and critical values
- Create LabResult entity from imported data

**LDT file upload endpoint:**
```csharp
app.MapPost("/api/labor/ldt-import", async (IFormFile file, ILdtImportService service) =>
{
    var result = await service.ImportAsync(file.OpenReadStream());
    return Results.Ok(result); // ImportSummary { Imported, Errors, Warnings }
}).RequireAuthorization("Klinisch");
```

### 22.3 Lab Monitoring Templates

```
MediPrax.Application/Catalogs/LabMonitoringCatalog.cs
```

Pre-defined monitoring panels per medication:

```csharp
new MonitoringPanel("Lithium", [
    new("Lithiumspiegel", "mmol/l", 0.6m, 0.8m, IntervalDays: 90),
    new("TSH", "mU/l", 0.4m, 4.0m, IntervalDays: 180),
    new("fT3", "pg/ml", 2.0m, 4.4m, IntervalDays: 180),
    new("fT4", "ng/dl", 0.93m, 1.7m, IntervalDays: 180),
    new("Kreatinin", "mg/dl", 0.7m, 1.2m, IntervalDays: 90),
    new("GFR", "ml/min", 60m, null, IntervalDays: 90)
]),
new MonitoringPanel("Clozapin", [
    new("Leukozyten", "/µl", 3500m, 10000m, IntervalDays: 7), // First 18 weeks
    new("Neutrophile", "/µl", 1500m, null, IntervalDays: 7),
    // After 18 weeks: IntervalDays: 30
]),
// ... Valproat, Carbamazepin, Atypische Antipsychotika
```

### 22.4 Blazor UI

#### Lab Results Page

```
MediPrax.Server/Components/Pages/Labor/Laborergebnisse.razor
```

- Lab result list per patient
- Color-coded values (normal=green, abnormal=yellow, critical=red)
- "Zur Kenntnis genommen" (acknowledged) button with user tracking
- Trend charts for selected parameters over time

#### Lab Import Page

```
MediPrax.Server/Components/Pages/Labor/LdtImport.razor
```

- File upload for LDT files
- Preview imported values before saving
- Patient matching confirmation
- Import history log

#### Lab Monitoring Dashboard Widget

- On Home dashboard: "Überfällige Laborkontrollen" alert
- On PatientDetail: Lab monitoring status per medication

### 22.5 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/LabResult.cs` |
| Create | `MediPrax.Core/Enums/LabResultStatus.cs` |
| Create | `MediPrax.Core/ValueObjects/LabValue.cs` |
| Create | `MediPrax.Application/Catalogs/LabMonitoringCatalog.cs` |
| Create | `MediPrax.Application/DTOs/LabResultDto.cs` |
| Create | `MediPrax.Application/Interfaces/ILabResultService.cs` |
| Create | `MediPrax.Application/Services/LabResultService.cs` |
| Create | `MediPrax.Application/Services/LdtImportService.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/LabResultConfiguration.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/MediPraxDbContext.cs` |
| Create | `MediPrax.Server/Components/Pages/Labor/Laborergebnisse.razor` |
| Create | `MediPrax.Server/Components/Pages/Labor/LdtImport.razor` |
| Modify | `MediPrax.Server/Components/Pages/Patienten/PatientDetail.razor` |
| Modify | `MediPrax.Server/Components/Pages/Home.razor` |
| Modify | `MediPrax.Server/Program.cs` |
| Create | `tests/MediPrax.IntegrationTests/LabResultServiceTests.cs` |
| Create | `tests/MediPrax.UnitTests/LdtImportServiceTests.cs` |

---

## Milestone 23: Billing Enhancements

> **Priority:** P1 | **Estimated Effort:** Large
> **Why:** Billing plausibility checks protect against KV rejections and Regress (financial recoupment). GOP suggestion engine improves revenue capture.

### 23.1 Plausibility Check Engine

```
MediPrax.Application/Services/BillingPlausibilityService.cs
```

#### Exclusion Rules

```
MediPrax.Application/Catalogs/GopExclusionCatalog.cs
```

Define which GOPs cannot be billed together:

```csharp
new ExclusionRule("16220", ["16210", "16211", "16212"], 
    "Grundpauschale und Gesprächsziffer am selben Tag nicht kombinierbar"),
new ExclusionRule("21220", ["21210", "21211", "21212"], 
    "Grundpauschale und Gesprächsziffer am selben Tag nicht kombinierbar"),
// ... 50+ rules for chapters 16, 21, 35
```

#### Quantity Limits

```csharp
new QuantityLimit("16220", MaxPerQuarter: 3, 
    "Neurologisches Gespräch max. 3x pro Quartal"),
new QuantityLimit("35100", MaxPerPatient: 6, 
    "Psychotherapeutische Sprechstunde max. 6 pro Patient"),
// ...
```

#### Time Documentation Checks

- GOP 16220/21220 require documented `DurationMinutes` ≥ 10 min
- GOP 35xxx require documented session duration matching 25 or 50 min
- Flag missing duration documentation

#### Method

```csharp
public async Task<List<PlausibilityWarning>> CheckAsync(
    Guid patientId, string quarter, CancellationToken ct = default)
{
    var items = await billingService.GetByPatientAndQuarterAsync(patientId, quarter);
    var warnings = new List<PlausibilityWarning>();
    
    // Check exclusion rules
    // Check quantity limits
    // Check time documentation
    // Check Kontingent (therapy sessions)
    
    return warnings;
}
```

### 23.2 GOP Suggestion Engine

```
MediPrax.Application/Services/GopSuggestionService.cs
```

Based on documented encounter data, suggest billable GOPs:

| Trigger | Suggested GOP | Condition |
|---------|--------------|-----------|
| Encounter created with duration ≥ 10 min | 16220 or 21220 | Neurology or Psychiatry |
| Psychometric test completed | 35600 | Any |
| First encounter of quarter | 16210/21210 (Grundpauschale) | Auto-add |
| EEG documented | 16320 | Neurology |
| Therapy session logged | 35150/35141/etc. | Based on TherapyCase type |
| Encounter notes mention "Notfall" | 21221 | Psychiatry |

### 23.3 Quarter-End Validation

```
MediPrax.Server/Components/Pages/Abrechnung/QuartalsValidierung.razor
```

Before KVDT export:
- Run plausibility checks on ALL patients in the quarter
- Show aggregated warnings: Critical (must fix) / Warning (review) / Info
- Exportable error report
- One-click fix suggestions where possible

### 23.4 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Application/Catalogs/GopExclusionCatalog.cs` |
| Create | `MediPrax.Application/Services/BillingPlausibilityService.cs` |
| Create | `MediPrax.Application/Services/GopSuggestionService.cs` |
| Create | `MediPrax.Application/DTOs/PlausibilityWarningDto.cs` |
| Create | `MediPrax.Application/DTOs/GopSuggestionDto.cs` |
| Modify | `MediPrax.Application/Services/BillingService.cs` — Integrate checks |
| Create | `MediPrax.Server/Components/Pages/Abrechnung/QuartalsValidierung.razor` |
| Modify | `MediPrax.Server/Components/Pages/Abrechnung/Abrechnungsuebersicht.razor` — Show warnings |
| Modify | `MediPrax.Server/Components/Pages/Dokumentation/EncounterFormular.razor` — Show suggestions |
| Modify | `MediPrax.Server/Program.cs` |
| Create | `tests/MediPrax.UnitTests/GopExclusionCatalogTests.cs` |
| Create | `tests/MediPrax.UnitTests/BillingPlausibilityServiceTests.cs` |
| Create | `tests/MediPrax.UnitTests/GopSuggestionServiceTests.cs` |

---

## Milestone 24: Missing KBV Forms

> **Priority:** P1 | **Estimated Effort:** Medium
> **Why:** Muster 2 (hospital admission), 13 (therapy prescriptions), and 26 (Soziotherapie) are used daily.

### 24.1 Forms to Implement

#### Muster 2 — Krankenhauseinweisung (Hospital Admission)

```
MediPrax.Reporting/Formulare/KrankenhauseinweisungDocument.cs
```

| Field | Type |
|-------|------|
| Patient data | Auto-populated |
| Insurance data | Auto-populated |
| Einweisungsdiagnose | ICD-10 + text |
| Befunde und Angaben | Free text |
| Einweisungsgrund | Dropdown + text |
| Krankenhaus | Text input |
| Fachabteilung | Text input |
| Notfall (yes/no) | Checkbox |

**Especially important for psychiatry:** Acute psychiatric hospital admissions.

#### Muster 7 — Überweisung vor Psychotherapie

```
MediPrax.Reporting/Formulare/UeberweisungPsychotherapieDocument.cs
```

- Similar to existing Überweisung but specific fields
- Somatische Abklärung completed (yes/no)
- Relevant somatic diagnoses
- Required before Richtlinien-Psychotherapie can begin

#### Muster 13 — Heilmittelverordnung (Therapy Prescription)

```
MediPrax.Reporting/Formulare/HeilmittelverordnungDocument.cs
```

| Field | Description |
|-------|-------------|
| Heilmittel type | Ergotherapie / Physiotherapie / Logopädie |
| Diagnose (ICD-10) | From Heilmittel-Diagnoseliste |
| Leitsymptomatik | Structured per Heilmittelkatalog |
| Therapieziel | Free text |
| Anzahl | Number of sessions |
| Frequenz | Sessions per week |
| Hausbesuch | Home visit checkbox |
| Erstverordnung / Folgeverordnung | Radio |
| Langfristiger Heilmittelbedarf | Checkbox |

**Critical for neurology:** Ergotherapy after stroke, physiotherapy for MS/Parkinson, speech therapy for aphasia.

#### Muster 14 — Häusliche Krankenpflege

```
MediPrax.Reporting/Formulare/HaeuslicheKrankenpflegeDocument.cs
```

- Psychiatric home nursing (Psychiatrische häusliche Krankenpflege)
- Medication supervision, symptom monitoring
- Duration and frequency specification

#### Muster 26 — Soziotherapie-Verordnung

```
MediPrax.Reporting/Formulare/SoziotherapieDocument.cs
```

**Unique to psychiatry.** For severely mentally ill patients:

| Field | Description |
|-------|-------------|
| Diagnose | ICD-10 (typically F20-F31) |
| Fähigkeitsstörungen | Structured checklist |
| Therapieziele | Free text |
| Verordnungsdauer | Max 120 Stunden / 3 Jahre |
| Vorherige Soziotherapie | Yes/No + dates |
| Soziotherapeut | Provider name |

### 24.2 Form Data Entities

Don't create separate entities per form. Instead, use the existing `Document` entity with `DocType` extended:

```csharp
// Add to DocumentType enum:
Krankenhauseinweisung,
UeberweisungPsychotherapie,
Heilmittelverordnung,
HaeuslicheKrankenpflege,
Soziotherapie
```

Store form-specific data in `Document.Content` as JSON.

### 24.3 Blazor UI

For each form, add a page under `/formulare/`:

```
MediPrax.Server/Components/Pages/Formulare/KrankenhauseinweisungFormular.razor
MediPrax.Server/Components/Pages/Formulare/HeilmittelverordnungFormular.razor
MediPrax.Server/Components/Pages/Formulare/SoziotherapieFormular.razor
MediPrax.Server/Components/Pages/Formulare/HaeuslicheKrankenpflegeFormular.razor
```

### 24.4 API Endpoints

Add to `Program.cs`:

```csharp
app.MapGet("/api/formulare/krankenhauseinweisung", ...);
app.MapGet("/api/formulare/heilmittelverordnung", ...);
app.MapGet("/api/formulare/soziotherapie", ...);
app.MapGet("/api/formulare/haeusliche-krankenpflege", ...);
```

### 24.5 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Reporting/Formulare/KrankenhauseinweisungDocument.cs` |
| Create | `MediPrax.Reporting/Formulare/UeberweisungPsychotherapieDocument.cs` |
| Create | `MediPrax.Reporting/Formulare/HeilmittelverordnungDocument.cs` |
| Create | `MediPrax.Reporting/Formulare/HaeuslicheKrankenpflegeDocument.cs` |
| Create | `MediPrax.Reporting/Formulare/SoziotherapieDocument.cs` |
| Modify | `MediPrax.Core/Enums/DocumentType.cs` — Add new types |
| Create | `MediPrax.Server/Components/Pages/Formulare/KrankenhauseinweisungFormular.razor` |
| Create | `MediPrax.Server/Components/Pages/Formulare/HeilmittelverordnungFormular.razor` |
| Create | `MediPrax.Server/Components/Pages/Formulare/SoziotherapieFormular.razor` |
| Create | `MediPrax.Server/Components/Pages/Formulare/HaeuslicheKrankenpflegeFormular.razor` |
| Modify | `MediPrax.Server/Program.cs` — Add endpoints |

---

## Milestone 25: Appointment Enhancements

> **Priority:** P2 | **Estimated Effort:** Medium
> **Why:** Psychotherapy requires recurring weekly appointments and session counting. Waitlist management addresses the critical psychotherapy access problem.

### 25.1 Recurring Appointments

#### New Entity: `AppointmentSeries`

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `PatientId` | `Guid` | FK → Patient |
| `DoctorId` | `Guid` | FK → User |
| `TherapyCaseId` | `Guid?` | FK → TherapyCase (optional) |
| `RecurrencePattern` | `RecurrencePattern` | Enum: Weekly, BiWeekly, Monthly |
| `DayOfWeek` | `DayOfWeek` | Which day |
| `StartTime` | `TimeOnly` | Start time |
| `DurationMinutes` | `int` | Duration |
| `SeriesStartDate` | `DateOnly` | First appointment |
| `SeriesEndDate` | `DateOnly?` | Last appointment (or open-ended) |
| `MaxOccurrences` | `int?` | Max appointments to generate |
| `Notes` | `string?` | Default notes for generated appointments |

**Service method:** `GenerateAppointmentsAsync(Guid seriesId)` — Creates individual `Appointment` entities for each occurrence, linked to the series.

### 25.2 Session Numbering

When an appointment belongs to a `TherapyCase`:
- Display "Sitzung X von Y" (Session X of Y approved)
- Auto-increment session count on completion
- Color-code: green (plenty left), yellow (approaching limit), red (at limit)

### 25.3 Waitlist Management

#### New Entity: `WaitlistEntry`

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `PatientId` | `Guid` | FK → Patient |
| `PreferredTherapistId` | `Guid?` | FK → User |
| `RequestDate` | `DateOnly` | When they were added |
| `TherapyTypeRequested` | `TherapyType?` | What they need |
| `Priority` | `WaitlistPriority` | Normal, Urgent |
| `PreferredDays` | `List<DayOfWeek>?` | JSONB |
| `PreferredTimeSlot` | `string?` | "Vormittags" / "Nachmittags" |
| `Notes` | `string?` | Notes |
| `Status` | `WaitlistStatus` | Waiting, Offered, Scheduled, Cancelled |
| `OfferedDate` | `DateOnly?` | When a slot was offered |

#### Waitlist Dashboard

```
MediPrax.Server/Components/Pages/Termine/Warteliste.razor
```

- Sorted by wait time
- Quick action: "Termin anbieten" → creates appointment
- Statistics: average wait time, queue length per therapist
- Filter by therapy type, therapist, priority

### 25.4 Appointment Type Templates

```
MediPrax.Application/Catalogs/AppointmentTypeCatalog.cs
```

| Template | Default Duration | Color |
|---------|-----------------|-------|
| Erstgespräch | 50 min | Blue |
| Folgetermin | 25 min | Green |
| Medikamentenkontrolle | 10 min | Yellow |
| Psychotherapie-Sitzung | 50 min | Purple |
| EEG | 30 min | Orange |
| EMG/NLG | 45 min | Orange |
| Akuttermin | 25 min | Red |
| Notfall | 15 min | Dark Red |

### 25.5 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/AppointmentSeries.cs` |
| Create | `MediPrax.Core/Entities/WaitlistEntry.cs` |
| Create | `MediPrax.Core/Enums/RecurrencePattern.cs` |
| Create | `MediPrax.Core/Enums/WaitlistPriority.cs` |
| Create | `MediPrax.Core/Enums/WaitlistStatus.cs` |
| Create | `MediPrax.Application/Catalogs/AppointmentTypeCatalog.cs` |
| Create | `MediPrax.Application/DTOs/AppointmentSeriesDto.cs` |
| Create | `MediPrax.Application/DTOs/WaitlistEntryDto.cs` |
| Modify | `MediPrax.Application/Services/AppointmentService.cs` — Add series + waitlist |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/AppointmentSeriesConfiguration.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/WaitlistEntryConfiguration.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/MediPraxDbContext.cs` |
| Create | `MediPrax.Server/Components/Pages/Termine/Warteliste.razor` |
| Create | `MediPrax.Server/Components/Pages/Termine/TerminserieFormular.razor` |
| Modify | `MediPrax.Server/Components/Pages/Termine/TerminFormular.razor` — Template selection |
| Modify | `MediPrax.Server/Components/Pages/Termine/Wochenansicht.razor` — Color codes |
| Modify | `MediPrax.Server/Program.cs` |
| Create | `tests/MediPrax.IntegrationTests/AppointmentSeriesTests.cs` |
| Create | `tests/MediPrax.IntegrationTests/WaitlistServiceTests.cs` |

---

## Milestone 26: Disease-Specific Documentation Modules

> **Priority:** P2 | **Estimated Effort:** Medium
> **Why:** Structured disease-specific documentation improves care quality and enables outcome tracking.

### 26.1 Epilepsy Module

#### Entity: `SeizureDiary` (Anfallskalender)

| Property | Type | Description |
|----------|------|-------------|
| `PatientId` | `Guid` | FK |
| `SeizureDate` | `DateTime` | When it occurred |
| `SeizureType` | `string` | Fokal / Generalisiert / Unklassifiziert |
| `DurationSeconds` | `int?` | Duration |
| `Trigger` | `string?` | Known trigger |
| `AuraPresent` | `bool` | Preceding aura |
| `AuraDescription` | `string?` | Aura details |
| `ConsciousnessImpaired` | `bool` | Loss of consciousness |
| `PostictalState` | `string?` | Post-seizure state |
| `MedicationAtTime` | `string?` | Current meds snapshot |
| `Notes` | `string?` | |

**Blazor page:** `Dokumentation/Anfallskalender.razor` — Calendar view with seizure entries, frequency charts.

### 26.2 Headache/Migraine Module

#### Entity: `HeadacheDiary`

| Property | Type | Description |
|----------|------|-------------|
| `PatientId` | `Guid` | FK |
| `Date` | `DateOnly` | Date |
| `StartTime` | `TimeOnly?` | When it started |
| `DurationHours` | `decimal?` | Duration |
| `Intensity` | `int` | VAS 0–10 |
| `Type` | `string` | Migräne / Spannungskopfschmerz / Cluster / Andere |
| `Location` | `string?` | Einseitig rechts / links / beidseits / frontal / etc. |
| `AuraPresent` | `bool` | |
| `Nausea` | `bool` | |
| `Photophobia` | `bool` | |
| `Phonophobia` | `bool` | |
| `Triggers` | `List<string>?` | JSONB |
| `MedicationTaken` | `string?` | What was taken |
| `MedicationEffective` | `bool?` | Did it help? |

**Blazor page:** `Dokumentation/KopfschmerzTagebuch.razor` — Monthly calendar view, frequency/intensity charts.

### 26.3 Multiple Sclerosis Module

#### Entity: `MsDocumentation`

| Property | Type | Description |
|----------|------|-------------|
| `PatientId` | `Guid` | FK |
| `DocumentationDate` | `DateOnly` | |
| `EdssScore` | `decimal` | EDSS 0.0–10.0 |
| `IsRelapse` | `bool` | Schub? |
| `RelapseDescription` | `string?` | |
| `MriDate` | `DateOnly?` | Last MRI |
| `MriFindings` | `string?` | Summary |
| `NewLesions` | `int?` | New T2 lesions |
| `GadEnhancing` | `int?` | Gadolinium-enhancing lesions |
| `CurrentDmt` | `string?` | Current disease-modifying therapy |
| `Notes` | `string?` | |

**Blazor page:** `Dokumentation/MsDokumentation.razor` — EDSS over time chart, relapse timeline.

### 26.4 Parkinson Module

#### Entity: `ParkinsonDocumentation`

| Property | Type | Description |
|----------|------|-------------|
| `PatientId` | `Guid` | FK |
| `DocumentationDate` | `DateOnly` | |
| `HoehnYahrStage` | `decimal` | 0–5 |
| `Tremor` | `int` | 0–4 scale |
| `Rigidity` | `int` | 0–4 scale |
| `Bradykinesia` | `int` | 0–4 scale |
| `PosturalInstability` | `int` | 0–4 scale |
| `OnOffFluctuations` | `bool` | |
| `Dyskinesia` | `bool` | |
| `FreezeOfGait` | `bool` | |
| `NonMotorSymptoms` | `List<string>?` | JSONB |
| `Notes` | `string?` | |

### 26.5 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/SeizureDiary.cs` |
| Create | `MediPrax.Core/Entities/HeadacheDiary.cs` |
| Create | `MediPrax.Core/Entities/MsDocumentation.cs` |
| Create | `MediPrax.Core/Entities/ParkinsonDocumentation.cs` |
| Create | `MediPrax.Application/DTOs/SeizureDiaryDto.cs` |
| Create | `MediPrax.Application/DTOs/HeadacheDiaryDto.cs` |
| Create | `MediPrax.Application/DTOs/MsDocumentationDto.cs` |
| Create | `MediPrax.Application/DTOs/ParkinsonDocumentationDto.cs` |
| Create | `MediPrax.Application/Interfaces/ISeizureDiaryService.cs` |
| Create | `MediPrax.Application/Interfaces/IHeadacheDiaryService.cs` |
| Create | `MediPrax.Application/Services/SeizureDiaryService.cs` |
| Create | `MediPrax.Application/Services/HeadacheDiaryService.cs` |
| Create | `MediPrax.Application/Services/MsDocumentationService.cs` |
| Create | `MediPrax.Application/Services/ParkinsonDocumentationService.cs` |
| Create | `MediPrax.Server/Components/Pages/Dokumentation/Anfallskalender.razor` |
| Create | `MediPrax.Server/Components/Pages/Dokumentation/KopfschmerzTagebuch.razor` |
| Create | `MediPrax.Server/Components/Pages/Dokumentation/MsDokumentation.razor` |
| Create | `MediPrax.Server/Components/Pages/Dokumentation/ParkinsonDokumentation.razor` |
| Create EF configs, modify DbContext, register services, add tests |

---

## Milestone 27: Suicidality Assessment & Safety Planning

> **Priority:** P2 | **Estimated Effort:** Small-Medium
> **Why:** Suicidality assessment is a core psychiatric responsibility. Structured documentation protects both patient and clinician. System-wide alerts prevent critical information from being missed.

### 27.1 Data Model

#### Entity: `SuicidalityAssessment`

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `PatientId` | `Guid` | FK → Patient |
| `EncounterId` | `Guid` | FK → Encounter |
| `AssessedById` | `Guid` | FK → User |
| `AssessmentDate` | `DateOnly` | |
| `RiskLevel` | `SuicidalityRiskLevel` | Enum: None, Low, Moderate, High, Acute |
| `SuicidalIdeation` | `bool` | Active suicidal thoughts |
| `SuicidalPlans` | `bool` | Concrete plans |
| `SuicidalIntent` | `bool` | Intent to act |
| `PriorAttempts` | `bool` | History of attempts |
| `PriorAttemptsDetails` | `string?` | Details |
| `RiskFactors` | `List<string>` | JSONB — selected risk factors |
| `ProtectiveFactors` | `List<string>` | JSONB — selected protective factors |
| `SafetyPlan` | `string?` | Documented safety plan |
| `ActionsTaken` | `List<string>` | JSONB — measures taken |
| `Notes` | `string?` | |

### 27.2 System-Wide Alert

Add to `Patient` entity:
```csharp
public SuicidalityRiskLevel? CurrentSuicidalityRisk { get; set; }
public DateTime? SuicidalityRiskUpdatedAt { get; set; }
```

**Display:** Red alert banner on every page that shows patient data (PatientDetail, EncounterFormular, Wochenansicht, Wartezimmer) when `CurrentSuicidalityRisk >= Moderate`.

### 27.3 Risk Factor / Protective Factor Catalogs

```
MediPrax.Application/Catalogs/SuicidalityFactorCatalog.cs
```

**Risk factors:** Vorherige Suizidversuche, Substanzmissbrauch, Verlust/Trennung, chronische Schmerzen, Hoffnungslosigkeit, Impulsivität, Zugang zu Mitteln, Isolation, psychiatrische Diagnose (Depression, Schizophrenie, BPS), männliches Geschlecht, Alter >65...

**Protective factors:** Soziales Netzwerk, Kinder, religiöse Überzeugungen, therapeutische Beziehung, Zukunftspläne, Distanzierungsfähigkeit, Behandlungsadhärenz...

### 27.4 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/SuicidalityAssessment.cs` |
| Create | `MediPrax.Core/Enums/SuicidalityRiskLevel.cs` |
| Modify | `MediPrax.Core/Entities/Patient.cs` — Add risk level field |
| Create | `MediPrax.Application/Catalogs/SuicidalityFactorCatalog.cs` |
| Create | `MediPrax.Application/DTOs/SuicidalityAssessmentDto.cs` |
| Create | `MediPrax.Application/Interfaces/ISuicidalityAssessmentService.cs` |
| Create | `MediPrax.Application/Services/SuicidalityAssessmentService.cs` |
| Create | `MediPrax.Server/Components/Pages/Dokumentation/Suizidalitaet.razor` |
| Create | `MediPrax.Server/Components/Shared/SuicidalityAlert.razor` — Reusable alert banner |
| Modify | `MediPrax.Server/Components/Pages/Patienten/PatientDetail.razor` |
| Modify | `MediPrax.Server/Components/Pages/Termine/Wartezimmer.razor` |
| + EF config, DbContext, DI, tests |

---

## Milestone 28: GDT Device Interface

> **Priority:** P2 | **Estimated Effort:** Large
> **Why:** Neurological practices depend on EEG, EMG, NLG, and Doppler devices. GDT (Gerätedatentransfer) is the standard interface for bidirectional data exchange with medical devices.

### 28.1 GDT Protocol Implementation

```
MediPrax.Application/Services/GdtService.cs
MediPrax.Application/Services/GdtParser.cs
MediPrax.Application/Services/GdtWriter.cs
```

**GDT 2.1 format** (universally supported):
- Fixed-width text file
- Field identifiers (Feldkennungen): 3000 (Patient name), 3100 (DOB), 6200 (Result text), etc.
- Satzarten: 6310 (Request), 6311 (Response with results)

**Workflow:**
1. **PVS → Device:** Write GDT file with patient data (Satzart 6310) to device's watch directory
2. **Device → PVS:** Device writes results file (Satzart 6311) to PVS watch directory
3. **PVS reads:** Parse results, create `Document` entity with findings

### 28.2 File Watcher Service

```
MediPrax.Server/Services/GdtFileWatcherService.cs : BackgroundService
```

- Monitor configured directories for incoming GDT files
- Parse and import automatically
- Create notification for reviewing clinician
- Configurable per device (EEG path, EMG path, etc.)

### 28.3 Device Configuration

```
MediPrax.Application/DTOs/GdtDeviceConfig.cs
```

| Property | Description |
|----------|-------------|
| `DeviceName` | E.g., "Nihon Kohden EEG-1200" |
| `DeviceType` | EEG, EMG, NLG, Doppler, EP |
| `ImportDirectory` | Path where device writes results |
| `ExportDirectory` | Path where PVS writes requests |
| `GdtVersion` | "2.1" or "3.5" |
| `Encoding` | "ISO-8859-1" (standard for GDT) |

Store in `appsettings.json` under `GdtDevices` section.

### 28.4 Structured Report Templates

When GDT results are imported, provide structured report templates per device type:

- **EEG Report:** Background activity, focal abnormalities, epileptiform discharges, HV response, PS response, conclusion
- **EMG Report:** Muscle examined, insertional activity, spontaneous activity, motor unit potentials, interference pattern, conclusion
- **NLG Report:** Nerve, DML, CMAP amplitude, MNCV, F-wave, SNAP amplitude, SNCV
- **Evoked Potentials:** Type (VEP/SEP/AEP/MEP), latencies, amplitudes, interpretation

### 28.5 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Application/Services/GdtParser.cs` |
| Create | `MediPrax.Application/Services/GdtWriter.cs` |
| Create | `MediPrax.Application/Services/GdtService.cs` |
| Create | `MediPrax.Application/DTOs/GdtDeviceConfig.cs` |
| Create | `MediPrax.Application/DTOs/GdtResultDto.cs` |
| Create | `MediPrax.Server/Services/GdtFileWatcherService.cs` |
| Create | `MediPrax.Server/Components/Pages/Geraete/GeraeteUebersicht.razor` |
| Create | `MediPrax.Server/Components/Pages/Geraete/GeraeteErgebnisse.razor` |
| Modify | `MediPrax.Server/Program.cs` — Register BackgroundService |
| Modify | `MediPrax.Server/appsettings.json` — GDT device config section |
| Create | `tests/MediPrax.UnitTests/GdtParserTests.cs` |
| Create | `tests/MediPrax.UnitTests/GdtWriterTests.cs` |

---

## Milestone 29: Videosprechstunde Integration

> **Priority:** P3 | **Estimated Effort:** Small-Medium
> **Why:** Post-COVID standard. Important for psychiatric follow-ups and patients with mobility limitations.

### 29.1 Approach

**No custom video implementation.** Instead, integrate with a KBV-certified provider (e.g., Doctolib Video, RED connect, Patientus) via:

1. **Link generation:** Create video consultation link and send to patient
2. **Consent tracking:** Document Einwilligungserklärung
3. **Session marking:** Flag appointments as video sessions
4. **Billing rules:** Enforce psychiatry-specific video restrictions

### 29.2 Data Model Changes

Add to `Appointment`:
```csharp
public bool IsVideoConsultation { get; set; }
public string? VideoLink { get; set; }
public bool VideoConsentGiven { get; set; }
```

### 29.3 Psychiatric Video Rules

```
MediPrax.Application/Services/VideoConsultationRules.cs
```

- First psychotherapeutic Sprechstunde MUST be in-person
- At least 50 minutes of Sprechstunde and Probatorik must be in-person
- Certain GOPs have video variants (add suffix)
- Track video vs. in-person ratio per therapy case

### 29.4 Files to Create/Modify

| Action | File |
|--------|------|
| Modify | `MediPrax.Core/Entities/Appointment.cs` — Add video fields |
| Create | `MediPrax.Application/Services/VideoConsultationRules.cs` |
| Create | `MediPrax.Application/DTOs/VideoConsultationDto.cs` |
| Modify | `MediPrax.Server/Components/Pages/Termine/TerminFormular.razor` — Video toggle |
| Modify | `MediPrax.Server/Components/Pages/Termine/Wochenansicht.razor` — Video indicator |
| Modify | `MediPrax.Infrastructure/Persistence/Configurations/AppointmentConfiguration.cs` |
| Create | `tests/MediPrax.UnitTests/VideoConsultationRulesTests.cs` |

---

## Milestone 30: Text Modules & Documentation Automation

> **Priority:** P3 | **Estimated Effort:** Medium
> **Why:** Textbausteine dramatically speed up documentation. Competitors like tomedo and EPIKUR excel here.

### 30.1 Data Model

#### Entity: `TextModule`

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `CreatedById` | `Guid` | FK → User |
| `Shortcut` | `string` | Trigger text (e.g., "#normalbefund") |
| `Title` | `string` | Descriptive title |
| `Content` | `string` | Template text (supports {Patient.Name} etc.) |
| `Category` | `string` | E.g., "Psychopathologie", "Neurologie", "Arztbrief" |
| `IsGlobal` | `bool` | Available to all users vs. personal |
| `UsageCount` | `int` | Track popularity |

### 30.2 Template Variables

```
MediPrax.Application/Services/TextModuleService.cs
```

Supported variables in text module content:

| Variable | Resolved To |
|----------|------------|
| `{Patient.Name}` | Full patient name |
| `{Patient.Geburtsdatum}` | Date of birth |
| `{Patient.Alter}` | Calculated age |
| `{Patient.Geschlecht}` | Gender (Herr/Frau) |
| `{Arzt.Name}` | Current doctor name |
| `{Datum}` | Today's date |
| `{Diagnosen}` | Active ICD-10 codes from encounter |

### 30.3 Default Text Modules (Seed Data)

Pre-install common templates:

| Shortcut | Content |
|----------|---------|
| `#normalpsy` | "Psychopathologischer Befund: Bewusstsein klar, allseits orientiert..." |
| `#normalneuro` | "Neurologischer Status: Hirnnerven o.p.B., Kraft allseits 5/5..." |
| `#depression` | "Stimmung deutlich gedrückt, Antrieb vermindert, Ein-/Durchschlafstörungen..." |
| `#rezidiv` | "Rezidiv einer depressiven Episode, aktuell mittelgradige Ausprägung..." |
| `#arztbrief_einleitung` | "Sehr geehrte Kollegin, sehr geehrter Kollege,\n\nwir berichten über..." |
| `#arztbrief_schluss` | "Für Rückfragen stehen wir jederzeit gerne zur Verfügung.\n\nMit freundlichen kollegialen Grüßen" |

### 30.4 Blazor UI

#### Text Module Management

```
MediPrax.Server/Components/Pages/Verwaltung/Textbausteine.razor
```

- CRUD for text modules
- Category filter
- Preview with variable resolution
- Import/export

#### Integration in Text Fields

All multi-line text inputs (`InputTextArea`) in encounters, Arztbriefe, and forms should:
- Listen for `#` prefix
- Show autocomplete dropdown matching text modules by shortcut
- Insert resolved content on selection

### 30.5 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/TextModule.cs` |
| Create | `MediPrax.Application/DTOs/TextModuleDto.cs` |
| Create | `MediPrax.Application/Interfaces/ITextModuleService.cs` |
| Create | `MediPrax.Application/Services/TextModuleService.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/TextModuleConfiguration.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/MediPraxDbContext.cs` |
| Create | `MediPrax.Server/Components/Pages/Verwaltung/Textbausteine.razor` |
| Create | `MediPrax.Server/Components/Shared/TextModuleAutocomplete.razor` — Reusable component |
| Modify | Multiple Razor pages — Integrate autocomplete in text fields |
| Modify | `MediPrax.Server/Program.cs` |
| Create | `tests/MediPrax.IntegrationTests/TextModuleServiceTests.cs` |

---

## Milestone 31: DMP Depression

> **Priority:** P3 | **Estimated Effort:** Medium
> **Why:** Disease Management Programme for depression — required for participating practices. Structured quarterly documentation with data transmission to KV.

### 31.1 Data Model

#### Entity: `DmpEnrollment`

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `PatientId` | `Guid` | FK → Patient |
| `DmpType` | `string` | "Depression" |
| `EnrollmentDate` | `DateOnly` | Date enrolled |
| `DisenrollmentDate` | `DateOnly?` | If unenrolled |
| `Status` | `DmpStatus` | Active, Paused, Disenrolled |

#### Entity: `DmpDocumentation`

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `DmpEnrollmentId` | `Guid` | FK |
| `DocumentationDate` | `DateOnly` | |
| `Quarter` | `string` | "2026-Q2" |
| `FormData` | `Dictionary<string, string>` | JSONB |
| `Submitted` | `bool` | Transmitted to KV |
| `SubmittedAt` | `DateTime?` | |

### 31.2 DMP Depression Documentation Fields

Per G-BA specification:
- Current PHQ-9 score
- Current medication
- Psychotherapy status
- Comorbidities
- Hospitalization since last documentation
- Schulung (patient education) offered
- Treatment goals assessment
- Referral to specialist if needed

### 31.3 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/DmpEnrollment.cs` |
| Create | `MediPrax.Core/Entities/DmpDocumentation.cs` |
| Create | `MediPrax.Application/DTOs/DmpDto.cs` |
| Create | `MediPrax.Application/Services/DmpService.cs` |
| Create | `MediPrax.Server/Components/Pages/Dmp/DmpUebersicht.razor` |
| Create | `MediPrax.Server/Components/Pages/Dmp/DmpDokumentation.razor` |
| + EF config, DbContext, DI, tests |

---

## Milestone 32: Involuntary Commitment & Legal Documentation

> **Priority:** P3 | **Estimated Effort:** Small
> **Why:** Niche but critical when needed. Psychiatric practices must document PsychKG-related decisions.

### 32.1 Templates

#### Ärztliches Zeugnis (PsychKG)

```
MediPrax.Reporting/Formulare/PsychKGZeugnisDocument.cs
```

| Field | Description |
|-------|-------------|
| Patient data | Auto-populated |
| Untersuchungsdatum/-zeit | Date and time of examination |
| Psychopathologischer Befund | From AMDP (Milestone 17) |
| Eigengefährdung | Self-endangerment details |
| Fremdgefährdung | Danger to others details |
| Diagnose | ICD-10 |
| Empfehlung | Recommendation (voluntary, involuntary) |
| Unterbringungsdauer | Suggested duration |

#### Betreuungsanregung

Template for suggestion to the Betreuungsgericht (guardianship court):
- Patient information
- Clinical findings
- Areas requiring guardianship (Aufgabenkreise)
- Urgency assessment

### 32.2 Legal Status Tracking

Add to `Patient`:
```csharp
public bool HasLegalGuardian { get; set; }
public string? GuardianName { get; set; }
public string? GuardianContact { get; set; }
public string? GuardianScope { get; set; } // Aufgabenkreise
```

### 32.3 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Reporting/Formulare/PsychKGZeugnisDocument.cs` |
| Create | `MediPrax.Reporting/Formulare/BetreuungsanregungDocument.cs` |
| Modify | `MediPrax.Core/Entities/Patient.cs` — Add guardian fields |
| Create | `MediPrax.Server/Components/Pages/Formulare/PsychKGZeugnis.razor` |
| + endpoints in Program.cs |

---

## Milestone 33: Private Billing (GOÄ Invoicing)

> **Priority:** P3 | **Estimated Effort:** Medium
> **Why:** PKV patients require proper invoices per GOÄ §12. Payment tracking and Mahnwesen are essential for revenue management.

### 33.1 Data Model

#### Entity: `PrivateInvoice`

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `PatientId` | `Guid` | FK |
| `InvoiceNumber` | `string` | Sequential: "RE-2026-0001" |
| `InvoiceDate` | `DateOnly` | |
| `DueDate` | `DateOnly` | +30 days default |
| `Items` | `List<InvoiceItem>` | JSONB |
| `TotalNet` | `decimal` | Total before tax |
| `TotalGross` | `decimal` | Total (no VAT for medical services) |
| `Status` | `InvoiceStatus` | Draft, Sent, PartiallyPaid, Paid, Overdue, Cancelled |
| `PaidAmount` | `decimal` | Amount received |
| `PaidDate` | `DateOnly?` | Last payment date |
| `PdfData` | `byte[]?` | Generated invoice PDF |
| `ReminderCount` | `int` | Number of reminders sent |
| `LastReminderDate` | `DateOnly?` | |

#### Value Object: `InvoiceItem`

| Property | Type |
|----------|------|
| `GoaNumber` | `string` (GOÄ number) |
| `Description` | `string` |
| `Date` | `DateOnly` |
| `Factor` | `decimal` (Steigerungsfaktor: 1.0–3.5) |
| `BaseAmount` | `decimal` |
| `TotalAmount` | `decimal` (Base × Factor) |
| `Justification` | `string?` (required if Factor > 2.3) |

### 33.2 Invoice PDF Generation

```
MediPrax.Reporting/Rechnung/PrivateInvoiceDocument.cs
```

Required per GOÄ §12:
- Doctor name, address, tax ID
- Patient name, address
- Invoice number, date
- Per item: date, GOÄ number, description, factor, amount
- Total
- Payment terms
- Bank details

### 33.3 Mahnwesen (Payment Reminders)

- Automated overdue detection (daily check)
- 1st reminder after 30 days
- 2nd reminder after 45 days
- 3rd reminder (Mahnung) after 60 days with late fee notice
- Dashboard widget: "Offene Rechnungen: X€"

### 33.4 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/PrivateInvoice.cs` |
| Create | `MediPrax.Core/Enums/InvoiceStatus.cs` |
| Create | `MediPrax.Application/DTOs/PrivateInvoiceDto.cs` |
| Create | `MediPrax.Application/Services/PrivateInvoiceService.cs` |
| Create | `MediPrax.Reporting/Rechnung/PrivateInvoiceDocument.cs` |
| Create | `MediPrax.Server/Components/Pages/Abrechnung/Privatrechnungen.razor` |
| Create | `MediPrax.Server/Components/Pages/Abrechnung/RechnungFormular.razor` |
| + EF config, DbContext, DI, tests, endpoints |

---

## Milestone 34: eAU Electronic Transmission

> **Priority:** P3 | **Estimated Effort:** Small
> **Why:** eAU (elektronische Arbeitsunfähigkeitsbescheinigung) electronic transmission is mandatory. Currently we only generate PDFs.

### 34.1 Enhancement

Extend existing AU functionality:

1. **After PDF generation,** transmit eAU data via TI (KIM) to the patient's health insurance
2. **Track transmission status:** Draft → Transmitted → Confirmed
3. **Erst-AU vs. Folge-AU** tracking per patient
4. **AU history** per patient with date ranges

### 34.2 TI Integration

```
MediPrax.Application/Interfaces/Telematik/IEauService.cs
MediPrax.Server/Services/Telematik/MockEauService.cs
```

Methods:
- `TransmitAsync(EauData data)` → eAU submission to KK
- `GetStatusAsync(string eauId)` → Check transmission status
- `CancelAsync(string eauId)` → Cancel a transmitted eAU

### 34.3 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Application/Interfaces/Telematik/IEauService.cs` |
| Create | `MediPrax.Application/DTOs/EauDto.cs` |
| Create | `MediPrax.Server/Services/Telematik/MockEauService.cs` |
| Modify | `MediPrax.Server/Components/Pages/Formulare/AuFormular.razor` — Add transmit button |
| Modify | `MediPrax.Server/Program.cs` — Register service |

---

## Milestone 35: BtM Management

> **Priority:** P3 | **Estimated Effort:** Medium
> **Why:** Controlled substances (Methylphenidat for ADHD, Benzodiazepines, medical Cannabis) require strict documentation per BtMVV.

### 35.1 Data Model

#### Entity: `BtmPrescription`

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `PatientId` | `Guid` | FK |
| `PrescribedById` | `Guid` | FK |
| `PrescriptionDate` | `DateOnly` | |
| `MedicationName` | `string` | |
| `Pzn` | `string?` | |
| `Substance` | `string` | Controlled substance name |
| `Amount` | `string` | Prescribed amount |
| `Dosierung` | `string` | Dosage instruction |
| `BtmRecipeNumber` | `string` | BtM recipe number (7-digit) |
| `PrescriberBtmNumber` | `string` | Doctor's BtM number |
| `IsEBtm` | `bool` | Electronic BtM prescription |
| `Notes` | `string?` | |

### 35.2 Höchstmengen Enforcement

```
MediPrax.Application/Services/BtmComplianceService.cs
```

Per BtMVV, check 30-day maximum quantities:

| Substance | Max per 30 days |
|-----------|----------------|
| Methylphenidat | 2640 mg |
| Dexamfetamin | 1320 mg |
| Diazepam | Varies per indication |
| Lorazepam | Varies |
| Cannabis (Blüten) | 100 g |

- Warn if prescribing would exceed limits
- Cross-patient BtM overview for compliance auditing
- 3-year retention of BtM Part III documentation

### 35.3 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/BtmPrescription.cs` |
| Create | `MediPrax.Application/Services/BtmComplianceService.cs` |
| Create | `MediPrax.Application/DTOs/BtmPrescriptionDto.cs` |
| Create | `MediPrax.Application/Catalogs/BtmSubstanceCatalog.cs` |
| Create | `MediPrax.Server/Components/Pages/Medikation/BtmVerwaltung.razor` |
| + EF config, DbContext, DI, tests |

---

## Milestone 36: Automated Mahnwesen (Payment Reminders)

> **Priority:** P3 | **Estimated Effort:** Small-Medium
> **Why:** Overdue private invoices require systematic follow-up. Manual tracking wastes MFA time and loses revenue.

### 36.1 Features

- **Automated overdue detection:** Daily background check for invoices past DueDate
- **3-stage reminder workflow:**
  - 1. Zahlungserinnerung (friendly reminder) after 14 days overdue
  - 2. Erste Mahnung (first formal reminder) after 30 days overdue
  - 3. Zweite Mahnung (with Mahngebuehr notice) after 45 days overdue
- **Reminder PDF generation:** Each stage generates a formal letter (QuestPDF)
- **Dashboard widget:** "Offene Rechnungen: X EUR, davon Y ueberfaellig"
- **Bulk actions:** Send all pending reminders at once
- **Tracking:** ReminderCount, LastReminderDate on PrivateInvoice (already exists)

### 36.2 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Server/Services/MahnwesenBackgroundService.cs` (daily check) |
| Create | `MediPrax.Reporting/Rechnung/MahnungDocument.cs` (PDF for each stage) |
| Modify | `MediPrax.Application/Services/PrivateInvoiceService.cs` (reminder logic) |
| Modify | `MediPrax.Server/Components/Pages/Abrechnung/Privatrechnungen.razor` (bulk actions) |
| Modify | `MediPrax.Server/Components/Pages/Home.razor` (dashboard widget) |

---

## Milestone 37: DATEV-Export Interface

> **Priority:** P3 | **Estimated Effort:** Small
> **Why:** Most practices use a Steuerberater who works with DATEV. Automated export saves manual bookkeeping and reduces errors.

### 37.1 Features

- **DATEV-Format Export:** Generate DATEV-compatible CSV (Buchungsstapel format)
- **Monthly/quarterly export:** Select period, generate file
- **Mapping:** GOP-Einnahmen, Privatrechnungen, Zahlungseingaenge mapped to DATEV Kontenrahmen SKR03/SKR04
- **Categories:** Separate accounts for GKV-Einnahmen, PKV-Einnahmen, Mahngebuehren
- **Download:** CSV file ready for Steuerberater import

### 37.2 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Application/Services/DatevExportService.cs` |
| Create | `MediPrax.Application/DTOs/DatevExportDto.cs` |
| Create | `MediPrax.Server/Components/Pages/Verwaltung/DatevExport.razor` |
| Modify | `MediPrax.Server/Program.cs` (export endpoint) |

---

## Milestone 38: Umsatzstatistik (Revenue Analytics)

> **Priority:** P3 | **Estimated Effort:** Medium
> **Why:** Practice owners need financial overview: revenue by period, by doctor, by insurance type. Essential for business decisions and KV-Abrechnung verification.

### 38.1 Features

- **Dashboard page** with charts and KPIs:
  - Umsatz pro Quartal (bar chart)
  - Umsatz pro Arzt (pie chart)
  - GKV vs. PKV Verteilung
  - Top-10 GOPs by frequency and revenue
  - Offene Privatrechnungen total
  - Vergleich zum Vorquartal
- **Filters:** Period (Quartal/Monat/Jahr), Doctor, Insurance type
- **Export:** CSV download of all data
- **Praxis-Kennzahlen:** Fallzahl, Scheinzahl, Fallwert, Punktzahlvolumen

### 38.2 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Application/Services/RevenueStatisticsService.cs` |
| Create | `MediPrax.Application/DTOs/RevenueStatisticsDto.cs` |
| Create | `MediPrax.Server/Components/Pages/Berichte/Umsatzstatistik.razor` |
| Modify | `MediPrax.Server/Components/Pages/Home.razor` (summary KPIs) |

---

## Milestone 39: Arztbrief Therapiebericht Vorlage

> **Priority:** P2 | **Estimated Effort:** Small
> **Why:** Therapy progress reports are the most common Arztbrief type in psychiatric practices. Auto-populating from the active TherapyCase saves significant documentation time.

### 39.1 Features

- **New Vorlage "Therapiebericht"** in ArztbriefFormular
- Detects active TherapyCase for the patient, auto-includes:
  - Therapieverfahren (VT, TP, etc.)
  - Status (Bewilligt, In Behandlung, etc.)
  - Sitzungszahl: "Bisher X von Y bewilligten Sitzungen durchgefuehrt"
  - Startdatum der Therapie
  - Diagnosen des Therapiefalls
  - Kassengenehmigung (Aktenzeichen)
- Falls back to standard Verlaufsbericht if no active TherapyCase
- Combined with existing Encounter data (medication, Befund)

### 39.2 Files to Modify

| Action | File |
|--------|------|
| Modify | `MediPrax.Server/Components/Pages/Dokumente/ArztbriefFormular.razor` (new Vorlage button + template logic) |
| Modify | `MediPrax.Application/Interfaces/ITherapyCaseService.cs` (GetActiveByPatientAsync) |
| Modify | `MediPrax.Application/Services/TherapyCaseService.cs` |

---

## Milestone 40: Sprechzeiten & Verfuegbarkeit

> **Priority:** P1 | **Estimated Effort:** Medium
> **Why:** Without doctor availability data, the system cannot suggest free slots, warn about conflicts, or visualize available time. Foundation for intelligent scheduling.

### 40.1 Data Model

#### Entity: `DoctorScheduleTemplate`

Weekly schedule template per doctor (Wochenvorlage):

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `DoctorId` | `Guid` | FK -> User |
| `DayOfWeek` | `DayOfWeek` | Monday-Saturday |
| `StartTime` | `TimeOnly` | Block start (e.g., 08:00) |
| `EndTime` | `TimeOnly` | Block end (e.g., 12:30) |
| `SlotDurationMinutes` | `int` | Default slot size (25 or 50) |
| `IsActive` | `bool` | Enable/disable this block |

Example: Dr. Meier has 2 blocks on Monday: 08:00-12:30 and 14:00-17:00.

#### Entity: `DoctorAbsence`

Planned absences (Urlaub, Fortbildung, Krank, Sperrzeiten):

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `DoctorId` | `Guid` | FK -> User |
| `StartDate` | `DateOnly` | First day |
| `EndDate` | `DateOnly` | Last day (inclusive) |
| `StartTime` | `TimeOnly?` | Null = full day, set = partial (Sperrzeit) |
| `EndTime` | `TimeOnly?` | Null = full day |
| `AbsenceType` | `AbsenceType` | Enum: Urlaub, Fortbildung, Krank, Sperrzeit |
| `Reason` | `string?` | Optional description |
| `SubstituteId` | `Guid?` | FK -> User (Vertretung) |

#### Enum: `AbsenceType`

Urlaub, Fortbildung, Krank, Sperrzeit

### 40.2 Availability Service

```
MediPrax.Application/Services/AvailabilityService.cs
```

Methods:
- `GetScheduleTemplateAsync(Guid doctorId)` -> weekly blocks
- `GetAbsencesAsync(Guid doctorId, DateOnly from, DateOnly to)` -> absences in range
- `GetFreeSlotsAsync(Guid doctorId, DateOnly date, int durationMinutes)` -> available time slots for a given day
- `IsAvailableAsync(Guid doctorId, DateTime start, int durationMinutes)` -> check if specific slot is free
- `GetAvailabilityOverviewAsync(DateOnly from, DateOnly to)` -> all doctors, for calendar overlay

Free slot calculation: `Sprechzeiten - Absences - Existing Appointments = Free Slots`

### 40.3 Calendar Visualization

- **Sprechzeiten:** Light background in timeline (white = available, grey = outside Sprechzeiten)
- **Urlaub:** Hatched/striped overlay with label "Urlaub" across the day
- **Sperrzeit:** Red-striped block with reason text
- **Fortbildung:** Blue-striped block

### 40.4 Administration UI

```
MediPrax.Server/Components/Pages/Verwaltung/Sprechzeiten.razor
```

- Per-doctor weekly schedule editor (drag blocks or form)
- Absence calendar: create/edit Urlaub, Fortbildung, Sperrzeiten
- Overview: all doctors' availability side by side

### 40.5 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/DoctorScheduleTemplate.cs` |
| Create | `MediPrax.Core/Entities/DoctorAbsence.cs` |
| Create | `MediPrax.Core/Enums/AbsenceType.cs` |
| Create | `MediPrax.Application/Interfaces/IAvailabilityService.cs` |
| Create | `MediPrax.Application/Services/AvailabilityService.cs` |
| Create | `MediPrax.Application/DTOs/AvailabilityDto.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/DoctorScheduleTemplateConfiguration.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/DoctorAbsenceConfiguration.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/MediPraxDbContext.cs` |
| Create | `MediPrax.Server/Components/Pages/Verwaltung/Sprechzeiten.razor` |
| Modify | `MediPrax.Server/Components/Pages/Termine/Wochenansicht.razor` (availability overlay) |
| Modify | `MediPrax.Server/Program.cs` |
| Create | `tests/MediPrax.UnitTests/AvailabilityServiceTests.cs` |
| Create | `tests/MediPrax.IntegrationTests/AvailabilityServiceTests.cs` |

---

## Milestone 41: Intelligente Terminvergabe

> **Priority:** P1 | **Estimated Effort:** Medium
> **Depends on:** M40 (Sprechzeiten & Verfuegbarkeit)
> **Why:** Automated slot finding saves MFA time, prevents double-bookings, and ensures therapy sessions are scheduled within available hours.

### 41.1 Slot Suggestion Engine

```
MediPrax.Application/Services/SlotSuggestionService.cs
```

#### Method: `SuggestNextSlotAsync`

Input: DoctorId, DurationMinutes, PreferredDayOfWeek?, PreferredTime?, SearchFromDate
Output: List of up to 5 suggested free slots

Logic:
1. Load doctor's Sprechzeiten template
2. Load absences in search range (next 8 weeks)
3. Load existing appointments in search range
4. For each day in range:
   a. Skip if outside Sprechzeiten
   b. Skip if Urlaub/Krank
   c. Find gaps >= requested duration
   d. Prefer same day/time as last appointment (for therapy continuity)
5. Return top 5 suggestions sorted by preference

#### Method: `SuggestNextTherapySessionAsync`

Input: TherapyCaseId
Output: Suggested slot based on:
- Same therapist
- Same day of week as last session
- Same time as last session
- Duration from TherapyCase.SessionDurationMinutes
- Warning if approaching session limit

### 41.2 UI Integration

#### In Wochenansicht (Calendar):
- Click on empty slot -> "Termin hier erstellen" with pre-filled date/time
- Visual indicator for suggested slots (pulsing border or highlight)

#### In TherapieFallDetail:
- Button "Naechste Sitzung planen"
- Shows 3-5 suggested slots
- One-click to create appointment from suggestion

#### In TerminFormular:
- "Naechsten freien Slot suchen" button
- Shows suggestions based on selected doctor + duration
- Warning if selected time conflicts with Sprechzeiten or Urlaub

### 41.3 Conflict Detection

When creating/editing any appointment:
- **Error:** Doctor is on Urlaub/Krank -> block creation
- **Warning:** Outside Sprechzeiten -> allow with confirmation
- **Warning:** Overlapping with existing appointment -> allow with confirmation
- **Info:** Approaching therapy session limit -> informational

### 41.4 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Application/Interfaces/ISlotSuggestionService.cs` |
| Create | `MediPrax.Application/Services/SlotSuggestionService.cs` |
| Create | `MediPrax.Application/DTOs/SlotSuggestionDto.cs` |
| Modify | `MediPrax.Server/Components/Pages/Termine/TerminFormular.razor` (slot suggestions + conflict warnings) |
| Modify | `MediPrax.Server/Components/Pages/Therapie/TherapieFallDetail.razor` (next session button) |
| Modify | `MediPrax.Server/Components/Pages/Termine/Wochenansicht.razor` (click-to-create on empty slot) |
| Modify | `MediPrax.Server/Program.cs` |
| Create | `tests/MediPrax.UnitTests/SlotSuggestionServiceTests.cs` |
| Create | `tests/MediPrax.IntegrationTests/SlotSuggestionServiceTests.cs` |

---

## Cross-Cutting Concerns

### Navigation Updates

Add new navigation items to `MainLayout.razor`:

```
├── Startseite
├── Patienten
├── Termine
│   ├── Wochenansicht
│   ├── Wartezimmer
│   └── Warteliste (M25)
├── Dokumentation
│   ├── Übersicht
│   ├── Psychopath. Befund (M17)
│   ├── Neurologischer Befund (M20)
│   ├── Suizidalität (M27)
│   └── Krankheitsspezifisch (M26)
│       ├── Anfallskalender
│       ├── Kopfschmerz-Tagebuch
│       ├── MS-Dokumentation
│       └── Parkinson
├── Testung (M18)
├── Therapie (M19)
│   ├── Therapiefälle
│   ├── PTV-Formulare
│   └── Kontingente
├── Medikation
│   ├── Medikationsplan
│   ├── Depot-Übersicht (M21)
│   ├── Interaktionen (M21)
│   └── BtM-Verwaltung (M35)
├── Labor (M22)
├── Geräte (M28)
├── Formulare
│   ├── Rezept
│   ├── AU
│   ├── Überweisung
│   ├── Krankenhauseinweisung (M24)
│   ├── Heilmittelverordnung (M24)
│   ├── Soziotherapie (M24)
│   └── PsychKG-Zeugnis (M32)
├── Dokumente
├── Abrechnung
│   ├── Übersicht
│   ├── Quartalsvalidierung (M23)
│   └── Privatrechnungen (M33)
├── DMP (M31)
├── Berichte
├── Telematik
├── Verwaltung
│   ├── Benutzer
│   ├── Textbausteine (M30)
│   ├── Geräte-Konfiguration (M28)
│   ├── Audit-Log
│   └── Datenimport
└── Konto
```

### Database Migrations

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
```

### Test Coverage Requirements

Each milestone must include:
- **Unit tests:** Catalog searches, business rules (contingents, plausibility, interactions)
- **Integration tests:** Service CRUD operations with test database
- Maintain existing test patterns (xUnit, TestDbContextFactory, IDisposable)

### Arztbrief Integration

The Arztbrief generator (`ArztbriefDocument.cs`) should be extended to optionally include:
- Psychopathological finding narrative (M17)
- Psychometric test results summary (M18)
- Neurological examination narrative (M20)
- Active medication plan with BMP format (M21)
- Lab results summary (M22)

---

## Dependency Graph

```
M17 (AMDP) ──────────────────────────────── standalone
M18 (Psychometric Tests) ───────────────── standalone
M19 (PTV / Therapy Cases) ──────────────── standalone
M20 (Neuro Exam) ───────────────────────── standalone

M21 (Advanced Medication) ──────────────── standalone (enhances existing)
M22 (Lab Integration) ──── depends on ──── M21 (monitoring alerts reference medication)
M23 (Billing) ──────────── depends on ──── M19 (therapy contingent billing linkage)

M24 (KBV Forms) ────────────────────────── standalone
M25 (Appointments) ─────── depends on ──── M19 (session tracking links to TherapyCase)
M26 (Disease Docs) ─────────────────────── standalone
M27 (Suicidality) ──────── depends on ──── M17 (references AMDP suicidality section)

M28 (GDT) ──────────────────────────────── standalone
M29 (Video) ─────────────── depends on ──── M25 (appointment enhancements)
M30 (Text Modules) ─────── depends on ──── M17, M20 (default templates use findings)

M31 (DMP) ───────────────── depends on ──── M18 (PHQ-9 integration)
M32 (PsychKG) ──────────── depends on ──── M17 (uses AMDP findings)
M33 (GOÄ Invoicing) ────────────────────── standalone
M34 (eAU) ──────────────────────────────── standalone (enhances existing)
M35 (BtM) ──────────────── depends on ──── M21 (medication category system)
```

### Recommended Implementation Order

```
Phase 1 (P0 — Parallel):     M17 + M18 + M19 + M20
Phase 2 (P1 — Sequential):   M21 → M22 → M23, M24 (parallel with M23)
Phase 3 (P2 — Mixed):        M25 → M26 + M27 (parallel)  → M28
Phase 4 (P3 — As needed):    M30, M29, M31, M32, M33, M34, M35
```

---

## Timeline Estimate

| Phase | Milestones | Estimated Size | Notes |
|-------|-----------|----------------|-------|
| **Phase 1** | M17, M18, M19, M20 | 4 large milestones | Core psychiatry + neurology. Start all 4 in parallel. |
| **Phase 2** | M21, M22, M23, M24 | 3 large + 1 medium | Patient safety + billing + forms. Sequential due to dependencies. |
| **Phase 3** | M25, M26, M27, M28 | 2 medium + 1 small + 1 large | Workflow + specialty docs + devices. |
| **Phase 4** | M29–M35 | 7 small–medium | Prioritize based on customer demand. |

---

## Summary

This plan adds **19 milestones** (M17–M35) covering:

- **4 P0 features:** AMDP psychopathology, psychometric testing, PTV/therapy management, neurological examination
- **4 P1 features:** Advanced medication, lab integration, billing plausibility, KBV forms
- **4 P2 features:** Appointment enhancements, disease documentation, suicidality, GDT devices
- **7 P3 features:** Video, text modules, DMP, PsychKG, GOÄ invoicing, eAU, BtM

Combined with the existing 16 milestones, this brings mediprax to full competitive parity with ELEFANT, psyprax, EPIKUR, and tomedo for psychiatric and neurological practices in Germany.

**New entities:** ~20 | **New services:** ~20 | **New Blazor pages:** ~25 | **New PDF documents:** ~10 | **New catalogs:** ~8
