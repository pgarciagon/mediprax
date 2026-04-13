# Privatrechnungen (GOAe)

Fuer privatversicherte Patienten (PKV) erstellen Sie Rechnungen nach der Gebuehrenordnung fuer Aerzte (GOAe). MediPrax unterstuetzt den gesamten Rechnungsprozess von der Erstellung bis zum Zahlungseingang. Sie erreichen die Verwaltung ueber die Registerkarte **Privatrechnungen (GOAe)** auf der Abrechnungsseite oder direkt unter `/abrechnung/privatrechnungen`.

## Rechnung erstellen

1. Navigieren Sie zu **Abrechnung** und klicken Sie auf die Registerkarte **Privatrechnungen (GOAe)**.
3. Klicken Sie auf **Neue Rechnung**.
4. Die **Rechnungsnummer** wird automatisch vergeben (Format: RE-JJJJ-NNNN, z. B. RE-2026-0042).

## GOAe-Positionen hinzufuegen

Fuer jede erbrachte Leistung fuegen Sie eine GOAe-Position hinzu:

| Feld | Beschreibung |
|---|---|
| **GOAe-Ziffer** | Nummer der Gebuehrenposition |
| **Beschreibung** | Leistungsbeschreibung (automatisch) |
| **Steigerungsfaktor** | Multiplikator fuer den Gebuehrensatz |
| **Betrag** | Berechnet sich automatisch aus Gebuehrensatz x Faktor |

### Steigerungsfaktor

| Faktorbereich | Regelung |
|---|---|
| **1.0 - 2.3** | Regelspanne, keine Begruendung erforderlich |
| **2.3 - 3.5** | Ueberschreitung, **Begruendung erforderlich** |
| **> 3.5** | Nicht zulaessig |

!!! warning "Begruendungspflicht"
    Bei einem Steigerungsfaktor **ueber 2.3** muessen Sie eine nachvollziehbare Begruendung angeben. Typische Begruendungen sind erhoehter Zeitaufwand, besondere Schwierigkeit oder aussergewoehnliche Umstaende.

## Rechnungsstatus

Jede Rechnung durchlaeuft folgenden Workflow:

| Status | Beschreibung |
|---|---|
| **Entwurf** | Rechnung ist in Bearbeitung |
| **Versendet** | Rechnung wurde an den Patienten verschickt |
| **Teilbezahlt** | Teilzahlung eingegangen |
| **Bezahlt** | Rechnung vollstaendig beglichen |
| **Ueberfaellig** | Zahlungsfrist ueberschritten |

## Zahlungsverfolgung

1. Oeffnen Sie die Rechnung.
2. Klicken Sie auf **Zahlung erfassen**.
3. Tragen Sie den **Zahlungsbetrag** und das **Datum** ein.
4. Der Status wird automatisch aktualisiert (Teilbezahlt/Bezahlt).

## Mahnwesen

Bei ueberfaelligen Rechnungen:

1. Filtern Sie Rechnungen nach Status **Ueberfaellig**.
2. Klicken Sie auf **Mahnung erstellen**.
3. Die Mahnung wird als PDF generiert.
4. Versenden Sie die Mahnung an den Patienten.

## GOAe-Rechnung als PDF

Sie koennen jede Rechnung als professionelles PDF-Dokument herunterladen:

1. Oeffnen Sie die Rechnungsuebersicht unter **Abrechnung > Privatrechnungen**.
2. Klicken Sie auf **Detail** bei der gewuenschten Rechnung.
3. Im Detail-Dialog klicken Sie auf **PDF herunterladen**.
4. Das PDF enthaelt alle Pflichtangaben nach § 14 UStG:
   - Praxisname und -adresse
   - Patientenname und -adresse
   - Rechnungsnummer und -datum
   - Auflistung aller GOAe-Positionen mit Ziffer, Beschreibung, Faktor, Punktzahl und Betrag
   - Gesamtbetrag
   - Zahlungsziel und Bankverbindung
   - Hinweis auf Umsatzsteuerbefreiung (§ 4 Nr. 14a UStG)

!!! tip "Tipp"
    Laden Sie die Rechnung als PDF herunter, bevor Sie den Status auf **Versendet** setzen, um eine Kopie fuer Ihre Unterlagen zu haben.

## Rechnungsuebersicht

Die Rechnungsuebersicht zeigt alle Rechnungen mit:

- Rechnungsnummer und Datum
- Patient
- Gesamtbetrag
- Status
- Offener Betrag

Filtern Sie nach Status, Zeitraum oder Patient.

!!! tip "Tipp"
    Erstellen Sie Rechnungen zeitnah nach der Behandlung. So vermeiden Sie Rueckstaende am Quartalsende und verbessern den Cashflow der Praxis.

!!! info "Hinweis"
    Privatrechnungen sind unabhaengig von der EBM-Abrechnung. PKV-Patienten werden nicht ueber den KVDT-Export abgerechnet.
