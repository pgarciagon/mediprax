# Automatische Diagnosevorschlaege

## Uebersicht

MediPrax kann automatisch ICD-10-Diagnosen vorschlagen, basierend auf den strukturierten Befunden einer Konsultation. Die Vorschlaege erscheinen im Bereich **ICD-10-GM Diagnosen** der Konsultation, sobald mindestens ein strukturierter Befund (psychopathologisch, neurologisch oder Suizidalitaet) erfasst wurde.

## Voraussetzungen

Die Diagnosevorschlaege werden nur angezeigt, wenn:

- Die Konsultation bereits gespeichert wurde (Bearbeitungsmodus)
- Mindestens ein strukturierter Befund vorhanden ist:
  - **Psychopathologischer Befund (AMDP)** mit Symptomen, deren Schweregrad > 0 ist
  - **Neurologischer Befund** mit pathologischen Ergebnissen
  - **Suizidalitaetsbewertung** mit positiver Suizidideation

## Funktionsweise

### Zugang

1. Oeffnen Sie eine bestehende Konsultation ueber **Patienten > [Patient] > Konsultation bearbeiten**
2. Erfassen Sie strukturierte Befunde ueber die Buttons im Bereich "Strukturierte Befunde"
3. Die Vorschlaege erscheinen automatisch unterhalb der manuell hinzugefuegten Diagnosen

### Vorschlagspanel

Das Panel zeigt fuer jeden Vorschlag:

- **Konfidenz-Badge**: Hoch (gruen), Mittel (gelb) oder Niedrig (grau) — gibt an, wie sicher der Vorschlag ist
- **ICD-10-Code**: z.B. F32.1
- **Beschreibung**: z.B. "Mittelgradige depressive Episode"
- **Begruendung**: Welche Befunde den Vorschlag ausgeloest haben (per Hover vollstaendig sichtbar)
- **Uebernehmen-Button (+)**: Klicken Sie, um den Vorschlag als Diagnose zu uebernehmen

### Unterstuetzte Diagnosen

**Aus psychopathologischem Befund (AMDP):**

| Diagnose | ICD-10 | Kriterien |
|----------|--------|-----------|
| Leichte Depression | F32.0 | Depressive Stimmung (Schweregrad 1+) + 2-3 Begleitsymptome |
| Mittelgradige Depression | F32.1 | Depressive Stimmung (Schweregrad 2+) + 4+ Begleitsymptome |
| Schwere Depression | F32.2 | Depressive Stimmung (Schweregrad 3) + 5+ Begleitsymptome |
| Schwere Depression mit Psychose | F32.3 | F32.2-Kriterien + Halluzinationen oder Wahn |
| Manie | F30.1 | Gehobene Stimmung (Schweregrad 2+) + Antriebssteigerung + weitere |
| Paranoide Schizophrenie | F20.0 | Wahn (Schweregrad 2+) oder Halluzinationen + Ich-Stoerungen |
| Wahnhafte Stoerung | F22.0 | Wahn (Schweregrad 2+) ohne Halluzinationen |
| Generalisierte Angststoerung | F41.1 | Angst (Schweregrad 2+) + Gruebeln + somatische Symptome |
| Zwangsgedanken | F42.0 | Zwangsgedanken (Schweregrad 2+) |
| Delir | F05.0 | Bewusstseinsstoerung + Orientierungsstoerung |
| Leichte kognitive Stoerung | F06.7 | Aufmerksamkeits- + Gedaechtnisdefizit |

**Aus neurologischem Befund:**

| Diagnose | ICD-10 | Kriterien |
|----------|--------|-----------|
| Parkinson-Syndrom | G20.0 | Rigor + kleinschrittiger Gang + vermindertes Mitschwingen |
| Polyneuropathie | G62.9 | Sensibilitaetsstoerung + abgeschwachte Achillesreflexe |
| Hemiparese | G81.9 | Einseitige Schwaeche in Arm und/oder Bein |
| Fazialisparese | G51.0 | Asymmetrische Gesichtsinnervation |
| Meningismus | R29.1 | Nackensteifigkeit, Kernig- oder Brudzinski-Zeichen |

**Aus Suizidalitaetsbewertung:**

| Diagnose | ICD-10 | Kriterien |
|----------|--------|-----------|
| Suizidalitaet | R45.8 | Suizidgedanken vorhanden |

## Wichtige Hinweise

- Die Vorschlaege sind **Entscheidungsunterstuetzung**, keine automatischen Diagnosen
- Der Arzt muss jeden Vorschlag klinisch bewerten, bevor er uebernommen wird
- Bereits hinzugefuegte Diagnosen werden im Panel als "hinzugefuegt" markiert
- Die Vorschlaege werden bei jedem Laden der Konsultation neu berechnet
