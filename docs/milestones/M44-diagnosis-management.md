# M44: Advanced Diagnosis Management (Dauerdiagnosen, Metadaten, Vererbung) -- AUSSTEHEND

> **Priority:** P1 | **Estimated Effort:** Large
> **Why:** Every certified German PVS maintains a persistent patient diagnosis list with metadata (certainty, laterality, active/historical status). This is required for correct KVDT export, Arztbrief generation, and EBM plausibility checks. Currently MediPrax stores ICD-10 codes as simple `List<string>` per encounter with no metadata and no persistence across encounters.

---

## 44.1 Problem Statement

Current state:
- `Encounter.Icd10Codes` is `List<string>` -- bare code strings without metadata
- Each encounter is independent -- chronic diagnoses must be re-entered every visit
- No distinction between "Dauerdiagnosen" (permanent/chronic) and encounter-specific diagnoses
- No Diagnosesicherheit (V/G/Z/A) as required by KBV for KVDT export
- No Seitenlokalisation (R/L/B) for lateralized diagnoses
- Arztbrief lists codes without certainty qualifiers

## 44.2 Data Model

### New Entity: `PatientDiagnosis`

```
MediPrax.Core/Entities/PatientDiagnosis.cs
```

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK (BaseEntity) |
| `PatientId` | `Guid` | FK -> Patient |
| `Icd10Code` | `string` | ICD-10-GM code (e.g., "F32.1") |
| `Icd10Description` | `string` | Cached description |
| `Certainty` | `DiagnosisCertainty` | G/V/Z/A (KBV-mandatory) |
| `Laterality` | `DiagnosisLaterality?` | R/L/B or null |
| `DiagnosisType` | `DiagnosisType` | Dauerdiagnose / Encounter-specific |
| `Status` | `DiagnosisStatus` | Active / Anamnestisch / Inactive |
| `OnsetDate` | `DateOnly?` | When the diagnosis was first made |
| `ResolvedDate` | `DateOnly?` | When the diagnosis was resolved (if inactive) |
| `Notes` | `string?` | Free text annotation |
| `CreatedByDoctorId` | `Guid` | FK -> User who made the diagnosis |

### New Entity: `EncounterDiagnosis` (join table)

```
MediPrax.Core/Entities/EncounterDiagnosis.cs
```

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK (BaseEntity) |
| `EncounterId` | `Guid` | FK -> Encounter |
| `PatientDiagnosisId` | `Guid` | FK -> PatientDiagnosis |
| `IsNewInThisEncounter` | `bool` | First time this diagnosis was documented |

### New Enums

```
MediPrax.Core/Enums/DiagnosisCertainty.cs
```

```csharp
public enum DiagnosisCertainty
{
    G,  // Gesichert -- confirmed
    V,  // Verdacht -- suspected
    Z,  // Zustand nach -- status post
    A   // Ausschluss -- ruled out
}
```

```csharp
public enum DiagnosisLaterality
{
    R,  // Rechts
    L,  // Links
    B   // Beidseitig
}
```

```csharp
public enum DiagnosisType
{
    Dauerdiagnose,      // Chronic/permanent -- auto-inherited by new encounters
    Encounterdiagnose   // Specific to this encounter only
}
```

```csharp
public enum DiagnosisStatus
{
    Active,         // Currently relevant
    Anamnestisch,   // Historical, still clinically relevant
    Inactive        // Resolved, no longer relevant
}
```

### Modify Entity: `Encounter`

```csharp
// Keep for backward compatibility during migration:
public List<string> Icd10Codes { get; set; } = [];

// New structured relationship:
public ICollection<EncounterDiagnosis> EncounterDiagnoses { get; set; } = [];
```

### Modify Entity: `Patient`

```csharp
public ICollection<PatientDiagnosis> Diagnoses { get; set; } = [];
```

## 44.3 Service Layer

### `IPatientDiagnosisService`

```
MediPrax.Application/Interfaces/IPatientDiagnosisService.cs
```

```csharp
public interface IPatientDiagnosisService
{
    Task<List<PatientDiagnosisDto>> GetByPatientAsync(Guid patientId, bool includeInactive = false, CancellationToken ct = default);
    Task<List<PatientDiagnosisDto>> GetDauerdiagnosenAsync(Guid patientId, CancellationToken ct = default);
    Task<PatientDiagnosisDto> CreateAsync(CreatePatientDiagnosisDto dto, CancellationToken ct = default);
    Task<PatientDiagnosisDto> UpdateAsync(UpdatePatientDiagnosisDto dto, CancellationToken ct = default);
    Task DeactivateAsync(Guid id, CancellationToken ct = default);
    Task<List<PatientDiagnosisDto>> InheritDauerdiagnosenForEncounterAsync(Guid encounterId, CancellationToken ct = default);
    Task MigrateLegacyCodesAsync(Guid patientId, CancellationToken ct = default);
}
```

### Key Method: `InheritDauerdiagnosenForEncounterAsync`

When a new encounter is created, automatically link all active Dauerdiagnosen to it via EncounterDiagnosis records. The doctor can then modify (add/remove) per encounter.

### Key Method: `MigrateLegacyCodesAsync`

Parses existing `Encounter.Icd10Codes` across all encounters for a patient, deduplicates, and creates `PatientDiagnosis` records with default certainty `G` and type `Encounterdiagnose`. Codes appearing in 3+ encounters are promoted to `Dauerdiagnose`.

## 44.4 Blazor UI

### New: Patient Diagnosis List (`PatientDiagnosen.razor`)

```
MediPrax.Server/Components/Pages/Patienten/PatientDiagnosen.razor
```

Route: `/patienten/{PatientId:guid}/diagnosen`

Full CRUD for patient's diagnosis master list:
- Table: ICD-10 | Description | Certainty (badge) | Laterality | Type (Dauer/Encounter) | Status | Since
- Filter by: Active only / All / Dauerdiagnosen only
- Add new diagnosis with metadata
- Edit certainty, laterality, status inline
- Deactivate (soft) with resolution date

### Modified: `EncounterFormular.razor`

Replace the simple ICD-10 search + chips with:

1. **Inherited Dauerdiagnosen** shown at top with checkmarks (pre-selected, can be unchecked)
2. **Encounter-specific diagnoses** added via ICD-10 search
3. Each diagnosis chip shows: `F32.1 (G)` or `G40.2 (V, R)` -- code + certainty + laterality
4. Click on chip to edit metadata (modal or inline)
5. Option to promote an encounter diagnosis to Dauerdiagnose

### Modified: `PatientDetail.razor`

Add "Diagnosen" section showing active Dauerdiagnosen as a quick overview.

## 44.5 KVDT Export Integration

Modify KVDT export to include KBV-mandated diagnosis fields:
- Feldkennung 6001: ICD-10 Code
- Feldkennung 6003: Diagnosesicherheit (G/V/Z/A)
- Feldkennung 6004: Seitenlokalisation (R/L/B)

## 44.6 Arztbrief Integration

Modify Arztbrief to display diagnoses with certainty:
- "F32.1 G -- Mittelgradige depressive Episode" (confirmed)
- "G40.2 V, R -- Lokalisationsbezogene symptomatische Epilepsie (Verdacht, rechts)"

## 44.7 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/PatientDiagnosis.cs` |
| Create | `MediPrax.Core/Entities/EncounterDiagnosis.cs` |
| Create | `MediPrax.Core/Enums/DiagnosisCertainty.cs` |
| Create | `MediPrax.Core/Enums/DiagnosisLaterality.cs` |
| Create | `MediPrax.Core/Enums/DiagnosisType.cs` |
| Create | `MediPrax.Core/Enums/DiagnosisStatus.cs` |
| Modify | `MediPrax.Core/Entities/Encounter.cs` -- add EncounterDiagnoses |
| Modify | `MediPrax.Core/Entities/Patient.cs` -- add Diagnoses |
| Create | `MediPrax.Application/DTOs/PatientDiagnosisDto.cs` |
| Create | `MediPrax.Application/DTOs/EncounterDiagnosisDto.cs` |
| Create | `MediPrax.Application/Interfaces/IPatientDiagnosisService.cs` |
| Create | `MediPrax.Application/Services/PatientDiagnosisService.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/PatientDiagnosisConfiguration.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/EncounterDiagnosisConfiguration.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/MediPraxDbContext.cs` |
| Create | `MediPrax.Server/Components/Pages/Patienten/PatientDiagnosen.razor` |
| Modify | `MediPrax.Server/Components/Pages/Dokumentation/EncounterFormular.razor` |
| Modify | `MediPrax.Server/Components/Pages/Patienten/PatientDetail.razor` |
| Modify | KVDT export logic -- add certainty/laterality fields |
| Modify | Arztbrief generation -- display certainty |
| Modify | `MediPrax.Server/Program.cs` |
| Create | EF Core migration |
| Create | `tests/MediPrax.IntegrationTests/PatientDiagnosisServiceTests.cs` |

## 44.8 Acceptance Criteria

- [ ] PatientDiagnosis entity with full metadata (certainty, laterality, type, status)
- [ ] Dauerdiagnosen auto-inherited when creating a new encounter
- [ ] Encounter UI shows inherited diagnoses + option to add encounter-specific ones
- [ ] Each diagnosis displays certainty badge (G/V/Z/A) and laterality
- [ ] Patient diagnosis list page with CRUD and filtering
- [ ] KVDT export includes Diagnosesicherheit and Seitenlokalisation
- [ ] Arztbrief displays diagnoses with certainty qualifier
- [ ] Migration tool converts legacy List<string> to PatientDiagnosis records
- [ ] Backward compatible with existing encounter data

---
