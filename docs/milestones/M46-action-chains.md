# M46: Action Chains (Aktionsketten / Behandlungskomplexe) -- AUSSTEHEND

> **Priority:** P1 | **Estimated Effort:** Large
> **Why:** Aktionsketten are the #1 efficiency feature in mature PVS systems like tomedo and CGM M1. A single shortcut triggers an entire treatment chain: diagnosis + billing codes + notes template + prescription. For a practice with 5-10 minute consultations, this is the difference between the system being a burden and a productivity multiplier.

---

## 46.1 Problem Statement

Currently in MediPrax, each step of a consultation is manual and independent:
1. Search and add ICD-10 diagnosis
2. Search and add GOP billing code
3. Select a notes template or type free text
4. Create a prescription separately

In tomedo, a single shortcut like `#dep` executes all of these simultaneously. For a high-volume psychiatric practice seeing 40-60 patients/day, this automation saves 2-3 minutes per patient, adding up to 1-2 hours/day.

## 46.2 Data Model

### New Entity: `ActionChain`

```
MediPrax.Core/Entities/ActionChain.cs
```

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK (BaseEntity) |
| `Shortcut` | `string` | Trigger (e.g., "#dep", "#epilepsie", "#erstgespräch") |
| `Title` | `string` | Display name ("Depression Erstgespräch") |
| `Description` | `string?` | What this chain does |
| `Category` | `string` | "Psychiatrie", "Neurologie", "Allgemein" |
| `CreatedById` | `Guid` | FK -> User |
| `IsGlobal` | `bool` | Available to all users vs. personal |
| `SortOrder` | `int` | Display order in lists |
| `IsActive` | `bool` | Can be deactivated without deletion |

### New Entity: `ActionChainStep`

```
MediPrax.Core/Entities/ActionChainStep.cs
```

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK (BaseEntity) |
| `ActionChainId` | `Guid` | FK -> ActionChain |
| `StepType` | `ActionStepType` | What kind of action |
| `SortOrder` | `int` | Execution order |
| `Configuration` | `string` | JSON payload (type-specific) |

### New Enum: `ActionStepType`

```csharp
public enum ActionStepType
{
    AddDiagnosis,       // Add ICD-10 code with metadata
    AddBillingCode,     // Add GOP (EBM/GOÄ)
    SetNoteTemplate,    // Pre-fill encounter section(s)
    CreatePrescription, // Create a draft prescription
    SetEncounterType,   // Set the encounter type
    SetDuration,        // Pre-fill duration
    CreateReferral,     // Create a draft Ueberweisung
    CreateRecall        // Schedule a Wiedervorlage
}
```

### Configuration JSON Examples

```json
// AddDiagnosis
{
  "icd10Code": "F32.1",
  "certainty": "G",
  "diagnosisType": "Encounterdiagnose"
}

// AddBillingCode
{
  "gopCode": "21220",
  "quantity": 1
}

// SetNoteTemplate
{
  "sectionType": "Befund",
  "content": "Stimmung deutlich gedrueckt, Antrieb vermindert..."
}

// CreatePrescription
{
  "medicationName": "Sertralin",
  "dosage": "50mg 1-0-0",
  "medicationPzn": "00955092"
}

// CreateRecall
{
  "intervalWeeks": 4,
  "reason": "Therapiekontrolle"
}
```

## 46.3 Service Layer

### `IActionChainService`

```
MediPrax.Application/Interfaces/IActionChainService.cs
```

```csharp
public interface IActionChainService
{
    Task<List<ActionChainDto>> GetAllAsync(CancellationToken ct = default);
    Task<List<ActionChainDto>> SearchAsync(string term, CancellationToken ct = default);
    Task<ActionChainDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ActionChainDto?> GetByShortcutAsync(string shortcut, CancellationToken ct = default);
    Task<ActionChainDto> CreateAsync(CreateActionChainDto dto, CancellationToken ct = default);
    Task<ActionChainDto> UpdateAsync(UpdateActionChainDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
```

### `IActionChainExecutor`

```
MediPrax.Application/Interfaces/IActionChainExecutor.cs
```

```csharp
public interface IActionChainExecutor
{
    Task<ActionChainResult> ExecuteAsync(Guid actionChainId, Guid encounterId, CancellationToken ct = default);
}
```

```csharp
public record ActionChainResult(
    List<string> AddedDiagnoses,
    List<string> AddedGops,
    List<string> CreatedPrescriptions,
    List<string> Warnings,  // e.g., "GOP 21220 already exists in this encounter"
    bool Success
);
```

The executor processes steps in order, calling existing services:
- `PatientDiagnosisService.CreateAsync()` for diagnoses (depends on M44)
- `BillingService.AddAsync()` for GOPs
- `EncounterSectionService.UpdateAsync()` for note templates (depends on M43)
- `PrescriptionService.CreateAsync()` for prescriptions
- `RecallService.CreateAsync()` for follow-ups

## 46.4 Default Action Chains (Seed Data)

Pre-install common chains for Psychiatrie/Neurologie:

| Shortcut | Title | Steps |
|----------|-------|-------|
| `#dep` | Depression Standard | F32.1(G) + 21220 + depressive template + Sertralin draft |
| `#angst` | Angststoerung | F41.1(G) + 21220 + anxiety template |
| `#epilepsie` | Epilepsie Kontrolle | G40.9(G) + 16220 + neuro template + 16311(EEG) |
| `#erstgespräch` | Psychiatrisches Erstgespräch | 21210(Grundpauschale) + 21220 + full psych template |
| `#schmerz` | Chronischer Schmerz | R52.2(G) + 16220 + pain template |
| `#demenz` | Demenz Kontrolle | F00.1(G) + 16220 + cognitive template + MMSE recall |
| `#parkinson` | Parkinson Kontrolle | G20(G) + 16220 + neuro template |
| `#ms` | Multiple Sklerose | G35(G) + 16220 + neuro template |

## 46.5 Blazor UI

### New: Action Chain Management

```
MediPrax.Server/Components/Pages/Verwaltung/Aktionsketten.razor
```

Route: `/verwaltung/aktionsketten`

- List all action chains with category filter
- Visual step builder (drag-and-drop or add/remove)
- Preview: shows what the chain will do
- Test mode: dry-run without saving

### Integration in EncounterFormular.razor

1. **Shortcut trigger:** When user types `#` in any text field, show autocomplete dropdown of matching action chains
2. **Toolbar button:** "Aktionskette ausfuehren" button with search/select
3. **Execution feedback:** After execution, show a summary toast: "Diagnose F32.1 hinzugefuegt, GOP 21220 erfasst, Befund-Vorlage eingefuegt"

### Keyboard shortcut

- `Ctrl+K` -> open action chain quick-search (like a command palette)

## 46.6 Dependencies

- **M43 (Structured Encounter):** For `SetNoteTemplate` step type to target specific sections
- **M44 (Diagnosis Management):** For `AddDiagnosis` step type with metadata
- **M30 (Textbausteine):** Can be used within SetNoteTemplate steps

If M43/M44 are not yet implemented, the executor falls back:
- `SetNoteTemplate` appends to Notes field
- `AddDiagnosis` adds to Icd10Codes list (no metadata)

## 46.7 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/ActionChain.cs` |
| Create | `MediPrax.Core/Entities/ActionChainStep.cs` |
| Create | `MediPrax.Core/Enums/ActionStepType.cs` |
| Create | `MediPrax.Application/DTOs/ActionChainDto.cs` |
| Create | `MediPrax.Application/DTOs/ActionChainStepDto.cs` |
| Create | `MediPrax.Application/Interfaces/IActionChainService.cs` |
| Create | `MediPrax.Application/Interfaces/IActionChainExecutor.cs` |
| Create | `MediPrax.Application/Services/ActionChainService.cs` |
| Create | `MediPrax.Application/Services/ActionChainExecutor.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/ActionChainConfiguration.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/ActionChainStepConfiguration.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/MediPraxDbContext.cs` |
| Create | `MediPrax.Server/Components/Pages/Verwaltung/Aktionsketten.razor` |
| Modify | `MediPrax.Server/Components/Pages/Dokumentation/EncounterFormular.razor` -- chain trigger |
| Modify | `MediPrax.Server/Program.cs` |
| Create | EF Core migration |
| Create | `tests/MediPrax.IntegrationTests/ActionChainExecutorTests.cs` |
| Create | `tests/MediPrax.UnitTests/ActionChainServiceTests.cs` |

## 46.8 Acceptance Criteria

- [ ] ActionChain entity with configurable steps
- [ ] Executor processes all step types (diagnoses, GOPs, templates, prescriptions, recalls)
- [ ] `#`-triggered autocomplete in encounter form
- [ ] Ctrl+K command palette for chain search
- [ ] Execution summary shows what was done
- [ ] Pre-installed seed chains for common Psych/Neuro scenarios
- [ ] Admin page for chain management with visual step builder
- [ ] Personal vs. global chains
- [ ] Graceful fallback if M43/M44 not yet implemented

---
