# M22: Laboratory Integration & Monitoring -- ABGESCHLOSSEN

> **Priority:** P1 | **Estimated Effort:** Medium-Large
> **Why:** Psychiatric medication monitoring requires regular lab tests. Without lab integration, results must be manually transcribed, risking errors and missed critical values.

#### 22.1 Data Model

##### Entity: `LabResult`

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `PatientId` | `Guid` | FK -> Patient |
| `OrderedById` | `Guid?` | FK -> User |
| `LabName` | `string` | Laboratory name |
| `OrderDate` | `DateOnly` | When ordered |
| `ResultDate` | `DateOnly?` | When results arrived |
| `Status` | `LabResultStatus` | Ordered, Received, Reviewed |
| `ReviewedById` | `Guid?` | FK -> User who reviewed |
| `ReviewedAt` | `DateTime?` | When reviewed |
| `Values` | `List<LabValue>` | JSONB -- individual lab values |
| `Notes` | `string?` | Clinician notes |
| `ImportSource` | `string?` | "LDT" or "Manual" |

##### Value Object: `LabValue` (JSONB)

| Property | Type | Description |
|----------|------|-------------|
| `ParameterName` | `string` | E.g., "Lithium", "TSH", "Leukozyten" |
| `ParameterCode` | `string?` | Lab code |
| `Value` | `string` | Result value |
| `Unit` | `string` | Unit (e.g., "mmol/l", "mcg/l") |
| `ReferenceRangeLow` | `decimal?` | Normal range lower bound |
| `ReferenceRangeHigh` | `decimal?` | Normal range upper bound |
| `IsAbnormal` | `bool` | Outside reference range |
| `IsCritical` | `bool` | Critical value requiring immediate attention |
| `Category` | `string?` | E.g., "Blutbild", "Schilddruese", "Niere" |

#### 22.2 LDT Import (Labordatentransfer)

```
MediPrax.Application/Services/LdtImportService.cs
```

- Parse LDT 3.0 file format (fixed-width text file, standard from KBV/DKG)
- Map patient by name + DOB or KVNR
- Map lab parameters to LabValue
- Flag abnormal and critical values
- Create LabResult entity from imported data

**LDT file upload endpoint:**
```csharp
app.MapPost("/api/labor/ldt-import", async (IFormFile file, ILdtImportService service) =>
{
    var result = await service.ImportAsync(file.OpenReadStream());
    return Results.Ok(result); // ImportSummary { Imported, Errors, Warnings }
}).RequireAuthorization("Klinisch");
```

#### 22.3 Lab Monitoring Templates

```
MediPrax.Application/Catalogs/LabMonitoringCatalog.cs
```

Pre-defined monitoring panels per medication:

```csharp
new MonitoringPanel("Lithium", [
    new("Lithiumspiegel", "mmol/l", 0.6m, 0.8m, IntervalDays: 90),
    new("TSH", "mU/l", 0.4m, 4.0m, IntervalDays: 180),
    new("fT3", "pg/ml", 2.0m, 4.4m, IntervalDays: 180),
    new("fT4", "ng/dl", 0.93m, 1.7m, IntervalDays: 180),
    new("Kreatinin", "mg/dl", 0.7m, 1.2m, IntervalDays: 90),
    new("GFR", "ml/min", 60m, null, IntervalDays: 90)
]),
new MonitoringPanel("Clozapin", [
    new("Leukozyten", "/mcl", 3500m, 10000m, IntervalDays: 7), // First 18 weeks
    new("Neutrophile", "/mcl", 1500m, null, IntervalDays: 7),
    // After 18 weeks: IntervalDays: 30
]),
// ... Valproat, Carbamazepin, Atypische Antipsychotika
```

#### 22.4 Blazor UI

##### Lab Results Page

```
MediPrax.Server/Components/Pages/Labor/Laborergebnisse.razor
```

- Lab result list per patient
- Color-coded values (normal=green, abnormal=yellow, critical=red)
- "Zur Kenntnis genommen" (acknowledged) button with user tracking
- Trend charts for selected parameters over time

##### Lab Import Page

```
MediPrax.Server/Components/Pages/Labor/LdtImport.razor
```

- File upload for LDT files
- Preview imported values before saving
- Patient matching confirmation
- Import history log

##### Lab Monitoring Dashboard Widget

- On Home dashboard: "Ueberfaellige Laborkontrollen" alert
- On PatientDetail: Lab monitoring status per medication

#### 22.5 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/LabResult.cs` |
| Create | `MediPrax.Core/Enums/LabResultStatus.cs` |
| Create | `MediPrax.Core/ValueObjects/LabValue.cs` |
| Create | `MediPrax.Application/Catalogs/LabMonitoringCatalog.cs` |
| Create | `MediPrax.Application/DTOs/LabResultDto.cs` |
| Create | `MediPrax.Application/Interfaces/ILabResultService.cs` |
| Create | `MediPrax.Application/Services/LabResultService.cs` |
| Create | `MediPrax.Application/Services/LdtImportService.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/LabResultConfiguration.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/MediPraxDbContext.cs` |
| Create | `MediPrax.Server/Components/Pages/Labor/Laborergebnisse.razor` |
| Create | `MediPrax.Server/Components/Pages/Labor/LdtImport.razor` |
| Modify | `MediPrax.Server/Components/Pages/Patienten/PatientDetail.razor` |
| Modify | `MediPrax.Server/Components/Pages/Home.razor` |
| Modify | `MediPrax.Server/Program.cs` |
| Create | `tests/MediPrax.IntegrationTests/LabResultServiceTests.cs` |
| Create | `tests/MediPrax.UnitTests/LdtImportServiceTests.cs` |

---

