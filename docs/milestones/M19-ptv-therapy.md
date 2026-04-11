# M19: PTV Forms & Therapy Approval Management -- ABGESCHLOSSEN

> **Priority:** P0 | **Estimated Effort:** X-Large
> **Why:** Any practice doing Richtlinien-Psychotherapie (behavioral therapy, depth psychology, analytical, systemic) MUST manage PTV forms and therapy approvals. This is a dealbreaker feature.

#### 19.1 Data Model

##### Entity: `TherapyCase` (Behandlungsfall Psychotherapie)

```
MediPrax.Core/Entities/TherapyCase.cs
```

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `PatientId` | `Guid` | FK -> Patient |
| `TherapistId` | `Guid` | FK -> User |
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
| `Diagnoses` | `List<string>` | JSONB -- ICD-10 codes |
| `Notes` | `string?` | Internal notes |

##### Enum: `TherapyType`

```csharp
public enum TherapyType
{
    PsychotherapeutischeSprechstunde,    // Max 6x25min
    Probatorik,                          // Max 4-6 sessions
    Akutbehandlung,                      // Max 24x25min or 12x50min
    KurzzeittherapieKZT1,               // 12 sessions (25 or 50 min)
    KurzzeittherapieKZT2,               // 12 more sessions
    LangzeittherapieVT,                  // Verhaltenstherapie (up to 60/80)
    LangzeittherapieTP,                  // Tiefenpsychologisch (up to 60/100)
    LangzeittherapieAP,                  // Analytisch (up to 160/300)
    LangzeittherapieST,                  // Systemisch (up to 36/48)
    NeuropsychologischeTherapie          // up to 60/120
}
```

##### Enum: `TherapyCaseStatus`

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

##### Enum: `GutachterStatus`

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

##### Entity: `TherapySession` (links to Appointment/Encounter)

```
MediPrax.Core/Entities/TherapySession.cs
```

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `TherapyCaseId` | `Guid` | FK -> TherapyCase |
| `EncounterId` | `Guid?` | FK -> Encounter |
| `AppointmentId` | `Guid?` | FK -> Appointment |
| `SessionNumber` | `int` | Sequential number within therapy |
| `SessionDate` | `DateOnly` | Date of session |
| `DurationMinutes` | `int` | 25 or 50 |
| `SessionType` | `SessionType` | Enum: Sprechstunde, Probatorik, Regular, Akut |
| `IsVideoSession` | `bool` | Videosprechstunde? |
| `Notes` | `string?` | Brief session notes |
| `BilledGop` | `string?` | Which GOP was billed |

##### Entity: `PtvForm`

```
MediPrax.Core/Entities/PtvForm.cs
```

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `TherapyCaseId` | `Guid` | FK -> TherapyCase |
| `FormType` | `PtvFormType` | Enum: PTV1, PTV2, PTV3, etc. |
| `CreatedDate` | `DateOnly` | Date generated |
| `FormData` | `Dictionary<string, string>` | JSONB -- form field values |
| `PdfData` | `byte[]?` | Generated PDF |
| `Status` | `PtvFormStatus` | Draft, Finalized, Sent |

#### 19.2 Therapy Contingent Rules (Business Logic)

```
MediPrax.Application/Services/TherapyContingentRules.cs
```

Encode KBV therapy contingent rules:

| Therapy Type | Initial Sessions | Extension | Max Total | Gutachter Required |
|-------------|-----------------|-----------|-----------|-------------------|
| Sprechstunde | 6x25min | -- | 6 | No |
| Probatorik (VT/TP/ST) | 4 | -- | 4 | No |
| Probatorik (AP) | 4 | +4 | 8 | No |
| Akutbehandlung | 24x25min | -- | 24 | No |
| KZT1 | 12 | -- | 12 | No |
| KZT2 | 12 | -- | 24 total | No |
| LZT-VT | 60 | -- | 80 | Yes |
| LZT-TP | 60 | -- | 100 | Yes |
| LZT-AP (Einzel) | 160 | -- | 300 | Yes |
| LZT-ST | 36 | -- | 48 | Yes |

**Warnings:**
- `WarningThreshold` at 80% of approved sessions (e.g., session 10 of 12)
- `ExtensionSuggestion` when approaching limit
- `BlockBilling` when sessions exceed approved contingent

#### 19.3 PTV Form Generation (PDF)

```
MediPrax.Reporting/PtvForms/
  Ptv1Document.cs       // Antrag auf Psychotherapie
  Ptv2Document.cs       // Information Therapeut -> KK
  Ptv3Document.cs       // Bericht an Gutachter
  Ptv10Document.cs      // Akutbehandlung
  Ptv11Document.cs      // Probatorische Sitzungen
  Ptv12Document.cs      // Akutbehandlung Anzeige
  PtvFormDataPopulator.cs // Auto-fill from patient + therapy case
```

Each PTV form auto-populates:
- Patient demographics from `Patient` entity
- Insurance data (KK name, number, KVNR)
- Diagnoses from `TherapyCase.Diagnoses`
- Therapist information from `User`
- Therapy type and session counts
- Date fields

#### 19.4 Service Layer

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

#### 19.5 Blazor UI

##### Therapy Case Overview

```
MediPrax.Server/Components/Pages/Therapie/TherapieFaelle.razor
```

- List of all active therapy cases
- Filter by therapist, status, therapy type
- Color-coded session countdown (green > yellow > red)
- Quick actions: Add session, Generate PTV form

##### Therapy Case Detail

```
MediPrax.Server/Components/Pages/Therapie/TherapieFallDetail.razor
```

- **Header:** Patient name, therapy type, status, progress bar (sessions used/approved)
- **Session list:** Numbered sessions with dates and notes
- **PTV forms section:** Generated forms with download links
- **Contingent alert:** Warning banner when approaching limit
- **Actions:** Change status, add session, generate form, request extension

##### PTV Form Editor

```
MediPrax.Server/Components/Pages/Therapie/PtvFormEditor.razor
```

- Form-based editor for PTV data
- Auto-populated fields (editable)
- Preview -> Finalize -> Generate PDF workflow
- Print button

#### 19.6 Files to Create/Modify

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
| Modify | `MediPrax.Server/Components/Layout/MainLayout.razor` -- Add "Therapie" nav |
| Modify | `MediPrax.Server/Program.cs` |
| Create | `tests/MediPrax.IntegrationTests/TherapyCaseServiceTests.cs` |
| Create | `tests/MediPrax.UnitTests/TherapyContingentRulesTests.cs` |

---

