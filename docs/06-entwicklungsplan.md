# 6. Entwicklungsplan

> **Hinweis:** Zeitschaetzungen beruecksichtigen die erhoehte Produktivitaet durch Claude Code. Die Gesamtdauer wird jedoch durch regulatorische Prozesse (Zertifizierung) bestimmt, nicht durch die Codeentwicklung.

---

## Inhaltsverzeichnis

- [1. Phasenuebersicht](#1-phasenuebersicht)
- [2. Meilenstein-Gesamtstatus](#2-meilenstein-gesamtstatus)
- [3. Phase 1: MVP (M1-M16) -- ABGESCHLOSSEN](#3-phase-1-mvp-m1-m16----abgeschlossen)
- [4. Phase 2: Psychiatrie & Neurologie (M17-M42)](#4-phase-2-psychiatrie--neurologie-m17-m42)
- [5. Phase 3: TI-Integration](#5-phase-3-ti-integration-konnektor-epa-e-rezept-kim)
- [6. Phase 4: Abrechnung](#6-phase-4-abrechnung-ebm-motor-kvdt-validiert)
- [7. Phase 5: Zertifizierung & Go-Live](#7-phase-5-zertifizierung--go-live)
- [8. Zeitvergleich: Mit vs. Ohne Claude Code](#8-zeitvergleich-mit-vs-ohne-claude-code)

---

## 1. Phasenuebersicht

| Phase | Zeitraum | Schwerpunkt | Status |
|-------|----------|-------------|--------|
| Phase 1: Grundlagen + MVP | Monate 1-6 | Basisinfrastruktur und Kernmodule (M1-M16) | ABGESCHLOSSEN |
| Phase 2: Psychiatrie & Neurologie | Monate 7-14 | Fachspezifische Dokumentation, klinische Workflows (M17-M42) | IN ARBEIT |
| Phase 3: TI-Integration | Monate 15-20 | Telematikinfrastruktur-Anbindung (Konnektor, ePA, E-Rezept, KIM) | AUSSTEHEND |
| Phase 4: Abrechnung | Monate 21-24 | EBM/GOAe-Motor, KVDT-Validierung | AUSSTEHEND |
| Phase 5: Zertifizierung + Go-Live | Monate 25-32 | KBV, gematik, Migration, Produktivbetrieb | AUSSTEHEND |

---

## 2. Meilenstein-Gesamtstatus

| # | Name | Phase | Prioritaet | Status |
|---|------|-------|------------|--------|
| M1 | Projekt-Setup: Repo, CI/CD, Clean Architecture, DB Schema | Phase 1 | -- | ABGESCHLOSSEN |
| M2 | Patientenverwaltung (CRUD, Suche, GKV/PKV) | Phase 1 | -- | ABGESCHLOSSEN |
| M3 | Terminplanung (Wochenansicht, Wartezimmer) | Phase 1 | -- | ABGESCHLOSSEN |
| M4 | Klinische Dokumentation (Encounters, ICD-10-GM, Vorlagen) | Phase 1 | -- | ABGESCHLOSSEN |
| M5 | Arztbrief-Generierung (QuestPDF, PDF-Download) | Phase 1 | -- | ABGESCHLOSSEN |
| M6 | Dashboard (KPIs, Termine, Aktivitaet) | Phase 1 | -- | ABGESCHLOSSEN |
| M7 | Authentifizierung (Login, BCrypt, Seitenschutz) | Phase 1 | -- | ABGESCHLOSSEN |
| M8 | Benutzerverwaltung & RBAC | Phase 1 | -- | ABGESCHLOSSEN |
| M9 | Audit-Logging & DSGVO-Compliance (EF Interceptor, Soft Delete) | Phase 1 | -- | ABGESCHLOSSEN |
| M10 | Abrechnung EBM/GOAe, GOP-Katalog, KVDT-Export | Phase 1 | -- | ABGESCHLOSSEN |
| M11 | Globale Suche, Berichte, Praxisstatistiken | Phase 1 | -- | ABGESCHLOSSEN |
| M12 | Tests, Docker-Deployment, Produktionshaertung | Phase 1 | -- | ABGESCHLOSSEN |
| M13 | UI/UX: Dark Mode, Responsive, Toast, Tastaturkuerzel | Phase 1 | -- | ABGESCHLOSSEN |
| M14 | Klinische Erweiterungen: Medikamentenplan, AU, Rezept, Ueberweisungen, Wiedervorlagen | Phase 1 | -- | ABGESCHLOSSEN |
| M15 | Infrastruktur & Stabilitaet: Backup, SignalR, Health Checks, Serilog, Rate Limiting | Phase 1 | -- | ABGESCHLOSSEN |
| M16 | Phase 2 Vorbereitung: FHIR/Firely SDK, Konnektor-Client, Zertifizierungsdoku | Phase 1 | -- | ABGESCHLOSSEN |
| M17 | Structured Psychopathological Assessment (AMDP, ~130 Symptome, Narrativ-Generator) | Phase 2 | P0 | ABGESCHLOSSEN |
| M18 | Psychometric Testing (PHQ-9, GAD-7, HAMD, PANSS, MMSE...) | Phase 2 | P0 | ABGESCHLOSSEN |
| M19 | PTV Forms & Therapy Case Management (Therapiegenehmigung, Kontingente) | Phase 2 | P0 | ABGESCHLOSSEN |
| M20 | Structured Neurological Examination | Phase 2 | P0 | ABGESCHLOSSEN |
| M21 | Advanced Medication Management: Interaktionen, Depots, Monitoring | Phase 2 | P1 | ABGESCHLOSSEN |
| M22 | Laboratory Integration & Monitoring (LDT) | Phase 2 | P1 | ABGESCHLOSSEN |
| M23 | Billing Enhancements: Plausibilitaet, GOP-Engine, GOAe-Rechnung | Phase 2 | P1 | AUSSTEHEND |
| M24 | Missing KBV Forms: Muster 2, 7, 13, 14, 26 | Phase 2 | P1 | AUSSTEHEND |
| M25 | Appointment Enhancements: Recurrentes, Warteliste, Session Tracking | Phase 2 | P2 | TEILWEISE |
| M26 | Disease-Specific Documentation: Epilepsie, MS, Parkinson, Cephalgie | Phase 2 | P2 | ABGESCHLOSSEN |
| M27 | Suicidality Assessment & Safety Planning | Phase 2 | P2 | ABGESCHLOSSEN |
| M28 | GDT Device Interface (EEG, EMG, Evozierte Potenziale) | Phase 2 | P2 | AUSSTEHEND |
| M29 | Videosprechstunde Integration | Phase 2 | P3 | AUSSTEHEND |
| M30 | Text Modules & Documentation Automation (Textbausteine) | Phase 2 | P3 | TEILWEISE |
| M31 | DMP Depression | Phase 2 | P3 | ABGESCHLOSSEN |
| M32 | Involuntary Commitment & Legal Documentation (PsychKG, BGB 1906) | Phase 2 | P3 | AUSSTEHEND |
| M33 | Private Billing GOAe: Rechnungen, Mahnwesen | Phase 2 | P3 | TEILWEISE |
| M34 | eAU Electronic Transmission | Phase 2 | P3 | AUSSTEHEND |
| M35 | BtM Management & E-BtM-Rezept | Phase 2 | P3 | ABGESCHLOSSEN |
| M36 | Automated Mahnwesen (Zahlungserinnerungen in 3 Stufen) | Phase 2 | P3 | AUSSTEHEND |
| M37 | DATEV-Export Interface (fuer Steuerberater) | Phase 2 | P3 | AUSSTEHEND |
| M38 | Umsatzstatistik / Revenue Analytics | Phase 2 | P3 | AUSSTEHEND |
| M39 | Arztbrief Therapiebericht Vorlage | Phase 2 | P2 | AUSSTEHEND |
| M40 | Sprechzeiten & Verfuegbarkeit (Aerzte-Zeitplaene und Abwesenheiten) | Phase 2 | P1 | ABGESCHLOSSEN |
| M41 | Intelligente Terminvergabe (automatische Slot-Vorschlaege) | Phase 2 | P1 | ABGESCHLOSSEN |
| M42 | Automatische Diagnosevorschlaege aus strukturierten Befunden | Phase 2 | P1 | AUSSTEHEND |
| M43 | Structured Encounter Documentation (ABDTP-Felder) | Phase 2 | P1 | AUSSTEHEND |
| M44 | Advanced Diagnosis Management (Dauerdiagnosen, Metadaten, Vererbung) | Phase 2 | P1 | AUSSTEHEND |
| M45 | Encounter Types (Karteieintragstypen) | Phase 2 | P2 | AUSSTEHEND |
| M46 | Action Chains (Aktionsketten / Behandlungskomplexe) | Phase 2 | P1 | AUSSTEHEND |
| M47 | Enhanced Textbausteine with Inline Expansion | Phase 2 | P2 | AUSSTEHEND |
| M48 | Medication Catalog & Prescription Search (BfArM/FHIR) | Phase 2 | P1 | AUSSTEHEND |

---

## 3. Phase 1: MVP (M1-M16) -- ABGESCHLOSSEN

**Ziel:** Funktionsfaehige Basis fuer den Praxisalltag

**Parallele Aktionen:** gematik-Registrierung starten, KBV kontaktieren, Referenzumgebung beantragen.

**Claude Code Schwerpunkt:** Projekt-Scaffolding, Datenbankschema, CRUD-Module, UI-Grundgeruest.

### M1: Projekt-Setup

Repository, CI/CD, Clean Architecture, Datenbankschema (EF Core + PostgreSQL). -- ABGESCHLOSSEN

### M2: Patientenverwaltung

CRUD, Suche, Versicherungsdaten, GKV/PKV. -- ABGESCHLOSSEN

### M3: Terminplanung

Wochenansicht, Terminformular, Wartezimmer. -- ABGESCHLOSSEN

### M4: Klinische Dokumentation

Encounters, ICD-10-GM-Suche, Neuro/Psych-Vorlagen. -- ABGESCHLOSSEN

### M5: Arztbrief-Generierung

QuestPDF, Vorlagen, PDF-Download. -- ABGESCHLOSSEN

### M6: Dashboard

KPIs, heutige Termine, letzte Aktivitaet, Schnellzugriffe. -- ABGESCHLOSSEN

### M7: Authentifizierung

Cookie-Login, BCrypt, Seitenprotection, Abmelden. -- ABGESCHLOSSEN

### M8: Benutzerverwaltung und RBAC

Verwaltung von Benutzerkonten und Durchsetzung rollenbasierter Zugriffsrechte.

- **User CRUD** -- `IUserService` erweitern: GetAll, Create, Update, Deactivate, ResetPassword
- **Admin-Seiten** -- `/verwaltung/benutzer` (Liste, Formular)
- **RBAC durchsetzen** -- Arzt (voller klinischer Zugriff), MFA (Patienten/Termine), Empfang (Check-in), Admin (Benutzerverwaltung)
- **BaseEntity erweitern** -- `CreatedById`, `UpdatedById` fuer Audit-Trail
- **Passwort aendern** -- `/konto/passwort` fuer eingeloggte Benutzer
- **Sidebar** -- Verwaltungsbereich nur fuer Admin sichtbar

-- ABGESCHLOSSEN

### M9: Audit-Logging und Datenschutz-Compliance

Pflicht fuer medizinische Software gemaess DSGVO, Paragraph 203 StGB, KBV-Richtlinien.

- **AuditLog-Entity** -- Timestamp, User, Action, EntityType, EntityId, OldValues/NewValues (JSON)
- **EF Core Interceptor** -- automatische Protokollierung aller Create/Update/Delete-Operationen
- **Login/Logout-Audit** -- inkl. IP-Adresse und Erfolg/Misserfolg
- **Patientenakten-Zugriff loggen** -- jeder Zugriff auf Patientendaten nachvollziehbar (DSGVO)
- **Soft Delete** -- `IsDeleted` + `DeletedAt` auf BaseEntity, Global Query Filter (Aufbewahrungspflicht 10 Jahre, Paragraph 630f BGB)
- **Audit-Log Viewer** -- `/verwaltung/audit-log` (nur Admin, Filterung, Paginierung)

-- ABGESCHLOSSEN

### M10: Abrechnung EBM/GOAe

Die `BillingItem`-Entity existiert bereits. Geschaeftslogik und UI aufbauen.

- **GOP-Katalog** -- EBM Kap. 16/21 (Neurologie/Psychiatrie) + GOAe-Ziffern als Stammdaten
- **IBillingService** -- Ziffern erfassen, Quartalsuebersicht, Plausibilitaetspruefung
- **Encounter-Formular erweitern** -- Abrechnung-Fieldset mit GOP-Suche
- **Abrechnungsuebersicht** -- `/abrechnung` mit Quartalsfilter, Arztfilter, Exportstatus
- **GOAe-Rechnung PDF** -- QuestPDF-basierte Privatrechnung

-- ABGESCHLOSSEN

### M11: Globale Suche, Berichte, Praxisstatistiken

- **Globale Suche** -- CommandPalette-Komponente mit Patientensuche, Terminsuche, Seitennavigation
- **Berichte** -- Monats-/Quartals-/Jahresberichte fuer Patienten, Termine, Abrechnung
- **Statistiken** -- Praxisstatistiken mit Charts und Trends

-- ABGESCHLOSSEN

### M12: Tests, Docker-Deployment, Produktionshaertung

- **Unit Tests** -- Alle Services mit xUnit + Testcontainers.PostgreSQL
- **Integration Tests** -- API-Endpunkte und Service-Interaktionen
- **Docker** -- Multi-Stage Build, docker-compose, Health Checks
- **CI/CD** -- GitHub Actions: Build, Test, Docker Push zu GHCR
- **Haertung** -- HTTPS, Security Headers, Rate Limiting

-- ABGESCHLOSSEN

### M13: UI/UX Verbesserungen

- **Dark Mode** -- CSS-Variablen, Toggle-Button, Persistenz
- **Responsive** -- Mobile-Optimierung, Sidebar-Collapse
- **Toast-Notifications** -- Erfolgs-/Fehler-/Warnmeldungen
- **Tastaturkuerzel** -- Navigation, Formularaktionen

-- ABGESCHLOSSEN

### M14: Klinische Erweiterungen

- **Medikamentenplan** -- Medikation erfassen, Plan generieren
- **AU-Bescheinigung** -- Arbeitsunfaehigkeitsbescheinigung (Muster 1)
- **Rezept** -- Rezeptformular (Muster 16)
- **Ueberweisungen** -- Ueberweisungsformular (Muster 6)
- **Wiedervorlagen** -- Follow-up-System mit Erinnerungen

-- ABGESCHLOSSEN

### M15: Infrastruktur und Stabilitaet

- **Backup** -- Automatische PostgreSQL-Backups
- **SignalR** -- Echtzeit-Benachrichtigungen
- **Health Checks** -- System-Status-Endpunkte
- **Serilog** -- Strukturiertes Logging
- **Rate Limiting** -- Schutz gegen Missbrauch

-- ABGESCHLOSSEN

### M16: Phase 2 Vorbereitung

- **FHIR/Firely SDK** -- Grundlagen fuer TI-Integration
- **Konnektor-Client** -- Mock-Client fuer TI-Simulation
- **Zertifizierungsdokumentation** -- Vorbereitung der formalen Unterlagen

-- ABGESCHLOSSEN

### Abhaengigkeiten Phase 1

```
M1-M7: Grundmodule (parallel)
 -> M8: Benutzerverwaltung & RBAC
     -> M9: Audit-Logging & Datenschutz (benoetigt Benutzer-Tracking)
          -> M10: Abrechnung EBM/GOAe (benoetigt Rollen + Audit)
               -> M11: Suche, Berichte, Statistiken (benoetigt Abrechnungsdaten)
                    -> M12: Tests, Docker, Produktion (benoetigt alle vorherigen)

M13: UI/UX (unabhaengig)
M14: Klinische Erweiterungen (unabhaengig, parallel zu M13)
M15: Infrastruktur (nach M13/M14, nutzt deren Ergebnisse)
M16: Phase 2 Vorbereitung (nach M15, vor TI-Phase)
```

### Ergebnis Phase 1

Produktionsreifes System mit Patientenverwaltung, Terminplanung, klinischer Dokumentation, Arztbriefen, Abrechnung, Audit-Trail und Docker-Deployment. Bereit fuer Parallelbetrieb.

### Zusaetzlich umgesetzt (ueber Plan hinaus)

- TI-Integration Simulation (eGK, E-Rezept, KIM, ePA mit Mock-Services)
- CI/CD Pipeline (GitHub Actions: Build, Test, Docker Push zu GHCR)
- KVDT-Export (EBM-Positionen fuer KVHB)
- CSV-Datenimport (Patientenmigration aus Medistar)
- 76+ Tests (Unit + Integration)

---

## 4. Phase 2: Psychiatrie & Neurologie (M17-M42)

> Comprehensive plan to bring MediPrax to competitive parity with leading German PVS
> (ELEFANT, psyprax, EPIKUR, SMARTY, tomedo) for psychiatric and neurological practices.

### 4.0 Uebersicht & Priorisierung

#### Priority Matrix

| Prioritaet | Meilensteine | Begruendung |
|------------|-------------|-------------|
| **P0 -- Must Have** | 17, 18, 19, 20 | Core psychiatric/neurological documentation. Cannot compete without these. |
| **P1 -- High** | 21, 22, 23, 24, 40, 41, 42, 43, 44, 46, 48 | Patient safety, revenue protection, core consultation workflow, diagnosis management, automation, medication catalog. |
| **P2 -- Medium** | 25, 26, 27, 28, 39, 45, 47 | Major workflow improvements, specialty documentation, encounter types, Textbausteine. |
| **P3 -- Nice to Have** | 29, 30, 31, 32, 33, 34, 35, 36, 37, 38 | Competitive differentiators, niche requirements. |

### 4.1 Architekturprinzipien

All new features follow the established MediPrax patterns:

- **Layers:** Core (Entity) -> Application (Service + DTOs + Interface) -> Infrastructure (EF Config) -> Server (Blazor Page + DI Registration)
- **Entities:** Inherit `BaseEntity`, soft-delete via global query filters
- **Services:** Inject `DbContext`, use `DbSet<T>` properties, return DTOs, async with `CancellationToken`
- **DTOs:** `XxxDto`, `CreateXxxDto`, `UpdateXxxDto`, `XxxListItemDto`
- **Pages:** `@rendermode InteractiveServer`, `@attribute [Authorize(Policy = "...")]`, loading/error pattern
- **German UI, English code**
- **Audit trail** on all significant operations
- Each milestone generates its own EF Core migration

### 4.2 Detaillierte Meilenstein-Spezifikationen

Die detaillierten Implementierungsspezifikationen fuer jeden Meilenstein befinden sich in separaten Dateien:

#### P0 -- Must Have (ABGESCHLOSSEN)

| # | Meilenstein | Spezifikation |
|---|------------|---------------|
| M17 | Structured Psychopathological Assessment (AMDP) | [M17-amdp.md](milestones/M17-amdp.md) |
| M18 | Psychometric Testing & Assessment Tools | [M18-psychometric-testing.md](milestones/M18-psychometric-testing.md) |
| M19 | PTV Forms & Therapy Approval Management | [M19-ptv-therapy.md](milestones/M19-ptv-therapy.md) |
| M20 | Structured Neurological Examination | [M20-neurological-exam.md](milestones/M20-neurological-exam.md) |

#### P1 -- High

| # | Meilenstein | Status | Spezifikation |
|---|------------|--------|---------------|
| M21 | Advanced Medication Management | ABGESCHLOSSEN | [M21-medication-management.md](milestones/M21-medication-management.md) |
| M22 | Laboratory Integration & Monitoring | ABGESCHLOSSEN | [M22-lab-integration.md](milestones/M22-lab-integration.md) |
| M23 | Billing Enhancements | AUSSTEHEND | [M23-billing-enhancements.md](milestones/M23-billing-enhancements.md) |
| M24 | Missing KBV Forms | AUSSTEHEND | [M24-kbv-forms.md](milestones/M24-kbv-forms.md) |
| M40 | Sprechzeiten & Verfuegbarkeit | ABGESCHLOSSEN | [M40-sprechzeiten.md](milestones/M40-sprechzeiten.md) |
| M41 | Intelligente Terminvergabe | ABGESCHLOSSEN | [M41-terminvergabe.md](milestones/M41-terminvergabe.md) |
| M42 | Automatische Diagnosevorschlaege | AUSSTEHEND | [M42-diagnosevorschlaege.md](milestones/M42-diagnosevorschlaege.md) |
| M43 | Structured Encounter Documentation (ABDTP) | AUSSTEHEND | [M43-structured-encounter.md](milestones/M43-structured-encounter.md) |
| M44 | Advanced Diagnosis Management | AUSSTEHEND | [M44-diagnosis-management.md](milestones/M44-diagnosis-management.md) |
| M46 | Action Chains (Aktionsketten) | AUSSTEHEND | [M46-action-chains.md](milestones/M46-action-chains.md) |
| M48 | Medication Catalog & Prescription Search | AUSSTEHEND | [M48-medication-catalog.md](milestones/M48-medication-catalog.md) |

#### P2 -- Medium

| # | Meilenstein | Status | Spezifikation |
|---|------------|--------|---------------|
| M25 | Appointment Enhancements | TEILWEISE | [M25-appointment-enhancements.md](milestones/M25-appointment-enhancements.md) |
| M26 | Disease-Specific Documentation | ABGESCHLOSSEN | [M26-disease-documentation.md](milestones/M26-disease-documentation.md) |
| M27 | Suicidality Assessment & Safety Planning | ABGESCHLOSSEN | [M27-suicidality-assessment.md](milestones/M27-suicidality-assessment.md) |
| M28 | GDT Device Interface | AUSSTEHEND | [M28-gdt-device-interface.md](milestones/M28-gdt-device-interface.md) |
| M39 | Arztbrief Therapiebericht Vorlage | AUSSTEHEND | [M39-therapiebericht.md](milestones/M39-therapiebericht.md) |
| M45 | Encounter Types (Karteieintragstypen) | AUSSTEHEND | [M45-encounter-types.md](milestones/M45-encounter-types.md) |
| M47 | Enhanced Textbausteine with Inline Expansion | AUSSTEHEND | [M47-textbausteine-enhancement.md](milestones/M47-textbausteine-enhancement.md) |

#### P3 -- Nice to Have

| # | Meilenstein | Status | Spezifikation |
|---|------------|--------|---------------|
| M29 | Videosprechstunde Integration | AUSSTEHEND | [M29-videosprechstunde.md](milestones/M29-videosprechstunde.md) |
| M30 | Text Modules & Documentation Automation | TEILWEISE | [M30-text-modules.md](milestones/M30-text-modules.md) |
| M31 | DMP Depression | ABGESCHLOSSEN | [M31-dmp-depression.md](milestones/M31-dmp-depression.md) |
| M32 | Involuntary Commitment & Legal Documentation | AUSSTEHEND | [M32-involuntary-commitment.md](milestones/M32-involuntary-commitment.md) |
| M33 | Private Billing GOAe | TEILWEISE | [M33-private-billing.md](milestones/M33-private-billing.md) |
| M34 | eAU Electronic Transmission | AUSSTEHEND | [M34-eau-transmission.md](milestones/M34-eau-transmission.md) |
| M35 | BtM Management & E-BtM-Rezept | ABGESCHLOSSEN | [M35-btm-management.md](milestones/M35-btm-management.md) |
| M36 | Automated Mahnwesen | AUSSTEHEND | [M36-automated-mahnwesen.md](milestones/M36-automated-mahnwesen.md) |
| M37 | DATEV-Export Interface | AUSSTEHEND | [M37-datev-export.md](milestones/M37-datev-export.md) |
| M38 | Umsatzstatistik / Revenue Analytics | AUSSTEHEND | [M38-revenue-analytics.md](milestones/M38-revenue-analytics.md) |

### 4.3 Cross-Cutting Concerns & Abhaengigkeiten

Uebergreifende Aspekte (Navigation, Migrationen, Test-Anforderungen, Arztbrief-Integration) sowie der Dependency Graph und die empfohlene Implementierungsreihenfolge: [_cross-cutting.md](milestones/_cross-cutting.md)

---

## 5. Phase 3: TI-Integration (Konnektor, ePA, E-Rezept, KIM)

**Ziel:** Anbindung an die Telematikinfrastruktur

> **Hohe Komplexitaet:** Diese Phase erfordert Zugang zur gematik-Referenzumgebung. Die Registrierung als Primaersystem-Hersteller muss fruehzeitig erfolgen (idealerweise in Phase 1). Claude Code kann den Integrationscode schreiben, aber der physische Zugang zum Konnektor erfordert manuelle Arbeit.

### Meilensteine

1. Registrierung als Primaersystem-Hersteller bei gematik
2. Konnektor-Integration (VSDM: eGK-Einlesen)
3. ePA-Implementierung (Lesen/Schreiben, FHIR mit Firely SDK)
4. E-Rezept (elektronische Verordnung)
5. KIM (sichere Kommunikation zwischen Leistungserbringern)
6. ECC-256-Kryptographie (Pflicht ab 2026, nativ implementiert)

### Ergebnis

MediPrax ist mit der TI verbunden: eGK-Einlesen, ePA, E-Rezept und KIM funktionieren.

---

## 6. Phase 4: Abrechnung (EBM-Motor, KVDT validiert)

**Ziel:** Vollstaendige EBM- und GOAe-Abrechnung

**Claude Code Schwerpunkt:** EBM-Regelwerk-Engine, KVDT-Generator, GOAe-Berechnung.

### Meilensteine

1. EBM-Abrechnungsmotor mit Neuro-/Psych-spezifischen GOPs (Kap. 16/21)
2. GOAe-Abrechnung fuer Privatpatienten
3. KVDT-Dateigenerierung fuer Versand an KVHB
4. Validierung gegen Pruefpaket KVDT der KBV
5. Hybrid-DRG (falls zutreffend)
6. Abrechnungsstatistiken und Berichte

### Ergebnis

Vollstaendige Abrechnungsfaehigkeit fuer GKV (EBM) und PKV (GOAe).

---

## 7. Phase 5: Zertifizierung & Go-Live

**Ziel:** Alle Zertifizierungen erlangen + Produktivbetrieb starten

> **Regulatorischer Schwerpunkt:** In dieser Phase verlagert sich der Schwerpunkt von der Softwareentwicklung auf regulatorische Prozesse. Claude Code unterstuetzt bei der Vorbereitung (Test-Automatisierung, Dokumentation), aber die formalen Verfahren erfordern menschliches Engagement.

### Meilensteine

1. KBV-Zertifizierung (Pruefverfahren durchlaufen)
2. gematik KOB (Konformitaetsbewertung)
3. DSGVO-Audit
4. Datenmigration aus Medistar
5. Parallelbetrieb (beide Systeme gleichzeitig)
6. Schulung des Praxispersonals
7. Go-Live und Stabilisierung
8. Medistar-Abloesung

### Ergebnis

MediPrax ist zertifiziert, migriert und im Produktivbetrieb.

---

## 8. Zeitvergleich: Mit vs. Ohne Claude Code

| Phase | Ohne KI (klassisches Team) | Mit Claude Code (1 Entwickler) | Hauptunterschied |
|-------|--------------------------|-------------------------------|-----------------|
| Phase 1: Grundlagen | 3-4 Monate (3-4 Entwickler) | 2-3 Monate | Boilerplate und CRUD schnell generiert |
| Phase 2: Psychiatrie & Neurologie | 6-8 Monate | 3-4 Monate | Fachspezifische Module schnell generiert |
| Phase 3: TI | 6-8 Monate | 5-6 Monate | Zeitersparnis begrenzt: Konnektor-Tests manuell |
| Phase 4: Abrechnung | 4-5 Monate | 3-4 Monate | EBM-Regelwerk und Tests schnell implementiert |
| Phase 5: Zertifizierung | 6-8 Monate | 5-6 Monate | Kaum Zeitersparnis: regulatorische Prozesse bestimmen |
| **Gesamt** | **25-33 Monate** | **18-23 Monate** | **Ca. 25-30% Zeitersparnis** |

> **Kosteneffekt:** Der groesste Vorteil von Claude Code liegt nicht in der reinen Zeitersparnis, sondern in der Reduzierung der Personalkosten. Ein Entwickler mit Claude Code ersetzt ein Team von 3-4 Personen. Die Gesamtentwicklungskosten koennen dadurch um 50-70% gesenkt werden.
