# Terminvorlagen

Terminvorlagen (Terminarten) definieren Standardwerte fuer verschiedene Termintypen. Sie erleichtern das Anlegen von Terminen, indem Dauer und Farbkodierung automatisch gesetzt werden.

## Verfuegbare Terminarten

MediPrax stellt folgende 8 Standard-Terminvorlagen bereit:

| Nr. | Terminart | Standarddauer | Farbe | Typischer Einsatz |
|---|---|---|---|---|
| 1 | **Erstgespraech** | 50 Min. | Dunkelblau | Erstvorstellung neuer Patienten |
| 2 | **Folgetermin** | 25 Min. | Blau | Regulaere Wiedervorstellungen |
| 3 | **Medikamentenkontrolle** | 10 Min. | Hellgruen | Kurze Medikationsbesprechung |
| 4 | **Psychotherapie-Sitzung** | 50 Min. | Violett | Einzeltherapie (VT, TP) |
| 5 | **EEG** | 30 Min. | Orange | Elektroenzephalographie |
| 6 | **EMG/NLG** | 45 Min. | Gelb | Elektrophysiologische Diagnostik |
| 7 | **Akuttermin** | 25 Min. | Rot | Kurzfristig eingeschobene Termine |
| 8 | **Notfall** | 15 Min. | Dunkelrot | Akute psychiatrische Notfaelle |

## Verwendung im Terminformular

Wenn Sie einen [neuen Termin anlegen](anlegen.md):

1. Waehlen Sie im Feld **Terminart** die gewuenschte Vorlage aus.
2. Die **Dauer** wird automatisch auf den Standardwert gesetzt.
3. Der Termin erhaelt die **Farbe** der Vorlage in der [Wochenansicht](wochenansicht.md).
4. Sie koennen die Dauer anschliessend manuell anpassen.

!!! info "Hinweis"
    Die Terminart ist ein optionales Feld. Sie koennen Termine auch ohne Vorlage mit individueller Dauer anlegen.

## Farbkodierung in der Wochenansicht

Die Farben der Terminarten ermoeglichen es, verschiedene Termintypen in der [Wochenansicht](wochenansicht.md) auf einen Blick zu unterscheiden. So erkennen Sie sofort, ob ein Slot fuer Psychotherapie, EEG oder einen Folgetermin reserviert ist.

## Zusammenspiel mit anderen Funktionen

| Funktion | Verknuepfung mit Terminvorlagen |
|---|---|
| **[Terminserien](serien.md)** | Terminart wird fuer die gesamte Serie uebernommen |
| **[Wartezimmer](wartezimmer.md)** | Terminart wird in der Wartezimmeransicht angezeigt |
| **[Abrechnung](../abrechnung/ebm.md)** | Die Dauer aus der Vorlage dient als Orientierung fuer zeitabhaengige GOPs |

!!! tip "Tipp"
    Nutzen Sie die Terminvorlagen konsequent, um einen einheitlichen Praxiskalender zu fuehren. Die farbliche Unterscheidung hilft besonders bei der Tagesplanung am Empfang.

!!! note "Hinweis"
    Die Terminvorlagen sind systemweit fuer alle Benutzer verfuegbar. Aenderungen an den Vorlagen wirken sich nur auf neue Termine aus -- bestehende Termine behalten ihre urspruengliche Dauer.
