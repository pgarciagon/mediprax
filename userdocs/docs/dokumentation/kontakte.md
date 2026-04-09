# Kontakte (Behandlungsdokumentation)

Ein **Kontakt** dokumentiert eine einzelne Behandlungsbegegnung mit einem Patienten. Hier erfassen Sie Diagnosen, Behandlungsdauer, klinische Notizen und Abrechnungsziffern.

## Neuen Kontakt erstellen

1. Oeffnen Sie die [Patientenakte](../patienten/akte.md) des Patienten.
2. Klicken Sie auf **Neuer Kontakt**.
3. Fuellen Sie die folgenden Felder aus:

| Feld | Beschreibung | Pflicht |
|---|---|---|
| **Datum** | Datum des Kontakts (Standard: heute) | Ja |
| **Dauer** | Behandlungsdauer in Minuten | Ja |
| **ICD-10-Diagnosen** | Ein oder mehrere Diagnosecodes | Ja |
| **Notizen** | Freitextnotizen zur Behandlung | Nein |

4. Klicken Sie auf **Speichern**.

## ICD-10-Diagnosen

Diagnosen werden ueber eine Suchfunktion erfasst:

1. Tippen Sie im Diagnosefeld den **ICD-10-Code** (z. B. "F32.1") oder einen **Suchbegriff** (z. B. "Depression").
2. Waehlen Sie die passende Diagnose aus der Vorschlagsliste.
3. Sie koennen **mehrere Diagnosen** pro Kontakt erfassen.

!!! info "Hinweis"
    Die ICD-10-Diagnosen sind Grundlage fuer die [GOP-Vorschlaege](../abrechnung/gop-vorschlaege.md) und die Zuordnung zu psychiatrischen (F-Codes) oder neurologischen (G-Codes) Abrechnungsziffern.

## Abrechnungsziffern (GOP)

Zu jedem Kontakt koennen Abrechnungsziffern hinzugefuegt werden:

1. Klicken Sie auf **GOP hinzufuegen** im Abrechnungsbereich des Kontakts.
2. Geben Sie den **GOP-Code** ein (z. B. "21220") oder nutzen Sie die [GOP-Vorschlaege](../abrechnung/gop-vorschlaege.md).
3. Pruefen Sie die automatisch ermittelte Punktzahl.
4. Speichern Sie den Kontakt.

!!! tip "GOP-Vorschlaege"
    MediPrax schlaegt automatisch passende GOPs vor, basierend auf:

    - Erster Kontakt im Quartal (Grundpauschale)
    - Behandlungsdauer (zeitbezogene GOPs ab 10 Min.)
    - Diagnosen (Fach-spezifische GOPs)
    
    Siehe [GOP-Vorschlaege](../abrechnung/gop-vorschlaege.md) fuer Details.

## Dauer-Tracking

Die Behandlungsdauer ist wichtig fuer die Abrechnung zeitabhaengiger GOPs:

- **Ab 10 Minuten**: Gespraechsziffern (16220, 21220) abrechenbar
- **Ab 50 Minuten**: Psychotherapie-Ziffern

!!! warning "Wichtig"
    Dokumentieren Sie die Dauer gewissenhaft. Sie ist Grundlage fuer die [Plausibilitaetspruefung](../abrechnung/plausibilitaet.md) und muss bei Zeitbezug nachvollziehbar sein.

## Kontakte einsehen

Alle Kontakte eines Patienten finden Sie in der [Patientenakte](../patienten/akte.md) im Tab **Kontakte**, chronologisch sortiert mit Datum, Diagnosen und abgerechneten GOPs.
