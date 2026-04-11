# M35: BtM Management & E-BtM-Rezept -- ABGESCHLOSSEN

> **Priority:** P3 | **Estimated Effort:** Medium
> **Why:** Controlled substances (Methylphenidat for ADHD, Benzodiazepines, medical Cannabis) require strict documentation per BtMVV.

#### 35.1 Data Model

##### Entity: `BtmPrescription`

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `PatientId` | `Guid` | FK |
| `PrescribedById` | `Guid` | FK |
| `PrescriptionDate` | `DateOnly` | |
| `MedicationName` | `string` | |
| `Pzn` | `string?` | |
| `Substance` | `string` | Controlled substance name |
| `Amount` | `string` | Prescribed amount |
| `Dosierung` | `string` | Dosage instruction |
| `BtmRecipeNumber` | `string` | BtM recipe number (7-digit) |
| `PrescriberBtmNumber` | `string` | Doctor's BtM number |
| `IsEBtm` | `bool` | Electronic BtM prescription |
| `Notes` | `string?` | |

#### 35.2 Hoechstmengen Enforcement

```
MediPrax.Application/Services/BtmComplianceService.cs
```

Per BtMVV, check 30-day maximum quantities:

| Substance | Max per 30 days |
|-----------|----------------|
| Methylphenidat | 2640 mg |
| Dexamfetamin | 1320 mg |
| Diazepam | Varies per indication |
| Lorazepam | Varies |
| Cannabis (Blueten) | 100 g |

- Warn if prescribing would exceed limits
- Cross-patient BtM overview for compliance auditing
- 3-year retention of BtM Part III documentation

#### 35.3 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/BtmPrescription.cs` |
| Create | `MediPrax.Application/Services/BtmComplianceService.cs` |
| Create | `MediPrax.Application/DTOs/BtmPrescriptionDto.cs` |
| Create | `MediPrax.Application/Catalogs/BtmSubstanceCatalog.cs` |
| Create | `MediPrax.Server/Components/Pages/Medikation/BtmVerwaltung.razor` |
| + EF config, DbContext, DI, tests |

---

