# 6. Entwicklungsplan

> **Hinweis:** Zeitschätzungen berücksichtigen die erhöhte Produktivität durch Claude Code. Die Gesamtdauer wird jedoch durch regulatorische Prozesse (Zertifizierung) bestimmt, nicht durch die Codeentwicklung.

## 6.1 Phasenübersicht

| Phase | Zeitraum | Schwerpunkt |
|-------|----------|------------|
| Phase 1: Grundlagen + MVP | Monate 1–6 | Basisinfrastruktur und Kernmodule |
| Phase 2: TI-Integration | Monate 7–12 | Telematikinfrastruktur-Anbindung |
| Phase 3: Abrechnung | Monate 13–16 | EBM/GOÄ, KVDT |
| Phase 4: Zertifizierung + Go-Live | Monate 17–24 | KBV, gematik, Migration, Produktivbetrieb |

## 6.2 Phase 1: Grundlagen + MVP (Monate 1–6)

**Ziel:** Funktionsfähige Basis für den Praxisalltag

**Parallele Aktionen:** gematik-Registrierung starten, KBV kontaktieren, Referenzumgebung beantragen.

**Claude Code Schwerpunkt:** Projekt-Scaffolding, Datenbankschema, CRUD-Module, UI-Grundgerüst.

### Meilensteine

1. Projekt-Setup: Repository, CI/CD, Testserver, Datenbankschema
2. Patientenverwaltung (CRUD, Suche, Versicherungsdaten)
3. Terminplanung (Tages-/Wochenansicht, Kurztermin-optimiert)
4. Klinische Dokumentation (Befunde, ICD-10-GM, Neuro/Psych-Vorlagen)
5. Arztbrief-Generierung (PDF-Vorlagen, strukturiert)
6. Basis-Dashboard und Benutzerrollen

### Ergebnis

Funktionsfähiges System, das Patientenverwaltung, Termine, Dokumentation und Arztbriefe abdeckt. Noch ohne TI-Anbindung und Abrechnung.

## 6.3 Phase 2: TI-Integration (Monate 7–12)

**Ziel:** Anbindung an die Telematikinfrastruktur

> **Hohe Komplexität:** Diese Phase erfordert Zugang zur gematik-Referenzumgebung. Die Registrierung als Primärsystem-Hersteller muss frühzeitig erfolgen (idealerweise in Phase 1). Claude Code kann den Integrationscode schreiben, aber der physische Zugang zum Konnektor erfordert manuelle Arbeit.

### Meilensteine

1. Registrierung als Primärsystem-Hersteller bei gematik
2. Konnektor-Integration (VSDM: eGK-Einlesen)
3. ePA-Implementierung (Lesen/Schreiben, FHIR mit Firely SDK)
4. E-Rezept (elektronische Verordnung)
5. KIM (sichere Kommunikation zwischen Leistungserbringern)
6. ECC-256-Kryptographie (Pflicht ab 2026, nativ implementiert)

### Ergebnis

MediPrax ist mit der TI verbunden: eGK-Einlesen, ePA, E-Rezept und KIM funktionieren.

## 6.4 Phase 3: Abrechnung (Monate 13–16)

**Ziel:** Vollständige EBM- und GOÄ-Abrechnung

**Claude Code Schwerpunkt:** EBM-Regelwerk-Engine, KVDT-Generator, GOÄ-Berechnung.

### Meilensteine

1. EBM-Abrechnungsmotor mit Neuro-/Psych-spezifischen GOPs (Kap. 16/21)
2. GOÄ-Abrechnung für Privatpatienten
3. KVDT-Dateigenerierung für Versand an KVHB
4. Validierung gegen Prüfpaket KVDT der KBV
5. Hybrid-DRG (falls zutreffend)
6. Abrechnungsstatistiken und Berichte

### Ergebnis

Vollständige Abrechnungsfähigkeit für GKV (EBM) und PKV (GOÄ).

## 6.5 Phase 4: Zertifizierung + Go-Live (Monate 17–24)

**Ziel:** Alle Zertifizierungen erlangen + Produktivbetrieb starten

> **Regulatorischer Schwerpunkt:** In dieser Phase verlagert sich der Schwerpunkt von der Softwareentwicklung auf regulatorische Prozesse. Claude Code unterstützt bei der Vorbereitung (Test-Automatisierung, Dokumentation), aber die formalen Verfahren erfordern menschliches Engagement.

### Meilensteine

1. KBV-Zertifizierung (Prüfverfahren durchlaufen)
2. gematik KOB (Konformitätsbewertung)
3. DSGVO-Audit
4. Datenmigration aus Medistar
5. Parallelbetrieb (beide Systeme gleichzeitig)
6. Schulung des Praxispersonals
7. Go-Live und Stabilisierung
8. Medistar-Ablösung

### Ergebnis

MediPrax ist zertifiziert, migriert und im Produktivbetrieb.

## 6.6 Zeitvergleich: Mit vs. Ohne Claude Code

| Phase | Ohne KI (klassisches Team) | Mit Claude Code (1 Entwickler) | Hauptunterschied |
|-------|--------------------------|-------------------------------|-----------------|
| Phase 1: Grundlagen | 3–4 Monate (3–4 Entwickler) | 2–3 Monate | Boilerplate und CRUD schnell generiert |
| Phase 2: TI | 6–8 Monate | 5–6 Monate | Zeitersparnis begrenzt: Konnektor-Tests manuell |
| Phase 3: Abrechnung | 4–5 Monate | 3–4 Monate | EBM-Regelwerk und Tests schnell implementiert |
| Phase 4: Zertifizierung | 6–8 Monate | 5–6 Monate | Kaum Zeitersparnis: regulatorische Prozesse bestimmen |
| **Gesamt** | **25–33 Monate** | **20–26 Monate** | **Ca. 20–25% Zeitersparnis** |

> **Kosteneffekt:** Der größte Vorteil von Claude Code liegt nicht in der reinen Zeitersparnis, sondern in der Reduzierung der Personalkosten. Ein Entwickler mit Claude Code ersetzt ein Team von 3–4 Personen. Die Gesamtentwicklungskosten können dadurch um 50–70% gesenkt werden.
