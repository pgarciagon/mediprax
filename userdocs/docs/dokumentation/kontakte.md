# Konsultationen (Behandlungsdokumentation)

Eine **Konsultation** dokumentiert eine einzelne Behandlungsbegegnung mit einem Patienten. Hier erfassen Sie Diagnosen, strukturierte Befunde, klinische Notizen und Abrechnungsziffern.

## Konsultation starten

Es gibt mehrere Wege, eine neue Konsultation zu erstellen:

### Vom Dashboard (empfohlen)

1. Im Dashboard unter **Termine heute** finden Sie Patienten im Status **Wartezimmer**.
2. Klicken Sie auf die gruene **K**-Schaltflaeche neben dem Patienten.
3. Die Konsultation oeffnet sich mit vorausgefuelltem Arzt, Dauer und Terminzuordnung.
4. Der Termin wechselt automatisch auf **In Behandlung**.

### Vom Termin

1. Oeffnen Sie einen Termin (z. B. aus dem Kalender).
2. Klicken Sie auf den gruenen Button **Konsultation starten**.
3. Arzt, Dauer und Terminzuordnung werden automatisch uebernommen.

### Von der Patientenakte

1. Oeffnen Sie die [Patientenakte](../patienten/akte.md) des Patienten.
2. Klicken Sie auf **+ Neue Konsultation**.

## Formular-Aufbau

Das Konsultationsformular ist in folgende Bereiche gegliedert:

### 1. Konsultation

| Feld | Beschreibung | Pflicht |
|---|---|---|
| **Arzt** | Behandelnder Arzt (vorausgefuellt bei Start vom Termin) | Ja |
| **Datum** | Datum der Konsultation (Standard: heute) | Ja |
| **Dauer (Min.)** | Behandlungsdauer in Minuten | Nein |

### 2. ICD-10-GM Diagnosen

Diagnosen werden ueber eine Suchfunktion erfasst:

1. Tippen Sie im Diagnosefeld den **ICD-10-Code** (z. B. "F32.1") oder einen **Suchbegriff** (z. B. "Depression").
2. Klicken Sie auf den **Pfeil** oder fokussieren Sie das Feld, um die vollstaendige Liste anzuzeigen.
3. Waehlen Sie die passende Diagnose aus der Vorschlagsliste.
4. Sie koennen **mehrere Diagnosen** pro Konsultation erfassen.

### 3. Strukturierte Befunde

Hier starten Sie spezialisierte Befundformulare:

- **Psychopath. Befund (AMDP)** -- Strukturierte psychopathologische Befunderhebung
- **Neurologischer Befund** -- Strukturierte neurologische Untersuchung
- **Suizidalitaet** -- Suizidalitaetsbewertung und Sicherheitsplanung

Die Buttons zeigen an, ob bereits ein Befund existiert:

- **Grauer Button**: Noch kein Befund erfasst -- klicken Sie, um einen neuen zu erstellen
- **Blauer Button mit Haekchen**: Befund vorhanden

!!! info "Automatische Speicherung"
    Bei einer neuen Konsultation wird diese automatisch gespeichert, wenn Sie auf einen strukturierten Befund klicken. Sie muessen nicht erst manuell speichern.

### 4. Krankheitsspezifische Module

Spezialisierte Dokumentationsmodule:

- **Anfallskalender** -- Fuer Epilepsie-Patienten
- **Kopfschmerz-Tagebuch** -- Fuer Migraene-/Kopfschmerzpatienten
- **MS-Dokumentation** -- Fuer Multiple-Sklerose-Patienten
- **Parkinson** -- Fuer Parkinson-Patienten

### 5. Befund / Notizen

Dieser Bereich zeigt eine integrierte Ansicht:

- **Strukturierte Narrative**: Wenn ein AMDP- oder Neuro-Befund existiert, wird der automatisch generierte Befundtext als blauer Block angezeigt. Klicken Sie auf **Bearbeiten**, um den Befund zu ueberarbeiten.
- **Vorlagen**: Schnellvorlagen fuer haeufige Befundtypen (Psychiatrisch, Neurologisch, Kurzkonsultation)
- **Freitext**: Zusaetzliche Notizen im Textfeld

### 6. Status

| Status | Bedeutung |
|---|---|
| **Offen** | Konsultation angelegt, Dokumentation in Arbeit |
| **In Bearbeitung** | Aktive Behandlung |
| **Abgeschlossen** | Dokumentation fertig -- Termin wechselt automatisch auf **Fertig** |

### 7. Abrechnung (GOP)

Abrechnungsziffern werden nach dem ersten Speichern freigeschaltet:

1. Tippen Sie den **GOP-Code** (z. B. "21220") oder einen Suchbegriff.
2. Klicken Sie auf den **Pfeil**, um den vollstaendigen GOP-Katalog anzuzeigen.
3. Waehlen Sie die passende Ziffer -- Punktzahl und Betrag werden automatisch berechnet.

!!! tip "Tipp"
    Die Behandlungsdauer ist wichtig fuer zeitabhaengige GOPs. Dokumentieren Sie sie gewissenhaft.

## Konsultationen einsehen

Alle Konsultationen eines Patienten finden Sie in der [Patientenakte](../patienten/akte.md) im Bereich **Konsultationen**, chronologisch sortiert.
