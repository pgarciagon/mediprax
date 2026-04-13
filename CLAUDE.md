# MediPrax — Context for Claude Code

## Project Description

**MediPrax** is a German medical practice management system (PVS — Praxisverwaltungssystem), developed as a replacement for CGM Medistar for the pilot practice **Neuropsychiatricum Bremen** (Gemeinschaftspraxis für Psychiatrie und Neurologie, Osterstraße 1a, 28199 Bremen).

- Developed by a single developer with Claude Code
- Proprietary, all rights reserved
- Applicable regulations: KBV, gematik, DSGVO, §630f BGB, §203 StGB

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend + Frontend | C# / .NET 10 + Blazor Server (InteractiveServer render mode) |
| Database | PostgreSQL 16+ via EF Core 10 + Npgsql |
| PDF | QuestPDF |
| FHIR | Firely SDK (.NET) |
| Tests | xUnit + Testcontainers.PostgreSQL + Playwright |
| CI/CD | GitHub Actions → GHCR |
| Deploy | Docker + docker-compose |
| Auth | Cookie-based, BCrypt, 12h session, rate limiting |

---

## Architecture (Layered DDD)

```
MediPrax.Core             → Entities, Enums, Repository Interfaces, ValueObjects
MediPrax.Application      → Services, DTOs, Service Interfaces, Catalogs (ICD-10, GOP, AMDP…)
MediPrax.Infrastructure   → EF Core DbContext, Configurations, Migrations, TI implementations
MediPrax.Reporting        → PDF Generators (QuestPDF): Arztbrief, Formulare, PtvForms
MediPrax.Server           → Blazor Host: Components/Pages, Program.cs, wwwroot
MediPrax.UI               → Shared UI component library
tests/MediPrax.UnitTests
tests/MediPrax.IntegrationTests
```

### Established Patterns

- Entities inherit `BaseEntity` (Id, CreatedAt, UpdatedAt, IsDeleted, DeletedAt) — global soft delete
- Services: inject `DbContext`, return DTOs, async with `CancellationToken`
- DTOs: `XxxDto`, `CreateXxxDto`, `UpdateXxxDto`, `XxxListItemDto`
- Blazor pages: `@rendermode InteractiveServer`, `@attribute [Authorize(Policy = "...")]`, loading/error pattern
- **Code in English, UI and medical domain in German**
- Audit trail on all significant operations (EF Core Interceptor)
- Each milestone generates its own EF Core migration

### RBAC Roles

`Admin` | `Arzt` | `MFA` (Medizinische Fachangestellte) | `Empfang`

---

## Current Status

**v1.0.0 MVP — Released 2026-04-08**

---

## Milestones

### Phase 1: MVP (M1–M16) — COMPLETED ✅

| # | Name |
|---|------|
| M1 | Projekt-Setup: Repo, CI/CD, Clean Architecture, DB Schema |
| M2 | Patientenverwaltung (CRUD, Suche, GKV/PKV) |
| M3 | Terminplanung (Wochenansicht, Wartezimmer) |
| M4 | Klinische Dokumentation (Encounters, ICD-10-GM, Vorlagen) |
| M5 | Arztbrief-Generierung (QuestPDF, PDF-Download) |
| M6 | Dashboard (KPIs, Termine, Aktivität) |
| M7 | Authentifizierung (Login, BCrypt, Seitenschutz) |
| M8 | Benutzerverwaltung & RBAC |
| M9 | Audit-Logging & DSGVO-Compliance (EF Interceptor, Soft Delete) |
| M10 | Abrechnung EBM/GOÄ, GOP-Katalog, KVDT-Export |
| M11 | Globale Suche, Berichte, Praxisstatistiken |
| M12 | Tests, Docker-Deployment, Produktionshärtung |
| M13 | UI/UX: Dark Mode, Responsive, Toast, Tastaturkürzel |
| M14 | Klinische Erweiterungen: Medikamentenplan, AU, Rezept, Überweisungen, Wiedervorlagen |
| M15 | Infrastruktur & Stabilität: Backup, SignalR, Health Checks, Serilog, Rate Limiting |
| M16 | Phase 2 Vorbereitung: FHIR/Firely SDK, Konnektor-Client, Zertifizierungsdoku |

Extras implemented beyond plan: Full TI simulation (eGK, E-Rezept, KIM, ePA with mock services), GitHub Actions CI/CD, KVDT-Export, CSV-Import (migration from Medistar), 76 automated tests.

---

### Phase 2: Psychiatry & Neurology (M17–M42)

Full specification in `IMPLEMENTATION_PLAN_PSYCHIATRY_NEUROLOGY.md`.

| # | Name | Priority | Status |
|---|------|----------|--------|
| M17 | Structured Psychopathological Assessment (AMDP, ~130 symptoms, narrative generator) | P0 | ✅ |
| M18 | Psychometric Testing (PHQ-9, GAD-7, HAMD, PANSS, MMSE…) | P0 | ✅ |
| M19 | PTV Forms & Therapy Case Management (Therapiegenehmigung, contingents) | P0 | ✅ |
| M20 | Structured Neurological Examination | P0 | ✅ |
| M21 | Advanced Medication Management: interactions, depot injections, monitoring | P1 | ✅ |
| M22 | Laboratory Integration & Monitoring (LDT) | P1 | ✅ |
| M23 | Billing Enhancements: plausibility, GOP engine, GOÄ-Rechnung | P1 | ✅ |
| M24 | Missing KBV Forms: Muster 2, 7, 13, 14, 26 | P1 | ✅ |
| M25 | Appointment Enhancements: recurring, Warteliste, session tracking | P2 | ⚠️ partial |
| M26 | Disease-Specific Documentation: Epilepsy, MS, Parkinson, Headache | P2 | ✅ |
| M27 | Suicidality Assessment & Safety Planning | P2 | ✅ |
| M28 | GDT Device Interface (EEG, EMG, evoked potentials) | P2 | ⏳ |
| M29 | Videosprechstunde Integration | P3 | ⏳ |
| M30 | Text Modules & Documentation Automation (Textbausteine) | P3 | ⚠️ partial |
| M31 | DMP Depression | P3 | ✅ |
| M32 | Involuntary Commitment & Legal Documentation (PsychKG, BGB 1906) | P3 | ⏳ |
| M33 | Private Billing GOÄ: Rechnungen, Mahnwesen | P3 | ⚠️ partial |
| M34 | eAU Electronic Transmission | P3 | ⏳ |
| M35 | BtM Management & E-BtM-Rezept | P3 | ✅ |
| M36 | Automated Mahnwesen (3-stage payment reminders) | P3 | ⏳ |
| M37 | DATEV-Export Interface (for Steuerberater) | P3 | ⏳ |
| M38 | Umsatzstatistik / Revenue Analytics | P3 | ⏳ |
| M39 | Arztbrief Therapiebericht Vorlage | P2 | ⏳ |
| M40 | Sprechzeiten & Verfügbarkeit (doctor schedules and absences) | P1 | ✅ |
| M41 | Intelligente Terminvergabe (automatic slot suggestion) | P1 | ✅ |
| M42 | Automatische Diagnosevorschläge aus strukturierten Befunden | P1 | ✅ |
| M43 | Structured Encounter Documentation (ABDTP fields) | P1 | ✅ |
| M44 | Advanced Diagnosis Management (Dauerdiagnosen, metadata, inheritance) | P1 | ✅ |
| M45 | Encounter Types (Karteieintragstypen) | P2 | ⏳ |
| M46 | Action Chains (Aktionsketten / Behandlungskomplexe) | P1 | ✅ |
| M47 | Enhanced Textbausteine with Inline Expansion | P2 | ✅ |
| M48 | Medication Catalog & Prescription Search (BfArM/FHIR) | P1 | ✅ |

**Phase 2 Summary:** ~24/32 completed. Priority pending: **M28**, **M39**, **M45** (P2).

---

### Future Phases (external infrastructure required)

| Phase | Content | Status |
|-------|---------|--------|
| Phase 2 TI | Real Konnektor, ePA, E-Rezept, KIM, ECC-256 | Simulated — pending gematik access |
| Phase 3 Billing | Full EBM engine, validated KVDT, Hybrid-DRG | Partially in M10/M23 |
| Phase 4 Certification | KBV, gematik KOB, DSGVO audit, Medistar migration, Go-Live | Pending |

---

## Useful Commands

```bash
# Local development (uses local PostgreSQL process, NOT Docker)
dotnet run --project src/MediPrax.Server      # Server at http://localhost:5079

# Database
dotnet ef migrations add <Name> \
  --project src/MediPrax.Infrastructure \
  --startup-project src/MediPrax.Server       # New migration
dotnet ef database update \
  --project src/MediPrax.Infrastructure \
  --startup-project src/MediPrax.Server       # Apply migrations

# Tests
dotnet test                                   # All tests
dotnet test tests/MediPrax.UnitTests          # Unit tests only
dotnet test tests/MediPrax.IntegrationTests   # Integration only (uses Testcontainers)

# Build & Docker (INT/VAL only — NOT for local development)
dotnet build                                  # Build solution
docker compose up --build                     # Full build + deploy (INT/VAL servers only)
```

**Local development:** Uses local PostgreSQL process (not Docker). Connection: `Host=localhost;Port=5432;Database=mediprax;Username=mediprax;Password=mediprax_dev`
**Local UI testing:** Always use the external Chrome instance (Claude in Chrome MCP tools: `mcp__Claude_in_Chrome__*`) for local UI testing instead of the Claude Preview tool. Navigate to `http://localhost:5079`, log in with the demo credentials, and verify features interactively.
**Local app port:** `http://localhost:5079`
**INT/VAL:** Use Docker Compose on the Hetzner server. See Deployment Environments section.

---

## Deployment Environments

3-environment strategy for medical software. Full documentation in `docs/10-deployment.md`.

### Hetzner Cloud (46.225.170.6)

| Environment | URL | Directory | Docker project | DB | Branch |
|-------------|-----|-----------|----------------|-----|--------|
| **INT** | `http://46.225.170.6:8081` | `~/mediprax-int/` | `mediprax-int` | `mediprax_int` | `main` / feature |
| **VAL** | `http://46.225.170.6:8082` | `~/mediprax-val/` | `mediprax-val` | `mediprax_val` | release tags |

- SSH: `deployer@46.225.170.6` (`docker` group, no sudo)
- Demo login: `meier@neuropsych-bremen.de` / `mediprax2026`
- No real data, no TI

```bash
# Update INT
ssh 46.225.170.6 "cd ~/mediprax-int && git pull && docker compose -p mediprax-int up --build -d"

# Deploy release to VAL
ssh 46.225.170.6 "cd ~/mediprax-val && git fetch --tags && git checkout v1.1.0 && docker compose -p mediprax-val up --build -d"
```

### Production (future) — Local server at the practice

- **OS:** Windows Server 2022 with Docker Desktop
- **Konnektor TI** on the same LAN
- Real data, DSGVO, encrypted backups
- Same Docker configuration, no demo seed, explicit migrations

### Required Fixes for Docker Deploy

Applied on server (not in Git repo):
1. Dockerfile: `dotnet restore` points to server project (not `.slnx`)
2. Program.cs: removed `createScopeForStatusCodePages`, added `PendingModelChangesWarning` ignore
3. Program.cs: `EnsureCreated()` + seed in all environments

---

## Code Conventions

- **Code language:** ALL source code in English — classes, methods, variables, properties, comments, file names, log messages, technical documentation, commit messages, PR descriptions, CLAUDE.md, docs/
- **UI language:** ALL end-user visible content in German — labels, buttons, error/success messages, navigation, page titles, placeholders, tooltips, `userdocs/` content
- **Medical domain:** German domain terms are kept as proper nouns in code (`Encounter`, `Arztbrief`, `Sprechzeiten`, `Wiedervorlage`, `Termin`…) when there is no clear English equivalent or when it is a German regulatory concept (KBV, GOP, EBM, KVDT)
- New entities always in `MediPrax.Core/Entities/`, inheriting `BaseEntity`
- New services: interface in `MediPrax.Application/Interfaces/`, implementation in `MediPrax.Application/Services/`
- EF configuration in `MediPrax.Infrastructure/Persistence/Configurations/`
- DI registration in `src/MediPrax.Server/Program.cs`
- Each new milestone requires: entity + service + DTO + EF configuration + migration + Blazor page + tests
- **Mandatory seed data:** every new feature MUST include test/seed data in the EF Core migration. Migrate existing impacted data when necessary. Seeds use raw SQL in the migration `Up()` with fixed GUIDs for idempotency.
- **Mandatory UI access:** every new feature MUST be accessible from the UI through a clear workflow. This includes: links in the sidebar/navigation (`MainLayout.razor`), buttons on relevant pages, and navigable routes. A feature is NOT considered complete if the user cannot reach it from the interface.
- When completing a milestone, update the user documentation (`userdocs/`) describing the new functionality for the end user
- **Memory sync:** whenever CLAUDE.md is updated (new rules, milestone status changes, conventions, deployment info), the corresponding memory files in `.claude/projects/-Users-pgarcgo-code-mediprax/memory/` MUST also be updated to stay in sync. This includes `MEMORY.md` (index) and the relevant `feedback_*.md` files. Rules in CLAUDE.md and memory must never contradict each other.

---

## Mandatory Testing Rules

1. **Every new feature MUST include tests.** A feature is not considered complete without its corresponding unit and/or integration tests.
2. **Before committing, run `dotnet test` and verify all tests pass.** Do not commit code that breaks existing tests.
3. **Test types:**
   - **Unit tests** (`tests/MediPrax.UnitTests/`): for DTOs, catalogs, text generators, pure logic without DB
   - **Integration tests** (`tests/MediPrax.IntegrationTests/`): for services using DbContext, require PostgreSQL via Testcontainers
   - **E2E/UI tests** (`tests/MediPrax.E2ETests/`): for every new UI feature. Uses Playwright + xUnit. Each new page or significant UI component MUST have E2E tests covering: page loads, key user interactions, navigation, and form workflows. Follow the pattern in `E2ETestBase.cs` (fresh browser context per test, auto-login). E2E tests run locally only (not in CI).
4. **Naming convention:** `[TestedClass]Tests.cs` or `[Feature]E2ETests.cs`, methods: `[Method]_[Scenario]_[Result]` or descriptive in English
5. **When fixing a bug, add a test that reproduces the bug** before the fix to prevent regressions
6. **When completing a feature, update the user documentation** (`userdocs/`) describing the new functionality for the end user before committing
7. **UI workflow tested:** every feature must have a workflow described and manually verified before committing. The workflow includes: how to access the feature from the menu/navigation, what steps to follow, and what result to expect. This workflow must be documented in `userdocs/`.
8. **E2E tests required:** every new feature with a UI component MUST include Playwright E2E tests in `tests/MediPrax.E2ETests/`. A feature is NOT considered complete without E2E tests covering the main user workflows. **Automatic rule:** Whenever UI components are added or modified, E2E tests MUST be created/updated AND executed locally before considering the task complete. Do not wait for the user to ask — proactively create and run E2E tests for every UI change.
9. **Before committing, verify:** (a) the feature is accessible from the UI (sidebar links, buttons, routes), (b) `dotnet test` passes, (c) the workflow works end-to-end, (d) E2E tests exist for new UI features
10. **After pushing to main, verify CI/CD passes:**
   - Run `gh run list --limit 3` to check the latest GitHub Actions run
   - If CI fails, diagnose with `gh run view <ID> --log-failed`, fix the issue, commit, push, and re-check — iterate until green
   - Deploy INT is triggered automatically when CI passes on main
   - E2E tests (`MediPrax.E2ETests`) are excluded from CI (require browser + running server); they run locally only

```bash
dotnet test                                   # All tests (before commit)
dotnet test tests/MediPrax.UnitTests          # Unit tests only (fast)
dotnet test tests/MediPrax.IntegrationTests   # Integration only (requires Docker)
```

---

## Additional Documentation

| File | Content |
|------|---------|
| `CHANGELOG.md` | Release notes v1.0.0 |
| `docs/06-entwicklungsplan.md` | Phase and milestone plan (general index M1–M42) |
| `docs/milestones/` | Detailed specifications per milestone (M17–M42) |
| `docs/05-architektur.md` | Technical architecture decisions |
| `docs/04-regulatorik.md` | Regulatory framework KBV, gematik, DSGVO |
| `docs/10-deployment.md` | Environment strategy, Hetzner, local production |
| `userdocs/` | End-user guide in German |
