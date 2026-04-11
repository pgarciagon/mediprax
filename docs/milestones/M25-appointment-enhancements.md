# M25: Appointment Enhancements -- TEILWEISE

> **Priority:** P2 | **Estimated Effort:** Medium
> **Why:** Psychotherapy requires recurring weekly appointments and session counting. Waitlist management addresses the critical psychotherapy access problem.

#### 25.1 Recurring Appointments

##### New Entity: `AppointmentSeries`

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `PatientId` | `Guid` | FK -> Patient |
| `DoctorId` | `Guid` | FK -> User |
| `TherapyCaseId` | `Guid?` | FK -> TherapyCase (optional) |
| `RecurrencePattern` | `RecurrencePattern` | Enum: Weekly, BiWeekly, Monthly |
| `DayOfWeek` | `DayOfWeek` | Which day |
| `StartTime` | `TimeOnly` | Start time |
| `DurationMinutes` | `int` | Duration |
| `SeriesStartDate` | `DateOnly` | First appointment |
| `SeriesEndDate` | `DateOnly?` | Last appointment (or open-ended) |
| `MaxOccurrences` | `int?` | Max appointments to generate |
| `Notes` | `string?` | Default notes for generated appointments |

**Service method:** `GenerateAppointmentsAsync(Guid seriesId)` -- Creates individual `Appointment` entities for each occurrence, linked to the series.

#### 25.2 Session Numbering

When an appointment belongs to a `TherapyCase`:
- Display "Sitzung X von Y" (Session X of Y approved)
- Auto-increment session count on completion
- Color-code: green (plenty left), yellow (approaching limit), red (at limit)

#### 25.3 Waitlist Management

##### New Entity: `WaitlistEntry`

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `PatientId` | `Guid` | FK -> Patient |
| `PreferredTherapistId` | `Guid?` | FK -> User |
| `RequestDate` | `DateOnly` | When they were added |
| `TherapyTypeRequested` | `TherapyType?` | What they need |
| `Priority` | `WaitlistPriority` | Normal, Urgent |
| `PreferredDays` | `List<DayOfWeek>?` | JSONB |
| `PreferredTimeSlot` | `string?` | "Vormittags" / "Nachmittags" |
| `Notes` | `string?` | Notes |
| `Status` | `WaitlistStatus` | Waiting, Offered, Scheduled, Cancelled |
| `OfferedDate` | `DateOnly?` | When a slot was offered |

##### Waitlist Dashboard

```
MediPrax.Server/Components/Pages/Termine/Warteliste.razor
```

- Sorted by wait time
- Quick action: "Termin anbieten" -> creates appointment
- Statistics: average wait time, queue length per therapist
- Filter by therapy type, therapist, priority

#### 25.4 Appointment Type Templates

```
MediPrax.Application/Catalogs/AppointmentTypeCatalog.cs
```

| Template | Default Duration | Color |
|---------|-----------------|-------|
| Erstgespraech | 50 min | Blue |
| Folgetermin | 25 min | Green |
| Medikamentenkontrolle | 10 min | Yellow |
| Psychotherapie-Sitzung | 50 min | Purple |
| EEG | 30 min | Orange |
| EMG/NLG | 45 min | Orange |
| Akuttermin | 25 min | Red |
| Notfall | 15 min | Dark Red |

#### 25.5 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/AppointmentSeries.cs` |
| Create | `MediPrax.Core/Entities/WaitlistEntry.cs` |
| Create | `MediPrax.Core/Enums/RecurrencePattern.cs` |
| Create | `MediPrax.Core/Enums/WaitlistPriority.cs` |
| Create | `MediPrax.Core/Enums/WaitlistStatus.cs` |
| Create | `MediPrax.Application/Catalogs/AppointmentTypeCatalog.cs` |
| Create | `MediPrax.Application/DTOs/AppointmentSeriesDto.cs` |
| Create | `MediPrax.Application/DTOs/WaitlistEntryDto.cs` |
| Modify | `MediPrax.Application/Services/AppointmentService.cs` -- Add series + waitlist |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/AppointmentSeriesConfiguration.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/WaitlistEntryConfiguration.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/MediPraxDbContext.cs` |
| Create | `MediPrax.Server/Components/Pages/Termine/Warteliste.razor` |
| Create | `MediPrax.Server/Components/Pages/Termine/TerminserieFormular.razor` |
| Modify | `MediPrax.Server/Components/Pages/Termine/TerminFormular.razor` -- Template selection |
| Modify | `MediPrax.Server/Components/Pages/Termine/Wochenansicht.razor` -- Color codes |
| Modify | `MediPrax.Server/Program.cs` |
| Create | `tests/MediPrax.IntegrationTests/AppointmentSeriesTests.cs` |
| Create | `tests/MediPrax.IntegrationTests/WaitlistServiceTests.cs` |

---

