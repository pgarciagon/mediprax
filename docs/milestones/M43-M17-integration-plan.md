# Integration Plan: AMDP (M17) + Structured Encounter Sections (M43)

> **Scope:** Connect PsychopathologicalFinding and NeurologicalExamination narratives with
> the new ABDTP section system, so the Befund (B) section becomes the single source of truth
> for all clinical findings — structured and free-text.
>
> **Approach:** Composition at read-time, not duplication at write-time. Each data source
> (AMDP, Neuro, free-text) stays in its own entity. The UI composes them visually inside
> the Befund tab, and the ArztbriefService composes them at export time.
>
> **Estimated Effort:** 2-3 days

---

## Current State

### AMDP (M17) — PsychopathologicalFinding

- Entity: `PsychopathologicalFinding` (1:1 per Encounter, unique index on EncounterId)
- Storage: `List<SymptomFinding>` as JSONB + `NarrativeText` string
- Entry: Dedicated page `/dokumentation/psychopath-befund/{EncounterId}`
- Text generation: `PsychopathFindingTextGenerator.Generate(findings)`
- Display in EncounterFormular: Read-only `<div class="narrative-block">` with "Bearbeiten" link

### Neurological Examination (M20) — NeurologicalExamination

- Same pattern as AMDP: entity with structured value objects + NarrativeText
- Entry: `/dokumentation/neurologischer-befund/{EncounterId}`
- Text generation: `NeuroExamTextGenerator.GenerateFromDto(...)`

### Structured Sections (M43) — EncounterSection

- Entity: `EncounterSection` (many per Encounter, keyed by SectionType)
- Enum: `EncounterSectionType` { Anamnese, Befund, Diagnose, Therapie, Procedere, Sonstiges }
- Entry: Inline textareas in EncounterFormular, saved via `SectionService.SaveAllAsync()`
- The Befund textarea currently contains ONLY free-text — no connection to AMDP/Neuro narratives

### Problem

The narratives from AMDP and Neuro appear as floating blocks above the ABDTP sections.
They are visually disconnected from the Befund (B) section. This means:

1. The Befund section is incomplete — it doesn't reflect the structured examinations
2. The Arztbrief cannot pull a complete Befund from a single source
3. The doctor sees findings in two separate places instead of one coherent view
4. Printing or exporting the encounter produces an incomplete Befund

---

## Target Architecture

```
                    EncounterFormular.razor
                    ┌─────────────────────────────────────┐
                    │  [A] [B] [D] [T] [P]                │
                    │                                       │
                    │  ┌─ Befund (B) ────────────────────┐ │
                    │  │                                   │ │
                    │  │  ╔═ AMDP ══════════╗ [Bearbeiten]│ │
 PsychopathFinding ─┼──│  ║ Bewusstsein klar║             │ │
 .NarrativeText     │  │  ║ Stimmung gedr.  ║             │ │
                    │  │  ╚═════════════════╝             │ │
                    │  │                                   │ │
                    │  │  ╔═ NEURO ═════════╗ [Bearbeiten]│ │
 NeurologicalExam  ─┼──│  ║ Hirnnerven opB  ║             │ │
 .NarrativeText     │  │  ║ Kraft 5/5       ║             │ │
                    │  │  ╚═════════════════╝             │ │
                    │  │                                   │ │
 EncounterSection  ─┼──│  ┌─ Ergaenzender Befund ───────┐│ │
 (Befund)           │  │  │ [editable textarea]          ││ │
                    │  │  └──────────────────────────────┘│ │
                    │  └───────────────────────────────────┘ │
                    └─────────────────────────────────────┘

                    ArztbriefService.GetCompositeBefundAsync()
                    ┌─────────────────────────────────────┐
                    │ 1. PsychopathFinding.NarrativeText   │
                    │ 2. NeurologicalExam.NarrativeText    │
                    │ 3. EncounterSection(Befund).Content   │
                    │ ─────────────────────────────────────│
                    │ → Combined text for Arztbrief PDF     │
                    └─────────────────────────────────────┘
```

**Key principle:** Three separate data sources, composed at display and export time. No duplication.

---

## Implementation Steps

### Step 1: Create `ICompositeBefundService`

New service that composes the full Befund from its three sources.

**Create:** `MediPrax.Application/Interfaces/ICompositeBefundService.cs`

```csharp
public interface ICompositeBefundService
{
    /// <summary>
    /// Composes the full Befund text from structured findings + free-text section.
    /// Used by ArztbriefService and any export that needs the complete Befund.
    /// </summary>
    Task<CompositeBefundDto> GetAsync(Guid encounterId, CancellationToken ct = default);
}
```

**Create:** `MediPrax.Application/DTOs/CompositeBefundDto.cs`

```csharp
public class CompositeBefundDto
{
    public string? PsychopathNarrative { get; set; }
    public Guid? PsychopathFindingId { get; set; }
    public DateOnly? PsychopathAssessmentDate { get; set; }

    public string? NeuroNarrative { get; set; }
    public Guid? NeuroExamId { get; set; }
    public DateOnly? NeuroExamDate { get; set; }

    public string? FreitextBefund { get; set; }

    /// <summary>
    /// Full composed text: AMDP + Neuro + Freitext, separated by double newlines.
    /// </summary>
    public string ComposedText
    {
        get
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(PsychopathNarrative))
                parts.Add($"Psychopathologischer Befund: {PsychopathNarrative}");

            if (!string.IsNullOrWhiteSpace(NeuroNarrative))
                parts.Add($"Neurologischer Befund: {NeuroNarrative}");

            if (!string.IsNullOrWhiteSpace(FreitextBefund))
                parts.Add(FreitextBefund);

            return string.Join("\n\n", parts);
        }
    }

    public bool HasStructuredFindings =>
        PsychopathFindingId.HasValue || NeuroExamId.HasValue;
}
```

**Create:** `MediPrax.Application/Services/CompositeBefundService.cs`

```csharp
public class CompositeBefundService(
    IPsychopathFindingService psychopathService,
    INeurologicalExamService neuroService,
    IEncounterSectionService sectionService) : ICompositeBefundService
{
    public async Task<CompositeBefundDto> GetAsync(Guid encounterId, CancellationToken ct = default)
    {
        var dto = new CompositeBefundDto();

        // 1. AMDP narrative
        var psychFinding = await psychopathService.GetByEncounterAsync(encounterId, ct);
        if (psychFinding is not null)
        {
            dto.PsychopathNarrative = psychFinding.NarrativeText;
            dto.PsychopathFindingId = psychFinding.Id;
            dto.PsychopathAssessmentDate = psychFinding.AssessmentDate;
        }

        // 2. Neuro narrative
        var neuroExam = await neuroService.GetByEncounterAsync(encounterId, ct);
        if (neuroExam is not null)
        {
            dto.NeuroNarrative = neuroExam.NarrativeText;
            dto.NeuroExamId = neuroExam.Id;
            dto.NeuroExamDate = neuroExam.ExamDate;
        }

        // 3. Free-text Befund section (M43)
        var sections = await sectionService.GetByEncounterAsync(encounterId, ct);
        var befundSection = sections.FirstOrDefault(
            s => s.SectionType == EncounterSectionType.Befund);
        dto.FreitextBefund = befundSection?.Content;

        return dto;
    }
}
```

**Register DI in** `Program.cs`:

```csharp
builder.Services.AddScoped<ICompositeBefundService, CompositeBefundService>();
```

---

### Step 2: Refactor EncounterFormular.razor — Befund Tab

Replace the current approach (narrative blocks above sections + plain textarea) with an
integrated Befund section.

**Remove from EncounterFormular.razor (lines ~146-166):**

The two standalone `narrative-block` divs that currently float above the ABDTP tabs.

**Replace the Befund textarea (inside the ABDTP loop) with a composite component:**

Change the ABDTP rendering loop to detect when `sectionType == Befund` and render the
composite view instead of a plain textarea:

```razor
<div class="abdtp-sections">
    @foreach (var sectionType in abdtpOrder)
    {
        <div class="abdtp-section">
            <label class="abdtp-label">
                <span class="abdtp-key">@GetSectionKey(sectionType)</span>
                @GetSectionLabel(sectionType)
            </label>

            @if (sectionType == EncounterSectionType.Befund && isEdit)
            {
                @* --- Integrated Befund with structured findings --- *@
                <div class="befund-composite">
                    @if (!string.IsNullOrEmpty(compositeBefund?.PsychopathNarrative))
                    {
                        <div class="structured-finding-block">
                            <div class="structured-finding-header">
                                <span class="finding-icon">🧠</span>
                                <span class="finding-title">Psychopathologischer Befund (AMDP)</span>
                                <a href="/dokumentation/psychopath-befund/@Id"
                                   class="btn btn-sm btn-outline-secondary">Bearbeiten</a>
                            </div>
                            <div class="structured-finding-text">
                                @compositeBefund.PsychopathNarrative
                            </div>
                        </div>
                    }
                    else if (isEdit)
                    {
                        <a href="/dokumentation/psychopath-befund/@Id"
                           class="btn btn-sm btn-outline-primary mb-2">
                            + Psychopathologischen Befund erheben
                        </a>
                    }

                    @if (!string.IsNullOrEmpty(compositeBefund?.NeuroNarrative))
                    {
                        <div class="structured-finding-block">
                            <div class="structured-finding-header">
                                <span class="finding-icon">🔬</span>
                                <span class="finding-title">Neurologischer Befund</span>
                                <a href="/dokumentation/neurologischer-befund/@Id"
                                   class="btn btn-sm btn-outline-secondary">Bearbeiten</a>
                            </div>
                            <div class="structured-finding-text">
                                @compositeBefund.NeuroNarrative
                            </div>
                        </div>
                    }
                    else if (isEdit)
                    {
                        <a href="/dokumentation/neurologischer-befund/@Id"
                           class="btn btn-sm btn-outline-primary mb-2">
                            + Neurologischen Befund erheben
                        </a>
                    }

                    <label class="form-label mt-2">Ergaenzender Befund</label>
                    <textarea class="form-control abdtp-textarea"
                              rows="4"
                              value="@sectionContents[EncounterSectionType.Befund]"
                              @onchange="e => sectionContents[EncounterSectionType.Befund] =
                                  e.Value?.ToString() ?? string.Empty"
                              placeholder="Weitere klinische Beobachtungen..." />
                </div>
            }
            else
            {
                @* --- Standard textarea for A, D, T, P, S --- *@
                <textarea class="form-control abdtp-textarea"
                          rows="@GetSectionRows(sectionType)"
                          value="@sectionContents[sectionType]"
                          @onchange="e => sectionContents[sectionType] =
                              e.Value?.ToString() ?? string.Empty"
                          placeholder="@GetSectionPlaceholder(sectionType)" />
            }
        </div>
    }
</div>
```

**Add to @code section:**

```csharp
@inject ICompositeBefundService CompositeBefundService

private CompositeBefundDto? compositeBefund;

// In OnInitializedAsync, replace the narrative loading block:
// REMOVE:
//   var psychFinding = await PsychopathFindingService.GetByEncounterAsync(Id!.Value);
//   if (psychFinding is not null) { psychopathNarrative = ... }
//   var neuroExam = await NeuroExamService.GetByEncounterAsync(Id!.Value);
//   if (neuroExam is not null) { neuroNarrative = ... }
//
// REPLACE WITH:
compositeBefund = await CompositeBefundService.GetAsync(Id!.Value);
```

**Remove fields that are no longer needed:**

```csharp
// REMOVE these fields:
// private string? psychopathNarrative;
// private Guid? psychopathFindingId;
// private string? neuroNarrative;
// private Guid? neuroExamId;
```

**Save logic stays the same:** `SectionService.SaveAllAsync()` saves only the free-text
content of each section. The AMDP and Neuro narratives remain in their own entities.

---

### Step 3: Refactor ArztbriefService — Use CompositeBefundService

Modify the Arztbrief generation to pull a complete Befund using the new service.

**Modify:** `MediPrax.Application/Services/ArztbriefService.cs`

Inject `ICompositeBefundService` and use it wherever the Arztbrief builds the Befund section:

```csharp
// BEFORE (current — only uses Notes or section text):
var befundText = encounter.Notes; // or section content

// AFTER:
var compositeBefund = await compositeBefundService.GetAsync(encounterId, ct);
var befundText = compositeBefund.ComposedText;
```

This ensures the Arztbrief always contains:
1. Psychopathological findings (if assessed)
2. Neurological findings (if examined)
3. Any additional free-text observations

---

### Step 4: CSS for Structured Finding Blocks

**Modify:** `MediPrax.Server/wwwroot/css/` (relevant stylesheet)

```css
.befund-composite {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
}

.structured-finding-block {
    border: 1px solid var(--border-color, #e2e8f0);
    border-radius: 0.5rem;
    padding: 0.75rem 1rem;
    background: var(--finding-bg, #f8fafc);
}

.structured-finding-header {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    margin-bottom: 0.5rem;
}

.finding-title {
    font-weight: 600;
    font-size: 0.875rem;
    flex: 1;
}

.structured-finding-text {
    font-size: 0.875rem;
    line-height: 1.5;
    color: var(--text-secondary, #475569);
    white-space: pre-line;
}

/* Dark mode */
[data-theme="dark"] .structured-finding-block {
    background: var(--finding-bg-dark, #1e293b);
    border-color: var(--border-color-dark, #334155);
}
```

---

### Step 5: Handle Navigation Return from AMDP/Neuro Pages

When the doctor navigates to PsychopathBefund.razor, edits symptoms, saves, and returns
to the EncounterFormular, the composite Befund needs to reflect the updated narrative.

**Current behavior:** After save, `PsychopathBefund.razor` calls `NavigationManager.NavigateTo`
back to the encounter form. The form re-runs `OnInitializedAsync` which reloads data.

**No change needed here** — the `CompositeBefundService.GetAsync()` call in
`OnInitializedAsync` will pick up the updated narrative automatically on page reload.

**Optional enhancement:** If you later want to avoid a full page reload, you can use
a SignalR notification or a shared state service to push the updated narrative to the
EncounterFormular component in real-time. This is not required for the initial integration.

---

### Step 6: Update PsychopathBefund.razor and NeurologischerBefund.razor (Minor)

Remove the "structured finding links" section from the old narrative block area in
EncounterFormular (if there were separate link buttons). These are now integrated into
the Befund composite view as "+ Psychopathologischen Befund erheben" and "Bearbeiten"
buttons.

**No changes needed in PsychopathBefund.razor itself** — it continues to save to
`PsychopathologicalFinding` via its own service. The integration is purely at the
display/composition layer.

---

### Step 7: Tests

**Create:** `tests/MediPrax.IntegrationTests/CompositeBefundServiceTests.cs`

```csharp
public class CompositeBefundServiceTests : IntegrationTestBase
{
    [Fact]
    public async Task GetAsync_WithAmdpAndNeuroAndFreitext_ComposesAll()
    {
        // Arrange: create encounter, psychopath finding, neuro exam, befund section
        // Act
        var result = await service.GetAsync(encounterId);
        // Assert
        Assert.NotNull(result.PsychopathNarrative);
        Assert.NotNull(result.NeuroNarrative);
        Assert.NotNull(result.FreitextBefund);
        Assert.Contains("Psychopathologischer Befund:", result.ComposedText);
        Assert.Contains("Neurologischer Befund:", result.ComposedText);
    }

    [Fact]
    public async Task GetAsync_WithOnlyFreitext_ReturnsFreitextOnly()
    {
        // Arrange: create encounter + befund section, no structured findings
        // Act
        var result = await service.GetAsync(encounterId);
        // Assert
        Assert.Null(result.PsychopathNarrative);
        Assert.Null(result.NeuroNarrative);
        Assert.False(result.HasStructuredFindings);
        Assert.Equal(freitextContent, result.ComposedText);
    }

    [Fact]
    public async Task GetAsync_WithOnlyAmdp_ReturnsAmdpOnly()
    {
        // Arrange: create encounter + psychopath finding, no section or neuro
        // Act
        var result = await service.GetAsync(encounterId);
        // Assert
        Assert.NotNull(result.PsychopathNarrative);
        Assert.True(result.HasStructuredFindings);
        Assert.StartsWith("Psychopathologischer Befund:", result.ComposedText);
    }

    [Fact]
    public async Task GetAsync_EmptyEncounter_ReturnsEmptyDto()
    {
        // Arrange: create encounter with nothing
        // Act
        var result = await service.GetAsync(encounterId);
        // Assert
        Assert.False(result.HasStructuredFindings);
        Assert.Equal(string.Empty, result.ComposedText);
    }
}
```

**Create:** `tests/MediPrax.UnitTests/CompositeBefundDtoTests.cs`

```csharp
public class CompositeBefundDtoTests
{
    [Fact]
    public void ComposedText_AllParts_JoinsWithDoubleNewline()
    {
        var dto = new CompositeBefundDto
        {
            PsychopathNarrative = "Bewusstsein klar",
            NeuroNarrative = "Hirnnerven o.p.B.",
            FreitextBefund = "Zusaetzlich: RR 130/80"
        };

        var text = dto.ComposedText;

        Assert.Contains("Psychopathologischer Befund: Bewusstsein klar", text);
        Assert.Contains("Neurologischer Befund: Hirnnerven o.p.B.", text);
        Assert.Contains("Zusaetzlich: RR 130/80", text);
    }

    [Fact]
    public void ComposedText_OnlyFreitext_NoHeaders()
    {
        var dto = new CompositeBefundDto { FreitextBefund = "RR 130/80" };
        Assert.Equal("RR 130/80", dto.ComposedText);
    }

    [Fact]
    public void ComposedText_Empty_ReturnsEmpty()
    {
        var dto = new CompositeBefundDto();
        Assert.Equal(string.Empty, dto.ComposedText);
    }

    [Fact]
    public void HasStructuredFindings_WithPsychopath_ReturnsTrue()
    {
        var dto = new CompositeBefundDto { PsychopathFindingId = Guid.NewGuid() };
        Assert.True(dto.HasStructuredFindings);
    }
}
```

---

## Files Summary

| Action | File | Description |
|--------|------|-------------|
| Create | `Application/Interfaces/ICompositeBefundService.cs` | Interface |
| Create | `Application/DTOs/CompositeBefundDto.cs` | Composite DTO with ComposedText |
| Create | `Application/Services/CompositeBefundService.cs` | Composition logic |
| Modify | `Server/Components/Pages/Dokumentation/EncounterFormular.razor` | Integrated Befund tab |
| Modify | `Application/Services/ArztbriefService.cs` | Use CompositeBefundService |
| Modify | `Server/Program.cs` | DI registration |
| Modify | `Server/wwwroot/css/` | Structured finding block styles |
| Create | `tests/IntegrationTests/CompositeBefundServiceTests.cs` | Integration tests |
| Create | `tests/UnitTests/CompositeBefundDtoTests.cs` | Unit tests for DTO |

**No database migration required.** No new entities or schema changes — this is purely
a composition/display refactor using existing data.

---

## Implementation Order

```
Step 1: CompositeBefundDto + ICompositeBefundService + CompositeBefundService
     ↓
Step 2: EncounterFormular.razor refactor (Befund tab)
     ↓  (can be tested manually at this point)
Step 3: ArztbriefService integration
     ↓
Step 4: CSS styling
     ↓
Step 5: Verify navigation return flow (likely no changes)
     ↓
Step 6: Clean up old narrative blocks / dead code
     ↓
Step 7: Tests
```

Steps 1-2 are the core. Steps 3-7 can be done incrementally.

---

## Future Extensions

Once this integration is in place, the same `CompositeBefundService` pattern can be
extended for:

- **Disease-specific modules** (M26): Epilepsy seizure summaries, MS EDSS scores, and
  Parkinson assessments could appear as additional structured blocks inside the Befund tab
- **Psychometric test summaries** (M18): PHQ-9/GAD-7 scores could appear as a compact
  block (e.g., "PHQ-9: 14/27 — mittelgradige Depression")
- **Suicidality assessment** (M27): Risk level summary block
- **Lab results** (M22): Latest relevant lab values as a compact block

Each would follow the same pattern: a read-only structured block inside the Befund
composite, with a link to its dedicated editing page, and a corresponding extension in
`CompositeBefundService.GetAsync()`.
