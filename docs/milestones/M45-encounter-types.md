# M45: Encounter Types (Karteieintragstypen) -- AUSSTEHEND

> **Priority:** P2 | **Estimated Effort:** Medium
> **Why:** German PVS distinguish between consultation types (Sprechstunde, Telefonat, Videosprechstunde, Hausbesuch, Konsil, Notfall). Each type has different billing rules, documentation requirements, and workflow implications. MediPrax currently has a single generic Encounter entity for everything.

---

## 45.1 Problem Statement

All encounters in MediPrax are treated identically regardless of context. A 5-minute phone call and a 50-minute therapy session use the same form. This causes:

- Incorrect billing: phone consultations use different GOPs (e.g., 01435 Telefonkonsultation)
- Missing documentation context: no way to filter/report by encounter type
- No Hausbesuch-specific fields (travel time, address)
- Videosprechstunde encounters should link to M29 video integration
- Statistical reporting cannot differentiate encounter types

## 45.2 Data Model

### New Enum: `EncounterType`

```
MediPrax.Core/Enums/EncounterType.cs
```

```csharp
public enum EncounterType
{
    Sprechstunde,       // In-person consultation (default)
    Telefonat,          // Phone consultation
    Videosprechstunde,  // Video consultation (links to M29)
    Hausbesuch,         // Home visit
    Konsil,             // Inter-physician consultation
    Notfall,            // Emergency consultation
    Kurzkontakt,        // Brief contact (< 5 min, e.g., prescription renewal)
    Gruppentherapie,    // Group therapy session
    Befundbesprechung   // Results discussion (lab, imaging)
}
```

### Modify Entity: `Encounter`

```csharp
public EncounterType EncounterType { get; set; } = EncounterType.Sprechstunde;

// Hausbesuch-specific
public string? VisitAddress { get; set; }
public int? TravelTimeMinutes { get; set; }
```

## 45.3 Service Layer

### Modify: `EncounterService`

- `CreateAsync`: accept EncounterType, set defaults based on type
- `GetByPatientAsync`: add optional type filter
- New: `GetStatisticsByTypeAsync(DateOnly from, DateOnly to)` -- count/duration per type

### New: `EncounterTypeHelper`

```
MediPrax.Application/Services/EncounterTypeHelper.cs
```

Static helper providing type-specific defaults:

```csharp
public static class EncounterTypeHelper
{
    public static EncounterSectionType[] GetDefaultSections(EncounterType type) => type switch
    {
        EncounterType.Sprechstunde => [Anamnese, Befund, Diagnose, Therapie, Procedere],
        EncounterType.Telefonat => [Anamnese, Therapie, Procedere],
        EncounterType.Kurzkontakt => [Procedere],
        EncounterType.Hausbesuch => [Anamnese, Befund, Diagnose, Therapie, Procedere],
        _ => [Anamnese, Befund, Diagnose, Therapie, Procedere]
    };

    public static string[] GetSuggestedGops(EncounterType type) => type switch
    {
        EncounterType.Telefonat => ["01435"],
        EncounterType.Hausbesuch => ["01410", "01413"],
        EncounterType.Videosprechstunde => ["01450", "01451"],
        EncounterType.Notfall => ["01210", "01212"],
        _ => []
    };
}
```

## 45.4 Blazor UI

### Modified: `EncounterFormular.razor`

- Add EncounterType selector at the top (dropdown or icon buttons)
- Show/hide fields based on type:
  - Hausbesuch: show VisitAddress + TravelTimeMinutes fields
  - Telefonat: simplified form (fewer sections)
  - Kurzkontakt: minimal form
- Auto-suggest GOPs based on encounter type

### Modified: Patient encounter list

- Show encounter type icon/badge next to each entry
- Filter by encounter type

### Modified: Dashboard / Statistics

- Breakdown of encounters by type

## 45.5 Dependency on M43

This milestone depends on M43 (Structured Encounter Sections) for the `GetDefaultSections` functionality. If M43 is not yet implemented, the encounter type still works for filtering, billing suggestions, and Hausbesuch fields, but without section defaults.

## 45.6 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Enums/EncounterType.cs` |
| Modify | `MediPrax.Core/Entities/Encounter.cs` -- add EncounterType, VisitAddress, TravelTimeMinutes |
| Create | `MediPrax.Application/Services/EncounterTypeHelper.cs` |
| Modify | `MediPrax.Application/Services/EncounterService.cs` -- type-aware logic |
| Modify | `MediPrax.Application/DTOs/EncounterDto.cs` -- add type fields |
| Modify | `MediPrax.Infrastructure/Persistence/Configurations/EncounterConfiguration.cs` |
| Modify | `MediPrax.Server/Components/Pages/Dokumentation/EncounterFormular.razor` |
| Modify | `MediPrax.Server/Components/Pages/Patienten/PatientDetail.razor` -- type badges |
| Modify | `MediPrax.Server/Program.cs` |
| Create | EF Core migration |
| Create | `tests/MediPrax.UnitTests/EncounterTypeHelperTests.cs` |

## 45.7 Acceptance Criteria

- [ ] EncounterType enum with 9 types
- [ ] Encounter form shows type selector
- [ ] Hausbesuch shows address and travel time fields
- [ ] Encounter type auto-suggests relevant GOPs
- [ ] Patient encounter list displays type badges
- [ ] Filter encounters by type
- [ ] Statistics breakdown by encounter type
- [ ] Default type is Sprechstunde for backward compatibility

---
