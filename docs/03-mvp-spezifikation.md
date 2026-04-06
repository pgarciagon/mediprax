# 3. MediPrax MVP-Spezifikation

> **MVP-Prinzip:** Nur das bauen, was die Praxis tatsächlich täglich braucht. Jede Funktion, die nicht in der ersten Woche vermisst wird, kommt in eine spätere Iteration.

## 3.1 Pflichtfunktionen (MVP — unverzichtbar)

| Modul | Funktionen | Begründung |
|-------|-----------|-----------|
| Patientenverwaltung | Stammdaten, Suche, Versicherungsdaten, eGK-Einlesen | Kernfunktion jedes PVS |
| Terminplanung | Tages-/Wochenansicht, 5–10-Min-Slots, Patientensuche | Optimiert für den Kurztermin-Workflow der Praxis |
| Klinische Dokumentation | Freitext-Befunde, ICD-10-Kodierung, Vorlagen Psych/Neuro | Minimum für den ärztlichen Alltag |
| Verordnung | E-Rezept-Anbindung, Medikamentenliste, BtM-Dokumentation | Gesetzlich vorgeschrieben |
| Abrechnung EBM | GOP-Erfassung (Kap. 16/21), Plausibilitätsprüfung, KVDT-Export | Ohne Abrechnung kein Praxisbetrieb |
| TI-Anbindung | eGK/VSDM, ePA Lesen/Schreiben, E-Rezept, KIM | Gesetzlich vorgeschrieben seit 2025/2026 |
| Arztbrief | Strukturierte Briefe, Versand über KIM | Tägliche Kommunikation mit überweisenden Ärzten |

## 3.2 Optionale Funktionen (spätere Iterationen)

| Modul | Priorität | Zeitrahmen |
|-------|----------|-----------|
| GOÄ-Abrechnung (Privatpatienten) | Hoch | Iteration 2 (Monat 5–6) |
| Online-Terminbuchung (Patienten-Portal) | Mittel | Iteration 3 (Monat 7–9) |
| Laboranbindung (elektronisch) | Mittel | Iteration 3 |
| Erweiterte Statistiken / Dashboard | Niedrig | Iteration 4 (Monat 10+) |
| Dokumentenscanning / DMS | Niedrig | Iteration 4 |
| Impfmanagement | Niedrig | Iteration 5 (bei Bedarf) |
| NFDM (Notfalldatenmanagement) | Niedrig | Iteration 5 |

## 3.3 Detaillierte MVP-Module

### Patientenverwaltung

- Anlegen / Bearbeiten / Suchen von Patientenstammdaten
- Versicherungsdaten (GKV/PKV) mit Kassennummer, Versichertennummer
- eGK-Einlesen über Konnektor/Kartenlesegerät (VSDM)
- Patientenhistorie (Besuche, Diagnosen, Verordnungen)
- Schnellsuche (Name, Geburtsdatum, Versichertennummer)

### Terminplanung

- Tages- und Wochenansicht
- Optimiert für 5–10-Minuten-Kurztermine (Hauptworkflow Neuropsychiatricum)
- Mehrere Kalender (pro Arzt)
- Terminvergabe mit Patientenzuordnung
- Wartezimmerverwaltung (wer ist da, wer wird gerade behandelt)

### Klinische Dokumentation

- Freitext-Befunddokumentation
- ICD-10-GM-Kodierung mit Suchfunktion (deutscher Katalog)
- Vorlagen für psychiatrische Befunde
- Vorlagen für neurologische Untersuchungen
- Dokumentation der Gesprächsdauer (Pflicht für GOPs 16220/21220)

### Verordnung / E-Rezept

- E-Rezept-Erstellung über TI
- Medikamentenliste pro Patient
- BtM-Dokumentation (Betäubungsmittel — relevant für Psychiatrie)
- Interaktionsprüfung (Basis)

### EBM-Abrechnung

- Erfassung von Gebührenordnungspositionen (GOPs)
- Kapitel 16 (Neurologie): GOP 16220 etc.
- Kapitel 21 (Psychiatrie): GOP 21213–21215, 21220 etc.
- Grundpauschalen für Nervenärzte
- Plausibilitätsprüfung (Ausschlüsse, Mengenbegrenzungen)
- KVDT-Export für Übermittlung an KV Bremen (KVHB)

### TI-Anbindung

- VSDM (Versichertenstammdatenmanagement) — eGK-Einlesen
- ePA Lesen und Schreiben (FHIR, Firely SDK)
- E-Rezept
- KIM (Kommunikation im Medizinwesen) — sichere Arztbriefe

### Arztbrief-Generierung

- Strukturierte PDF-Arztbriefe
- Konfigurierbare Vorlagen (Briefkopf, Textbausteine)
- Versand über KIM
- Empfängerverzeichnis (überweisende Ärzte)

## 3.4 Pilotpraxis: Neuropsychiatricum Bremen

### Profil

| Aspekt | Details |
|--------|---------|
| Praxisform | Gemeinschaftspraxis |
| Fachrichtungen | Psychiatrie und Neurologie |
| Adresse | Ärztehaus am Rotes Kreuz Krankenhaus, Osterstraße 1a, 28199 Bremen |
| Sprechzeiten | Mo, Di, Do: 8–12 Uhr und 14–17 Uhr; Fr: 8–12 Uhr |
| Schwerpunkt | Medizinisch-psychiatrische Konsultationen (keine Langzeitpsychotherapie) |
| Terminart | Kurze Termine (5–10 Minuten) |

### Vorhandene IT-Infrastruktur

| Komponente | Spezifikation |
|-----------|--------------|
| Arbeitsplätze | Windows 11 |
| Server | Windows Server 2022 |
| Aktuelle Software | CGM Medistar |
| TI-Anbindung | Konnektor PTV4+ |
| Karten | eHBA und SMC-B |

### Typischer Workflow

1. **Patientenempfang:** eGK-Einlesen, Versicherungsdatenprüfung (VSDM)
2. **Konsultation:** Symptomerfassung, neurologische/psychiatrische Untersuchung, ICD-10-Diagnose
3. **Verordnung:** E-Rezept, Psychopharmaka mit BtM-Kontrollen
4. **Dokumentation:** ePA-Aktualisierung, Arztbriefe, Befundberichte
5. **Abrechnung:** EBM-Kodierung (Kap. 16/21 GOPs), GOÄ für Privatpatienten
6. **Terminverwaltung:** Kalender mit häufigen Kurzterminen
