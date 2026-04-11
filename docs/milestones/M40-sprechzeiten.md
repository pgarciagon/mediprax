# M40: Sprechzeiten & Verfuegbarkeit -- ABGESCHLOSSEN

> **Priority:** P1 | **Estimated Effort:** Medium
> **Why:** Without doctor availability data, the system cannot suggest free slots, warn about conflicts, or visualize available time. Foundation for intelligent scheduling.

#### 40.1 Data Model

##### Entity: `DoctorScheduleTemplate`

Weekly schedule template per doctor (Wochenvorlage):

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `DoctorId` | `Guid` | FK -> User |
| `DayOfWeek` | `DayOfWeek` | Monday-Saturday |
| `StartTime` | `TimeOnly` | Block start (e.g., 08:00) |
| `EndTime` | `TimeOnly` | Block end (e.g., 12:30) |
| `SlotDurationMinutes` | `int` | Default slot size (25 or 50) |
| `IsActive` | `bool` | Enable/disable this block |

Example: Dr. Meier has 2 blocks on Monday: 08:00-12:30 and 14:00-17:00.

##### Entity: `DoctorAbsence`

Planned absences (Urlaub, Fortbildung, Krank, Sperrzeiten):

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `DoctorId` | `Guid` | FK -> User |
| `StartDate` | `DateOnly` | First day |
| `EndDate` | `DateOnly` | Last day (inclusive) |
| `StartTime` | `TimeOnly?` | Null = full day, set = partial (Sperrzeit) |
| `EndTime` | `TimeOnly?` | Null = full day |
| `AbsenceType` | `AbsenceType` | Enum: Urlaub, Fortbildung, Krank, Sperrzeit |
| `Reason` | `string?` | Optional description |
| `SubstituteId` | `Guid?` | FK -> User (Vertretung) |

##### Enum: `AbsenceType`

Urlaub, Fortbildung, Krank, Sperrzeit

#### 40.2 Availability Service

```
MediPrax.Application/Services/AvailabilityService.cs
```

Methods:
- `GetScheduleTemplateAsync(Guid doctorId)` -> weekly blocks
- `GetAbsencesAsync(Guid doctorId, DateOnly from, DateOnly to)` -> absences in range
- `GetFreeSlotsAsync(Guid doctorId, DateOnly date, int durationMinutes)` -> available time slots for a given day
- `IsAvailableAsync(Guid doctorId, DateTime start, int durationMinutes)` -> check if specific slot is free
- `GetAvailabilityOverviewAsync(DateOnly from, DateOnly to)` -> all doctors, for calendar overlay

Free slot calculation: `Sprechzeiten - Absences - Existing Appointments = Free Slots`

#### 40.3 Calendar Visualization

- **Sprechzeiten:** Light background in timeline (white = available, grey = outside Sprechzeiten)
- **Urlaub:** Hatched/striped overlay with label "Urlaub" across the day
- **Sperrzeit:** Red-striped block with reason text
- **Fortbildung:** Blue-striped block

#### 40.4 Administration UI

```
MediPrax.Server/Components/Pages/Verwaltung/Sprechzeiten.razor
```

- Per-doctor weekly schedule editor (drag blocks or form)
- Absence calendar: create/edit Urlaub, Fortbildung, Sperrzeiten
- Overview: all doctors' availability side by side

#### 40.5 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/DoctorScheduleTemplate.cs` |
| Create | `MediPrax.Core/Entities/DoctorAbsence.cs` |
| Create | `MediPrax.Core/Enums/AbsenceType.cs` |
| Create | `MediPrax.Application/Interfaces/IAvailabilityService.cs` |
| Create | `MediPrax.Application/Services/AvailabilityService.cs` |
| Create | `MediPrax.Application/DTOs/AvailabilityDto.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/DoctorScheduleTemplateConfiguration.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/DoctorAbsenceConfiguration.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/MediPraxDbContext.cs` |
| Create | `MediPrax.Server/Components/Pages/Verwaltung/Sprechzeiten.razor` |
| Modify | `MediPrax.Server/Components/Pages/Termine/Wochenansicht.razor` (availability overlay) |
| Modify | `MediPrax.Server/Program.cs` |
| Create | `tests/MediPrax.UnitTests/AvailabilityServiceTests.cs` |
| Create | `tests/MediPrax.IntegrationTests/AvailabilityServiceTests.cs` |

---

