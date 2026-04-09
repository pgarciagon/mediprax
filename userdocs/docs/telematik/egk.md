# eGK-Lesung

Die elektronische Gesundheitskarte (eGK) enthaelt die Stammdaten des Versicherten. MediPrax liest diese Daten ueber das Kartenlesegeraet ein und kann damit Patientenakten automatisch anlegen oder aktualisieren.

## eGK einlesen

1. Bitten Sie den Patienten, die **eGK** in das Kartenlesegeraet einzufuehren.
2. Klicken Sie in MediPrax auf **eGK einlesen** (in der Patientenaufnahme oder Patientenliste).
3. MediPrax liest die Kartendaten ueber den Konnektor.
4. Die Daten werden angezeigt und koennen uebernommen werden.

## Gelesene Daten

Folgende Stammdaten werden von der eGK uebernommen:

| Feld | Beschreibung |
|---|---|
| **KVNR** | Krankenversichertennummer (10-stellig, lebenslang) |
| **Vorname** | Vorname des Versicherten |
| **Nachname** | Nachname des Versicherten |
| **Geburtsdatum** | Geburtsdatum |
| **Geschlecht** | Geschlecht |
| **Strasse** | Strasse und Hausnummer |
| **PLZ / Ort** | Postleitzahl und Wohnort |
| **Krankenkasse** | Name der Krankenkasse |
| **Versicherungsnummer** | Versichertennummer |
| **Versicherungsschutz** | Gueltigkeitszeitraum |

## Neuen Patienten anlegen

Wenn die KVNR noch nicht im System vorhanden ist:

1. MediPrax erkennt automatisch, dass es sich um einen **neuen Patienten** handelt.
2. Die Stammdaten werden in das Formular [Patient anlegen](../patienten/anlegen.md) uebernommen.
3. Pruefen Sie die Daten und ergaenzen Sie fehlende Angaben.
4. Klicken Sie auf **Speichern**.

## Bestehenden Patienten aktualisieren

Wenn die KVNR bereits im System vorhanden ist:

1. MediPrax ordnet die Kartendaten dem **bestehenden Patienten** zu.
2. Geaenderte Daten (z. B. neue Adresse, Kassenwechsel) werden angezeigt.
3. Uebernehmen Sie die Aktualisierungen.

## VSDM (Versichertenstammdatenmanagement)

Die eGK-Lesung fuehrt gleichzeitig ein **Versichertenstammdatenmanagement (VSDM)** durch. Dabei wird der Versicherungsschutz online geprueft und bestaetigt.

!!! info "Versicherungsnachweis"
    Die eGK-Lesung dient als Nachweis des Versicherungsschutzes und muss gemaess KBV-Vorgaben mindestens einmal pro Quartal je Patient durchgefuehrt werden.

## Fehlerbehebung

| Problem | Loesung |
|---|---|
| Karte wird nicht erkannt | Karte herausnehmen, reinigen, erneut einfuehren |
| "Kein Kartenlesegeraet" | USB-Verbindung und TI-Status pruefen |
| Daten nicht lesbar | Karte moeglicherweise defekt -- Patient an Krankenkasse verweisen |
| VSDM-Pruefung fehlgeschlagen | TI-Verbindung pruefen, ggf. spaeter erneut versuchen |

!!! tip "Tipp"
    Lesen Sie die eGK bei jedem Patientenbesuch ein. So bleiben die Stammdaten immer aktuell und der Versicherungsnachweis ist dokumentiert.

!!! warning "Abgelaufene Karten"
    Bei abgelaufenen eGK ist eine Behandlung weiterhin moeglich, jedoch sollte der Patient auf die Notwendigkeit einer neuen Karte hingewiesen werden.
