# KBV-Formulare

MediPrax unterstuetzt die gaengigen KBV-Standardformulare (Muster) fuer den Praxisalltag. Alle Formulare sind patientenbezogen und werden mit Stammdaten vorausgefuellt. Sie erreichen sie ueber die [Patientenakte](../patienten/akte.md) unter `/patienten/{id}/formulare`.

## Verfuegbare Formulare

| Muster | Bezeichnung | Beschreibung |
|---|---|---|
| **Muster 16** | Rezept (Kassenrezept) | Verordnung von Arzneimitteln fuer GKV-Patienten |
| **--** | Privatrezept | Verordnung fuer PKV-Patienten |
| **Muster 6** | Ueberweisung | Ueberweisung an Fachkollegen mit Diagnose und Fragestellung |
| **Muster 1** | AU-Bescheinigung | Arbeitsunfaehigkeitsbescheinigung |
| **Muster 2** | Krankenhauseinweisung | Einweisung in stationaere Behandlung |
| **Muster 13** | Heilmittelverordnung | Verordnung von Heilmitteln (Ergo-/Physiotherapie, Logopaedie) |
| **Muster 12** | Haeusliche Krankenpflege | Verordnung haeuslicher Krankenpflege |
| **Muster 26** | Soziotherapie | Verordnung von Soziotherapie fuer psychisch Kranke |
| **Muster 7** | Ueberweisung zur Psychotherapie | Ueberweisung an Psychotherapeuten mit Therapieverfahren und somatischer Abklaerung |

## Formular erstellen

1. Oeffnen Sie die Patientenakte.
2. Klicken Sie auf **Formulare** oder navigieren Sie zu `/patienten/{id}/formulare`.
3. Waehlen Sie das gewuenschte **Muster**.
4. Die **Patientendaten** werden automatisch eingetragen (Name, Geburtsdatum, KVNR, Krankenkasse).
5. Fuellen Sie die formularspezifischen Felder aus.
6. Klicken Sie auf **Speichern** und dann auf **PDF erstellen**.

## Formularspezifische Hinweise

### Ueberweisung (Muster 6)

| Feld | Beschreibung |
|---|---|
| Fachrichtung | Ziel-Fachrichtung der Ueberweisung |
| Diagnose | ICD-10-Code und Klartext |
| Fragestellung | Konkrete Fragestellung an den Kollegen |
| Befunde | Relevante Vorbefunde |

### AU-Bescheinigung (Muster 1)

| Feld | Beschreibung |
|---|---|
| AU seit | Beginn der Arbeitsunfaehigkeit |
| Voraussichtlich bis | Ende der AU |
| Erst-AU / Folge-AU | Erstbescheinigung oder Folgebescheinigung |
| Diagnose | ICD-10-Code (wird auf AU nicht im Klartext gedruckt) |

Fuer die elektronische Uebermittlung siehe [eAU](eau.md).

### Krankenhauseinweisung (Muster 2)

| Feld | Beschreibung |
|---|---|
| Krankenhaus | Name der Zielklinik |
| Diagnose | Einweisungsdiagnose |
| Begruendung | Klinische Begruendung fuer die Einweisung |

### Heilmittelverordnung (Muster 13)

| Feld | Beschreibung |
|---|---|
| Heilmittel | Art (Ergotherapie, Physiotherapie, Logopaedie) |
| Diagnosegruppe | ICD-10 und Heilmittel-Diagnosegruppe |
| Verordnungsmenge | Anzahl der Behandlungen |
| Frequenz | Behandlungshaeufigkeit pro Woche |

### Ueberweisung zur Psychotherapie (Muster 7)

| Feld | Beschreibung |
|---|---|
| Diagnose | ICD-10-Code und Klartext der Verdachtsdiagnose |
| Therapieverfahren | Verhaltenstherapie (VT), Tiefenpsychologisch fundierte PT (TP), Analytische PT (AP) oder Systemische Therapie (ST) |
| Somatische Abklaerung | Bestaetigung, dass eine somatische Abklaerung erfolgt ist |
| Befunde | Zusammenfassung relevanter Vorbefunde und Anamnese |
| Dringlichkeit | Kennzeichnung als dringende Ueberweisung |

### Soziotherapie (Muster 26)

| Feld | Beschreibung |
|---|---|
| Diagnose | Psychiatrische Diagnose (F-Code) |
| Ziele | Soziotherapeutische Behandlungsziele |
| Verordnungsmenge | Anzahl der Einheiten |

!!! tip "Tipp"
    Nutzen Sie die automatische Datenvorausfuellung. Pruefen Sie die vorausgefuellten Felder und ergaenzen Sie nur die formularspezifischen Angaben.

!!! info "Hinweis"
    Alle erstellten Formulare werden in der Patientenakte archiviert und sind jederzeit als PDF abrufbar.
