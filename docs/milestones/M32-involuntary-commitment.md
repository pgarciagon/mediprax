# M32: Involuntary Commitment & Legal Documentation -- AUSSTEHEND

> **Priority:** P3 | **Estimated Effort:** Small
> **Why:** Niche but critical when needed. Psychiatric practices must document PsychKG-related decisions.

#### 32.1 Templates

##### Aerztliches Zeugnis (PsychKG)

```
MediPrax.Reporting/Formulare/PsychKGZeugnisDocument.cs
```

| Field | Description |
|-------|-------------|
| Patient data | Auto-populated |
| Untersuchungsdatum/-zeit | Date and time of examination |
| Psychopathologischer Befund | From AMDP (Milestone 17) |
| Eigengefaehrdung | Self-endangerment details |
| Fremdgefaehrdung | Danger to others details |
| Diagnose | ICD-10 |
| Empfehlung | Recommendation (voluntary, involuntary) |
| Unterbringungsdauer | Suggested duration |

##### Betreuungsanregung

Template for suggestion to the Betreuungsgericht (guardianship court):
- Patient information
- Clinical findings
- Areas requiring guardianship (Aufgabenkreise)
- Urgency assessment

#### 32.2 Legal Status Tracking

Add to `Patient`:
```csharp
public bool HasLegalGuardian { get; set; }
public string? GuardianName { get; set; }
public string? GuardianContact { get; set; }
public string? GuardianScope { get; set; } // Aufgabenkreise
```

#### 32.3 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Reporting/Formulare/PsychKGZeugnisDocument.cs` |
| Create | `MediPrax.Reporting/Formulare/BetreuungsanregungDocument.cs` |
| Modify | `MediPrax.Core/Entities/Patient.cs` -- Add guardian fields |
| Create | `MediPrax.Server/Components/Pages/Formulare/PsychKGZeugnis.razor` |
| + endpoints in Program.cs |

---

