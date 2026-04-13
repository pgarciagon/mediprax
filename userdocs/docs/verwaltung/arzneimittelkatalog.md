# Arzneimittelkatalog

Der Arzneimittelkatalog enthält eine umfassende Datenbank mit ca. 300 Medikamenten aus dem Bereich Psychiatrie und Neurologie.

## Zugang

**Navigation:** Verwaltung > Arzneimittelkatalog

**Berechtigung:** Arzt, Admin

## Funktionen

### Suche

- Geben Sie mindestens **2 Zeichen** in das Suchfeld ein
- Gesucht wird nach **Handelsname**, **Wirkstoff** oder **PZN** (Pharmazentralnummer)
- Die Suche ist nicht zwischen Groß- und Kleinschreibung unterscheidend

### Kategorie-Filter

Filtern Sie nach Medikamentenkategorie:

- **Antidepressiva** (SSRI, SNRI, Trizyklische, Andere)
- **Antipsychotika** (Atypisch, Typisch, Depot)
- **Stimmungsstabilisierer** (Lithium, Valproat, Carbamazepin, Lamotrigin)
- **Anxiolytika** (Benzodiazepine, Buspiron)
- **Hypnotika** (Z-Substanzen)
- **Antikonvulsiva** (Levetiracetam, Lacosamid, Pregabalin u.a.)
- **Parkinson-Medikamente** (Levodopa, Dopaminagonisten, MAO-B-Hemmer)
- **MS-Therapeutika** (Dimethylfumarat, Fingolimod, Ocrelizumab u.a.)
- **Migräne-Therapeutika** (Triptane, CGRP-Antikörper, Prophylaxe)
- **Stimulanzien** (Methylphenidat, Lisdexamfetamin, Atomoxetin)
- **Substitutionsmittel** (Methadon, Buprenorphin)
- **Sonstige** (Melatonin, Anticholinergika, Suchttherapeutika)

### Statistik-Übersicht

Am oberen Rand der Seite werden angezeigt:

- **Gesamtanzahl** der Katalogeinträge
- **BtM-Präparate** (Betäubungsmittel) — rot markiert
- **T-Rezept-pflichtige** Medikamente — gelb markiert
- Anzahl pro Kategorie

### Ergebnistabelle

Jeder Eintrag zeigt:

| Spalte | Beschreibung |
|--------|-------------|
| Handelsname | Name des Fertigarzneimittels |
| Wirkstoff | Aktiver Wirkstoff |
| ATC | ATC-Klassifikationscode |
| Stärke | Wirkstoffgehalt (z.B. 50 mg) |
| Darreichungsform | Tabletten, Kapseln, Tropfen etc. |
| Packung | Packungsgröße |
| Norm | Normpackungsgröße (N1/N2/N3) |
| PZN | Pharmazentralnummer (8-stellig) |
| Hersteller | Pharmazeutisches Unternehmen |
| Status | BtM- oder T-Rezept-Kennzeichnung |

## Medikamentensuche in anderen Bereichen

Die Medikamentensuche steht auch als Suchkomponente in folgenden Bereichen zur Verfügung:

- **Medikationsplan** — beim Hinzufügen neuer Medikamente
- **E-Rezept** — bei der Rezepterstellung

Dort erscheint ein Suchfeld, das ab 2 Zeichen automatisch Vorschläge aus dem Katalog anzeigt. BtM-Präparate werden mit einem roten "BtM"-Badge gekennzeichnet.

## Datenquelle

Die Katalogdaten werden als Seed-Daten mit der Anwendung ausgeliefert. Die PZN-Nummern sind Platzhalter und müssen vor dem Produktivbetrieb gegen echte PZN aus der AMDatabase/IFA ersetzt werden.
