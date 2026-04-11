# M18: Psychometric Testing & Assessment Tools -- ABGESCHLOSSEN

> **Priority:** P0 | **Estimated Effort:** Large
> **Why:** Standardized screening instruments (PHQ-9, GAD-7, etc.) are expected by every psychiatrist. They enable outcome measurement, therapy evaluation, and billing (GOP 35600).

#### 18.1 Data Model

##### Entity: `PsychometricTest`

```
MediPrax.Core/Entities/PsychometricTest.cs
```

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `PatientId` | `Guid` | FK -> Patient |
| `EncounterId` | `Guid?` | FK -> Encounter (optional) |
| `AdministeredById` | `Guid` | FK -> User |
| `TestType` | `PsychometricTestType` | Enum: PHQ9, GAD7, BDI2, etc. |
| `TestDate` | `DateOnly` | Date administered |
| `Responses` | `List<TestResponse>` | JSONB -- individual item responses |
| `TotalScore` | `int` | Calculated total score |
| `Interpretation` | `string` | Auto-generated interpretation text |
| `Notes` | `string?` | Clinician notes |
| `Status` | `TestStatus` | Enum: InProgress, Completed |

##### Enum: `PsychometricTestType`

```csharp
public enum PsychometricTestType
{
    PHQ9,       // Patient Health Questionnaire (Depression)
    GAD7,       // Generalized Anxiety Disorder
    BDI2,       // Beck Depression Inventory II
    HAMD,       // Hamilton Depression Scale
    PANSS,      // Positive and Negative Syndrome Scale
    BPRS,       // Brief Psychiatric Rating Scale
    CGI,        // Clinical Global Impression
    WHO5,       // WHO-5 Wellbeing Index
    AUDIT,      // Alcohol Use Disorders Identification Test
    PCL5,       // PTSD Checklist
    PSQI,       // Pittsburgh Sleep Quality Index
    MMSE,       // Mini Mental State (also neuro)
    MoCA,       // Montreal Cognitive Assessment (also neuro)
    ClockDrawing // Clock Drawing Test (also neuro)
}
```

##### Value Object: `TestResponse` (JSONB)

| Property | Type | Description |
|----------|------|-------------|
| `ItemNumber` | `int` | Question number (1-based) |
| `ItemText` | `string` | Question text (German) |
| `ResponseValue` | `int` | Selected answer value |
| `ResponseText` | `string?` | Selected answer label |

#### 18.2 Test Definition Catalog

```
MediPrax.Application/Catalogs/PsychometricTestCatalog.cs
```

Each test definition includes:

| Property | Description |
|----------|-------------|
| `TestType` | Enum value |
| `Name` | Full German name |
| `Abbreviation` | Short name |
| `Description` | What it measures |
| `Items` | List of questions with answer options |
| `ScoreRanges` | Interpretation thresholds |
| `MaxScore` | Maximum possible score |
| `ScoringMethod` | Sum, average, or custom |
| `Source` | Citation / reference |

**PHQ-9 Example:**

```csharp
new TestDefinition
{
    TestType = PsychometricTestType.PHQ9,
    Name = "Patient Health Questionnaire - 9",
    Items = [
        new("Wenig Interesse oder Freude an Ihren Taetigkeiten", [
            ("Ueberhaupt nicht", 0),
            ("An einzelnen Tagen", 1),
            ("An mehr als der Haelfte der Tage", 2),
            ("Beinahe jeden Tag", 3)
        ]),
        // ... 8 more items
    ],
    ScoreRanges = [
        (0, 4, "Minimal", "Keine depressive Stoerung"),
        (5, 9, "Leicht", "Leichte depressive Symptomatik"),
        (10, 14, "Mittelgradig", "Mittelgradige depressive Stoerung"),
        (15, 19, "Mittelschwer", "Mittelschwere depressive Stoerung"),
        (20, 27, "Schwer", "Schwere depressive Stoerung")
    ],
    MaxScore = 27,
    SuggestedGop = "35600"
}
```

**Tests to implement in order of priority:**
1. PHQ-9 (depression -- most commonly used)
2. GAD-7 (anxiety)
3. WHO-5 (general wellbeing)
4. CGI (clinical global impression)
5. AUDIT (alcohol screening)
6. BDI-II (depression -- more detailed)
7. MMSE (cognitive screening -- neurology)
8. MoCA (cognitive assessment -- neurology)
9. PANSS (schizophrenia)
10. PCL-5 (PTSD)
11. PSQI (sleep)
12. HAMD (Hamilton depression -- clinician-rated)
13. BPRS (brief psychiatric rating)
14. Clock Drawing Test

#### 18.3 Service Layer

```
MediPrax.Application/Interfaces/IPsychometricTestService.cs
MediPrax.Application/Services/PsychometricTestService.cs
```

| Method | Description |
|--------|-------------|
| `GetByIdAsync(Guid id)` | Get test with full responses |
| `GetByPatientAsync(Guid patientId, PsychometricTestType? type)` | All tests for patient, optionally filtered by type |
| `GetScoreHistoryAsync(Guid patientId, PsychometricTestType type)` | Score over time for charts |
| `CreateAsync(CreatePsychometricTestDto)` | Start a new test |
| `SubmitResponsesAsync(Guid testId, List<TestResponse>)` | Submit answers, calculate score |
| `DeleteAsync(Guid id)` | Soft delete |

#### 18.4 Blazor UI

##### Test Administration Page

```
MediPrax.Server/Components/Pages/Testung/TestDurchfuehrung.razor
```

**Layout:**
- Step-by-step wizard: one question per screen (tablet-friendly for patient self-administration)
- Alternative: all questions on one page (for clinician administration)
- Progress bar showing completion
- Response buttons with clear German labels
- Real-time score calculation

##### Test Results / History Page

```
MediPrax.Server/Components/Pages/Testung/TestErgebnisse.razor
```

**Layout:**
- Score history chart per test type (line chart using Blazor charting library or simple SVG)
- Color-coded severity bands on chart
- Table of all tests with: Date, Type, Score, Interpretation
- Click to view full test details
- PDF export of test results

##### Integration Points

- `PatientDetail.razor` -- New "Testung" section showing recent scores
- `EncounterFormular.razor` -- Button "Test durchfuehren" linking to test
- Billing suggestion: auto-prompt to add GOP 35600 when test is completed

#### 18.5 Score Visualization

Use inline SVG for score-over-time charts (no external JS dependency):

```razor
<svg width="600" height="200" class="score-chart">
    <!-- Score bands (background colors for severity zones) -->
    <rect y="0" height="30" fill="#fee" /> <!-- Severe -->
    <rect y="30" height="37" fill="#fec" /> <!-- Moderate -->
    <rect y="67" height="37" fill="#ffc" /> <!-- Mild -->
    <rect y="104" height="96" fill="#efe" /> <!-- Minimal -->

    <!-- Score line with data points -->
    @foreach (var point in scoreHistory)
    {
        <circle cx="@point.X" cy="@point.Y" r="4" fill="#333" />
    }
    <polyline points="@string.Join(" ", scoreHistory.Select(p => $"{p.X},{p.Y}"))"
              fill="none" stroke="#333" stroke-width="2" />
</svg>
```

#### 18.6 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/PsychometricTest.cs` |
| Create | `MediPrax.Core/Enums/PsychometricTestType.cs` |
| Create | `MediPrax.Core/Enums/TestStatus.cs` |
| Create | `MediPrax.Core/ValueObjects/TestResponse.cs` |
| Create | `MediPrax.Application/Catalogs/PsychometricTestCatalog.cs` |
| Create | `MediPrax.Application/DTOs/PsychometricTestDto.cs` |
| Create | `MediPrax.Application/Interfaces/IPsychometricTestService.cs` |
| Create | `MediPrax.Application/Services/PsychometricTestService.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/PsychometricTestConfiguration.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/MediPraxDbContext.cs` |
| Create | `MediPrax.Server/Components/Pages/Testung/TestDurchfuehrung.razor` |
| Create | `MediPrax.Server/Components/Pages/Testung/TestErgebnisse.razor` |
| Create | `MediPrax.Server/Components/Pages/Testung/TestAuswahl.razor` |
| Modify | `MediPrax.Server/Components/Pages/Patienten/PatientDetail.razor` -- Add Testung section |
| Modify | `MediPrax.Server/Components/Pages/Dokumentation/EncounterFormular.razor` -- Add test link |
| Modify | `MediPrax.Server/Components/Layout/MainLayout.razor` -- Add navigation item |
| Modify | `MediPrax.Server/Program.cs` |
| Create | `tests/MediPrax.IntegrationTests/PsychometricTestServiceTests.cs` |
| Create | `tests/MediPrax.UnitTests/PsychometricTestCatalogTests.cs` |

---

