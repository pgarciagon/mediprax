# M27: Suicidality Assessment & Safety Planning -- ABGESCHLOSSEN

> **Priority:** P2 | **Estimated Effort:** Small-Medium
> **Why:** Suicidality assessment is a core psychiatric responsibility. Structured documentation protects both patient and clinician. System-wide alerts prevent critical information from being missed.

#### 27.1 Data Model

##### Entity: `SuicidalityAssessment`

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `PatientId` | `Guid` | FK -> Patient |
| `EncounterId` | `Guid` | FK -> Encounter |
| `AssessedById` | `Guid` | FK -> User |
| `AssessmentDate` | `DateOnly` | |
| `RiskLevel` | `SuicidalityRiskLevel` | Enum: None, Low, Moderate, High, Acute |
| `SuicidalIdeation` | `bool` | Active suicidal thoughts |
| `SuicidalPlans` | `bool` | Concrete plans |
| `SuicidalIntent` | `bool` | Intent to act |
| `PriorAttempts` | `bool` | History of attempts |
| `PriorAttemptsDetails` | `string?` | Details |
| `RiskFactors` | `List<string>` | JSONB -- selected risk factors |
| `ProtectiveFactors` | `List<string>` | JSONB -- selected protective factors |
| `SafetyPlan` | `string?` | Documented safety plan |
| `ActionsTaken` | `List<string>` | JSONB -- measures taken |
| `Notes` | `string?` | |

#### 27.2 System-Wide Alert

Add to `Patient` entity:
```csharp
public SuicidalityRiskLevel? CurrentSuicidalityRisk { get; set; }
public DateTime? SuicidalityRiskUpdatedAt { get; set; }
```

**Display:** Red alert banner on every page that shows patient data (PatientDetail, EncounterFormular, Wochenansicht, Wartezimmer) when `CurrentSuicidalityRisk >= Moderate`.

#### 27.3 Risk Factor / Protective Factor Catalogs

```
MediPrax.Application/Catalogs/SuicidalityFactorCatalog.cs
```

**Risk factors:** Vorherige Suizidversuche, Substanzmissbrauch, Verlust/Trennung, chronische Schmerzen, Hoffnungslosigkeit, Impulsivitaet, Zugang zu Mitteln, Isolation, psychiatrische Diagnose (Depression, Schizophrenie, BPS), maennliches Geschlecht, Alter >65...

**Protective factors:** Soziales Netzwerk, Kinder, religioese Ueberzeugungen, therapeutische Beziehung, Zukunftsplaene, Distanzierungsfaehigkeit, Behandlungsadhaerenz...

#### 27.4 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/SuicidalityAssessment.cs` |
| Create | `MediPrax.Core/Enums/SuicidalityRiskLevel.cs` |
| Modify | `MediPrax.Core/Entities/Patient.cs` -- Add risk level field |
| Create | `MediPrax.Application/Catalogs/SuicidalityFactorCatalog.cs` |
| Create | `MediPrax.Application/DTOs/SuicidalityAssessmentDto.cs` |
| Create | `MediPrax.Application/Interfaces/ISuicidalityAssessmentService.cs` |
| Create | `MediPrax.Application/Services/SuicidalityAssessmentService.cs` |
| Create | `MediPrax.Server/Components/Pages/Dokumentation/Suizidalitaet.razor` |
| Create | `MediPrax.Server/Components/Shared/SuicidalityAlert.razor` -- Reusable alert banner |
| Modify | `MediPrax.Server/Components/Pages/Patienten/PatientDetail.razor` |
| Modify | `MediPrax.Server/Components/Pages/Termine/Wartezimmer.razor` |
| + EF config, DbContext, DI, tests |

---

