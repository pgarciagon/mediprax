# Wochenansicht

Die Wochenansicht ist die zentrale Terminuebersicht in MediPrax. Sie erreichen sie ueber **Termine** in der Seitenleiste oder direkt unter `/termine`.

## Aufbau

Die Wochenansicht bietet drei Darstellungsmodi:

- **Tag**: Tagesansicht mit Zeitleiste fuer einen einzelnen Tag
- **Kalender**: Wochenraster von Montag bis Samstag mit Zeitslots
- **Liste**: Kompakte Listenansicht aller Termine der Woche

### Standardansicht fuer Aerzte

Wenn Sie als **Arzt** angemeldet sind, oeffnet sich die Wochenansicht standardmaessig in der **Tagesansicht** mit dem **eigenen Arztfilter** vorausgewaehlt. So sehen Sie sofort Ihre heutigen Termine.

MFA, Empfang und Admin sehen standardmaessig die Kalenderansicht mit allen Aerzten.

### Farbkodierung der Status

| Farbe | Status | Bedeutung |
|---|---|---|
| Blau | **Erwartet** | Termin geplant, Patient noch nicht eingetroffen |
| Gelb | **Im Wartezimmer** | Patient ist eingetroffen und wartet |
| Gruen | **In Behandlung** | Patient wird gerade behandelt |
| Grau | **Fertig** | Behandlung abgeschlossen |
| Rot | **Nicht erschienen** | Patient ist nicht zum Termin gekommen |

### Arztfilter

Wenn mehrere Aerzte in der Praxis arbeiten, koennen Sie die Ansicht nach **einzelnen Aerzten** filtern. Waehlen Sie den gewuenschten Arzt im Dropdown oberhalb des Kalenders aus.

### Terminart

Jeder Termin zeigt die zugeordnete **Terminart** an (z. B. Erstgespraech, Folgetermin, Psychotherapie-Sitzung). Die Terminart wird beim Erstellen oder Bearbeiten eines Termins festgelegt und ist dauerhaft gespeichert.

## Navigation

| Aktion | Beschreibung |
|---|---|
| **Zurueck (<)** | Vorheriger Tag/Woche anzeigen |
| **Vor (>)** | Naechster Tag/Woche anzeigen |
| **Heute** | Zum aktuellen Tag/Woche springen |

## Termin bearbeiten

Klicken Sie auf einen **bestehenden Termin** im Raster, um die Termindetails zu oeffnen. Dort koennen Sie:

- Datum und Uhrzeit aendern
- Terminart festlegen
- Notizen hinzufuegen
- Eine **Konsultation starten** (gruener Button)

!!! tip "Konsultation starten"
    Im Terminformular gibt es den gruenen Button **Konsultation starten**. Dieser oeffnet direkt eine neue Konsultation mit vorausgefuelltem Arzt, Dauer und Terminzuordnung. Der Terminstatus wechselt automatisch auf **In Behandlung**.

## Schnellaktionen

Von der Wochenansicht aus erreichen Sie direkt:

- **[Neuer Termin](anlegen.md)** -- Button oberhalb des Kalenders
- **[Terminserie erstellen](serien.md)** -- Fuer wiederkehrende Termine
- **[Warteliste](warteliste.md)** -- Patienten ohne festen Termin verwalten

!!! tip "Tipp"
    Klicken Sie auf eine leere Stelle im Kalender, um direkt einen neuen Termin fuer diesen Zeitpunkt zu erstellen. Datum und Uhrzeit werden automatisch vorausgefuellt.
