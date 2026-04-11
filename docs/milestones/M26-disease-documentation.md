# M26: Disease-Specific Documentation Modules -- ABGESCHLOSSEN

> **Priority:** P2 | **Estimated Effort:** Medium
> **Why:** Structured disease-specific documentation improves care quality and enables outcome tracking.

#### 26.1 Epilepsy Module

##### Entity: `SeizureDiary` (Anfallskalender)

| Property | Type | Description |
|----------|------|-------------|
| `PatientId` | `Guid` | FK |
| `SeizureDate` | `DateTime` | When it occurred |
| `SeizureType` | `string` | Fokal / Generalisiert / Unklassifiziert |
| `DurationSeconds` | `int?` | Duration |
| `Trigger` | `string?` | Known trigger |
| `AuraPresent` | `bool` | Preceding aura |
| `AuraDescription` | `string?` | Aura details |
| `ConsciousnessImpaired` | `bool` | Loss of consciousness |
| `PostictalState` | `string?` | Post-seizure state |
| `MedicationAtTime` | `string?` | Current meds snapshot |
| `Notes` | `string?` | |

**Blazor page:** `Dokumentation/Anfallskalender.razor` -- Calendar view with seizure entries, frequency charts.

#### 26.2 Headache/Migraine Module

##### Entity: `HeadacheDiary`

| Property | Type | Description |
|----------|------|-------------|
| `PatientId` | `Guid` | FK |
| `Date` | `DateOnly` | Date |
| `StartTime` | `TimeOnly?` | When it started |
| `DurationHours` | `decimal?` | Duration |
| `Intensity` | `int` | VAS 0-10 |
| `Type` | `string` | Migraene / Spannungskopfschmerz / Cluster / Andere |
| `Location` | `string?` | Einseitig rechts / links / beidseits / frontal / etc. |
| `AuraPresent` | `bool` | |
| `Nausea` | `bool` | |
| `Photophobia` | `bool` | |
| `Phonophobia` | `bool` | |
| `Triggers` | `List<string>?` | JSONB |
| `MedicationTaken` | `string?` | What was taken |
| `MedicationEffective` | `bool?` | Did it help? |

**Blazor page:** `Dokumentation/KopfschmerzTagebuch.razor` -- Monthly calendar view, frequency/intensity charts.

#### 26.3 Multiple Sclerosis Module

##### Entity: `MsDocumentation`

| Property | Type | Description |
|----------|------|-------------|
| `PatientId` | `Guid` | FK |
| `DocumentationDate` | `DateOnly` | |
| `EdssScore` | `decimal` | EDSS 0.0-10.0 |
| `IsRelapse` | `bool` | Schub? |
| `RelapseDescription` | `string?` | |
| `MriDate` | `DateOnly?` | Last MRI |
| `MriFindings` | `string?` | Summary |
| `NewLesions` | `int?` | New T2 lesions |
| `GadEnhancing` | `int?` | Gadolinium-enhancing lesions |
| `CurrentDmt` | `string?` | Current disease-modifying therapy |
| `Notes` | `string?` | |

**Blazor page:** `Dokumentation/MsDokumentation.razor` -- EDSS over time chart, relapse timeline.

#### 26.4 Parkinson Module

##### Entity: `ParkinsonDocumentation`

| Property | Type | Description |
|----------|------|-------------|
| `PatientId` | `Guid` | FK |
| `DocumentationDate` | `DateOnly` | |
| `HoehnYahrStage` | `decimal` | 0-5 |
| `Tremor` | `int` | 0-4 scale |
| `Rigidity` | `int` | 0-4 scale |
| `Bradykinesia` | `int` | 0-4 scale |
| `PosturalInstability` | `int` | 0-4 scale |
| `OnOffFluctuations` | `bool` | |
| `Dyskinesia` | `bool` | |
| `FreezeOfGait` | `bool` | |
| `NonMotorSymptoms` | `List<string>?` | JSONB |
| `Notes` | `string?` | |

#### 26.5 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/SeizureDiary.cs` |
| Create | `MediPrax.Core/Entities/HeadacheDiary.cs` |
| Create | `MediPrax.Core/Entities/MsDocumentation.cs` |
| Create | `MediPrax.Core/Entities/ParkinsonDocumentation.cs` |
| Create | `MediPrax.Application/DTOs/SeizureDiaryDto.cs` |
| Create | `MediPrax.Application/DTOs/HeadacheDiaryDto.cs` |
| Create | `MediPrax.Application/DTOs/MsDocumentationDto.cs` |
| Create | `MediPrax.Application/DTOs/ParkinsonDocumentationDto.cs` |
| Create | `MediPrax.Application/Interfaces/ISeizureDiaryService.cs` |
| Create | `MediPrax.Application/Interfaces/IHeadacheDiaryService.cs` |
| Create | `MediPrax.Application/Services/SeizureDiaryService.cs` |
| Create | `MediPrax.Application/Services/HeadacheDiaryService.cs` |
| Create | `MediPrax.Application/Services/MsDocumentationService.cs` |
| Create | `MediPrax.Application/Services/ParkinsonDocumentationService.cs` |
| Create | `MediPrax.Server/Components/Pages/Dokumentation/Anfallskalender.razor` |
| Create | `MediPrax.Server/Components/Pages/Dokumentation/KopfschmerzTagebuch.razor` |
| Create | `MediPrax.Server/Components/Pages/Dokumentation/MsDokumentation.razor` |
| Create | `MediPrax.Server/Components/Pages/Dokumentation/ParkinsonDokumentation.razor` |
| Create EF configs, modify DbContext, register services, add tests |

---

