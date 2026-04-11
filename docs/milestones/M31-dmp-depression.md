# M31: DMP Depression -- ABGESCHLOSSEN

> **Priority:** P3 | **Estimated Effort:** Medium
> **Why:** Disease Management Programme for depression -- required for participating practices. Structured quarterly documentation with data transmission to KV.

#### 31.1 Data Model

##### Entity: `DmpEnrollment`

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `PatientId` | `Guid` | FK -> Patient |
| `DmpType` | `string` | "Depression" |
| `EnrollmentDate` | `DateOnly` | Date enrolled |
| `DisenrollmentDate` | `DateOnly?` | If unenrolled |
| `Status` | `DmpStatus` | Active, Paused, Disenrolled |

##### Entity: `DmpDocumentation`

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `DmpEnrollmentId` | `Guid` | FK |
| `DocumentationDate` | `DateOnly` | |
| `Quarter` | `string` | "2026-Q2" |
| `FormData` | `Dictionary<string, string>` | JSONB |
| `Submitted` | `bool` | Transmitted to KV |
| `SubmittedAt` | `DateTime?` | |

#### 31.2 DMP Depression Documentation Fields

Per G-BA specification:
- Current PHQ-9 score
- Current medication
- Psychotherapy status
- Comorbidities
- Hospitalization since last documentation
- Schulung (patient education) offered
- Treatment goals assessment
- Referral to specialist if needed

#### 31.3 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/DmpEnrollment.cs` |
| Create | `MediPrax.Core/Entities/DmpDocumentation.cs` |
| Create | `MediPrax.Application/DTOs/DmpDto.cs` |
| Create | `MediPrax.Application/Services/DmpService.cs` |
| Create | `MediPrax.Server/Components/Pages/Dmp/DmpUebersicht.razor` |
| Create | `MediPrax.Server/Components/Pages/Dmp/DmpDokumentation.razor` |
| + EF config, DbContext, DI, tests |

---

