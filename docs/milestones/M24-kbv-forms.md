# M24: Missing KBV Forms -- AUSSTEHEND

> **Priority:** P1 | **Estimated Effort:** Medium
> **Why:** Muster 2 (hospital admission), 13 (therapy prescriptions), and 26 (Soziotherapie) are used daily.

#### 24.1 Forms to Implement

##### Muster 2 -- Krankenhauseinweisung (Hospital Admission)

```
MediPrax.Reporting/Formulare/KrankenhauseinweisungDocument.cs
```

| Field | Type |
|-------|------|
| Patient data | Auto-populated |
| Insurance data | Auto-populated |
| Einweisungsdiagnose | ICD-10 + text |
| Befunde und Angaben | Free text |
| Einweisungsgrund | Dropdown + text |
| Krankenhaus | Text input |
| Fachabteilung | Text input |
| Notfall (yes/no) | Checkbox |

**Especially important for psychiatry:** Acute psychiatric hospital admissions.

##### Muster 7 -- Ueberweisung vor Psychotherapie

```
MediPrax.Reporting/Formulare/UeberweisungPsychotherapieDocument.cs
```

- Similar to existing Ueberweisung but specific fields
- Somatische Abklaerung completed (yes/no)
- Relevant somatic diagnoses
- Required before Richtlinien-Psychotherapie can begin

##### Muster 13 -- Heilmittelverordnung (Therapy Prescription)

```
MediPrax.Reporting/Formulare/HeilmittelverordnungDocument.cs
```

| Field | Description |
|-------|-------------|
| Heilmittel type | Ergotherapie / Physiotherapie / Logopaedie |
| Diagnose (ICD-10) | From Heilmittel-Diagnoseliste |
| Leitsymptomatik | Structured per Heilmittelkatalog |
| Therapieziel | Free text |
| Anzahl | Number of sessions |
| Frequenz | Sessions per week |
| Hausbesuch | Home visit checkbox |
| Erstverordnung / Folgeverordnung | Radio |
| Langfristiger Heilmittelbedarf | Checkbox |

**Critical for neurology:** Ergotherapy after stroke, physiotherapy for MS/Parkinson, speech therapy for aphasia.

##### Muster 14 -- Haeusliche Krankenpflege

```
MediPrax.Reporting/Formulare/HaeuslicheKrankenpflegeDocument.cs
```

- Psychiatric home nursing (Psychiatrische haeusliche Krankenpflege)
- Medication supervision, symptom monitoring
- Duration and frequency specification

##### Muster 26 -- Soziotherapie-Verordnung

```
MediPrax.Reporting/Formulare/SoziotherapieDocument.cs
```

**Unique to psychiatry.** For severely mentally ill patients:

| Field | Description |
|-------|-------------|
| Diagnose | ICD-10 (typically F20-F31) |
| Faehigkeitsstoerungen | Structured checklist |
| Therapieziele | Free text |
| Verordnungsdauer | Max 120 Stunden / 3 Jahre |
| Vorherige Soziotherapie | Yes/No + dates |
| Soziotherapeut | Provider name |

#### 24.2 Form Data Entities

Don't create separate entities per form. Instead, use the existing `Document` entity with `DocType` extended:

```csharp
// Add to DocumentType enum:
Krankenhauseinweisung,
UeberweisungPsychotherapie,
Heilmittelverordnung,
HaeuslicheKrankenpflege,
Soziotherapie
```

Store form-specific data in `Document.Content` as JSON.

#### 24.3 Blazor UI

For each form, add a page under `/formulare/`:

```
MediPrax.Server/Components/Pages/Formulare/KrankenhauseinweisungFormular.razor
MediPrax.Server/Components/Pages/Formulare/HeilmittelverordnungFormular.razor
MediPrax.Server/Components/Pages/Formulare/SoziotherapieFormular.razor
MediPrax.Server/Components/Pages/Formulare/HaeuslicheKrankenpflegeFormular.razor
```

#### 24.4 API Endpoints

Add to `Program.cs`:

```csharp
app.MapGet("/api/formulare/krankenhauseinweisung", ...);
app.MapGet("/api/formulare/heilmittelverordnung", ...);
app.MapGet("/api/formulare/soziotherapie", ...);
app.MapGet("/api/formulare/haeusliche-krankenpflege", ...);
```

#### 24.5 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Reporting/Formulare/KrankenhauseinweisungDocument.cs` |
| Create | `MediPrax.Reporting/Formulare/UeberweisungPsychotherapieDocument.cs` |
| Create | `MediPrax.Reporting/Formulare/HeilmittelverordnungDocument.cs` |
| Create | `MediPrax.Reporting/Formulare/HaeuslicheKrankenpflegeDocument.cs` |
| Create | `MediPrax.Reporting/Formulare/SoziotherapieDocument.cs` |
| Modify | `MediPrax.Core/Enums/DocumentType.cs` -- Add new types |
| Create | `MediPrax.Server/Components/Pages/Formulare/KrankenhauseinweisungFormular.razor` |
| Create | `MediPrax.Server/Components/Pages/Formulare/HeilmittelverordnungFormular.razor` |
| Create | `MediPrax.Server/Components/Pages/Formulare/SoziotherapieFormular.razor` |
| Create | `MediPrax.Server/Components/Pages/Formulare/HaeuslicheKrankenpflegeFormular.razor` |
| Modify | `MediPrax.Server/Program.cs` -- Add endpoints |

---

