# GDT-Geraeteschnittstelle

Die GDT-Schnittstelle (Geraetedatentransfer) ermoeglicht die Kommunikation zwischen MediPrax und medizinischen Geraeten nach dem GDT-Standard 2.1. So koennen Untersuchungsergebnisse automatisch in die Patientenakte uebernommen werden.

## Unterstuetzte Geraete

| Geraet | Untersuchung | Typische Anwendung |
|---|---|---|
| **EEG** | Elektroenzephalographie | Epilepsiediagnostik, Anfallsklassifikation |
| **EMG** | Elektromyographie | Neuromuskulaere Diagnostik |
| **NLG** | Nervenleitgeschwindigkeit | Polyneuropathie-Diagnostik |
| **Doppler** | Dopplersonographie | Gefaessdiagnostik hirnversorgender Gefaesse |
| **Evozierte Potenziale** | VEP, AEP, SEP, MEP | MS-Diagnostik, Hirnstammlaesionen |

## GDT-Protokoll

MediPrax kommuniziert ueber den **GDT 2.1 Standard** mit zwei Satzarten:

| Satzart | Richtung | Beschreibung |
|---|---|---|
| **6310** | MediPrax → Geraet | Patientendaten an das Geraet senden (Untersuchungsanforderung) |
| **6311** | Geraet → MediPrax | Ergebnisse vom Geraet empfangen |

## Untersuchung starten

1. Oeffnen Sie die [Patientenakte](../patienten/akte.md).
2. Klicken Sie auf **Geraeteuntersuchung starten**.
3. Waehlen Sie das **Geraet** (z. B. EEG).
4. MediPrax sendet die Patientendaten (Satzart 6310) an das Geraet.
5. Fuehren Sie die Untersuchung am Geraet durch.
6. Die Ergebnisse werden automatisch empfangen.

## Automatischer Dateiempfang

MediPrax ueberwacht automatisch einen konfigurierten **Dateiordner** auf eingehende GDT-Dateien:

- Neue Dateien im GDT-Verzeichnis werden erkannt
- Die Daten werden der korrekten Patientenakte zugeordnet
- Das Ergebnis wird als **Dokument** in der Patientenakte gespeichert

!!! info "Konfiguration"
    Das GDT-Verzeichnis und die Geraetezuordnung werden vom Administrator im Verwaltungsbereich konfiguriert. Wenden Sie sich an Ihren Admin bei Aenderungen an der Geraeteanbindung.

## Manueller Import

Falls die automatische Uebertragung nicht funktioniert:

1. Navigieren Sie zu **Geraete** > **GDT-Import**.
2. Waehlen Sie die **GDT-Datei** von Ihrem Rechner.
3. MediPrax liest die Datei ein und ordnet sie dem Patienten zu.
4. Das Ergebnis erscheint in der Patientenakte unter **Dokumente**.

## Ergebnisse einsehen

Alle empfangenen Geraeteergebnisse finden Sie in der Patientenakte:

- Im Bereich **Dokumente** als eigenstaendiges Dokument
- Mit Datum, Geraetetyp und Ergebnisdaten
- Aeltere Befunde bleiben fuer den Verlaufsvergleich erhalten

!!! tip "Tipp"
    Starten Sie die Untersuchung immer ueber MediPrax (Satzart 6310), damit die Patientendaten korrekt am Geraet angezeigt werden und die Zuordnung der Ergebnisse automatisch funktioniert.

!!! warning "Wichtig"
    Stellen Sie sicher, dass das Geraet und MediPrax auf dasselbe GDT-Verzeichnis zugreifen. Bei Netzwerkproblemen nutzen Sie den manuellen Import.
