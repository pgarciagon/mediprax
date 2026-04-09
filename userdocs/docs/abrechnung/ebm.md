# EBM-Abrechnung

Die Abrechnung nach dem Einheitlichen Bewertungsmassstab (EBM) ist die Grundlage der Verguetung fuer GKV-Patienten. MediPrax unterstuetzt Sie bei der Erfassung, Pruefung und dem Export der Abrechnungsdaten.

## Grundlagen

### GOP (Gebuehrenordnungsposition)

Jede aerztliche Leistung wird durch eine **GOP** (Gebuehrenordnungsposition) abgebildet. Die GOP hat eine Punktzahl, die mit dem aktuellen Punktwert multipliziert die Verguetung ergibt.

**Formel:** Verguetung = Punktzahl x Punktwert

!!! info "Punktwert"
    Der Punktwert wird quartalsweise von den KVen festgelegt und liegt bei ca. 11-12 Cent pro Punkt (variiert nach Region und Fachgruppe).

### Quartalsabrechnung

Die EBM-Abrechnung erfolgt **quartalsweise**:

| Quartal | Zeitraum |
|---|---|
| Q1 | Januar - Maerz |
| Q2 | April - Juni |
| Q3 | Juli - September |
| Q4 | Oktober - Dezember |

## Relevante EBM-Kapitel

Fuer psychiatrisch-neurologische Praxen sind folgende Kapitel relevant:

| Kapitel | Bereich | Typische GOPs |
|---|---|---|
| **Kap. 16** | Neurologie | 16210-16233 (Grundpauschalen, Gespraeche, Diagnostik) |
| **Kap. 21** | Psychiatrie | 21210-21235 (Grundpauschalen, Gespraeche, Krisenintervention) |
| **Kap. 35** | Psychotherapie | 35100-35179 (Sprechstunde, Probatorik, Einzel-/Gruppentherapie) |

## GOPs erfassen

GOPs werden an [Kontakten](../dokumentation/kontakte.md) dokumentiert:

1. Erstellen oder oeffnen Sie einen Kontakt.
2. Klicken Sie auf **GOP hinzufuegen**.
3. Geben Sie den **GOP-Code** ein oder nutzen Sie die [GOP-Vorschlaege](gop-vorschlaege.md).
4. Die **Punktzahl** wird automatisch angezeigt.
5. Speichern Sie den Kontakt.

## Wichtige GOPs im Ueberblick

| GOP | Beschreibung | Punktzahl |
|---|---|---|
| 16210/16211 | Grundpauschale Neurologie (bis 59 / ab 60 J.) | ~170-200 Pkt |
| 21210/21211 | Grundpauschale Psychiatrie (bis 59 / ab 60 J.) | ~170-200 Pkt |
| 16220 | Neurologisches Gespraech (mind. 10 Min.) | ~130 Pkt |
| 21220 | Psychiatrisches Gespraech (mind. 10 Min.) | ~130 Pkt |
| 35100 | Psychotherapeutische Sprechstunde | ~250 Pkt |
| 35110 | Probatorische Sitzung | ~250 Pkt |
| 35150 | Verhaltenstherapie Einzelsitzung | ~1400 Pkt |

## Abrechnungszyklus

1. **Quartal ueber**: GOPs an Kontakten dokumentieren
2. **Quartalsende**: [Quartalsvalidierung](quartalsvalidierung.md) durchfuehren
3. **Plausibilitaet pruefen**: [Plausibilitaetspruefung](plausibilitaet.md) beachten
4. **Export**: [KVDT-Export](kvdt.md) an die KV

!!! tip "Tipp"
    Nutzen Sie die [GOP-Vorschlaege](gop-vorschlaege.md) von MediPrax, um keine abrechnungsrelevanten Leistungen zu vergessen. Das System schlaegt passende GOPs basierend auf Diagnosen und Behandlungsdauer vor.

!!! warning "Wichtig"
    Achten Sie auf die [Plausibilitaetsregeln](plausibilitaet.md). Ausschluesse und Mengenbegrenzungen fuehren zu Absetzungen durch die KV, wenn sie nicht beachtet werden.
