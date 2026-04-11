# M17: Structured Psychopathological Assessment (AMDP) -- ABGESCHLOSSEN

> **Priority:** P0 | **Estimated Effort:** Large
> **Why:** The AMDP-based psychopathological finding is the #1 most-used documentation tool in German psychiatry. Every competitor has this.

#### 17.1 Data Model

##### New Entity: `PsychopathologicalFinding`

```
MediPrax.Core/Entities/PsychopathologicalFinding.cs
```

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK (BaseEntity) |
| `EncounterId` | `Guid` | FK -> Encounter |
| `PatientId` | `Guid` | FK -> Patient |
| `AssessedById` | `Guid` | FK -> User (doctor) |
| `AssessmentDate` | `DateOnly` | Date of assessment |
| `Findings` | `List<SymptomFinding>` | JSONB -- structured findings |
| `NarrativeText` | `string?` | Auto-generated or manual free text |
| `Notes` | `string?` | Additional clinical notes |

##### Value Object: `SymptomFinding` (stored as JSONB)

| Property | Type | Description |
|----------|------|-------------|
| `CategoryCode` | `string` | AMDP category (e.g., "BEW" for consciousness) |
| `SymptomCode` | `string` | AMDP symptom code (e.g., "BEW-1") |
| `SymptomName` | `string` | German name (e.g., "Bewusstseinslage") |
| `Severity` | `int` | 0 = not present, 1 = mild, 2 = moderate, 3 = severe |
| `Comment` | `string?` | Free text for this symptom |

#### 17.2 AMDP Symptom Catalog

Create a static catalog class (like the existing `Icd10Catalog` and `GopCatalog`):

```
MediPrax.Application/Catalogs/AmdpCatalog.cs
```

**Categories to implement** (per AMDP 9th edition):

| Code | Category (German) | # Symptoms |
|------|-------------------|------------|
| BEW | Bewusstseinsstoerungen | 5 |
| ORI | Orientierungsstoerungen | 4 |
| AUF | Aufmerksamkeits- und Gedaechtnisstoerungen | 6 |
| FDS | Formale Denkstoerungen | 14 |
| BEF | Befuerchtungen und Zwaenge | 5 |
| WAH | Wahn | 10 |
| SIN | Sinnestaeauschungen | 8 |
| ICH | Ich-Stoerungen | 4 |
| AFF | Stoerungen der Affektivitaet | 14 |
| ANT | Antriebs- und psychomotorische Stoerungen | 10 |
| CIR | Circadiane Besonderheiten | 2 |
| AND | Andere Stoerungen | 4 |
| SUI | Suizidalitaet | 4 |
| SOM | Somatische Befunde | ~40 |

**Total: ~130 items** -- hardcoded as `IReadOnlyList<AmdpSymptom>` with `Search(string term)` method.

#### 17.3 Narrative Text Generator ("Befundomat")

```
MediPrax.Application/Services/PsychopathFindingTextGenerator.cs
```

Takes a `List<SymptomFinding>` and generates grammatically correct German narrative text.

**Logic:**
- Group findings by category
- For severity 0: skip or mention "unauffaellig" for the category
- For severity 1-3: generate sentence with severity qualifier
- Example output: *"Bewusstsein klar, allseits orientiert. Formales Denken: leichte Verlangsamung, keine Denkstoerungen. Stimmung deutlich gedrueckt, Affekt eingeengt. Antrieb vermindert. Keine Suizidalitaet."*

Template-based approach with per-symptom sentence fragments stored in the catalog.

#### 17.4 Service Layer

```
MediPrax.Application/Interfaces/IPsychopathFindingService.cs
MediPrax.Application/Services/PsychopathFindingService.cs
```

| Method | Description |
|--------|-------------|
| `GetByEncounterAsync(Guid encounterId)` | Get finding for encounter |
| `GetByPatientAsync(Guid patientId)` | Get all findings for longitudinal view |
| `CreateAsync(CreatePsychopathFindingDto)` | Create new assessment |
| `UpdateAsync(UpdatePsychopathFindingDto)` | Update existing assessment |
| `GenerateNarrativeAsync(List<SymptomFinding>)` | Generate text from structured input |
| `CompareAsync(Guid findingId1, Guid findingId2)` | Compare two findings side-by-side |

#### 17.5 Blazor UI

```
MediPrax.Server/Components/Pages/Dokumentation/PsychopathBefund.razor
```

**Layout:**
1. **Category accordion** -- Each AMDP category is a collapsible section
2. **Symptom rows** -- Each symptom shows: Name | Severity selector (0/1/2/3 radio buttons) | Comment field
3. **Live narrative preview** -- Right panel or bottom section showing auto-generated text updating in real-time
4. **Manual override** -- Allow editing the generated narrative text directly
5. **Longitudinal view** -- Side-by-side comparison with previous finding (color-coded changes: improved=green, worsened=red, unchanged=grey)
6. **Quick presets** -- "Unauffaellig" button sets all to 0; "Depression typisch" preset; "Psychose typisch" preset

**Integration:**
- Accessible from `EncounterFormular.razor` via "Psychopathologischer Befund" button
- Finding is linked to the Encounter
- Auto-saved on each change (debounced)
- PDF inclusion in Arztbrief generator

#### 17.6 Database Migration

- Table: `psychopathological_findings`
- JSONB column for `Findings`
- Indexes: `EncounterId` (unique), `PatientId + AssessmentDate`

#### 17.7 Tests

- `PsychopathFindingServiceTests` -- CRUD operations
- `AmdpCatalogTests` -- Catalog search, completeness
- `PsychopathFindingTextGeneratorTests` -- Narrative generation for various symptom combinations

#### 17.8 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/PsychopathologicalFinding.cs` |
| Create | `MediPrax.Core/ValueObjects/SymptomFinding.cs` |
| Create | `MediPrax.Application/Catalogs/AmdpCatalog.cs` |
| Create | `MediPrax.Application/DTOs/PsychopathFindingDto.cs` |
| Create | `MediPrax.Application/Interfaces/IPsychopathFindingService.cs` |
| Create | `MediPrax.Application/Services/PsychopathFindingService.cs` |
| Create | `MediPrax.Application/Services/PsychopathFindingTextGenerator.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/PsychopathologicalFindingConfiguration.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/MediPraxDbContext.cs` -- Add DbSet + query filter |
| Create | `MediPrax.Server/Components/Pages/Dokumentation/PsychopathBefund.razor` |
| Modify | `MediPrax.Server/Components/Pages/Dokumentation/EncounterFormular.razor` -- Add link |
| Modify | `MediPrax.Server/Program.cs` -- Register service |
| Modify | `MediPrax.Reporting/Arztbrief/ArztbriefDocument.cs` -- Include finding in PDF |
| Create | `tests/MediPrax.IntegrationTests/PsychopathFindingServiceTests.cs` |
| Create | `tests/MediPrax.UnitTests/AmdpCatalogTests.cs` |
| Create | `tests/MediPrax.UnitTests/PsychopathFindingTextGeneratorTests.cs` |

---

