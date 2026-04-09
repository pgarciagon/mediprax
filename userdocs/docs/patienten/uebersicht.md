# Patientenuebersicht

Die Patientenliste ist Ihre zentrale Anlaufstelle fuer die Verwaltung aller Patienten in MediPrax. Sie erreichen sie ueber **Patienten** in der Seitenleiste oder direkt unter `/patienten`.

## Aufbau der Liste

Die Patientenliste zeigt alle erfassten Patienten in einer Tabelle mit folgenden Spalten:

| Spalte | Beschreibung |
|---|---|
| **Name** | Nachname, Vorname des Patienten |
| **Geburtsdatum** | Geburtsdatum im Format TT.MM.JJJJ |
| **Versicherung** | Versicherungstyp (GKV oder PKV) |
| **Telefon** | Telefonnummer fuer schnellen Kontakt |

## Suchen und Filtern

Oberhalb der Tabelle steht Ihnen ein **Suchfeld** zur Verfuegung:

1. Geben Sie den **Namen** oder einen Teil des Namens ein.
2. Die Liste wird automatisch gefiltert.
3. Sie koennen zusaetzlich nach **Versicherungstyp** filtern (GKV/PKV).

!!! tip "Tipp"
    Nutzen Sie die Suche, um bei grossen Patientenlisten schnell den gewuenschten Patienten zu finden. Auch die [globale Suche](../erste-schritte/suche.md) in der Seitenleiste durchsucht Patienten.

## Aktionen

- **Patient oeffnen**: Klicken Sie auf eine Zeile, um die [Patientenakte](akte.md) zu oeffnen.
- **Neuen Patienten anlegen**: Klicken Sie auf den Button **Neuer Patient**, um zur [Patientenanlage](anlegen.md) zu gelangen.

## Sortierung

Klicken Sie auf eine **Spaltenueberschrift**, um die Liste nach dieser Spalte zu sortieren. Ein erneuter Klick kehrt die Sortierreihenfolge um.

| Sortierung | Beispiel |
|---|---|
| **Name aufsteigend (A-Z)** | Standardsortierung |
| **Name absteigend (Z-A)** | Erneuter Klick auf Name |
| **Geburtsdatum** | Aelteste oder juengste Patienten zuerst |

## Zugriff nach Benutzerrolle

| Rolle | Berechtigungen in der Patientenliste |
|---|---|
| **Arzt / Admin** | Vollzugriff, Patient oeffnen, anlegen, bearbeiten |
| **MFA** | Patienten einsehen, anlegen und bearbeiten |
| **Empfang** | Patienten einsehen und Termine zuordnen |

!!! note "Hinweis"
    Die Patientenliste zeigt alle aktiven Patienten der Praxis. Es gibt keine Begrenzung der angezeigten Eintraege, jedoch empfiehlt sich bei grossen Praxen die Nutzung der Suchfunktion.

!!! info "eGK-Lesung"
    Wenn ein neuer Patient mit der [eGK](../telematik/egk.md) eingelesen wird, erscheint er automatisch in der Patientenliste. Bestehende Patienten werden bei erneuter eGK-Lesung aktualisiert.
