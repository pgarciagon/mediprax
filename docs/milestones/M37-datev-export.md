# M37: DATEV-Export Interface -- AUSSTEHEND

> **Priority:** P3 | **Estimated Effort:** Small
> **Why:** Most practices use a Steuerberater who works with DATEV. Automated export saves manual bookkeeping and reduces errors.

#### 37.1 Features

- **DATEV-Format Export:** Generate DATEV-compatible CSV (Buchungsstapel format)
- **Monthly/quarterly export:** Select period, generate file
- **Mapping:** GOP-Einnahmen, Privatrechnungen, Zahlungseingaenge mapped to DATEV Kontenrahmen SKR03/SKR04
- **Categories:** Separate accounts for GKV-Einnahmen, PKV-Einnahmen, Mahngebuehren
- **Download:** CSV file ready for Steuerberater import

#### 37.2 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Application/Services/DatevExportService.cs` |
| Create | `MediPrax.Application/DTOs/DatevExportDto.cs` |
| Create | `MediPrax.Server/Components/Pages/Verwaltung/DatevExport.razor` |
| Modify | `MediPrax.Server/Program.cs` (export endpoint) |

---

