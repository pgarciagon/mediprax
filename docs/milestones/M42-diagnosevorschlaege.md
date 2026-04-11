# M42: Automatische Diagnosevorschlaege -- AUSSTEHEND

> **Priority:** P1 | **Estimated Effort:** Medium
> **Why:** Der Arzt dokumentiert Befunde strukturiert (AMDP, Neuro-Befund, Suizidalitaet), muss aber die ICD-10-Diagnosen manuell suchen und eingeben. Das System kann aus den Befundmustern automatisch passende Diagnosen vorschlagen. Der Arzt akzeptiert oder verwirft jeden Vorschlag mit einem Klick.

#### 42.1 Architektur

Statischer Katalog (`DiagnosisSuggestionCatalog`) -- kein neuer Service, keine DB, kein ML. Nimmt die bereits geladenen DTOs (AMDP-Findings, Neuro-Exam, Suizidalitaet) und gibt eine Liste von Vorschlaegen zurueck. Gleiche Pattern wie `DrugInteractionCatalog`.

```
MediPrax.Application/Catalogs/DiagnosisSuggestionCatalog.cs
```

#### 42.2 Datenmodell

```csharp
public enum SuggestionConfidence { Low, Medium, High }

public record DiagnosisSuggestion(
    string Icd10Code,
    string Description,
    SuggestionConfidence Confidence,
    string Rationale  // Welche Befunde den Vorschlag ausgeloest haben
);
```

**Public API:**
```csharp
public static IReadOnlyList<DiagnosisSuggestion> GetSuggestions(
    List<SymptomFinding>? amdpFindings,
    NeurologicalExaminationDto? neuroExam,
    SuicidalityAssessmentDto? suicidalityAssessment)
```

#### 42.3 Klinische Regeln (AMDP -> ICD-10)

| Muster | ICD-10 | AMDP-Kriterien |
|--------|--------|----------------|
| Depressive Episode leicht | F32.0 | AFF-5>=1 + 2-3 Stuetzkriterien (ANT-1, SOM-1, AFF-6, AUF-2, etc.) |
| Depressive Episode mittelgradig | F32.1 | AFF-5>=2 + 4+ Stuetzkriterien, Schwere ~2 |
| Depressive Episode schwer | F32.2 | AFF-5>=3 + 5+ Stuetzkriterien, Schwere 3 |
| Depressive Episode schwer + Psychose | F32.3 | F32.2-Kriterien + INH>=2 oder SIN>=2 |
| Manie | F30.1 | AFF-8>=2 + ANT-3 + FDS-7/8 |
| Bipolare Stoerung | F31.x | Manie + Depression gleichzeitig |
| Paranoide Schizophrenie | F20.0 | INH-5-14>=2 oder SIN-2>=2 oder ICH-3-6>=2 oder FDS-11>=2 |
| Wahnhafte Stoerung | F22.0 | INH>=2 ohne Halluzinationen/Ich-Stoerungen |
| Generalisierte Angst | F41.1 | AFF-7>=2 + AFF-11 + SOM-1 |
| Angst und Depression gemischt | F41.2 | AFF-7>=2 + AFF-5>=1 |
| Zwangsstoerung | F42.0 | INH-1>=2 |
| Delir | F05.0 | BEW-1/2>=2 + ORI>=1 |
| Leichte kognitive Stoerung | F06.7 | AUF-3>=2 + AUF-4>=2 + ORI>=1 |
| Suizidalitaet | R45.8 | SUI-1-4>=1 oder RiskLevel>=Low |

#### 42.4 Klinische Regeln (Neuro-Befund -> ICD-10)

| Muster | ICD-10 | Neuro-Kriterien |
|--------|--------|-----------------|
| Parkinson | G20 | Rigor + kleinschrittiges Gangbild + vermindertes Armschwingen |
| Polyneuropathie | G62.9 | Distale Sensibilitaetsstoerung + ASR bds. <=1 |
| Hemiparese | G81.9 | Einseitige Kraftminderung <5 |
| Fazialisparese | G51.0 | Asymmetrische Gesichtsinnervation |
| Meningismus | R29.1 | Nackensteifigkeit oder Kernig oder Brudzinski |

#### 42.5 UI -- Sektion "Diagnosevorschlaege"

Platzierung im EncounterFormular: zwischen "Strukturierte Befunde" und "Befund / Notizen". Erscheint nur bei vorhandenen Vorschlaegen.

Jeder Vorschlag als Chip:
- Konfidenz-Indikator (gruen/gelb/grau)
- ICD-10-Code + Beschreibung
- Rationale (welche Befunde ausgeloest haben)
- **+** gruener Button -> uebernimmt Code in ICD-10-Liste des Encounters
- **x** grauer Button -> verwirft Vorschlag

#### 42.6 Berechnung

- In `OnInitializedAsync` nach Laden der Findings-DTOs
- Synchroner Aufruf: `DiagnosisSuggestionCatalog.GetSuggestions(...)`
- Filtert bereits hinzugefuegte und verworfene Codes
- Aktualisiert sich automatisch beim Neuladen der Seite (nach Rueckkehr von AMDP/Neuro)

#### 42.7 Fehlende ICD-10-Codes im Katalog ergaenzen

```
F30.0  Hypomanie
F30.1  Manie ohne psychotische Symptome
F30.2  Manie mit psychotischen Symptomen
G81.9  Hemiparese, nicht naeher bezeichnet
R29.1  Meningismus
R45.8  Sonstige Symptome, die die Stimmung betreffen
```

#### 42.8 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Application/Catalogs/DiagnosisSuggestionCatalog.cs` |
| Modify | `MediPrax.Application/Data/Icd10Catalog.cs` (6 neue Codes) |
| Modify | `MediPrax.Server/Components/Pages/Dokumentation/EncounterFormular.razor` (Suggestion-UI + Logik) |
| Modify | `MediPrax.Server/Components/Pages/Dokumentation/EncounterFormular.razor.css` (Chip-Styles) |
| Create | `tests/MediPrax.UnitTests/DiagnosisSuggestionCatalogTests.cs` |

---

