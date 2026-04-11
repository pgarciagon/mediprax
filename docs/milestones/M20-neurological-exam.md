# M20: Structured Neurological Examination -- ABGESCHLOSSEN

> **Priority:** P0 | **Estimated Effort:** Medium
> **Why:** Every neurological practice needs structured exam documentation. The current basic templates are insufficient for systematic neurological status recording.

#### 20.1 Data Model

##### Entity: `NeurologicalExamination`

```
MediPrax.Core/Entities/NeurologicalExamination.cs
```

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `EncounterId` | `Guid` | FK -> Encounter |
| `PatientId` | `Guid` | FK -> Patient |
| `ExaminedById` | `Guid` | FK -> User |
| `ExamDate` | `DateOnly` | Date of exam |
| `CranialNerves` | `CranialNerveFindings` | JSONB |
| `MotorSystem` | `MotorFindings` | JSONB |
| `Reflexes` | `ReflexFindings` | JSONB |
| `SensorySystem` | `SensoryFindings` | JSONB |
| `Coordination` | `CoordinationFindings` | JSONB |
| `Gait` | `GaitFindings` | JSONB |
| `MeningealSigns` | `MeningealFindings` | JSONB |
| `NarrativeText` | `string?` | Auto-generated or manual |
| `Notes` | `string?` | Additional notes |

##### Value Objects (all JSONB):

**`CranialNerveFindings`:**
- `I_Olfactory` (string: "normal" / "eingeschraenkt" / "aufgehoben" + comment)
- `II_Visual` (Visus, Gesichtsfeld, Pupillen, Funduskopie)
- `III_IV_VI_Oculomotor` (Augenbewegungen, Doppelbilder, Nystagmus)
- `V_Trigeminal` (Sensibilitaet, Masseter, Kornealreflex)
- `VII_Facial` (Stirnrunzeln, Mundwinkel, Geschmack)
- `VIII_Vestibulocochlear` (Gehoer, Schwindel, Weber/Rinne)
- `IX_X_Glossopharyngeal_Vagus` (Gaumensegel, Schlucken, Wuergereflex)
- `XI_Accessory` (Kopfdrehung, Schulterhebung)
- `XII_Hypoglossal` (Zungenbeweglichkeit, Atrophie, Fibrillationen)

**`MotorFindings`:**
- Per body region (RArm, LArm, RLeg, LLeg):
  - `Strength` (MRC grade 0-5)
  - `Tone` (normal / spastisch / rigide / schlaff)
  - `Bulk` (normal / Atrophie)
  - `Fasciculations` (bool)

**`ReflexFindings`:**
- Per reflex (BSR, TSR, RPR, PSR, ASR):
  - `Right` (0 = absent, 1 = abgeschwaecht, 2 = normal, 3 = lebhaft, 4 = gesteigert+Kloni)
  - `Left` (same scale)
- Pathological: `BabinskiRight`, `BabinskiLeft`, `HoffmannTroemer`, etc.

**`SensoryFindings`:**
- `Touch`, `Pain`, `Temperature`, `Vibration`, `Proprioception`
- Each: (normal / eingeschraenkt / aufgehoben) + distribution pattern

**`CoordinationFindings`:**
- `FingerNose` (normal / Dysmetrie / Intentionstremor)
- `HeelShin` (normal / Dysmetrie)
- `Romberg` (normal / unsicher / pathologisch)
- `TandemGait` (normal / unsicher / nicht moeglich)
- `Diadochokinese` (normal / Dysdiadochokinese)

**`GaitFindings`:**
- `GaitPattern` (normal / spastisch / ataktisch / kleinschrittig / Steppergang / etc.)
- `ArmSwing` (normal / vermindert)
- `Stability` (normal / unsicher / Fallneigung)

**`MeningealFindings`:**
- `NeckStiffness` (bool)
- `Kernig` (bool)
- `Brudzinski` (bool)
- `Lasegue` (Right: degrees / Left: degrees)

#### 20.2 Narrative Text Generator

```
MediPrax.Application/Services/NeuroExamTextGenerator.cs
```

Generates standard neurological report text from structured findings:

*"Hirnnerven: Pupillen isokor, prompt reagibel. Augenbewegungen frei, kein Nystagmus. Mimische Muskulatur seitengleich innerviert. Zunge gerade herausgestreckt. Motorik: Kraftgrade allseits 5/5, Tonus unauffaellig. Muskeleigenreflexe seitengleich mittellebhaft ausloesbar, keine pathologischen Reflexe. Sensibilitaet fuer alle Qualitaeten intakt. Koordination: Finger-Nase-Versuch zielsicher, Stand nach Romberg sicher. Gangbild unauffaellig. Meningismus: nicht vorhanden."*

#### 20.3 Blazor UI

```
MediPrax.Server/Components/Pages/Dokumentation/NeurologischerBefund.razor
```

**Layout:**
1. **Tabbed sections:** Hirnnerven | Motorik | Reflexe | Sensibilitaet | Koordination | Gang | Meningismus
2. **Reflex diagram:** Visual body diagram with clickable reflex points (HTML/SVG)
3. **Motor strength grid:** Body region x MRC grade table
4. **"Normalbefund" button:** Sets all to normal with one click (most common workflow)
5. **Live narrative preview** at bottom
6. **Side-by-side comparison** with previous exam

#### 20.4 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/NeurologicalExamination.cs` |
| Create | `MediPrax.Core/ValueObjects/CranialNerveFindings.cs` |
| Create | `MediPrax.Core/ValueObjects/MotorFindings.cs` |
| Create | `MediPrax.Core/ValueObjects/ReflexFindings.cs` |
| Create | `MediPrax.Core/ValueObjects/SensoryFindings.cs` |
| Create | `MediPrax.Core/ValueObjects/CoordinationFindings.cs` |
| Create | `MediPrax.Core/ValueObjects/GaitFindings.cs` |
| Create | `MediPrax.Core/ValueObjects/MeningealFindings.cs` |
| Create | `MediPrax.Application/DTOs/NeurologicalExaminationDto.cs` |
| Create | `MediPrax.Application/Interfaces/INeurologicalExamService.cs` |
| Create | `MediPrax.Application/Services/NeurologicalExamService.cs` |
| Create | `MediPrax.Application/Services/NeuroExamTextGenerator.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/NeurologicalExaminationConfiguration.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/MediPraxDbContext.cs` |
| Create | `MediPrax.Server/Components/Pages/Dokumentation/NeurologischerBefund.razor` |
| Modify | `MediPrax.Server/Components/Pages/Dokumentation/EncounterFormular.razor` |
| Modify | `MediPrax.Server/Program.cs` |
| Modify | `MediPrax.Reporting/Arztbrief/ArztbriefDocument.cs` |
| Create | `tests/MediPrax.IntegrationTests/NeurologicalExamServiceTests.cs` |
| Create | `tests/MediPrax.UnitTests/NeuroExamTextGeneratorTests.cs` |

---

