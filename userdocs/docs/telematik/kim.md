# KIM (Kommunikation im Medizinwesen)

KIM ist der sichere E-Mail-Dienst der Telematikinfrastruktur. Er ermoeglicht den verschluesselten Austausch von Nachrichten und Dokumenten zwischen Leistungserbringern im Gesundheitswesen.

## Funktionsumfang

Ueber KIM koennen Sie:

- **Arztbriefe** verschluesselt an Kollegen versenden
- **eAU-Bescheinigungen** an Krankenkassen uebermitteln
- **Befunde** und andere Dokumente sicher austauschen
- **Nachrichten** von anderen Praxen und Kliniken empfangen

## Nachricht senden

1. Navigieren Sie zu **Telematik** > **KIM** oder oeffnen Sie den KIM-Posteingang.
2. Klicken Sie auf **Neue Nachricht**.
3. Suchen Sie den **Empfaenger** im KIM-Adressbuch (Aerzte, Kliniken, Krankenkassen).
4. Geben Sie einen **Betreff** ein.
5. Verfassen Sie die **Nachricht** oder haengen Sie Dokumente an.
6. Klicken Sie auf **Senden**.

## Arztbriefe per KIM versenden

Der haeufigste Anwendungsfall ist der Versand von [Arztbriefen](../formulare/arztbriefe.md):

1. Erstellen Sie den Arztbrief in der Patientenakte.
2. Generieren Sie das **PDF**.
3. Klicken Sie auf **Per KIM versenden**.
4. Waehlen Sie den **Empfaenger** (Ueberweiser, Hausarzt, Klinik).
5. Der Brief wird verschluesselt uebermittelt.

## Posteingang

Im KIM-Posteingang finden Sie alle empfangenen Nachrichten:

| Spalte | Beschreibung |
|---|---|
| **Absender** | Name und KIM-Adresse des Absenders |
| **Betreff** | Betreffzeile der Nachricht |
| **Datum** | Empfangsdatum und -uhrzeit |
| **Anhaenge** | Anzahl der Dateianhänge |
| **Status** | Gelesen / Ungelesen |

### Nachrichten verarbeiten

1. Klicken Sie auf eine Nachricht, um sie zu oeffnen.
2. Lesen Sie den Inhalt und pruefen Sie Anhaenge.
3. Bei patientenbezogenen Dokumenten: Weisen Sie die Nachricht einem **Patienten** zu.
4. Das Dokument wird in der Patientenakte gespeichert.

## Adressbuch

Das KIM-Adressbuch ermoeglicht die Suche nach Empfaengern:

- Suche nach **Name**, **Fachrichtung** oder **Ort**
- Alle am KIM-Dienst teilnehmenden Leistungserbringer sind auffindbar
- Haeufige Empfaenger koennen als **Favoriten** gespeichert werden

## Verschluesselung

!!! info "Ende-zu-Ende-Verschluesselung"
    Alle KIM-Nachrichten sind **Ende-zu-Ende-verschluesselt**. Nur der vorgesehene Empfaenger kann die Nachricht lesen. Dies erfuellt die hohen Datenschutzanforderungen im Gesundheitswesen.

## Fehlerbehebung

| Problem | Loesung |
|---|---|
| Versand fehlgeschlagen | TI-Verbindung und SMC-B pruefen |
| Empfaenger nicht gefunden | Pruefen Sie, ob der Empfaenger am KIM-Dienst teilnimmt |
| Anhang zu gross | Reduzieren Sie die Dateigroesse (z. B. PDF komprimieren) |

!!! tip "Tipp"
    Versenden Sie Arztbriefe bevorzugt per KIM statt per Post. Der Versand ist schneller, sicherer und spart Porto- und Materialkosten.

!!! warning "Wichtig"
    KIM ist nur fuer die Kommunikation zwischen Leistungserbringern vorgesehen. Fuer die Kommunikation mit Patienten nutzen Sie andere sichere Kanaele.
