# Quartalsvalidierung

Die Quartalsvalidierung fuehrt eine umfassende Plausibilitaetspruefung ueber **alle Patienten** des Quartals durch. Sie erreichen sie ueber die Registerkarte **Quartalsvalidierung** auf der Abrechnungsseite oder direkt unter `/abrechnung/validierung`.

## Zweck

Vor dem [KVDT-Export](kvdt.md) an die KV sollten alle Abrechnungsdaten auf Fehler geprueft werden. Die Quartalsvalidierung fasst alle [Plausibilitaetsmeldungen](plausibilitaet.md) zusammen und bietet eine Gesamtuebersicht.

## Validierung durchfuehren

1. Navigieren Sie zu **Abrechnung** und klicken Sie auf die Registerkarte **Quartalsvalidierung**.
2. Waehlen Sie das zu pruefende **Quartal** (z. B. Q1/2026).
3. Klicken Sie auf **Validierung starten**.
4. Die Pruefung laeuft ueber alle Patienten und Kontakte des Quartals.
5. Die Ergebnisse werden angezeigt.

## Zusammenfassungskarten

Im oberen Bereich zeigen Uebersichtskarten die wichtigsten Kennzahlen:

| Karte | Beschreibung |
|---|---|
| **Patienten** | Gesamtanzahl der Patienten mit Kontakten im Quartal |
| **Fehler** | Anzahl der Fehler (rot), die korrigiert werden muessen |
| **Warnungen** | Anzahl der Warnungen (gelb), die geprueft werden sollten |
| **Infos** | Anzahl informativer Hinweise |

## Ergebnistabelle

Die detaillierte Ergebnistabelle zeigt alle Meldungen:

| Spalte | Beschreibung |
|---|---|
| **Schweregrad** | Fehler (rot), Warnung (gelb), Info (blau) |
| **Patient** | Name des betroffenen Patienten |
| **GOP** | Betroffene Gebuehrenordnungsposition |
| **Pruefungsart** | Ausschluss, Mengenlimit, Zeitdokumentation |
| **Meldung** | Beschreibung des Problems |

## Quartalnavigation

Wechseln Sie zwischen Quartalen ueber die Navigation:

- **Zurueck (<)** -- Vorheriges Quartal
- **Vor (>)** -- Naechstes Quartal
- **Aktuelles Quartal** -- Springt zum laufenden Quartal

## Fehler beheben

1. Klicken Sie auf eine Zeile in der Ergebnistabelle.
2. Sie werden zum betroffenen Kontakt des Patienten weitergeleitet.
3. Korrigieren Sie die gemeldete GOP oder Dokumentation.
4. Fuehren Sie die Validierung erneut durch, um die Korrektur zu pruefen.

## Empfohlener Workflow

1. **Quartalsmitte**: Erste Validierung als Zwischenpruefung
2. **2 Wochen vor Quartalsende**: Umfassende Validierung und Korrektur
3. **Quartalende**: Abschliessende Validierung, alle Fehler behoben
4. **Nach Quartalsende**: [KVDT-Export](kvdt.md)

!!! warning "Wichtig"
    Exportieren Sie die KVDT-Datei erst, wenn alle **Fehler** (rot) behoben sind. Warnungen sollten geprueft werden, sind aber nicht zwingend exportverhindernd.

!!! tip "Tipp"
    Fuehren Sie die Validierung regelmaessig waehrend des Quartals durch -- nicht erst am letzten Tag. So verteilen Sie den Korrekturaufwand ueber das gesamte Quartal.
