# Labormonitoring

Das Labormonitoring ueberwacht faellige Laborkontrollen fuer bestimmte Medikamente und erinnert automatisch an ausstehende Untersuchungen. Es ist eng mit der [Medikamentenverwaltung](verwaltung.md) verknuepft.

## Funktionsweise

Wenn ein Patient ein Medikament mit Monitoring-Pflicht erhaelt, generiert MediPrax automatisch **Erinnerungen** fuer die faelligen Laborkontrollen basierend auf medikamentenspezifischen Zeitplaenen.

## Monitoring-Zeitplaene (Beispiele)

| Medikament | Laborparameter | Intervall |
|---|---|---|
| **Lithium** | Lithiumspiegel, Nierenwerte, Schilddruese | Alle 3 Monate |
| **Clozapin** | Differentialblutbild | Woechentlich (erste 18 Wochen), dann monatlich |
| **Valproat** | Valproatspiegel, Leberwerte, Blutbild | Alle 3-6 Monate |
| **Carbamazepin** | Carbamazepinspiegel, Blutbild, Leberwerte, Natrium | Alle 3-6 Monate |
| **Lamotrigin** | Leberwerte, Blutbild | Alle 6 Monate |
| **Quetiapin** | Nuechternblutzucker, Lipide, Gewicht | Alle 3 Monate |
| **Olanzapin** | Blutzucker, Lipide, Blutbild, Gewicht | Alle 3 Monate |
| **Methylphenidat** | Blutdruck, Puls, Gewicht | Alle 6 Monate |

## Faellige Kontrollen einsehen

1. Navigieren Sie zur [Patientenakte](../patienten/akte.md).
2. Im Bereich **Labormonitoring** sehen Sie alle faelligen und kommenden Kontrollen.
3. Faellige Kontrollen werden **farblich hervorgehoben** (rot = ueberfaellig, gelb = bald faellig).

Das [Dashboard](../erste-schritte/dashboard.md) zeigt ebenfalls faellige Laborkontrollen in den offenen Aufgaben.

## Laborergebnis erfassen

1. Klicken Sie auf die faellige Kontrolle.
2. Tragen Sie die **Laborwerte** ein.
3. MediPrax zeigt automatisch die **Referenzbereiche** an.
4. Werte ausserhalb des Referenzbereichs werden **markiert**.
5. Klicken Sie auf **Speichern**.

## Referenzbereiche

| Parameter | Referenzbereich | Einheit |
|---|---|---|
| Lithium-Spiegel | 0.6 - 0.8 (Akut: bis 1.2) | mmol/l |
| Valproat-Spiegel | 50 - 100 | mg/l |
| Carbamazepin-Spiegel | 4 - 12 | mg/l |
| Leukozyten | 4.000 - 10.000 | /ul |
| GOT/GPT | bis 35 / bis 45 | U/l |
| TSH | 0.27 - 4.2 | mU/l |

## Automatische Erinnerungen

MediPrax berechnet das naechste faellige Kontrolldatum automatisch basierend auf:

- Dem Medikament und seinem Monitoring-Zeitplan
- Dem Datum der letzten Laborkontrolle

!!! warning "Ueberfaellige Kontrollen"
    Ueberfaellige Laborkontrollen stellen ein Sicherheitsrisiko dar. Kontaktieren Sie den Patienten zeitnah, um einen Termin fuer die Blutentnahme zu vereinbaren.

!!! tip "Tipp"
    Nutzen Sie die Monitoring-Uebersicht fuer die Wochenplanung: Welche Patienten muessen diese Woche zur Blutentnahme kommen?

!!! info "Hinweis"
    Die Monitoring-Zeitplaene koennen im Verwaltungsbereich angepasst werden. Die oben genannten Intervalle entsprechen den Empfehlungen der Fachinformationen.
