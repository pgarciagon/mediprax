# Strukturierte Dokumentation (ABDTP-Sektionen)

## Überblick

Ab sofort werden klinische Befunde in **strukturierten Sektionen** dokumentiert, statt in einem einzelnen Freitextfeld. Die Sektionen orientieren sich am bewährten **ABDTP-Schema**:

| Kürzel | Sektion | Beschreibung |
|--------|---------|-------------|
| **A** | Anamnese | Aktuelle Beschwerden, Vorgeschichte |
| **B** | Befund | Klinische Untersuchungsergebnisse |
| **D** | Diagnose | Diagnostische Einordnung, Differentialdiagnosen |
| **T** | Therapie | Behandlung, Medikation, Maßnahmen |
| **P** | Procedere | Weiteres Vorgehen, Wiedervorstellung |
| **S** | Sonstiges | Zusätzliche Anmerkungen |

## Neue Konsultation anlegen

Beim Anlegen einer neuen Konsultation sehen Sie sechs Textfelder, jeweils mit farbigem Buchstaben-Badge (A, B, D, T, P, S). Füllen Sie die relevanten Sektionen aus — leere Sektionen werden nicht gespeichert.

### Vorlagen nutzen

Vorlagen befüllen automatisch die passenden Sektionen. Wenn Sie z.B. die Vorlage „Erstgespräch Psychiatrie" wählen, wird der Anamnese-Bereich vorausgefüllt, der Befund-Bereich mit Platzhaltern für den psychopathologischen Befund usw.

## Integrierter Befund (B-Sektion)

Die Befund-Sektion (B) zeigt alle klinischen Befunde aus verschiedenen Quellen **zusammengefasst** an:

### Strukturierte Befunde

Wenn für die Konsultation ein **psychopathologischer Befund** (AMDP) oder eine **neurologische Untersuchung** erhoben wurde, erscheinen diese als eigene Blöcke innerhalb der B-Sektion:

- **Psychopathologischer Befund** — Der automatisch generierte Narrativtext aus der AMDP-Erhebung. Über den Button „Bearbeiten" gelangen Sie direkt zur Befunderhebung.
- **Neurologischer Befund** — Der Narrativtext aus der strukturierten neurologischen Untersuchung. Auch hier führt „Bearbeiten" direkt zur Untersuchungsseite.

Falls noch kein strukturierter Befund vorliegt, können Sie über die Buttons „+ Psychopathologischen Befund erheben" bzw. „+ Neurologischen Befund erheben" direkt einen neuen Befund anlegen.

### Ergänzender Freitext-Befund

Unterhalb der strukturierten Befunde steht weiterhin ein Freitextfeld für **ergänzende Befunde** zur Verfügung (z.B. Vitalzeichen, körperliche Untersuchung). Hier können Sie auch Textbausteine mit `#` verwenden.

### Vorteile der Integration

- **Keine Doppeldokumentation** — Strukturierte Befunde werden automatisch im Befund-Tab angezeigt
- **Alle Befunde an einem Ort** — AMDP, Neuro und Freitext in einer Ansicht
- **Direkte Navigation** — Von der Befund-Ansicht direkt zur Bearbeitung der strukturierten Erhebung

## Bestehende Konsultationen (Legacy)

Konsultationen, die vor der Umstellung angelegt wurden, zeigen weiterhin das alte Freitext-Notizfeld an. Zusätzlich erscheint ein gelber Hinweis:

!!! info "Migration"
    Klicken Sie auf **„Zu Sektionen migrieren"**, um den bestehenden Text automatisch in ABDTP-Sektionen aufzuteilen. Der Parser erkennt Überschriften wie `Anamnese:`, `Befund:` oder Kürzel wie `A:`, `B:` etc.

Falls keine Überschriften erkannt werden, wird der gesamte Text in die Sektion **Sonstiges** übernommen.

## Suche nach Sektionen

In der globalen Suche können Sie gezielt in bestimmten Sektionen suchen:

- `tremor in:befund` — sucht „tremor" nur in Befund-Sektionen
- `lithium in:therapie` — sucht „lithium" nur in Therapie-Sektionen
- `in:anamnese schlafstörung` — sucht in Anamnese-Sektionen

## Arztbrief-Integration

Beim Erstellen eines Arztbriefs aus einer Konsultation mit ABDTP-Sektionen wird der Brieftext automatisch aus den einzelnen Sektionen zusammengesetzt. Bei älteren Konsultationen ohne Sektionen wird wie bisher das Notizfeld verwendet.
