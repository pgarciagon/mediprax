# Kopfschmerz-Tagebuch

Das Kopfschmerz-Tagebuch ermoeglicht die systematische Erfassung von Kopfschmerzattacken und unterstuetzt die Diagnosestellung sowie Therapieueberwachung. Sie finden es in der [Patientenakte](../patienten/akte.md) im Bereich **Kopfschmerz-Tagebuch**.

## Eintrag erstellen

1. Oeffnen Sie die Patientenakte.
2. Navigieren Sie zum **Kopfschmerz-Tagebuch**.
3. Klicken Sie auf **Neuer Eintrag**.
4. Dokumentieren Sie die Attacke:

| Feld | Beschreibung | Pflicht |
|---|---|---|
| **Datum** | Datum der Kopfschmerzattacke | Ja |
| **Kopfschmerztyp** | Migraene, Spannungskopfschmerz, Cluster | Ja |
| **Intensitaet (VAS)** | Visuelle Analogskala 0-10 | Ja |
| **Dauer** | Dauer der Attacke in Stunden | Ja |
| **Lokalisation** | Einseitig rechts/links, beidseitig, frontal, okzipital | Nein |
| **Begleitsymptome** | Uebelkeit, Photophobie, Phonophobie | Nein |
| **Aura** | Visuelle, sensorische oder andere Aura-Symptome | Nein |
| **Eingenommene Medikation** | Welches Medikament wurde genommen | Nein |
| **Medikamentenwirksamkeit** | Gut, Maessig, Keine Wirkung | Nein |

5. Klicken Sie auf **Speichern**.

## Kopfschmerztypen

| Typ | Typische Merkmale |
|---|---|
| **Migraene** | Einseitig, pulsierend, 4-72h, mit Uebelkeit/Photophobie |
| **Spannungskopfschmerz** | Beidseitig, drueckend, 30min-7Tage, leicht-mittel |
| **Cluster-Kopfschmerz** | Einseitig periorbital, heftigst, 15-180min, mit autonomen Symptomen |

## Intensitaetsskala (VAS)

| VAS-Wert | Einstufung |
|---|---|
| 0 | Kein Schmerz |
| 1-3 | Leichter Schmerz |
| 4-6 | Maessiger Schmerz |
| 7-9 | Starker Schmerz |
| 10 | Staerkster vorstellbarer Schmerz |

## Begleitsymptome

Bei jedem Eintrag koennen typische Begleitsymptome per Checkbox dokumentiert werden:

- Uebelkeit / Erbrechen
- Lichtempfindlichkeit (Photophobie)
- Laermempfindlichkeit (Phonophobie)
- Verschlechterung bei koerperlicher Aktivitaet
- Autonome Symptome (Traenenfluss, Rhinorrhoe)

## Statistiken

MediPrax berechnet automatisch:

| Kennzahl | Beschreibung |
|---|---|
| **Kopfschmerztage/Monat** | Durchschnittliche Anzahl Tage mit Kopfschmerzen |
| **Mittlere Intensitaet** | Durchschnittlicher VAS-Wert |
| **Medikamententage/Monat** | Tage mit Akutmedikation (relevant fuer Medikamentenuebergebrauch) |
| **Haeufigster Typ** | Vorherrschender Kopfschmerztyp |

!!! warning "Medikamentenuebergebrauch"
    Achten Sie auf die Anzahl der Medikamententage pro Monat. Ab **10-15 Tagen/Monat** (je nach Praeparat) besteht das Risiko eines Medikamentenuebergebrauchskopfschmerzes.

!!! tip "Tipp"
    Das Kopfschmerz-Tagebuch eignet sich hervorragend, um den Therapieerfolg zu dokumentieren. Vergleichen Sie die Kopfschmerzhaeufigkeit vor und nach Therapiebeginn.
