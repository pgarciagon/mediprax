# MediPrax

**Das moderne Praxisverwaltungssystem** — KI-gestützte Entwicklung mit Claude Code.

MediPrax ist ein modulares Praxisverwaltungssystem (PVS), das als Ersatz für CGM Medistar entwickelt wird. Es wird KI-gestützt mit Claude Code gebaut und richtet sich zunächst an die Pilotpraxis Neuropsychiatricum Bremen (Gemeinschaftspraxis für Psychiatrie und Neurologie).

## Status

### v1.0.0 — MVP Complete (Released 2026-04-08)

| Phase | Meilensteine | Status |
|-------|-------------|--------|
| **Phase 1: MVP** | M1–M16 (Setup, Patienten, Termine, Doku, Arztbrief, Dashboard, Auth, RBAC, Audit, Abrechnung, Suche, Tests, Dark Mode, Medikationsplan, TI-Simulation, FHIR) | ✅ Released |
| **Phase 2: Psychiatrie & Neurologie** | M17–M35 (AMDP, Testung, PTV/Therapie, Neuro-Befund, Labor, Billing+, Formulare, GDT, Video, DMP, BtM) | In Planung |

See [CHANGELOG.md](CHANGELOG.md) for release details.
See [IMPLEMENTATION_PLAN_PSYCHIATRY_NEUROLOGY.md](IMPLEMENTATION_PLAN_PSYCHIATRY_NEUROLOGY.md) for the Phase 2 roadmap.

## Dokumentation

| Dokument | Beschreibung |
|----------|-------------|
| [Geschäftsplan](docs/01-geschaeftsplan.md) | Geschäftsmodell, Unternehmensstruktur, Stufenplan |
| [Kostenanalyse](docs/02-kostenanalyse.md) | IST-Kosten CGM Medistar, Entwicklungskosten, Break-Even |
| [MVP-Spezifikation](docs/03-mvp-spezifikation.md) | Pflichtfunktionen, optionale Module, Priorisierung |
| [Regulatorischer Rahmen](docs/04-regulatorik.md) | KBV, gematik, KOB, ePA, TI, DSGVO |
| [Technische Architektur](docs/05-architektur.md) | Stack, Datenbankdesign, API, Deployment |
| [Entwicklungsplan](docs/06-entwicklungsplan.md) | Phasen, Zeitplan, Meilensteine |
| [Risikoanalyse](docs/07-risikoanalyse.md) | Risiken, Bewertung, Maßnahmen |

## Tech Stack

| Schicht | Technologie |
|---------|------------|
| Backend + Frontend | C# / .NET 10 mit Blazor Server |
| Datenbank | PostgreSQL 16+ |
| API | ASP.NET Core Web API (REST) |
| FHIR | Firely SDK (.NET) |
| TI | Konnektor SOAP/REST API |
| PDF-Erzeugung | QuestPDF |
| Tests | xUnit + Playwright |
| CI/CD | GitHub Actions |

## Pilotpraxis

**Neuropsychiatricum Bremen**
Gemeinschaftspraxis für Psychiatrie und Neurologie
Ärztehaus am Rotes Kreuz Krankenhaus, Osterstraße 1a, 28199 Bremen

## Lizenz

Proprietär — Alle Rechte vorbehalten.
