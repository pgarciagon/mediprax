# M21: Advanced Medication Management -- ABGESCHLOSSEN

> **Priority:** P1 | **Estimated Effort:** Large
> **Why:** Psychiatric polypharmacy is complex. Drug interactions, monitoring requirements, and depot management are patient safety concerns.

#### 21.1 Enhancements to Existing `Medication` Entity

Add new properties to `MediPrax.Core/Entities/Medication.cs`:

| Property | Type | Description |
|----------|------|-------------|
| `Category` | `MedicationCategory` | Enum: Antidepressant, Antipsychotic, etc. |
| `TargetDose` | `string?` | Target dosage (for titration) |
| `TitrationSchedule` | `string?` | Free text titration plan |
| `IsDepot` | `bool` | Depot/injection medication |
| `DepotIntervalDays` | `int?` | Injection interval (e.g., 28 days) |
| `LastDepotDate` | `DateOnly?` | Last injection date |
| `NextDepotDate` | `DateOnly?` | Computed: Last + Interval |
| `RequiresMonitoring` | `bool` | Triggers lab monitoring |
| `MonitoringType` | `string?` | E.g., "Lithium", "Clozapine", "Metabolic" |

#### 21.2 Enum: `MedicationCategory`

```csharp
public enum MedicationCategory
{
    Antidepressivum,
    Antipsychotikum,
    Stimmungsstabilisierer,
    Anxiolytikum,
    Hypnotikum,
    Stimulans,           // ADHD
    Antikonvulsivum,     // Antiepileptika
    Substitutionsmittel, // Opioid substitution
    Sonstiges
}
```

#### 21.3 Drug Interaction Service

```
MediPrax.Application/Services/DrugInteractionService.cs
```

**Approach:** Embedded rule-based system (not external API -- keeps it offline-capable):

```
MediPrax.Application/Catalogs/DrugInteractionCatalog.cs
```

| Interaction Type | Priority | Examples |
|-----------------|----------|---------|
| QTc prolongation | Critical | Haloperidol + Citalopram |
| Serotonin syndrome | Critical | SSRI + MAO inhibitor |
| CYP2D6 inhibition | Warning | Fluoxetine + Codeine |
| CYP3A4 induction | Warning | Carbamazepine + Oral contraceptives |
| Sedation potentiation | Info | Benzodiazepine + Opiate |
| Anticholinergic load | Warning | Amitriptyline + Biperiden |

**Method:** `CheckInteractionsAsync(Guid patientId) -> List<InteractionWarning>`
- Checks all active medications for the patient
- Returns severity-sorted list of interaction warnings
- Displayed on `PatientDetail.razor` medication section

#### 21.4 Monitoring Schedule Service

```
MediPrax.Application/Services/MedicationMonitoringService.cs
```

**Monitoring templates per medication:**

| Medication | Monitoring | Frequency |
|-----------|-----------|-----------|
| Lithium | Serum level, TSH, Creatinine, GFR | Every 3 months |
| Clozapine | WBC, Neutrophils | Weekly (first 18 weeks), then monthly |
| Valproate | Serum level, Liver function, Blood count | Every 3-6 months |
| Atypical antipsychotics | Weight, Glucose, HbA1c, Lipids | Every 3-6 months |
| Carbamazepine | Serum level, Blood count, Sodium, Liver | Every 3-6 months |

**Method:** `GetOverdueMonitoringAsync(Guid? patientId) -> List<MonitoringAlert>`
- Checks medication start dates and last monitoring dates
- Generates alerts: "Lithiumspiegel ueberfaellig (letzte Kontrolle: 15.01.2026)"
- Dashboard integration + patient detail alerts

#### 21.5 Depot Medication Tracking

- **Calendar view** of upcoming depot injections across all patients
- **Dashboard widget:** "Anstehende Depot-Injektionen diese Woche"
- **Recall integration:** Auto-create `Recall` when depot is due

#### 21.6 Bundeseinheitlicher Medikationsplan (BMP)

```
MediPrax.Reporting/Medikationsplan/BmpDocument.cs
```

Generate BMP per KBV specification:
- Standard A4 format with structured medication table
- QR code containing medication data (per BMP specification 2.6+)
- Columns: Wirkstoff | Handelsname | Staerke | Form | Morgens | Mittags | Abends | Nachts | Hinweis
- Auto-generated from active medications

#### 21.7 Files to Create/Modify

| Action | File |
|--------|------|
| Modify | `MediPrax.Core/Entities/Medication.cs` -- Add new properties |
| Create | `MediPrax.Core/Enums/MedicationCategory.cs` |
| Create | `MediPrax.Application/Catalogs/DrugInteractionCatalog.cs` |
| Create | `MediPrax.Application/Services/DrugInteractionService.cs` |
| Create | `MediPrax.Application/Services/MedicationMonitoringService.cs` |
| Create | `MediPrax.Application/DTOs/InteractionWarningDto.cs` |
| Create | `MediPrax.Application/DTOs/MonitoringAlertDto.cs` |
| Modify | `MediPrax.Application/Services/MedicationService.cs` -- Integrate checks |
| Create | `MediPrax.Reporting/Medikationsplan/BmpDocument.cs` |
| Create | `MediPrax.Reporting/Medikationsplan/BmpModel.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/Configurations/MedicationConfiguration.cs` |
| Modify | `MediPrax.Server/Components/Pages/Patienten/PatientDetail.razor` -- Show warnings |
| Create | `MediPrax.Server/Components/Pages/Medikation/DepotUebersicht.razor` |
| Modify | `MediPrax.Server/Program.cs` |
| Create | `tests/MediPrax.UnitTests/DrugInteractionCatalogTests.cs` |
| Create | `tests/MediPrax.IntegrationTests/MedicationMonitoringServiceTests.cs` |

---

