# M38: Umsatzstatistik / Revenue Analytics -- AUSSTEHEND

> **Priority:** P3 | **Estimated Effort:** Medium
> **Why:** Practice owners need financial overview: revenue by period, by doctor, by insurance type. Essential for business decisions and KV-Abrechnung verification.

#### 38.1 Features

- **Dashboard page** with charts and KPIs:
  - Umsatz pro Quartal (bar chart)
  - Umsatz pro Arzt (pie chart)
  - GKV vs. PKV Verteilung
  - Top-10 GOPs by frequency and revenue
  - Offene Privatrechnungen total
  - Vergleich zum Vorquartal
- **Filters:** Period (Quartal/Monat/Jahr), Doctor, Insurance type
- **Export:** CSV download of all data
- **Praxis-Kennzahlen:** Fallzahl, Scheinzahl, Fallwert, Punktzahlvolumen

#### 38.2 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Application/Services/RevenueStatisticsService.cs` |
| Create | `MediPrax.Application/DTOs/RevenueStatisticsDto.cs` |
| Create | `MediPrax.Server/Components/Pages/Berichte/Umsatzstatistik.razor` |
| Modify | `MediPrax.Server/Components/Pages/Home.razor` (summary KPIs) |

---

