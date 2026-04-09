# Wochenansicht

Die Wochenansicht ist die zentrale Terminuebersicht in MediPrax. Sie erreichen sie ueber **Termine** in der Seitenleiste oder direkt unter `/termine`.

## Aufbau

Die Wochenansicht zeigt ein **Raster von Montag bis Samstag** mit Zeitslots. Jeder Termin wird als farbiger Block in der entsprechenden Spalte und Zeile dargestellt.

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

## Navigation

| Aktion | Beschreibung |
|---|---|
| **Zurueck (<)** | Vorherige Woche anzeigen |
| **Vor (>)** | Naechste Woche anzeigen |
| **Heute** | Zur aktuellen Woche springen |

## Termin bearbeiten

Klicken Sie auf einen **bestehenden Termin** im Raster, um die Termindetails zu oeffnen. Dort koennen Sie:

- Datum und Uhrzeit aendern
- Notizen hinzufuegen
- Den Termin loeschen
- Den [Wartezimmerstatus](wartezimmer.md) aendern

## Schnellaktionen

Von der Wochenansicht aus erreichen Sie direkt:

- **[Neuer Termin](anlegen.md)** -- Button oberhalb des Kalenders
- **[Terminserie erstellen](serien.md)** -- Fuer wiederkehrende Termine
- **[Warteliste](warteliste.md)** -- Patienten ohne festen Termin verwalten

!!! tip "Tipp"
    Klicken Sie auf eine leere Stelle im Kalender, um direkt einen neuen Termin fuer diesen Zeitpunkt zu erstellen. Datum und Uhrzeit werden automatisch vorausgefuellt.

!!! note "Hinweis"
    Die Wochenansicht zeigt standardmaessig die aktuelle Woche an. Samstage werden mit angezeigt fuer Praxen mit Samstagssprechstunde.
