# Patientenakte

Die Patientenakte buendelt alle Informationen zu einem Patienten an einem Ort. Sie erreichen sie, indem Sie in der [Patientenliste](uebersicht.md) auf einen Patienten klicken.

## Aufbau

Die Patientenakte gliedert sich in mehrere Bereiche:

### Stammdaten

Im Kopfbereich sehen Sie die wichtigsten Angaben:

- Name, Geburtsdatum, Alter
- Versicherungstyp und -nummer
- Kontaktdaten
- Betreuerangaben (falls vorhanden)

!!! warning "Suizidalitaets-Warnung"
    Wenn beim Patienten eine Suizidalitaetseinschaetzung mit Risikostufe **Mittel** oder hoeher dokumentiert ist, wird am oberen Rand der Akte ein **auffaelliges Warnbanner** angezeigt. Dieses Banner ist in der gesamten Patientenakte sichtbar. Siehe [Suizidalitaet](../dokumentation/suizidalitaet.md).

### Verknuepfte Daten

Ueber Tabs oder Abschnitte erreichen Sie alle verknuepften Informationen:

| Bereich | Beschreibung |
|---|---|
| **Kontakte** | Alle dokumentierten [Behandlungskontakte](../dokumentation/kontakte.md) mit Diagnosen und Abrechnungsziffern |
| **Termine** | Vergangene und zukuenftige [Termine](../termine/wochenansicht.md) |
| **Medikation** | Aktuelle und fruehere [Medikamente](../medikation/verwaltung.md) |
| **Dokumente** | Arztbriefe, Formulare, importierte Befunde |
| **Laborergebnisse** | Laborwerte und [Monitoring](../medikation/monitoring.md) |
| **Psychometrie** | [Psychometrische Tests](../dokumentation/psychometrie.md) (PHQ-9, GAD-7, etc.) mit Verlauf |
| **Therapiefaelle** | [Laufende Therapien](../therapie/faelle.md) mit Sitzungszaehler |

### Krankheitsspezifische Module

Je nach Diagnose koennen fachspezifische Module aktiviert werden:

- **[Anfallskalender](../krankheiten/epilepsie.md)** -- Dokumentation epileptischer Anfaelle
- **[Kopfschmerz-Tagebuch](../krankheiten/kopfschmerz.md)** -- Migaene- und Kopfschmerzerfassung
- **[MS-Dokumentation](../krankheiten/ms.md)** -- EDSS, Schuebe, MRT-Befunde
- **[Parkinson-Dokumentation](../krankheiten/parkinson.md)** -- Hoehn & Yahr, Motorik-Scores

## Aktionen aus der Patientenakte

Direkt aus der Akte koennen Sie:

- **Neuen Kontakt erstellen** -- Behandlung dokumentieren
- **Termin vergeben** -- Neuen Termin fuer diesen Patienten anlegen
- **Formular erstellen** -- KBV-Formulare unter `/patienten/{id}/formulare`
- **Medikation verwalten** -- Medikamente hinzufuegen oder aendern
- **Psychometrischen Test durchfuehren** -- Test auswaehlen und ausfuellen

!!! tip "Tipp"
    Nutzen Sie die Patientenakte als Ausgangspunkt fuer alle patientenbezogenen Taetigkeiten. Alle relevanten Formulare und Module sind von hier aus erreichbar.

!!! note "Hinweis"
    Die angezeigten Bereiche koennen je nach Ihrer Benutzerrolle variieren. Einige klinische Module sind nur fuer Aerzte sichtbar.
