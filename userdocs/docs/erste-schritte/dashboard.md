# Dashboard

Das Dashboard ist Ihre zentrale Anlaufstelle nach der Anmeldung. Es passt sich automatisch an Ihre Benutzerrolle an und zeigt die fuer Sie relevanten Informationen.

## Rollenbasierte Ansichten

### Arzt-Cockpit

Aerzte sehen ein auf ihre Arbeit zugeschnittenes Dashboard:

- **Untertitel**: "Arzt-Cockpit"
- **KPIs**: Termine heute (eigene), Offene Arztbriefe (eigene), Wiedervorlagen faellig (eigene)
- **Warnungen**: Patienten mit erhoehtem Suizidalitaetsrisiko, Therapie-Kontingente am Limit
- **Termine heute**: Alle eigenen Termine des Tages, sortiert nach Status
- **Letzte Aktivitaet**: Eigene Konsultationen und Arztbriefe
- **Schnellzugriff**: Neuer Termin, Kalender, Wartezimmer, Wiedervorlagen

### Praxis-Assistenz (MFA)

- **KPIs**: Termine heute (alle), Im Wartezimmer, Warteliste
- **Patientendaten**: Hinweis auf fehlende Stammdaten (KVNR, Krankenkasse)
- **Schnellzugriff**: Neuer Patient, Neuer Termin, Kalender, Wartezimmer, Warteliste

### Empfang & Anmeldung

- **KPIs**: Termine heute, Im Wartezimmer, Warteliste
- **Schnellzugriff**: Neuer Patient, Neuer Termin, Kalender, Wartezimmer, Warteliste

### Praxisverwaltung (Admin)

- **KPIs**: Alle KPIs inklusive Offene Rechnungen, Patienten gesamt
- **Schnellzugriff**: Alle Aktionen plus Abrechnung und Sprechzeiten

## Termine heute

Die Terminliste zeigt alle heutigen Termine in einer einzigen Liste, sortiert nach Prioritaet:

1. **In Behandlung** (oben) -- Patient wird gerade behandelt
2. **Wartezimmer** -- Patient wartet, mit gruener **K**-Schaltflaeche fuer Konsultation
3. **Erwartet** -- Patient noch nicht eingetroffen

### Aktionen in der Terminliste

| Element | Aktion |
|---|---|
| **Uhrzeit** (z.B. 10:00) | Oeffnet den Termin zur Bearbeitung |
| **Patientenname** | Oeffnet die Patientenakte |
| **Terminart** (blau) | Zeigt die Art des Termins an |
| **K** (gruener Button) | Startet die Konsultation -- nur bei Wartezimmer-Status |
| **Erwartet** (klickbar) | Check-in: Patient wird im Wartezimmer eingecheckt und die Wartezimmer-Seite geoeffnet |

!!! tip "Konsultation starten"
    Klicken Sie auf die gruene **K**-Schaltflaeche, um direkt eine neue Konsultation fuer den Patienten zu oeffnen. Arzt, Dauer und Terminzuordnung werden automatisch vorausgefuellt. Der Termin wechselt automatisch auf **In Behandlung**.

!!! info "Automatischer Statuswechsel"
    Beim Starten einer Konsultation wechselt der Termin automatisch auf **In Behandlung**. Beim Abschliessen der Konsultation (Status "Abgeschlossen") wechselt der Termin automatisch auf **Fertig**.

## Warnungen

Aerzte und Admins sehen klinische Warnungen:

- **Suizidalitaetsrisiko**: Patienten mit moderatem oder hoeherem Risiko werden hervorgehoben
- **Therapie-Kontingent**: Therapien mit 4 oder weniger verbleibenden Sitzungen

## Letzte Aktivitaet

Zeigt die letzten Konsultationen und Arztbriefe. Bei Aerzten werden nur die eigenen Eintraege angezeigt.

## Navigation

Die Seitenleiste zeigt rollenabhaengige Menuepunkte:

| Menuepunkt | Sichtbar fuer |
|---|---|
| **Patienten** | Alle |
| **Termine** | Alle |
| **Wartezimmer** | Alle |
| **Abrechnung** | Alle |
| **Verwaltung > Sprechzeiten** | Arzt, Admin |
| **Verwaltung > ICD-10 Katalog** | Arzt, Admin |
| **Verwaltung > Benutzer, Import, Audit** | Nur Admin |

!!! note "Hinweis"
    Die angezeigten Daten aktualisieren sich bei jedem Seitenaufruf. Laden Sie die Seite neu, um aktuelle Aenderungen zu sehen.
