# M30: Text Modules & Documentation Automation -- TEILWEISE

> **Priority:** P3 | **Estimated Effort:** Medium
> **Why:** Textbausteine dramatically speed up documentation. Competitors like tomedo and EPIKUR excel here.

#### 30.1 Data Model

##### Entity: `TextModule`

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | PK |
| `CreatedById` | `Guid` | FK -> User |
| `Shortcut` | `string` | Trigger text (e.g., "#normalbefund") |
| `Title` | `string` | Descriptive title |
| `Content` | `string` | Template text (supports {Patient.Name} etc.) |
| `Category` | `string` | E.g., "Psychopathologie", "Neurologie", "Arztbrief" |
| `IsGlobal` | `bool` | Available to all users vs. personal |
| `UsageCount` | `int` | Track popularity |

#### 30.2 Template Variables

```
MediPrax.Application/Services/TextModuleService.cs
```

Supported variables in text module content:

| Variable | Resolved To |
|----------|------------|
| `{Patient.Name}` | Full patient name |
| `{Patient.Geburtsdatum}` | Date of birth |
| `{Patient.Alter}` | Calculated age |
| `{Patient.Geschlecht}` | Gender (Herr/Frau) |
| `{Arzt.Name}` | Current doctor name |
| `{Datum}` | Today's date |
| `{Diagnosen}` | Active ICD-10 codes from encounter |

#### 30.3 Default Text Modules (Seed Data)

Pre-install common templates:

| Shortcut | Content |
|----------|---------|
| `#normalpsy` | "Psychopathologischer Befund: Bewusstsein klar, allseits orientiert..." |
| `#normalneuro` | "Neurologischer Status: Hirnnerven o.p.B., Kraft allseits 5/5..." |
| `#depression` | "Stimmung deutlich gedrueckt, Antrieb vermindert, Ein-/Durchschlafstoerungen..." |
| `#rezidiv` | "Rezidiv einer depressiven Episode, aktuell mittelgradige Auspraegung..." |
| `#arztbrief_einleitung` | "Sehr geehrte Kollegin, sehr geehrter Kollege,\n\nwir berichten ueber..." |
| `#arztbrief_schluss` | "Fuer Rueckfragen stehen wir jederzeit gerne zur Verfuegung.\n\nMit freundlichen kollegialen Gruessen" |

#### 30.4 Blazor UI

##### Text Module Management

```
MediPrax.Server/Components/Pages/Verwaltung/Textbausteine.razor
```

- CRUD for text modules
- Category filter
- Preview with variable resolution
- Import/export

##### Integration in Text Fields

All multi-line text inputs (`InputTextArea`) in encounters, Arztbriefe, and forms should:
- Listen for `#` prefix
- Show autocomplete dropdown matching text modules by shortcut
- Insert resolved content on selection

#### 30.5 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Core/Entities/TextModule.cs` |
| Create | `MediPrax.Application/DTOs/TextModuleDto.cs` |
| Create | `MediPrax.Application/Interfaces/ITextModuleService.cs` |
| Create | `MediPrax.Application/Services/TextModuleService.cs` |
| Create | `MediPrax.Infrastructure/Persistence/Configurations/TextModuleConfiguration.cs` |
| Modify | `MediPrax.Infrastructure/Persistence/MediPraxDbContext.cs` |
| Create | `MediPrax.Server/Components/Pages/Verwaltung/Textbausteine.razor` |
| Create | `MediPrax.Server/Components/Shared/TextModuleAutocomplete.razor` -- Reusable component |
| Modify | Multiple Razor pages -- Integrate autocomplete in text fields |
| Modify | `MediPrax.Server/Program.cs` |
| Create | `tests/MediPrax.IntegrationTests/TextModuleServiceTests.cs` |

---

