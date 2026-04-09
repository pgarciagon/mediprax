# Plausibilitaetspruefung

Die Plausibilitaetspruefung stellt sicher, dass die abgerechneten GOPs den KBV-Regelungen entsprechen. MediPrax prueft automatisch auf Ausschluesse, Mengenbegrenzungen und Zeitdokumentation.

## Pruefungsarten

### Ausschlussregeln (18 Regeln)

Bestimmte GOPs duerfen nicht zusammen abgerechnet werden. MediPrax kennt **18 Ausschlussregeln**, darunter:

| Regel | Beschreibung |
|---|---|
| Grundpauschalen-Ausschluss | Neurologische und psychiatrische Grundpauschalen schliessen sich gegenseitig aus |
| Kapitel-Ausschluss | Bestimmte GOPs aus Kap. 16 und 21 duerfen nicht am selben Tag |
| Gespraechs-Ausschluss | Gesprachsziffern mit Grundpauschalen-Zuschlaegen |
| Psychotherapie-Ausschluss | Therapiesitzung und Gespraechsziffer am selben Tag |

### Mengenbegrenzungen (10 Limits)

Bestimmte GOPs duerfen nur begrenzt haeufig pro Quartal abgerechnet werden:

| GOP | Begrenzung | Regel |
|---|---|---|
| Grundpauschale (16210/21210) | 1x pro Quartal | Pro Patient nur einmal |
| Gespraech (16220/21220) | 3x pro Quartal | Maximale Abrechnung im Quartal |
| Psychotherapeutische Sprechstunde | Nach KBV-Kontingent | Abhängig von Genehmigung |

### Zeitdokumentation

Fuer zeitgebundene GOPs muss die Behandlungsdauer dokumentiert sein:

| GOP | Mindestzeit |
|---|---|
| 16220 (Neurologisches Gespraech) | 10 Minuten |
| 21220 (Psychiatrisches Gespraech) | 10 Minuten |
| 35150 (VT Einzelsitzung) | 50 Minuten |

## Schweregrade

Jede Pruefungsmeldung hat einen Schweregrad:

| Schweregrad | Symbol | Beschreibung |
|---|---|---|
| **Fehler** | Rot | Muss korrigiert werden -- fuehrt zu Absetzung |
| **Warnung** | Gelb | Sollte geprueft werden -- moeglicherweise problematisch |
| **Info** | Blau | Hinweis -- zur Information, keine Korrektur noetig |

## Pruefung ausfuehren

Die Plausibilitaetspruefung laeuft automatisch:

- **Beim Speichern** eines Kontakts mit GOPs
- **Bei der [Quartalsvalidierung](quartalsvalidierung.md)** fuer alle Patienten

Pruefungsergebnisse werden direkt am Kontakt und in der Quartalsvalidierung angezeigt.

## Meldungen beheben

1. Lesen Sie die **Fehlermeldung** aufmerksam.
2. Oeffnen Sie den betroffenen [Kontakt](../dokumentation/kontakte.md).
3. Korrigieren Sie die GOPs (z. B. Ausschluss entfernen, Zeitdokumentation ergaenzen).
4. Speichern Sie den Kontakt erneut.
5. Die Pruefung wird automatisch aktualisiert.

!!! warning "Fehler vor Export beheben"
    Korrigieren Sie alle **Fehler** (rot) vor dem [KVDT-Export](kvdt.md). Warnungen sollten geprueft, muessen aber nicht zwingend korrigiert werden.

!!! tip "Tipp"
    Fuehren Sie die Plausibilitaetspruefung nicht erst am Quartalsende durch. Pruefen Sie regelmaessig waehrend des Quartals, um Korrekturen rechtzeitig vorzunehmen.
