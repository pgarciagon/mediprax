# M33: Private Billing GOAe -- TEILWEISE

> **Priority:** P3 | **Estimated Effort:** Medium
> **Why:** PKV patients require proper invoices per GOAe Paragraph 12. Payment tracking and Mahnwesen are essential for revenue management.

#### 33.1 Data Model

##### Entity: `PrivateInvoice`

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `PatientId` | `Guid` | FK |
| `InvoiceNumber` | `string` | Sequential: "RE-2026-0001" |
| `InvoiceDate` | `DateOnly` | |
| `DueDate` | `DateOnly` | +30 days default |
| `Items` | `List<InvoiceItem>` | JSONB |
| `TotalNet` | `decimal` | Total before tax |
| `TotalGross` | `decimal` | Total (no VAT for medical services) |
| `Status` | `InvoiceStatus` | Draft, Sent, PartiallyPaid, Paid, Overdue, Cancelled |
| `PaidAmount` | `decimal` | Amount received |
| `PaidDate` | `DateOnly?` | Last payment date |
| `PdfData` | `byte[]?` | Generated invoice PDF |
| `ReminderCount` | `int` | Number of reminders sent |
| `LastReminderDate` | `DateOnly?` | |

##### Value Object: `InvoiceItem`

| Property | Type |
|----------|------|
| `GoaNumber` | `string` (GOAe number) |
| `Description` | `string` |
| `Date` | `DateOnly` |
| `Factor` | `decimal` (Steigerungsfaktor: 1.0-3.5) |
| `BaseAmount` | `decimal` |
| `TotalAmount` | `decimal` (Base x Factor) |
| `Justification` | `string?` (required if Factor > 2.3) |

#### 33.2 Invoice PDF Generation

```
MediPrax.Reporting/Rechnung/PrivateInvoiceDocument.cs
```

Required per GOAe Paragraph 12:
- Doctor name, address, tax ID
- Patient name, address
- Invoice number, date
- Per item: date, GOAe number, description, factor, amount
- Total
- Payment terms
- Bank details

#### 33.3 Mahnwesen (Payment Reminders)

- Automated overdue detection (daily check)
- 1st reminder after 30 days
- 2nd reminder after 45 days
- 3rd reminder (Mahnung) after 60 days with late fee notice
- Dashboard widget: "Offene Rechnungen: X EUR"

#### 33.4 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/PrivateInvoice.cs` |
| Create | `MediPrax.Core/Enums/InvoiceStatus.cs` |
| Create | `MediPrax.Application/DTOs/PrivateInvoiceDto.cs` |
| Create | `MediPrax.Application/Services/PrivateInvoiceService.cs` |
| Create | `MediPrax.Reporting/Rechnung/PrivateInvoiceDocument.cs` |
| Create | `MediPrax.Server/Components/Pages/Abrechnung/Privatrechnungen.razor` |
| Create | `MediPrax.Server/Components/Pages/Abrechnung/RechnungFormular.razor` |
| + EF config, DbContext, DI, tests, endpoints |

---

