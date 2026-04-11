# M36: Automated Mahnwesen -- AUSSTEHEND

> **Priority:** P3 | **Estimated Effort:** Small-Medium
> **Why:** Overdue private invoices require systematic follow-up. Manual tracking wastes MFA time and loses revenue.

#### 36.1 Features

- **Automated overdue detection:** Daily background check for invoices past DueDate
- **3-stage reminder workflow:**
  - 1. Zahlungserinnerung (friendly reminder) after 14 days overdue
  - 2. Erste Mahnung (first formal reminder) after 30 days overdue
  - 3. Zweite Mahnung (with Mahngebuehr notice) after 45 days overdue
- **Reminder PDF generation:** Each stage generates a formal letter (QuestPDF)
- **Dashboard widget:** "Offene Rechnungen: X EUR, davon Y ueberfaellig"
- **Bulk actions:** Send all pending reminders at once
- **Tracking:** ReminderCount, LastReminderDate on PrivateInvoice (already exists)

#### 36.2 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Server/Services/MahnwesenBackgroundService.cs` (daily check) |
| Create | `MediPrax.Reporting/Rechnung/MahnungDocument.cs` (PDF for each stage) |
| Modify | `MediPrax.Application/Services/PrivateInvoiceService.cs` (reminder logic) |
| Modify | `MediPrax.Server/Components/Pages/Abrechnung/Privatrechnungen.razor` (bulk actions) |
| Modify | `MediPrax.Server/Components/Pages/Home.razor` (dashboard widget) |

---

