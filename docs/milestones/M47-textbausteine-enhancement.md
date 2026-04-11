# M47: Enhanced Textbausteine with Variables and Inline Expansion -- AUSSTEHEND

> **Priority:** P2 | **Estimated Effort:** Medium
> **Why:** M30 defined the Textbausteine data model and basic CRUD but is only partially implemented. This milestone completes the system with inline `#`-expansion in all text fields, hierarchical categories, variable resolution, and the autocomplete UX that makes tomedo's documentation flow so fast. This is the "last mile" that turns Textbausteine from a feature into a daily-use workflow accelerator.

---

## 47.1 Scope (delta from M30)

M30 defined:
- TextModule entity with Shortcut, Title, Content, Category, IsGlobal, UsageCount
- Template variables ({Patient.Name}, {Datum}, {Diagnosen}, etc.)
- Default seed modules
- CRUD management page

**This milestone adds:**
- Inline `#`-expansion autocomplete component (reusable across all textareas)
- Hierarchical categories with folder-like navigation
- Extended variable set including encounter and structured finding data
- Import/export (JSON) for sharing between practices
- Usage analytics (most used, unused modules)
- Integration with M43 (section-specific Textbausteine)

## 47.2 Reusable Autocomplete Component

### `TextModuleAutocomplete.razor`

```
MediPrax.Server/Components/Shared/TextModuleAutocomplete.razor
```

A Blazor component that wraps any `textarea` and adds `#`-triggered autocomplete:

**Behavior:**
1. User types `#` in any textarea
2. Dropdown appears with matching text modules (filtered by typed text after `#`)
3. Arrow keys navigate, Enter selects
4. Selected module's content replaces the `#shortcut` text
5. Variables are resolved at insertion time
6. Dropdown shows: Shortcut | Title | Category | Preview (first 50 chars)

**Integration points:**
- `EncounterFormular.razor` -- all note/section fields
- `ArztbriefFormular.razor` -- body text
- `PsychopathBefund.razor` -- narrative text
- `NeurologischerBefund.razor` -- narrative text
- Any future textarea fields

### Parameters

```csharp
@code {
    [Parameter] public string Value { get; set; }
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public string? CategoryFilter { get; set; } // Optional: only show modules from this category
    [Parameter] public Guid? EncounterId { get; set; }      // For variable resolution
    [Parameter] public Guid? PatientId { get; set; }        // For variable resolution
}
```

## 47.3 Extended Variable Set

Building on M30's variables, add encounter-context variables:

| Variable | Resolved To |
|----------|------------|
| `{Patient.Name}` | Full patient name |
| `{Patient.Geburtsdatum}` | Date of birth |
| `{Patient.Alter}` | Calculated age |
| `{Patient.Geschlecht}` | "Herr" / "Frau" |
| `{Patient.Versicherung}` | Insurance name |
| `{Arzt.Name}` | Current doctor name |
| `{Arzt.Titel}` | "Dr. med." etc. |
| `{Datum}` | Today's date (DD.MM.YYYY) |
| `{Datum.Lang}` | "11. April 2026" |
| `{Diagnosen}` | Active ICD-10 codes with descriptions |
| `{Dauerdiagnosen}` | Permanent diagnoses only (depends on M44) |
| `{Medikation}` | Current medication plan |
| `{LetzterBefund.Psychopath}` | Last AMDP narrative |
| `{LetzterBefund.Neuro}` | Last neuro exam narrative |
| `{Encounter.Datum}` | Current encounter date |
| `{Encounter.Dauer}` | Current encounter duration |

## 47.4 Hierarchical Categories

Extend TextModule with subcategories:

```csharp
// Category uses path notation: "Psychiatrie/Depression", "Neurologie/Epilepsie"
// UI shows as collapsible tree in management page
```

Seed categories:
- Psychiatrie / Depression
- Psychiatrie / Angst
- Psychiatrie / Psychose
- Psychiatrie / Sucht
- Neurologie / Epilepsie
- Neurologie / MS
- Neurologie / Parkinson
- Neurologie / Kopfschmerz
- Allgemein / Arztbrief
- Allgemein / Kurzkonsultation

## 47.5 Section-Specific Modules (M43 Integration)

If M43 is implemented, TextModules can be tagged with a target section:

```csharp
// Add to TextModule entity:
public EncounterSectionType? TargetSection { get; set; }
```

When inserting into a section-specific textarea, only show modules matching that section:
- In the Anamnese field: show modules tagged `Anamnese` + untagged
- In the Befund field: show modules tagged `Befund` + untagged

## 47.6 Import/Export

```csharp
public interface ITextModuleService
{
    // ... existing methods from M30
    Task<string> ExportAsJsonAsync(string? category = null, CancellationToken ct = default);
    Task<int> ImportFromJsonAsync(string json, bool overwriteExisting = false, CancellationToken ct = default);
}
```

JSON format:
```json
{
  "version": "1.0",
  "exportedAt": "2026-04-11",
  "modules": [
    {
      "shortcut": "#normalpsy",
      "title": "Normaler psychopathologischer Befund",
      "content": "Bewusstsein klar, allseits orientiert...",
      "category": "Psychiatrie/Allgemein",
      "targetSection": "Befund"
    }
  ]
}
```

## 47.7 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Server/Components/Shared/TextModuleAutocomplete.razor` |
| Modify | `MediPrax.Core/Entities/TextModule.cs` -- add TargetSection |
| Modify | `MediPrax.Application/Services/TextModuleService.cs` -- variables, import/export |
| Modify | `MediPrax.Server/Components/Pages/Verwaltung/Textbausteine.razor` -- tree view, import/export |
| Modify | `MediPrax.Server/Components/Pages/Dokumentation/EncounterFormular.razor` -- use autocomplete component |
| Modify | Multiple Razor pages -- integrate TextModuleAutocomplete |
| Create | EF Core migration (if TargetSection added) |
| Create | `tests/MediPrax.UnitTests/TextModuleVariableResolverTests.cs` |

## 47.8 Acceptance Criteria

- [ ] `#`-triggered autocomplete works in all textareas across the application
- [ ] Variables resolve correctly at insertion time
- [ ] Hierarchical categories with tree navigation in management page
- [ ] Section-specific filtering when used inside structured encounter sections
- [ ] Import/export JSON format
- [ ] Usage tracking (most popular modules)
- [ ] Keyboard-navigable dropdown (arrow keys + Enter)
- [ ] Works as standalone component (no dependency on M43/M44, but enhanced by them)

---
