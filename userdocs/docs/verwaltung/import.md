# Datenimport

Der Datenimport ermoeglicht die Uebernahme von Patientendaten aus anderen Praxisverwaltungssystemen (PVS) in MediPrax. Dies ist besonders bei Praxisuebernahmen oder Systemwechseln relevant.

## Unterstuetzte Formate

| Format | Beschreibung |
|---|---|
| **CSV** | Kommagetrennte Textdatei (haeufigster Fall) |
| **Strukturierte Dateien** | Weitere strukturierte Formate nach Absprache |

## Import-Workflow

### 1. Datei vorbereiten

Stellen Sie sicher, dass Ihre Exportdatei folgende Felder enthaelt (sofern verfuegbar):

| Feld | Pflicht | Beschreibung |
|---|---|---|
| Nachname | Ja | Nachname des Patienten |
| Vorname | Ja | Vorname des Patienten |
| Geburtsdatum | Ja | Format: TT.MM.JJJJ oder JJJJ-MM-TT |
| Geschlecht | Nein | M, W, D |
| Versicherungstyp | Nein | GKV, PKV |
| KVNR | Nein | Krankenversichertennummer |
| Versicherungsnummer | Nein | Versichertennummer |
| Strasse | Nein | Strasse und Hausnummer |
| PLZ | Nein | Postleitzahl |
| Ort | Nein | Wohnort |
| Telefon | Nein | Telefonnummer |
| E-Mail | Nein | E-Mail-Adresse |

### 2. Datei hochladen

1. Navigieren Sie zu **Verwaltung** > **Datenimport**.
2. Klicken Sie auf **Datei auswaehlen**.
3. Waehlen Sie die vorbereitete CSV-Datei.
4. MediPrax liest die Datei ein und zeigt eine **Vorschau**.

### 3. Feldzuordnung (Mapping)

MediPrax versucht, die Spalten der CSV-Datei automatisch den Systemfeldern zuzuordnen. Pruefen und korrigieren Sie die Zuordnung:

1. Fuer jede Spalte der CSV-Datei wird ein **MediPrax-Feld** vorgeschlagen.
2. Korrigieren Sie falsche Zuordnungen ueber das Dropdown.
3. Nicht benoetigte Spalten koennen mit **Ignorieren** uebersprungen werden.

### 4. Import starten

1. Pruefen Sie die **Vorschau** der ersten Datensaetze.
2. Klicken Sie auf **Import starten**.
3. MediPrax importiert die Daten und zeigt einen **Ergebnisbericht**.

## Ergebnisbericht

Nach dem Import erhalten Sie eine Zusammenfassung:

| Kennzahl | Beschreibung |
|---|---|
| **Erfolgreich importiert** | Anzahl der erfolgreich importierten Patienten |
| **Uebersprungen** | Bereits vorhandene Patienten (Duplikaterkennung) |
| **Fehler** | Datensaetze mit fehlenden Pflichtfeldern oder ungueltigem Format |

## Duplikaterkennung

MediPrax erkennt moegliche Duplikate anhand von:

- Uebereinstimmung von **Nachname** + **Vorname** + **Geburtsdatum**
- Uebereinstimmung der **KVNR**

Duplikate werden uebersprungen und im Ergebnisbericht aufgelistet.

!!! warning "Wichtig"
    Fuehren Sie den Import **vor dem produktiven Betrieb** durch. Testen Sie zunaechst mit einer kleinen Stichprobe, um die korrekte Feldzuordnung sicherzustellen.

!!! tip "Tipp"
    Bereinigen Sie die Exportdatei vor dem Import: Entfernen Sie leere Zeilen, pruefen Sie Datumsformate und stellen Sie sicher, dass die Zeichenkodierung UTF-8 ist.

!!! info "Hinweis"
    Der Datenimport betrifft ausschliesslich Patientenstammdaten. Klinische Daten (Befunde, Kontakte, Medikation) muessen separat migriert oder neu erfasst werden.
