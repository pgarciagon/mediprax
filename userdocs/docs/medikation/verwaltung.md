# Medikamentenverwaltung

Die Medikamentenverwaltung erfasst und verfolgt alle Medikamente eines Patienten. Sie finden die aktuelle Medikation in der [Patientenakte](../patienten/akte.md) im Bereich **Medikation**.

## Medikament hinzufuegen

1. Oeffnen Sie die Patientenakte.
2. Navigieren Sie zum Bereich **Medikation**.
3. Klicken Sie auf **Medikament hinzufuegen**.
4. Fuellen Sie die Felder aus:

| Feld | Beschreibung | Pflicht |
|---|---|---|
| **Medikament** | Name des Praeparats | Ja |
| **Wirkstoff** | Wirkstoffname | Nein |
| **Dosierung** | Dosierung und Einnahmeschema (z. B. "1-0-0-1") | Ja |
| **Startdatum** | Beginn der Einnahme | Ja |
| **Enddatum** | Ende der Einnahme (leer = laufend) | Nein |
| **Verordnender Arzt** | Name des verordnenden Arztes | Ja |
| **Kategorie** | Medikamentenkategorie | Nein |
| **Notizen** | Besondere Hinweise | Nein |

5. Klicken Sie auf **Speichern**.

## Medikamentenkategorien

Fuer psychiatrische und neurologische Praxen stehen spezialisierte Kategorien bereit:

| Kategorie | Beispiele |
|---|---|
| **Antidepressiva** | Sertralin, Venlafaxin, Mirtazapin, Amitriptylin |
| **Antipsychotika** | Quetiapin, Risperidon, Aripiprazol, Clozapin |
| **Antiepileptika** | Valproat, Levetiracetam, Lamotrigin, Carbamazepin |
| **Anxiolytika/Sedativa** | Lorazepam, Diazepam, Pregabalin |
| **Parkinson-Medikamente** | Levodopa, Pramipexol, Rasagilin |
| **MS-Therapeutika** | Interferon beta, Natalizumab, Ocrelizumab |
| **Stimmungsstabilisierer** | Lithium, Valproat, Lamotrigin |
| **Psychostimulanzien** | Methylphenidat, Lisdexamfetamin |
| **Sonstige** | Alle weiteren Medikamente |

## Aktive und abgesetzte Medikamente

Die Medikationsliste zeigt standardmaessig **aktive Medikamente** (ohne Enddatum). Abgesetzte Medikamente koennen ueber einen Filter eingeblendet werden.

### Medikament absetzen

1. Klicken Sie auf das aktive Medikament.
2. Setzen Sie das **Enddatum**.
3. Dokumentieren Sie ggf. den **Absetzgrund**.
4. Speichern Sie die Aenderung.

## Zusammenspiel mit anderen Modulen

| Modul | Verknuepfung |
|---|---|
| **[Monitoring](monitoring.md)** | Laborkontrollen fuer bestimmte Medikamente |
| **[BtM-Verwaltung](btm.md)** | Betaeubungsmittelrezepte fuer BtM-pflichtige Praeparate |
| **[Rezeptdruck](rezepte.md)** | Rezepterstellung fuer Verordnungen |
| **[Anfallskalender](../krankheiten/epilepsie.md)** | Medikationssnapshot bei Anfaellen |

!!! tip "Tipp"
    Halten Sie die Medikationsliste stets aktuell. Sie bildet die Grundlage fuer Labormonitoring, Rezeptdruck und die Uebersicht in Arztbriefen.

!!! warning "Wichtig"
    Beachten Sie bei BtM-pflichtigen Medikamenten (z. B. Methylphenidat) die besonderen Dokumentationspflichten in der [BtM-Verwaltung](btm.md).
