# Textbausteine

Textbausteine sind wiederverwendbare Textvorlagen, die Ihnen die Dokumentation erheblich erleichtern. Sie verwalten Textbausteine unter **Verwaltung > Textbausteine** oder direkt unter `/verwaltung/textbausteine`.

## Ueberblick

Textbausteine koennen in der gesamten klinischen Dokumentation verwendet werden -- in ABDTP-Sektionen der Konsultation, Arztbriefen und weiteren Freitextfeldern. Jeder Baustein hat ein **Kuerzel** (mit #-Praefix), das als Schnellzugriff dient.

**Zeitersparnis:** Statt wiederkehrende Befundtexte manuell zu tippen, genuegt ein Kuerzel wie `#normpsy` -- der vollstaendige Text wird sofort eingefuegt, inklusive automatisch aufgeloester Patientendaten.

## Inline-Expansion (#-Kuerzel)

In jedem Textfeld mit Textbaustein-Unterstuetzung:

1. Tippen Sie `#` — eine **Dropdown-Liste** erscheint sofort mit allen verfuegbaren Bausteinen (gefiltert nach der aktuellen Sektion)
2. Tippen Sie weiter (z.B. `#norm`), um die Liste einzugrenzen
3. Navigieren Sie mit **Pfeiltasten** (hoch/runter) durch die Liste
4. Druecken Sie **Enter** oder klicken Sie auf den gewuenschten Eintrag
5. Der `#Kuerzel`-Text wird durch den Baustein-Inhalt ersetzt
6. **Variablen** werden automatisch mit den aktuellen Patientendaten aufgeloest
7. **Escape** schliesst das Dropdown ohne Auswahl

!!! tip "Sektionsspezifisch"
    Bausteine koennen einer **Zielsektion** zugeordnet werden (z.B. Befund, Anamnese). Im Befund-Feld werden dann nur Befund-Bausteine und allgemeine Bausteine angezeigt.

## Textbaustein erstellen

1. Navigieren Sie zu **Verwaltung > Textbausteine**
2. Klicken Sie auf **+ Neuer Textbaustein**
3. Fuellen Sie die Felder aus:

| Feld | Beschreibung | Beispiel |
|---|---|---|
| **Kuerzel** | Schnellzugriff (ohne #) | `normpsy` |
| **Titel** | Beschreibender Name | Normaler psychopathologischer Befund |
| **Kategorie** | Hierarchische Zuordnung (mit `/` trennen) | `Psychiatrie/Befund` |
| **Zielsektion** | Optionale ABDTP-Sektion | Befund |
| **Inhalt** | Der eigentliche Textinhalt mit optionalen Variablen | Bewusstsein klar, allseits orientiert... |
| **Global** | Fuer alle Benutzer sichtbar | Ja / Nein |

4. Klicken Sie auf **Speichern**

## Kategorien

Kategorien koennen hierarchisch mit `/` getrennt werden:

| Kategorie | Typische Verwendung |
|---|---|
| **Psychiatrie/Befund** | Standardbefunde, Befundkonstellationen |
| **Psychiatrie/Anamnese** | Erstgespraech-Vorlagen |
| **Neurologie/Befund** | Neurologische Normalbefunde |
| **Neurologie/Anamnese** | Kopfschmerz-Anamnese etc. |
| **Allgemein/Arztbrief** | Einleitungen, Schlusstexte |
| **Allgemein/Therapie** | Therapieplan-Vorlagen |
| **Allgemein/Procedere** | Wiedervorstellung, Laborkontrolle |

## Variablen

Textbausteine koennen **Variablen** enthalten, die beim Einfuegen automatisch mit aktuellen Daten ersetzt werden:

### Patientendaten

| Variable | Wird ersetzt durch | Beispiel |
|---|---|---|
| `{Patient.Name}` | Vollstaendiger Patientenname | Max Mustermann |
| `{Patient.Geburtsdatum}` | Geburtsdatum | 15.03.1985 |
| `{Patient.Alter}` | Berechnetes Alter | 41 |
| `{Patient.Geschlecht}` | Anrede | Herr / Frau |

### Arztdaten

| Variable | Wird ersetzt durch | Beispiel |
|---|---|---|
| `{Arzt.Name}` | Name des behandelnden Arztes | Dr. Thomas Meier |
| `{Arzt.Titel}` | Akademischer Titel | Dr. med. |

### Datum

| Variable | Wird ersetzt durch | Beispiel |
|---|---|---|
| `{Datum}` | Aktuelles Datum (kurz) | 12.04.2026 |
| `{Datum.Lang}` | Aktuelles Datum (lang) | 12. April 2026 |

### Konsultationsdaten

| Variable | Wird ersetzt durch | Beispiel |
|---|---|---|
| `{Encounter.Datum}` | Datum der Konsultation | 12.04.2026 |
| `{Encounter.Dauer}` | Dauer in Minuten | 50 |

### Klinische Daten

| Variable | Wird ersetzt durch |
|---|---|
| `{Diagnosen}` | Aktive ICD-10-Diagnosen mit Beschreibung |
| `{Dauerdiagnosen}` | Alle Dauerdiagnosen des Patienten |
| `{Medikation}` | Aktuelle Medikation mit Dosierung |
| `{LetzterBefund.Psychopath}` | Letzter psychopathologischer Befund |
| `{LetzterBefund.Neuro}` | Letzter neurologischer Befund |

**Beispiel-Textbaustein:**

> Sehr geehrte Kolleginnen und Kollegen,
>
> wir berichten ueber o.g. Patienten, {Patient.Name}, geb. am {Patient.Geburtsdatum} ({Patient.Alter} Jahre), der sich am {Datum} in unserer Sprechstunde vorstellte.

## Zielsektion

Bausteine koennen optional einer ABDTP-Sektion zugeordnet werden:

| Sektion | Beschreibung |
|---|---|
| **Anamnese** | Nur in Anamnese-Feldern angezeigt |
| **Befund** | Nur in Befund-Feldern angezeigt |
| **Diagnose** | Nur in Diagnose-Feldern angezeigt |
| **Therapie** | Nur in Therapie-Feldern angezeigt |
| **Procedere** | Nur in Procedere-Feldern angezeigt |
| **Alle Sektionen** | Ueberall verfuegbar (Standard) |

## Global vs. Persoenlich

| Typ | Sichtbarkeit | Verwendung |
|---|---|---|
| **Global** | Fuer alle Benutzer sichtbar | Praxisweite Standardtexte |
| **Persoenlich** | Nur fuer den Ersteller | Individuelle Dokumentationsvorlieben |

## Import / Export

Sie koennen Textbausteine als JSON-Datei exportieren und importieren:

- **Exportieren:** Klicken Sie auf **Exportieren** -- alle Bausteine (oder nur die aktuelle Kategorie) werden als JSON-Datei heruntergeladen
- **Importieren:** Klicken Sie auf **Importieren** und waehlen Sie eine JSON-Datei. Bereits vorhandene Kuerzel werden uebersprungen.

!!! info "Austausch zwischen Praxen"
    Das JSON-Format ermoeglicht den einfachen Austausch von Textbaustein-Sammlungen zwischen Praxen oder Kollegen.

## Verwendungsstatistik

Die Verwaltungsseite zeigt fuer jeden Baustein:

- **Verwendungen:** Wie oft der Baustein eingefuegt wurde
- **Zuletzt verwendet:** Wann der Baustein zuletzt benutzt wurde

Sortieren Sie nach **Meistverwendet** oder **Zuletzt verwendet**, um die beliebtesten Bausteine schnell zu finden.

## Vorinstallierte Bausteine

Folgende Bausteine sind vorinstalliert:

| Kuerzel | Titel | Sektion |
|---|---|---|
| `#normpsy` | Normaler psychopathologischer Befund | Befund |
| `#depbefund` | Depressiver Befund | Befund |
| `#normneuro` | Normaler neurologischer Befund | Befund |
| `#erstanamnese` | Erstgespraech Anamnese | Anamnese |
| `#therplan` | Therapieplan Standard | Therapie |
| `#wv` | Wiedervorstellung | Procedere |
| `#briefein` | Arztbrief Einleitung | Alle |
| `#briefschluss` | Arztbrief Schluss | Alle |
| `#diagliste` | Diagnosenliste | Diagnose |
| `#medliste` | Aktuelle Medikation | Therapie |
| `#kurz` | Kurzkonsultation | Befund |
| `#ksanamnese` | Kopfschmerz-Anamnese | Anamnese |

!!! tip "Eigene Bausteine"
    Erstellen Sie zusaetzlich Ihre eigenen Bausteine fuer wiederkehrende Dokumentationsaufgaben. Kurze Kuerzel wie `#wv`, `#kurz` oder `#med` sparen am meisten Zeit.
