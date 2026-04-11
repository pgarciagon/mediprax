# M43: Structured Encounter Documentation (ABDTP Fields) -- AUSSTEHEND

> **Priority:** P1 | **Estimated Effort:** Large
> **Why:** All major German PVS (CGM M1, tomedo, medatixx, Latido) use structured Karteieintrag fields (Anamnese, Befund, Diagnose, Therapie, Procedere). This is the single most impactful architectural gap in MediPrax. Without it, clinical notes are unsearchable free text, Arztbriefe cannot auto-populate sections, and regulatory documentation requirements (Paragraph 630f BGB) for structured records are harder to meet.

---

## 43.1 Problem Statement

Currently, `Encounter.Notes` is a single `string?` field. All clinical information (Anamnese, Befund, Therapie, Procedere) is mixed together in free text. This means:

- No structured search ("show all encounters where Befund mentions tremor")
- Arztbrief generation cannot auto-extract individual sections
- Templates pre-fill the textarea but the system loses structure after editing
- No per-section timestamps or author tracking
- No standardized export (e.g., for ePA structured documents)

## 43.2 Data Model

### New Entity: `EncounterSection`

```
MediPrax.Core/Entities/EncounterSection.cs
```

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK (BaseEntity) |
| `EncounterId` | `Guid` | FK -> Encounter |
| `SectionType` | `EncounterSectionType` | Enum: Anamnese, Befund, Diagnose, Therapie, Procedere, Sonstiges |
| `Content` | `string` | Section text content |
| `SortOrder` | `int` | Display order within encounter |
| `AuthorId` | `Guid` | FK -> User who wrote this section |

### New Enum: `EncounterSectionType`

```
MediPrax.Core/Enums/EncounterSectionType.cs
```

```csharp
public enum EncounterSectionType
{
    Anamnese,       // A - History, patient-reported symptoms
    Befund,         // B - Examination findings, observations
    Diagnose,       // D - Diagnosis assessment (narrative, complements ICD-10 codes)
    Therapie,       // T - Treatment plan, prescriptions, recommendations
    Procedere,      // P - Next steps, follow-up, referrals
    Sonstiges       // Other/free-form notes
}
```

### Modify Entity: `Encounter`

Add navigation property:

```csharp
public ICollection<EncounterSection> Sections { get; set; } = [];
```

**Migration strategy for existing data:** The `Notes` field is NOT removed. Existing encounters keep their notes as-is. A one-time migration service can parse existing notes (using the template markers like "PSYCHISCHER BEFUND:", "BEURTEILUNG:", "PROCEDERE:") and create EncounterSection records. New encounters use sections by default.

## 43.3 Service Layer

### `IEncounterSectionService`

```
MediPrax.Application/Interfaces/IEncounterSectionService.cs
```

```csharp
public interface IEncounterSectionService
{
    Task<List<EncounterSectionDto>> GetByEncounterAsync(Guid encounterId, CancellationToken ct = default);
    Task<EncounterSectionDto> CreateAsync(CreateEncounterSectionDto dto, CancellationToken ct = default);
    Task<EncounterSectionDto> UpdateAsync(UpdateEncounterSectionDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<string> GetCombinedTextAsync(Guid encounterId, CancellationToken ct = default);
    Task MigrateNotesToSectionsAsync(Guid encounterId, CancellationToken ct = default);
}
```

### Key Method: `GetCombinedTextAsync`

Concatenates all sections in order (A -> B -> D -> T -> P) for Arztbrief generation and legacy compatibility.

### Key Method: `MigrateNotesToSectionsAsync`

Parses existing `Notes` content using known template markers and creates structured sections. Used for one-time migration of existing encounters.

## 43.4 DTOs

```
MediPrax.Application/DTOs/EncounterSectionDto.cs
```

- `EncounterSectionDto` -- full representation
- `CreateEncounterSectionDto` -- EncounterId, SectionType, Content
- `UpdateEncounterSectionDto` -- Id, Content

## 43.5 Blazor UI

### Modified: `EncounterFormular.razor`

Replace the single textarea with tabbed or accordion sections:

```
[Anamnese (A)] [Befund (B)] [Diagnose (D)] [Therapie (T)] [Procedere (P)]

+--------------------------------------------------+
| Anamnese                                          |
|                                                    |
| [Textarea with Textbausteine support]              |
|                                                    |
+--------------------------------------------------+
```

**Behavior:**
- Each tab is a separate `EncounterSection`
- Auto-saved on blur (debounced)
- Templates (Psychiatrisch, Neurologisch, Kurzkonsultation) now populate the correct sections instead of dumping everything into one field
- Existing encounters with only `Notes` show a "Zu Sektionen migrieren" button
- Structured findings (AMDP narrative, Neuro narrative) auto-populate the Befund section

### Keyboard shortcuts

- `Alt+A` -> focus Anamnese
- `Alt+B` -> focus Befund
- `Alt+D` -> focus Diagnose
- `Alt+T` -> focus Therapie
- `Alt+P` -> focus Procedere

## 43.6 Arztbrief Integration

Modify `ArztbriefService` to pull individual sections:
- "Anamnese" section of the Arztbrief -> `EncounterSection.Anamnese`
- "Befund" section -> `EncounterSection.Befund` + AMDP narrative + Neuro narrative
- "Diagnosen" -> ICD-10 codes + `EncounterSection.Diagnose`
- "Therapie und Procedere" -> `EncounterSection.Therapie` + `EncounterSection.Procedere`

## 43.7 Search Integration

Extend `GlobalSearchService` to search within specific section types:
- "tremor in:befund" -> searches only Befund sections
- Default search continues to search all sections

## 43.8 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/EncounterSection.cs` |
| Create | `MediPrax.Core/Enums/EncounterSectionType.cs` |
| Modify | `MediPrax.Core/Entities/Encounter.cs` -- add Sections collection |
| Create | `MediPrax.Application/DTOs/EncounterSectionDto.cs` |
| Create | `MediPrax.Application/Interfaces/IEncounterSectionService.cs` |
| Create | `MediPrax.Application/Services/EncounterSectionService.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/EncounterSectionConfiguration.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/MediPraxDbContext.cs` |
| Modify | `MediPrax.Server/Components/Pages/Dokumentation/EncounterFormular.razor` -- major refactor |
| Modify | `MediPrax.Application/Services/ArztbriefService.cs` -- section-aware generation |
| Modify | `MediPrax.Application/Services/GlobalSearchService.cs` -- section-type filter |
| Modify | `MediPrax.Server/Program.cs` -- DI registration |
| Create | EF Core migration |
| Create | `tests/MediPrax.IntegrationTests/EncounterSectionServiceTests.cs` |

## 43.9 Acceptance Criteria

- [ ] New encounters show ABDTP tabbed sections by default
- [ ] Each section is independently editable and auto-saves
- [ ] Templates populate correct sections
- [ ] AMDP/Neuro narratives auto-fill Befund section
- [ ] Arztbrief generation pulls from individual sections
- [ ] Existing encounters with Notes continue to work (backward compatible)
- [ ] Migration tool converts old Notes to sections
- [ ] Global search supports section-type filtering

---
