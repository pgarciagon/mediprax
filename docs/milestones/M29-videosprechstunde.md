# M29: Videosprechstunde Integration -- AUSSTEHEND

> **Priority:** P3 | **Estimated Effort:** Small-Medium
> **Why:** Post-COVID standard. Important for psychiatric follow-ups and patients with mobility limitations.

#### 29.1 Approach

**No custom video implementation.** Instead, integrate with a KBV-certified provider (e.g., Doctolib Video, RED connect, Patientus) via:

1. **Link generation:** Create video consultation link and send to patient
2. **Consent tracking:** Document Einwilligungserklaerung
3. **Session marking:** Flag appointments as video sessions
4. **Billing rules:** Enforce psychiatry-specific video restrictions

#### 29.2 Data Model Changes

Add to `Appointment`:
```csharp
public bool IsVideoConsultation { get; set; }
public string? VideoLink { get; set; }
public bool VideoConsentGiven { get; set; }
```

#### 29.3 Psychiatric Video Rules

```
MediPrax.Application/Services/VideoConsultationRules.cs
```

- First psychotherapeutic Sprechstunde MUST be in-person
- At least 50 minutes of Sprechstunde and Probatorik must be in-person
- Certain GOPs have video variants (add suffix)
- Track video vs. in-person ratio per therapy case

#### 29.4 Files to Create/Modify

| Action | File |
|--------|------|
| Modify | `MediPrax.Core/Entities/Appointment.cs` -- Add video fields |
| Create | `MediPrax.Application/Services/VideoConsultationRules.cs` |
| Create | `MediPrax.Application/DTOs/VideoConsultationDto.cs` |
| Modify | `MediPrax.Server/Components/Pages/Termine/TerminFormular.razor` -- Video toggle |
| Modify | `MediPrax.Server/Components/Pages/Termine/Wochenansicht.razor` -- Video indicator |
| Modify | `MediPrax.Infrastructure/Persistence/Configurations/AppointmentConfiguration.cs` |
| Create | `tests/MediPrax.UnitTests/VideoConsultationRulesTests.cs` |

---

