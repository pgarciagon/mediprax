# Arztbriefe

MediPrax unterstuetzt die Erstellung von Arztbriefen ueber ein vorlagenbasiertes System mit automatischer Befuellung von Patientendaten. Die fertigen Briefe werden als PDF generiert.

## Arztbrief erstellen

1. Oeffnen Sie die [Patientenakte](../patienten/akte.md).
2. Navigieren Sie zu **Dokumente** > **Neuer Arztbrief**.
3. Waehlen Sie eine **Vorlage** (z. B. Entlassbrief, Befundbericht, Konsilbericht).
4. Die Patientendaten werden **automatisch eingetragen**.
5. Ergaenzen Sie die klinischen Inhalte.
6. Klicken Sie auf **Speichern**.

## Vorlagenstruktur

Ein typischer Arztbrief umfasst folgende Abschnitte:

| Abschnitt | Inhalt | Vorausgefuellt |
|---|---|---|
| **Briefkopf** | Praxisadresse, Anschrift Empfaenger | Ja |
| **Patientendaten** | Name, Geburtsdatum, Versicherung | Ja |
| **Diagnosen** | ICD-10-Diagnosen mit Klartext | Ja (aus Kontakten) |
| **Anamnese** | Krankheitsgeschichte, aktuelle Beschwerden | Nein |
| **Befunde** | Psychopathologischer / Neurologischer Befund | Ja (wenn dokumentiert) |
| **Therapie** | Aktuelle Behandlung, Medikation | Teilweise |
| **Medikation** | Aktuelle Medikamentenliste | Ja (aus Medikation) |
| **Empfehlungen** | Weitere Empfehlungen, Procedere | Nein |
| **Grussformel** | Kollegiale Gruesse | Ja |

## Automatische Datenuebernahme

Folgende Daten werden automatisch aus der Patientenakte uebernommen:

- **Stammdaten**: Name, Geburtsdatum, Adresse
- **Diagnosen**: Alle aktiven ICD-10-Diagnosen
- **Befunde**: Letzter psychopathologischer und/oder neurologischer Befund
- **Medikation**: Aktuelle Medikamentenliste mit Dosierungen
- **Psychometrie**: Letzte Testergebnisse (PHQ-9, GAD-7, etc.)

!!! tip "Textbausteine nutzen"
    Verwenden Sie [Textbausteine](../dokumentation/textbausteine.md) fuer haeufig wiederkehrende Formulierungen. Tippen Sie einfach das #-Kuerzel im Textfeld, um den Baustein einzufuegen.

## PDF-Generierung

1. Oeffnen Sie den fertigen Arztbrief.
2. Klicken Sie auf **PDF erstellen**.
3. Das PDF wird mit Briefkopf und professionellem Layout generiert.
4. **Herunterladen** oder **Drucken** Sie das PDF.

## Versand

Arztbriefe koennen auf verschiedenen Wegen versendet werden:

- **Druck und Post**: PDF drucken und per Post versenden
- **KIM**: Verschluesselter Versand ueber die [KIM-Kommunikation](../telematik/kim.md) an andere Praxen/Kliniken
- **E-Mail**: Versand per E-Mail (Datenschutz beachten)

## Archivierung

Alle erstellten Arztbriefe werden in der Patientenakte im Bereich **Dokumente** archiviert und sind jederzeit abrufbar.

!!! warning "Datenschutz"
    Arztbriefe enthalten sensible Patientendaten. Verwenden Sie fuer den elektronischen Versand moeglichst die verschluesselte [KIM-Kommunikation](../telematik/kim.md).

!!! info "Hinweis"
    Die Vorlagen koennen im Verwaltungsbereich angepasst werden. Wenden Sie sich an Ihren Administrator, wenn Sie aenderungen an den Briefvorlagen wuenschen.
