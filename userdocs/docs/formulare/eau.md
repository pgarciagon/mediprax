# Elektronische Arbeitsunfaehigkeitsbescheinigung (eAU)

Die eAU ermoeglicht die elektronische Uebermittlung von Arbeitsunfaehigkeitsbescheinigungen ueber die Telematikinfrastruktur direkt an die Krankenkasse des Patienten.

## eAU erstellen

1. Oeffnen Sie die [Patientenakte](../patienten/akte.md).
2. Navigieren Sie zu **Formulare** > **AU-Bescheinigung**.
3. Fuellen Sie die Felder aus:

| Feld | Beschreibung | Pflicht |
|---|---|---|
| **Erst-AU / Folge-AU** | Erstbescheinigung oder Folgebescheinigung | Ja |
| **AU seit** | Beginn der Arbeitsunfaehigkeit | Ja |
| **Voraussichtlich AU bis** | Voraussichtliches Ende | Ja |
| **Festgestellt am** | Datum der Feststellung (Standard: heute) | Ja |
| **Diagnose** | ICD-10-Code (vertraulich, nur an Krankenkasse) | Ja |
| **Unfall / Unfallfolgen** | Arbeitsunfall, Wegeunfall, sonstiger Unfall | Nein |
| **Sonstiger Unfall** | Naehere Angaben bei Unfall | Nein |

4. Klicken Sie auf **Speichern**.

## Erst-AU vs. Folge-AU

| Typ | Beschreibung |
|---|---|
| **Erst-AU** | Erste Bescheinigung fuer diese Erkrankungsepisode |
| **Folge-AU** | Verlaengerung einer bestehenden AU. Referenziert die Erst-AU |

!!! info "Hinweis"
    Bei einer Folge-AU muss das Datum "AU seit" mit dem Beginn der urspruenglichen Arbeitsunfaehigkeit uebereinstimmen.

## Elektronische Uebermittlung

Die eAU wird ueber die Telematikinfrastruktur (TI) per [KIM](../telematik/kim.md) an die Krankenkasse des Patienten uebermittelt:

1. Pruefen Sie die AU-Bescheinigung auf Vollstaendigkeit.
2. Klicken Sie auf **Elektronisch uebermitteln**.
3. Die eAU wird signiert und ueber KIM versendet.
4. Der Status wird aktualisiert.

## Statusverfolgung

| Status | Beschreibung |
|---|---|
| **Entwurf** | AU ist in Bearbeitung |
| **Uebermittelt** | eAU wurde an die Krankenkasse gesendet |
| **Bestaetigt** | Eingangsbestaetigung von der Krankenkasse erhalten |
| **Fehlgeschlagen** | Uebermittlung fehlgeschlagen (Fehler pruefen) |

## Patientenausdruck

Auch bei elektronischer Uebermittlung erhaelt der Patient einen **Ausdruck**:

1. Klicken Sie auf **PDF erstellen** nach der Uebermittlung.
2. Drucken Sie das Exemplar fuer den Patienten aus.
3. Das Patientenexemplar enthaelt keine Diagnoseinformation.

!!! tip "Tipp"
    Erstellen Sie die eAU direkt waehrend des Patientenkontakts. So vermeiden Sie nachtraeglichen Aufwand und der Patient erhaelt seinen Ausdruck sofort.

!!! warning "TI-Verbindung erforderlich"
    Die elektronische Uebermittlung erfordert eine aktive Verbindung zur [Telematikinfrastruktur](../telematik/uebersicht.md). Pruefen Sie den TI-Status, wenn die Uebermittlung fehlschlaegt.

!!! note "Hinweis"
    Der Arbeitgeber ruft die AU-Daten direkt bei der Krankenkasse ab. Ein separater Versand an den Arbeitgeber ist seit der eAU-Einfuehrung nicht mehr erforderlich.
