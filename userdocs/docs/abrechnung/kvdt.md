# KVDT-Export

Der KVDT-Export generiert die KBV-konforme Abrechnungsdatei fuer die quartalsweise Einreichung bei der Kassenaerztlichen Vereinigung (KV).

## Voraussetzungen

Bevor Sie den Export durchfuehren:

1. Fuehren Sie die [Quartalsvalidierung](quartalsvalidierung.md) durch.
2. Beheben Sie alle **Fehler** (rote Meldungen).
3. Pruefen Sie alle **Warnungen** (gelbe Meldungen).

!!! warning "Nur fuer Administratoren"
    Der KVDT-Export ist ausschliesslich fuer Benutzer mit der Rolle **Admin** verfuegbar.

## Export durchfuehren

1. Navigieren Sie zu **Abrechnung** > **KVDT-Export** oder rufen Sie die API direkt auf.
2. Waehlen Sie das **Quartal** (z. B. "2026-Q1").
3. Klicken Sie auf **Export starten**.
4. MediPrax generiert die KVDT-Datei.
5. Laden Sie die Datei herunter.

**API-Endpunkt:** `/api/kvdt-export/{quarter}` (z. B. `/api/kvdt-export/2026-Q1`)

## KVDT-Format

Die exportierte Datei entspricht dem **KVDT-Standard der KBV** und enthaelt:

- Praxis- und Arztstammdaten
- Patientendaten (verschluesselt)
- Alle dokumentierten GOPs mit Datum und Diagnosen
- ICD-10-Diagnosen mit Zusatzkennzeichen

## Nach dem Export

1. **Datei pruefen**: Oeffnen Sie die Datei mit dem KBV-Pruefmodul (falls verfuegbar).
2. **An die KV uebermitteln**: Laden Sie die Datei im KV-Portal hoch oder uebermitteln Sie sie per KV-Connect.
3. **Bestaetigung abwarten**: Die KV bestaetigt den Eingang und meldet ggf. Fehler.

## Fristen

| Quartal | Einreichungsfrist (ca.) |
|---|---|
| Q1 (Jan-Maerz) | Bis ca. 10. April |
| Q2 (Apr-Juni) | Bis ca. 10. Juli |
| Q3 (Jul-Sep) | Bis ca. 10. Oktober |
| Q4 (Okt-Dez) | Bis ca. 10. Januar |

!!! info "Hinweis"
    Die genauen Fristen werden von Ihrer Kassenaerztlichen Vereinigung festgelegt und koennen regional variieren. Informieren Sie sich rechtzeitig ueber die aktuellen Termine.

## Fehlerbehebung

| Problem | Loesung |
|---|---|
| Export nicht moeglich | Pruefen Sie die Quartalsvalidierung auf offene Fehler |
| KV meldet Fehler | Korrigieren Sie die Daten in MediPrax und exportieren Sie erneut |
| Fehlende Diagnosen | Ergaenzen Sie ICD-10-Codes an den betroffenen Kontakten |

!!! tip "Tipp"
    Exportieren Sie die KVDT-Datei fruehzeitig nach Quartalsende. So haben Sie genuegend Zeit, eventuelle Fehler zu korrigieren, bevor die Einreichungsfrist ablaeuft.
