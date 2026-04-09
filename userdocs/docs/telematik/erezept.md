# E-Rezept

Das E-Rezept (elektronisches Rezept) ersetzt das klassische Papierrezept fuer verschreibungspflichtige Medikamente. MediPrax erstellt, signiert und uebermittelt E-Rezepte ueber die Telematikinfrastruktur.

## E-Rezept erstellen

1. Oeffnen Sie die [Patientenakte](../patienten/akte.md).
2. Navigieren Sie zu **Rezepte** > **Neues E-Rezept**.
3. Fuegen Sie die zu verordnenden **Medikamente** hinzu:

| Feld | Beschreibung |
|---|---|
| **Medikament** | Praeparatname |
| **PZN** | Pharmazentralnummer |
| **Dosierung** | Dosieranweisung |
| **Menge** | Packungsgroesse (N1, N2, N3) |
| **Aut idem** | Austausch zulaessig (Ja/Nein) |

4. Pruefen Sie die Angaben.
5. Klicken Sie auf **E-Rezept erstellen und signieren**.

## Signierung

Das E-Rezept wird mit dem **Heilberufsausweis (HBA)** des Arztes qualifiziert elektronisch signiert:

1. Stellen Sie sicher, dass der **HBA** im Kartenlesegeraet steckt.
2. Geben Sie die **PIN** des HBA ein.
3. Das Rezept wird signiert und an den E-Rezept-Fachdienst uebermittelt.

## Einloesung durch den Patienten

Der Patient kann das E-Rezept einloesen ueber:

- **eGK**: Karte in der Apotheke vorlegen
- **E-Rezept-App**: QR-Code in der App anzeigen
- **Papierausdruck**: QR-Code-Ausdruck in der Apotheke vorlegen

## Statusverfolgung

| Status | Beschreibung |
|---|---|
| **Erstellt** | E-Rezept wurde erstellt, aber noch nicht signiert |
| **Signiert** | E-Rezept wurde signiert und uebermittelt |
| **In Einloesung** | Patient hat das Rezept in einer Apotheke vorgelegt |
| **Dispensiert** | Medikament wurde von der Apotheke ausgegeben |
| **Storniert** | E-Rezept wurde zurueckgezogen |

## E-Rezept stornieren

Falls ein E-Rezept fehlerhaft ist und noch nicht eingeloest wurde:

1. Oeffnen Sie das E-Rezept in der Rezepthistorie.
2. Klicken Sie auf **Stornieren**.
3. Bestaetigen Sie die Stornierung.
4. Erstellen Sie bei Bedarf ein korrigiertes E-Rezept.

!!! warning "Stornierung nur vor Einloesung"
    Ein E-Rezept kann nur storniert werden, solange es noch nicht in einer Apotheke eingeloest wird.

## Patientenausdruck

Fuer Patienten ohne E-Rezept-App koennen Sie einen **Ausdruck mit QR-Code** erstellen:

1. Klicken Sie nach der Signierung auf **Ausdruck erstellen**.
2. Der Ausdruck mit QR-Code wird als PDF generiert.
3. Drucken Sie das PDF fuer den Patienten.

!!! tip "Tipp"
    Informieren Sie Patienten ueber die E-Rezept-App der gematik. Damit koennen Rezepte direkt an eine Wunschapotheke uebermittelt werden.

!!! info "Voraussetzung"
    Fuer E-Rezepte ist eine aktive [TI-Verbindung](uebersicht.md), ein gesteckter HBA und eine gueltige SMC-B erforderlich.
