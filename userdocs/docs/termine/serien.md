# Terminserien

Terminserien ermoeglischen es, wiederkehrende Termine automatisch zu erstellen -- ideal fuer regelmaessige Psychotherapie-Sitzungen oder Kontrolltermine. Navigieren Sie zu **Termine** > **Terminserie** oder direkt zu `/terminserie/neu`.

## Terminserie erstellen

1. **Patient auswaehlen**: Waehlen Sie den Patienten ueber die Suchfunktion.
2. **Arzt/Aerztin waehlen**: Legen Sie den behandelnden Arzt fest.
3. **Wiederholung festlegen**: Waehlen Sie das Intervall.
4. **Wochentag waehlen**: An welchem Tag soll der Termin stattfinden.
5. **Uhrzeit eingeben**: Feste Uhrzeit fuer alle Termine der Serie.
6. **Dauer festlegen**: Dauer in Minuten (z. B. 50 Minuten fuer Psychotherapie).
7. **Startdatum**: Ab wann sollen die Termine generiert werden.
8. **Enddatum** oder **maximale Anzahl**: Legen Sie fest, wann die Serie endet.
9. Klicken Sie auf **Serie erstellen**.

## Wiederholungsintervalle

| Intervall | Beschreibung | Typischer Einsatz |
|---|---|---|
| **Woechentlich** | Jede Woche am gleichen Tag | Psychotherapie (VT/TP) |
| **Zweiwoechentlich** | Alle 2 Wochen | Folgetermine, niederfrequente Therapie |
| **Monatlich** | Einmal pro Monat | Medikamentenkontrollen, Laborkontrollen |

## Wie Serien funktionieren

Nach dem Erstellen generiert MediPrax automatisch **einzelne Termine** gemaess den Serieneinstellungen. Jeder generierte Termin:

- Erscheint in der [Wochenansicht](wochenansicht.md) wie ein normaler Termin
- Kann einzeln bearbeitet oder verschoben werden
- Kann einzeln geloescht werden, ohne die restliche Serie zu beeinflussen

## Serien verwalten

In der **Serienuebersicht** sehen Sie alle aktiven Terminserien in einer Tabelle:

- Patient und Arzt
- Intervall und Wochentag
- Naechster geplanter Termin
- Anzahl generierter Termine

### Serie loeschen

1. Oeffnen Sie die Serienuebersicht.
2. Klicken Sie auf **Loeschen** neben der gewuenschten Serie.
3. Waehlen Sie, ob nur zukuenftige oder alle Termine entfernt werden sollen.

!!! warning "Achtung"
    Das Loeschen einer Terminserie kann viele Termine auf einmal entfernen. Pruefen Sie vor dem Loeschen, ob bereits Kontakte an Terminen der Serie dokumentiert wurden.

!!! tip "Tipp"
    Erstellen Sie Terminserien direkt zu Beginn eines Therapiefalls, um die gesamte bewilligte Sitzungsanzahl voraus zu planen.
