# M23: Billing Enhancements -- AUSSTEHEND

> **Priority:** P1 | **Estimated Effort:** Large
> **Why:** Billing plausibility checks protect against KV rejections and Regress (financial recoupment). GOP suggestion engine improves revenue capture.

#### 23.1 Plausibility Check Engine

```
MediPrax.Application/Services/BillingPlausibilityService.cs
```

##### Exclusion Rules

```
MediPrax.Application/Catalogs/GopExclusionCatalog.cs
```

Define which GOPs cannot be billed together:

```csharp
new ExclusionRule("16220", ["16210", "16211", "16212"], 
    "Grundpauschale und Gespraechsziffer am selben Tag nicht kombinierbar"),
new ExclusionRule("21220", ["21210", "21211", "21212"], 
    "Grundpauschale und Gespraechsziffer am selben Tag nicht kombinierbar"),
// ... 50+ rules for chapters 16, 21, 35
```

##### Quantity Limits

```csharp
new QuantityLimit("16220", MaxPerQuarter: 3, 
    "Neurologisches Gespraech max. 3x pro Quartal"),
new QuantityLimit("35100", MaxPerPatient: 6, 
    "Psychotherapeutische Sprechstunde max. 6 pro Patient"),
// ...
```

##### Time Documentation Checks

- GOP 16220/21220 require documented `DurationMinutes` >= 10 min
- GOP 35xxx require documented session duration matching 25 or 50 min
- Flag missing duration documentation

##### Method

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

#### 23.2 GOP Suggestion Engine

```
MediPrax.Application/Services/GopSuggestionService.cs
```

Based on documented encounter data, suggest billable GOPs:

| Trigger | Suggested GOP | Condition |
|---------|--------------|-----------|
| Encounter created with duration >= 10 min | 16220 or 21220 | Neurology or Psychiatry |
| Psychometric test completed | 35600 | Any |
| First encounter of quarter | 16210/21210 (Grundpauschale) | Auto-add |
| EEG documented | 16320 | Neurology |
| Therapy session logged | 35150/35141/etc. | Based on TherapyCase type |
| Encounter notes mention "Notfall" | 21221 | Psychiatry |

#### 23.3 Quarter-End Validation

```
MediPrax.Server/Components/Pages/Abrechnung/QuartalsValidierung.razor
```

Before KVDT export:
- Run plausibility checks on ALL patients in the quarter
- Show aggregated warnings: Critical (must fix) / Warning (review) / Info
- Exportable error report
- One-click fix suggestions where possible

#### 23.4 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Application/Catalogs/GopExclusionCatalog.cs` |
| Create | `MediPrax.Application/Services/BillingPlausibilityService.cs` |
| Create | `MediPrax.Application/Services/GopSuggestionService.cs` |
| Create | `MediPrax.Application/DTOs/PlausibilityWarningDto.cs` |
| Create | `MediPrax.Application/DTOs/GopSuggestionDto.cs` |
| Modify | `MediPrax.Application/Services/BillingService.cs` -- Integrate checks |
| Create | `MediPrax.Server/Components/Pages/Abrechnung/QuartalsValidierung.razor` |
| Modify | `MediPrax.Server/Components/Pages/Abrechnung/Abrechnungsuebersicht.razor` -- Show warnings |
| Modify | `MediPrax.Server/Components/Pages/Dokumentation/EncounterFormular.razor` -- Show suggestions |
| Modify | `MediPrax.Server/Program.cs` |
| Create | `tests/MediPrax.UnitTests/GopExclusionCatalogTests.cs` |
| Create | `tests/MediPrax.UnitTests/BillingPlausibilityServiceTests.cs` |
| Create | `tests/MediPrax.UnitTests/GopSuggestionServiceTests.cs` |

---

