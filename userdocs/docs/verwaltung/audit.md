# Audit-Protokoll

Das Audit-Protokoll zeichnet alle relevanten Systemaktionen lueckenlos auf. Es dient der Nachvollziehbarkeit, Qualitaetssicherung und der Einhaltung der DSGVO-Anforderungen.

## Was wird protokolliert?

Jede Aktion in MediPrax wird mit folgenden Informationen erfasst:

| Feld | Beschreibung | Beispiel |
|---|---|---|
| **Benutzer** | Wer hat die Aktion durchgefuehrt | Dr. Mueller |
| **Zeitstempel** | Wann wurde die Aktion durchgefuehrt | 09.04.2026, 14:32:15 |
| **Aktionstyp** | Art der Aktion | Erstellen, Bearbeiten, Loeschen, Anmelden |
| **Entitaet** | Welches Objekt ist betroffen | Patient, Kontakt, Rezept, Termin |
| **Details** | Was genau wurde geaendert | "Patient Mueller: Telefon geaendert" |
| **IP-Adresse** | Von welchem Geraet | 192.168.1.42 |

## Protokollierte Aktionen

| Bereich | Aktionen |
|---|---|
| **Authentifizierung** | Anmeldung, Abmeldung, Fehlgeschlagene Anmeldeversuche |
| **Patientendaten** | Anlegen, Bearbeiten, Lesen von Patientenstammdaten |
| **Klinische Daten** | Erstellen/Bearbeiten von Kontakten, Befunden, Medikation |
| **Abrechnung** | GOP-Aenderungen, KVDT-Export, Rechnungserstellung |
| **Formulare** | Erstellen von Rezepten, Arztbriefen, AU-Bescheinigungen |
| **Verwaltung** | Benutzer anlegen/aendern, Konfigurationsaenderungen |
| **TI-Aktionen** | eGK-Lesung, E-Rezept-Signierung, KIM-Versand |

## Audit-Protokoll einsehen

1. Navigieren Sie zu **Verwaltung** > **Audit-Protokoll**.
2. Die Eintraege werden chronologisch angezeigt (neueste zuerst).

## Filtern

Nutzen Sie die Filter, um gezielt nach Eintraegen zu suchen:

| Filter | Beschreibung |
|---|---|
| **Benutzer** | Aktionen eines bestimmten Benutzers anzeigen |
| **Aktionstyp** | Nach Art der Aktion filtern (Erstellen, Bearbeiten, etc.) |
| **Datum von / bis** | Zeitraum eingrenzen |
| **Entitaet** | Nach betroffenem Objekt filtern (Patient, Kontakt, etc.) |

## DSGVO-Compliance

Das Audit-Protokoll unterstuetzt die Einhaltung der DSGVO:

- **Art. 5 DSGVO**: Nachvollziehbarkeit der Datenverarbeitung
- **Art. 30 DSGVO**: Verzeichnis der Verarbeitungstaetigkeiten
- **Art. 33 DSGVO**: Erkennung und Dokumentation von Datenschutzverletzungen

!!! info "Aufbewahrung"
    Audit-Eintraege werden dauerhaft gespeichert und koennen nicht geloescht oder veraendert werden. Dies gewaehrleistet die Integritaet des Protokolls.

## Anwendungsfaelle

| Szenario | Vorgehensweise |
|---|---|
| **Wer hat Patientendaten geaendert?** | Filtern nach Entitaet "Patient" + Patientenname |
| **Welche Aktionen hat ein Benutzer durchgefuehrt?** | Filtern nach Benutzer |
| **Gab es unberechtigte Zugriffe?** | Fehlgeschlagene Anmeldeversuche pruefen, ungewoehnliche Uhrzeiten |
| **DSGVO-Auskunftsanfrage** | Alle Eintraege zu einem bestimmten Patienten exportieren |

!!! warning "Datenschutz"
    Das Audit-Protokoll enthaelt sensible Informationen. Der Zugriff ist auf **Administratoren** beschraenkt.

!!! tip "Tipp"
    Pruefen Sie das Audit-Protokoll regelmaessig auf ungewoehnliche Aktivitaeten. Dies ist ein wichtiger Bestandteil des IT-Sicherheitsmanagements Ihrer Praxis.
