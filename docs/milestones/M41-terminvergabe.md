# M41: Intelligente Terminvergabe -- ABGESCHLOSSEN

> **Priority:** P1 | **Estimated Effort:** Medium
> **Depends on:** M40 (Sprechzeiten & Verfuegbarkeit)
> **Why:** Automated slot finding saves MFA time, prevents double-bookings, and ensures therapy sessions are scheduled within available hours.

#### 41.1 Slot Suggestion Engine

```
MediPrax.Application/Services/SlotSuggestionService.cs
```

##### Method: `SuggestNextSlotAsync`

Input: DoctorId, DurationMinutes, PreferredDayOfWeek?, PreferredTime?, SearchFromDate
Output: List of up to 5 suggested free slots

Logic:
1. Load doctor's Sprechzeiten template
2. Load absences in search range (next 8 weeks)
3. Load existing appointments in search range
4. For each day in range:
   a. Skip if outside Sprechzeiten
   b. Skip if Urlaub/Krank
   c. Find gaps >= requested duration
   d. Prefer same day/time as last appointment (for therapy continuity)
5. Return top 5 suggestions sorted by preference

##### Method: `SuggestNextTherapySessionAsync`

Input: TherapyCaseId
Output: Suggested slot based on:
- Same therapist
- Same day of week as last session
- Same time as last session
- Duration from TherapyCase.SessionDurationMinutes
- Warning if approaching session limit

#### 41.2 UI Integration

##### In Wochenansicht (Calendar):
- Click on empty slot -> "Termin hier erstellen" with pre-filled date/time
- Visual indicator for suggested slots (pulsing border or highlight)

##### In TherapieFallDetail:
- Button "Naechste Sitzung planen"
- Shows 3-5 suggested slots
- One-click to create appointment from suggestion

##### In TerminFormular:
- "Naechsten freien Slot suchen" button
- Shows suggestions based on selected doctor + duration
- Warning if selected time conflicts with Sprechzeiten or Urlaub

#### 41.3 Conflict Detection

When creating/editing any appointment:
- **Error:** Doctor is on Urlaub/Krank -> block creation
- **Warning:** Outside Sprechzeiten -> allow with confirmation
- **Warning:** Overlapping with existing appointment -> allow with confirmation
- **Info:** Approaching therapy session limit -> informational

#### 41.4 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Application/Interfaces/ISlotSuggestionService.cs` |
| Create | `MediPrax.Application/Services/SlotSuggestionService.cs` |
| Create | `MediPrax.Application/DTOs/SlotSuggestionDto.cs` |
| Modify | `MediPrax.Server/Components/Pages/Termine/TerminFormular.razor` (slot suggestions + conflict warnings) |
| Modify | `MediPrax.Server/Components/Pages/Therapie/TherapieFallDetail.razor` (next session button) |
| Modify | `MediPrax.Server/Components/Pages/Termine/Wochenansicht.razor` (click-to-create on empty slot) |
| Modify | `MediPrax.Server/Program.cs` |
| Create | `tests/MediPrax.UnitTests/SlotSuggestionServiceTests.cs` |
| Create | `tests/MediPrax.IntegrationTests/SlotSuggestionServiceTests.cs` |

---

