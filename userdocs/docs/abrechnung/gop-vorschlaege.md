# GOP-Vorschlaege

Der GOP-Vorschlagsassistent schlaegt automatisch passende Abrechnungsziffern vor, basierend auf dem aktuellen Behandlungskontext. Er unterstuetzt Sie dabei, keine abrechenbaren Leistungen zu uebersehen.

## Funktionsweise

Beim Erstellen oder Bearbeiten eines [Kontakts](../dokumentation/kontakte.md) analysiert MediPrax folgende Faktoren und generiert passende Vorschlaege:

### 1. Grundpauschale (Erster Kontakt im Quartal)

Beim **ersten Kontakt im Quartal** wird automatisch die passende Grundpauschale vorgeschlagen:

| Bedingung | Vorgeschlagene GOP |
|---|---|
| Neurologie, Patient bis 59 Jahre | 16210 |
| Neurologie, Patient ab 60 Jahre | 16211 |
| Psychiatrie, Patient bis 59 Jahre | 21210 |
| Psychiatrie, Patient ab 60 Jahre | 21211 |

Die **Alterserkennung** erfolgt automatisch anhand des Geburtsdatums des Patienten.

!!! info "Hinweis"
    Ob eine neurologische oder psychiatrische Grundpauschale vorgeschlagen wird, haengt von den dokumentierten ICD-10-Diagnosen ab (F-Codes = Psychiatrie, G-Codes = Neurologie).

### 2. Zeitbezogene GOPs (Dauer >= 10 Minuten)

Wenn die dokumentierte Behandlungsdauer mindestens **10 Minuten** betraegt, werden zeitabhaengige Gespraechsziffern vorgeschlagen:

| Bedingung | Vorgeschlagene GOP |
|---|---|
| Psychiatrische Diagnosen (F-Codes), >= 10 Min. | 21220 |
| Neurologische Diagnosen (G-Codes), >= 10 Min. | 16220 |

### 3. Fachspezifische Vorschlaege nach ICD-10

Basierend auf den dokumentierten Diagnosen werden spezialisierte GOPs vorgeschlagen:

| Diagnosegruppe | Fachrichtung | Moeglich GOPs |
|---|---|---|
| **F00-F99** | Psychiatrie | 21220, 21230, 21235 |
| **G00-G99** | Neurologie | 16220, 16230, 16231, 16233 |
| **F32/F33** | Depression | 21220 + ggf. Psychotherapie-GOPs |
| **G40** | Epilepsie | 16220, 16310 (EEG) |

## Vorschlaege annehmen

1. Oeffnen Sie einen Kontakt.
2. Im Bereich **Abrechnung** werden die Vorschlaege angezeigt.
3. Klicken Sie auf **Uebernehmen** neben der gewuenschten GOP.
4. Die GOP wird dem Kontakt hinzugefuegt.
5. Sie koennen Vorschlaege auch **ablehnen** oder **ignorieren**.

## Vorschlaege vs. manuelle Eingabe

Die Vorschlaege sind **Empfehlungen** und ersetzen nicht die aerztliche Entscheidung. Sie koennen jederzeit:

- Vorgeschlagene GOPs ablehnen
- Zusaetzliche GOPs manuell hinzufuegen
- Die automatischen Vorschlaege als Ausgangspunkt verwenden

!!! tip "Tipp"
    Die GOP-Vorschlaege sind besonders am Quartalsbeginn hilfreich, wenn die Grundpauschalen fuer alle Patienten neu abgerechnet werden muessen. Das System erinnert automatisch daran.

!!! warning "Achtung"
    Die Vorschlaege beruecksichtigen die [Plausibilitaetsregeln](plausibilitaet.md). Bereits abgerechnete GOPs (z. B. Grundpauschale schon im Quartal vorhanden) werden nicht erneut vorgeschlagen.
