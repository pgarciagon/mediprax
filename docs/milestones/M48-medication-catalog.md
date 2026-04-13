# M48: Medication Catalog & Prescription Search (Arzneimittelkatalog) -- AUSSTEHEND

> **Priority:** P1 | **Estimated Effort:** Large
> **Why:** Every certified German PVS provides medication search by name, Wirkstoff, or PZN
> against a comprehensive drug database. MediPrax currently has free-text medication entry
> with no catalog, no autocomplete, and no validation. This is the foundation for correct
> prescriptions, E-Rezept FHIR resources, interaction checks, and eventual AVWG certification.

---

## 48.1 Problem Statement

Current state in MediPrax:

- `Medication.Name` and `Prescription.MedicationName` are free-text strings
- `Medication.Pzn` and `Prescription.MedicationPzn` are optional free-text — no validation
- `Medication.Wirkstoff` is free-text — no standardized active substance lookup
- No medication autocomplete or search in the UI
- No ATC classification
- No price information or Festbetrag data
- No Darreichungsform (dosage form) standardization
- No Packungsgroesse (package size) data
- The E-Rezept page has a plain text field for PZN with no lookup

This means:
- Typos in medication names go undetected
- No Wirkstoff-based search ("show me all SSRIs")
- No basis for interaction checks (M21 has monitoring but no drug-drug interactions)
- AVWG certification (KBV) impossible without a certified medication database

## 48.2 Data Source Strategy

### Phase A: Development & Validation (free data)

Three complementary free sources, combined into a local catalog:

#### Source 1: BfArM Terminologieserver — ATC + ASK (FHIR)

- **URL:** https://terminologien.bfarm.de
- **Format:** FHIR R4 CodeSystem (JSON)
- **Content:**
  - ATC-DE (Anatomisch-Therapeutisch-Chemische Klassifikation, German edition)
  - ASK (Arzneimittel-Stoffkatalog) — standardized substance identifiers
- **License:** Free for use in healthcare software
- **Integration:** Download FHIR packages, parse with Firely SDK (already in project via M16)

#### Source 2: BfArM AMIce Public Part (PharmNet.Bund)

- **URL:** https://www.pharmnet-bund.de
- **Content:** Administrative data on all approved medications in Germany
  - Trade name (Handelsname)
  - Active substance (Wirkstoff)
  - Dosage form (Darreichungsform)
  - Manufacturer (Zulassungsinhaber)
  - Approval status
- **Limitation:** No PZN per individual package, no prices
- **License:** Free public access

#### Source 3: Curated Psychiatry/Neurology Seed Data

A hand-curated catalog of ~300 most-prescribed medications in psychiatry and neurology,
with PZN, Wirkstoff, ATC, Staerke, Packungsgroesse, and Darreichungsform. This covers
the daily reality of the pilot practice (Neuropsychiatricum Bremen).

Categories to cover:
- Antidepressiva (SSRIs, SNRIs, Trizyklische, MAO-Hemmer)
- Antipsychotika (typische, atypische)
- Stimmungsstabilisierer (Lithium, Valproat, Carbamazepin, Lamotrigin)
- Anxiolytika & Hypnotika (Benzodiazepine, Z-Substanzen)
- Antikonvulsiva (Levetiracetam, Lacosamid, etc.)
- Parkinson-Medikamente (L-Dopa, Dopaminagonisten)
- MS-Therapie (Disease-Modifying Therapies)
- Schmerzmittel (Triptane, Antikonvulsiva bei Neuropathie)
- BtM (Methylphenidat, Opioide — marked as IsBtm)

### Phase B: Certification (commercial license)

For AVWG certification (KBV Anlage 23 BMV-Ae), one of these must be licensed:

- **ifap praxisCENTER** (market leader, 60+ PVS partners)
  - Integration: ifap webSERVICE (XML/HTTPS REST)
  - Content: Complete PZN catalog, prices, Festbetraege, interactions, Aut-idem
  - Update cycle: biweekly (as required by AVWG)
- **MMI Pharmindex Pro** (alternative)
  - Integration: REST and SOAP APIs
  - Content: Gelbe Liste data, equivalent coverage
- **data4doc** (used by psyprax as ifap alternative)

The local catalog (Phase A) is designed so that the service interface (`IMedicationCatalogService`)
stays identical — only the implementation swaps from local SQLite/JSON to commercial API.

## 48.3 Data Model

### New Entity: `MedicationCatalogEntry`

```
MediPrax.Core/Entities/MedicationCatalogEntry.cs
```

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK (BaseEntity) |
| `Pzn` | `string` | Pharmazentralnummer (8 digits, unique) |
| `Handelsname` | `string` | Trade name (e.g., "Sertralin HEXAL") |
| `Wirkstoff` | `string` | Active substance (e.g., "Sertralin") |
| `WirkstoffAsk` | `string?` | ASK number (BfArM substance catalog) |
| `AtcCode` | `string?` | ATC classification (e.g., "N06AB06") |
| `Staerke` | `string?` | Strength (e.g., "100 mg") |
| `Darreichungsform` | `string?` | Dosage form (e.g., "Filmtabletten") |
| `DarreichungsformCode` | `string?` | EDQM standard code |
| `Packungsgroesse` | `string?` | Package size (e.g., "100 St") |
| `NormPackungsgroesse` | `string?` | N1, N2, N3 |
| `Hersteller` | `string?` | Manufacturer / MAH |
| `IsBtm` | `bool` | Controlled substance (Betaeubungsmittel) |
| `IsTRezeptPflichtig` | `bool` | Requires T-Rezept (e.g., Thalidomid, Lenalidomid) |
| `IsApothekenpflichtig` | `bool` | Pharmacy-only |
| `IsVerschreibungspflichtig` | `bool` | Prescription-required |
| `Festbetrag` | `decimal?` | Fixed amount (Festbetrag) in EUR |
| `Avp` | `decimal?` | Pharmacy retail price (Apothekenverkaufspreis) |
| `Category` | `MedicationCategory?` | Reuse existing enum (Antidepressivum, etc.) |
| `IsActive` | `bool` | Currently marketed (vs. withdrawn) |
| `DataSource` | `string` | "SEED", "BFARM_ATC", "BFARM_AMICE", "IFAP", "MMI" |
| `LastUpdated` | `DateTime` | When this entry was last refreshed |

**Index:** Unique on `Pzn`. Additional indexes on `Wirkstoff`, `AtcCode`, `Handelsname`.

### New Enum: None needed

Reuses existing `MedicationCategory` enum from M21.

### No changes to existing entities

`Medication` and `Prescription` keep their current fields. The catalog provides
lookup/autocomplete — the selected entry populates the existing fields.

## 48.4 Service Layer

### `IMedicationCatalogService`

```
MediPrax.Application/Interfaces/IMedicationCatalogService.cs
```

```csharp
public interface IMedicationCatalogService
{
    /// <summary>
    /// Search by trade name, active substance, PZN, or ATC code.
    /// Returns max 20 results, ordered by relevance.
    /// </summary>
    Task<IReadOnlyList<MedicationCatalogEntryDto>> SearchAsync(
        string term, int maxResults = 20, CancellationToken ct = default);

    /// <summary>
    /// Exact lookup by PZN.
    /// </summary>
    Task<MedicationCatalogEntryDto?> GetByPznAsync(
        string pzn, CancellationToken ct = default);

    /// <summary>
    /// All entries for a given active substance (e.g., all Sertralin preparations).
    /// </summary>
    Task<IReadOnlyList<MedicationCatalogEntryDto>> GetByWirkstoffAsync(
        string wirkstoff, CancellationToken ct = default);

    /// <summary>
    /// All entries matching an ATC prefix (e.g., "N06AB" for all SSRIs).
    /// </summary>
    Task<IReadOnlyList<MedicationCatalogEntryDto>> GetByAtcPrefixAsync(
        string atcPrefix, CancellationToken ct = default);

    /// <summary>
    /// Statistics about catalog coverage.
    /// </summary>
    Task<CatalogStatisticsDto> GetStatisticsAsync(CancellationToken ct = default);
}
```

### `MedicationCatalogService` (local implementation)

```
MediPrax.Application/Services/MedicationCatalogService.cs
```

Queries `MedicationCatalogEntry` from the local database. Search logic:

```csharp
public async Task<IReadOnlyList<MedicationCatalogEntryDto>> SearchAsync(
    string term, int maxResults = 20, CancellationToken ct = default)
{
    var normalized = term.Trim().ToLowerInvariant();

    // 1. Exact PZN match (if term is 7-8 digits)
    if (Regex.IsMatch(normalized, @"^\d{7,8}$"))
    {
        var byPzn = await GetByPznAsync(normalized, ct);
        if (byPzn is not null) return [byPzn];
    }

    // 2. Prefix match on Handelsname, Wirkstoff, AtcCode
    return await context.MedicationCatalogEntries
        .Where(e => e.IsActive &&
            (EF.Functions.ILike(e.Handelsname, $"{normalized}%") ||
             EF.Functions.ILike(e.Wirkstoff, $"{normalized}%") ||
             EF.Functions.ILike(e.AtcCode!, $"{normalized}%") ||
             EF.Functions.ILike(e.Handelsname, $"%{normalized}%") ||
             EF.Functions.ILike(e.Wirkstoff, $"%{normalized}%")))
        .OrderByDescending(e =>
            EF.Functions.ILike(e.Handelsname, $"{normalized}%") ||
            EF.Functions.ILike(e.Wirkstoff, $"{normalized}%"))
        .ThenBy(e => e.Handelsname)
        .Take(maxResults)
        .Select(e => MapToDto(e))
        .ToListAsync(ct);
}
```

### Future: `IfapMedicationCatalogService` (Phase B)

Same interface, but calls ifap webSERVICE (XML/HTTPS) instead of local DB.
Registered via DI configuration flag:

```csharp
if (config.UseIfapCatalog)
    builder.Services.AddScoped<IMedicationCatalogService, IfapMedicationCatalogService>();
else
    builder.Services.AddScoped<IMedicationCatalogService, MedicationCatalogService>();
```

## 48.5 Seed Data

### Migration with Curated Seed

The EF Core migration includes seed data as raw SQL with fixed GUIDs (per project convention).

**Seed structure:** ~300 entries covering the most common medications in the pilot practice.

Example entries:

```sql
INSERT INTO medication_catalog_entries (id, pzn, handelsname, wirkstoff, atc_code, staerke,
    darreichungsform, packungsgroesse, norm_packungsgroesse, hersteller,
    is_btm, is_verschreibungspflichtig, category, is_active, data_source, last_updated)
VALUES
-- SSRIs
('...guid...', '00955092', 'Sertralin HEXAL 50mg', 'Sertralin', 'N06AB06', '50 mg',
 'Filmtabletten', '100 St', 'N3', 'Hexal AG', false, true, 0, true, 'SEED', NOW()),
('...guid...', '00955107', 'Sertralin HEXAL 100mg', 'Sertralin', 'N06AB06', '100 mg',
 'Filmtabletten', '100 St', 'N3', 'Hexal AG', false, true, 0, true, 'SEED', NOW()),
('...guid...', '02293819', 'Cipralex 10mg', 'Escitalopram', 'N06AB10', '10 mg',
 'Filmtabletten', '100 St', 'N3', 'Lundbeck GmbH', false, true, 0, true, 'SEED', NOW()),

-- Antipsychotics
('...guid...', '04869065', 'Risperdal 2mg', 'Risperidon', 'N05AX08', '2 mg',
 'Filmtabletten', '100 St', 'N3', 'Janssen-Cilag', false, true, 1, true, 'SEED', NOW()),
('...guid...', '01223756', 'Zyprexa 10mg', 'Olanzapin', 'N05AH03', '10 mg',
 'Filmtabletten', '70 St', 'N3', 'Lilly Deutschland', false, true, 1, true, 'SEED', NOW()),

-- Mood stabilizers
('...guid...', '00654387', 'Quilonum retard 450mg', 'Lithiumcarbonat', 'N05AN01', '450 mg',
 'Retardtabletten', '100 St', 'N3', 'GlaxoSmithKline', false, true, 2, true, 'SEED', NOW()),

-- Anticonvulsants
('...guid...', '02837862', 'Keppra 500mg', 'Levetiracetam', 'N03AX14', '500 mg',
 'Filmtabletten', '200 St', 'N3', 'UCB Pharma', false, true, 6, true, 'SEED', NOW()),

-- BtM
('...guid...', '02084907', 'Ritalin 10mg', 'Methylphenidat', 'N06BA04', '10 mg',
 'Tabletten', '50 St', 'N2', 'Novartis', true, true, 5, true, 'SEED', NOW()),
-- ...
```

### BfArM ATC Import Tool

A one-time import command or admin action that:

1. Downloads ATC-DE FHIR CodeSystem from terminologien.bfarm.de
2. Parses with Firely SDK (`CodeSystem` resource)
3. Creates/updates `MedicationCatalogEntry` records for each ATC leaf node
4. Maps ATC hierarchy for prefix-based search (e.g., "N06AB" = all SSRIs)

```csharp
public interface IMedicationCatalogImportService
{
    /// <summary>
    /// Import ATC classification from BfArM FHIR package.
    /// </summary>
    Task<int> ImportAtcFromFhirAsync(Stream fhirPackage, CancellationToken ct = default);

    /// <summary>
    /// Import curated seed data from embedded JSON resource.
    /// </summary>
    Task<int> ImportSeedDataAsync(CancellationToken ct = default);
}
```

## 48.6 Blazor UI

### New: Medication Search Component (Reusable)

```
MediPrax.Server/Components/Shared/MedicationSearch.razor
```

Reusable autocomplete component for medication lookup:

```razor
<MedicationSearch @bind-SelectedMedication="selectedMed"
                  Placeholder="Medikament suchen (Name, Wirkstoff oder PZN)..."
                  ShowDetails="true" />
```

**Behavior:**
1. User types >= 2 characters
2. Debounced search (300ms) calls `MedicationCatalogService.SearchAsync()`
3. Dropdown shows results: `Handelsname | Wirkstoff | Staerke | Packung | PZN`
4. BtM entries marked with warning badge
5. Click selects and populates bound property
6. Tab/Enter confirms selection

**Result display format:**

```
┌──────────────────────────────────────────────────────┐
│ 🔍 Sertra...                                         │
├──────────────────────────────────────────────────────┤
│ Sertralin HEXAL 50mg          Sertralin    00955092  │
│ Filmtabletten, 100 St (N3)   Hexal AG               │
├──────────────────────────────────────────────────────┤
│ Sertralin HEXAL 100mg         Sertralin    00955107  │
│ Filmtabletten, 100 St (N3)   Hexal AG               │
├──────────────────────────────────────────────────────┤
│ Sertralin-ratiopharm 50mg     Sertralin    03897174  │
│ Filmtabletten, 100 St (N3)   ratiopharm             │
└──────────────────────────────────────────────────────┘
```

### Modified: Medication Plan Page

```
MediPrax.Server/Components/Pages/Patienten/Medikationsplan.razor
```

Replace free-text `Name` input with `MedicationSearch` component.
On selection, auto-fill: Name, Wirkstoff, Pzn, Staerke, Darreichungsform, IsBtm.

### Modified: E-Rezept Page

```
MediPrax.Server/Components/Pages/Telematik/ERezept.razor
```

Replace free-text PZN/Name fields with `MedicationSearch` component.
On selection, auto-fill PZN, name, and mark BtM flag.

### Modified: Prescription creation (wherever prescriptions are created)

Integrate `MedicationSearch` for medication selection.

### New: Catalog Admin Page (optional)

```
MediPrax.Server/Components/Pages/Verwaltung/Arzneimittelkatalog.razor
```

Route: `/verwaltung/arzneimittelkatalog`

- Browse catalog with search and filters (by category, ATC prefix, BtM only)
- Show statistics (total entries, by category, by data source)
- Manual add/edit for custom entries
- Import button for BfArM FHIR data
- Last update timestamp per data source

## 48.7 FHIR Integration (E-Rezept)

When creating an E-Rezept FHIR `MedicationRequest` resource (M16/Phase 3 TI), the catalog
provides standardized data:

```csharp
// Current: free-text, error-prone
new Medication { Code = new CodeableConcept("http://fhir.de/CodeSystem/ifa/pzn", pzn) }

// With catalog: validated, complete
var entry = await catalogService.GetByPznAsync(pzn);
var medication = new Medication
{
    Code = new CodeableConcept("http://fhir.de/CodeSystem/ifa/pzn", entry.Pzn, entry.Handelsname),
    Form = new CodeableConcept("http://standardterms.edqm.eu", entry.DarreichungsformCode, entry.Darreichungsform),
    Ingredient =
    {
        new() { Item = new CodeableConcept("http://fhir.de/CodeSystem/ask", entry.WirkstoffAsk, entry.Wirkstoff) }
    }
};
```

## 48.8 Interaction Check Foundation

The `AtcCode` field enables future drug-drug interaction checks:

- ATC-based interaction rules (e.g., two drugs with same ATC = therapeutic duplication)
- Wirkstoff-based contraindication lookup
- Preparation for ifap/MMI interaction database in Phase B

This does NOT implement interaction checking — only provides the data foundation.

## 48.9 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/MedicationCatalogEntry.cs` |
| Create | `MediPrax.Application/DTOs/MedicationCatalogEntryDto.cs` |
| Create | `MediPrax.Application/DTOs/CatalogStatisticsDto.cs` |
| Create | `MediPrax.Application/Interfaces/IMedicationCatalogService.cs` |
| Create | `MediPrax.Application/Interfaces/IMedicationCatalogImportService.cs` |
| Create | `MediPrax.Application/Services/MedicationCatalogService.cs` |
| Create | `MediPrax.Application/Services/MedicationCatalogImportService.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/MedicationCatalogEntryConfiguration.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/MediPraxDbContext.cs` — add DbSet |
| Create | `MediPrax.Server/Components/Shared/MedicationSearch.razor` |
| Modify | `MediPrax.Server/Components/Pages/Patienten/Medikationsplan.razor` — use search |
| Modify | `MediPrax.Server/Components/Pages/Telematik/ERezept.razor` — use search |
| Create | `MediPrax.Server/Components/Pages/Verwaltung/Arzneimittelkatalog.razor` |
| Modify | `MediPrax.Server/Program.cs` — DI registration |
| Create | EF Core migration with seed data (~300 entries) |
| Create | `tests/MediPrax.IntegrationTests/MedicationCatalogServiceTests.cs` |
| Create | `tests/MediPrax.UnitTests/MedicationSearchTests.cs` |

## 48.10 Dependencies

- **M21 (Medication Management):** Already implemented — provides `MedicationCategory` enum
  and the `Medication` entity that the catalog populates
- **M16 (FHIR/Firely SDK):** Already implemented — used for parsing BfArM FHIR packages
- **M35 (BtM Management):** Already implemented — catalog marks BtM entries for special
  prescription handling

No blocking dependencies. Can be implemented independently.

## 48.11 Migration Path to Commercial Database

The `IMedicationCatalogService` interface is designed for a clean swap:

```
Phase A (now):
  IMedicationCatalogService → MedicationCatalogService (local PostgreSQL)
  Data: seed + BfArM FHIR imports
  Cost: free

Phase B (certification):
  IMedicationCatalogService → IfapMedicationCatalogService (ifap webSERVICE)
  Data: complete German drug market, biweekly updates
  Cost: commercial license (practice operational cost)

Transition:
  - Same interface, same DTOs, same UI
  - Only the DI registration changes
  - Local catalog can serve as fallback/cache
```

## 48.12 Acceptance Criteria

- [ ] MedicationCatalogEntry entity with PZN, Wirkstoff, ATC, Staerke, Darreichungsform
- [ ] Search by name, Wirkstoff, PZN, and ATC prefix with autocomplete
- [ ] Reusable MedicationSearch Blazor component
- [ ] Integrated in Medikationsplan page (replaces free-text name input)
- [ ] Integrated in E-Rezept page (replaces free-text PZN input)
- [ ] Seed data with ~300 common psych/neuro medications
- [ ] BfArM ATC FHIR import tool
- [ ] Catalog admin page with browse, search, stats, import
- [ ] BtM entries visually marked in search results
- [ ] FHIR MedicationRequest can be populated from catalog data
- [ ] Interface designed for future swap to ifap/MMI commercial API

---

## Appendix: Medication Categories for Seed Data

| Category | ATC Prefix | Example Wirkstoffe | Estimated Count |
|----------|-----------|-------------------|-----------------|
| Antidepressiva | N06A | Sertralin, Escitalopram, Venlafaxin, Mirtazapin, Amitriptylin | ~60 |
| Antipsychotika | N05A | Risperidon, Olanzapin, Quetiapin, Aripiprazol, Clozapin | ~50 |
| Stimmungsstabilisierer | N05AN, N03AG | Lithium, Valproat, Carbamazepin, Lamotrigin | ~20 |
| Anxiolytika | N05BA/BB | Lorazepam, Diazepam, Oxazepam, Zopiclon, Zolpidem | ~25 |
| Antikonvulsiva | N03A | Levetiracetam, Lacosamid, Topiramat, Gabapentin | ~40 |
| Parkinson | N04 | L-Dopa/Benserazid, Pramipexol, Ropinirol, Entacapon | ~30 |
| MS-Therapie | L04AA/AX | Dimethylfumarat, Fingolimod, Natalizumab, Ocrelizumab | ~20 |
| Schmerz/Migraene | N02C, N02A | Sumatriptan, Rizatriptan, Pregabalin | ~25 |
| Stimulanzien (BtM) | N06BA | Methylphenidat, Lisdexamfetamin | ~15 |
| Sonstige | various | Melatonin, Biperiden, Promethazin | ~15 |
| **Total** | | | **~300** |
