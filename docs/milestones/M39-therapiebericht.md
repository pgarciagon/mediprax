# M39: Arztbrief Therapiebericht Vorlage -- AUSSTEHEND

> **Priority:** P2 | **Estimated Effort:** Small
> **Why:** Therapy progress reports are the most common Arztbrief type in psychiatric practices. Auto-populating from the active TherapyCase saves significant documentation time.

#### 39.1 Features

- **New Vorlage "Therapiebericht"** in ArztbriefFormular
- Detects active TherapyCase for the patient, auto-includes:
  - Therapieverfahren (VT, TP, etc.)
  - Status (Bewilligt, In Behandlung, etc.)
  - Sitzungszahl: "Bisher X von Y bewilligten Sitzungen durchgefuehrt"
  - Startdatum der Therapie
  - Diagnosen des Therapiefalls
  - Kassengenehmigung (Aktenzeichen)
- Falls back to standard Verlaufsbericht if no active TherapyCase
- Combined with existing Encounter data (medication, Befund)

#### 39.2 Files to Modify

| Action | File |
|--------|------|
| Modify | `MediPrax.Server/Components/Pages/Dokumente/ArztbriefFormular.razor` (new Vorlage button + template logic) |
| Modify | `MediPrax.Application/Interfaces/ITherapyCaseService.cs` (GetActiveByPatientAsync) |
| Modify | `MediPrax.Application/Services/TherapyCaseService.cs` |

---

