# MediPrax — Contexto para Claude Code

## Descripción del proyecto

**MediPrax** es un sistema de gestión de consultas médicas alemanas (PVS — Praxisverwaltungssystem), desarrollado como reemplazo de CGM Medistar para la práctica piloto **Neuropsychiatricum Bremen** (Gemeinschaftspraxis für Psychiatrie und Neurologie, Osterstraße 1a, 28199 Bremen).

- Desarrollado por un solo desarrollador con Claude Code
- Propietario, todos los derechos reservados
- Regulación aplicable: KBV, gematik, DSGVO, §630f BGB, §203 StGB

---

## Stack técnico

| Capa | Tecnología |
|------|-----------|
| Backend + Frontend | C# / .NET 10 + Blazor Server (InteractiveServer render mode) |
| Base de datos | PostgreSQL 16+ via EF Core 10 + Npgsql |
| PDF | QuestPDF |
| FHIR | Firely SDK (.NET) |
| Tests | xUnit + Testcontainers.PostgreSQL + Playwright |
| CI/CD | GitHub Actions → GHCR |
| Deploy | Docker + docker-compose |
| Auth | Cookie-based, BCrypt, 12h session, rate limiting |

---

## Arquitectura (DDD en capas)

```
MediPrax.Core             → Entidades, Enums, Interfaces de repositorio, ValueObjects
MediPrax.Application      → Services, DTOs, Interfaces de servicio, Catalogs (ICD-10, GOP, AMDP…)
MediPrax.Infrastructure   → EF Core DbContext, Configuraciones, Migraciones, implementaciones TI
MediPrax.Reporting        → Generadores PDF (QuestPDF): Arztbrief, Formulare, PtvForms
MediPrax.Server           → Host Blazor: Components/Pages, Program.cs, wwwroot
MediPrax.UI               → Librería de componentes UI compartidos
tests/MediPrax.UnitTests
tests/MediPrax.IntegrationTests
```

### Patrones establecidos

- Entidades heredan `BaseEntity` (Id, CreatedAt, UpdatedAt, IsDeleted, DeletedAt) — soft delete global
- Services: inyectan `DbContext`, devuelven DTOs, async con `CancellationToken`
- DTOs: `XxxDto`, `CreateXxxDto`, `UpdateXxxDto`, `XxxListItemDto`
- Páginas Blazor: `@rendermode InteractiveServer`, `@attribute [Authorize(Policy = "...")]`, patrón loading/error
- **Código en inglés, UI y dominio médico en alemán**
- Audit trail en todas las operaciones significativas (EF Core Interceptor)
- Cada milestone genera su propia migración EF Core

### Roles RBAC

`Admin` | `Arzt` | `MFA` (Medizinische Fachangestellte) | `Empfang`

---

## Estado actual

**v1.0.0 MVP — Released 2026-04-08**

---

## Milestones

### Fase 1: MVP (M1–M16) — COMPLETADOS ✅

| # | Nombre |
|---|--------|
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

Extras implementados sobre el plan: TI-Simulation completa (eGK, E-Rezept, KIM, ePA con mock services), GitHub Actions CI/CD, KVDT-Export, CSV-Import (migración desde Medistar), 76 tests automatizados.

---

### Fase 2: Psiquiatría & Neurología (M17–M42)

Especificación completa en `IMPLEMENTATION_PLAN_PSYCHIATRY_NEUROLOGY.md`.

| # | Nombre | Prioridad | Estado |
|---|--------|-----------|--------|
| M17 | Structured Psychopathological Assessment (AMDP, ~130 síntomas, generador narrativo) | P0 | ✅ |
| M18 | Psychometric Testing (PHQ-9, GAD-7, HAMD, PANSS, MMSE…) | P0 | ✅ |
| M19 | PTV Forms & Therapy Case Management (Therapiegenehmigung, contingentes) | P0 | ✅ |
| M20 | Structured Neurological Examination | P0 | ✅ |
| M21 | Advanced Medication Management: interacciones, depósitos, monitoreo | P1 | ✅ |
| M22 | Laboratory Integration & Monitoring (LDT) | P1 | ✅ |
| M23 | Billing Enhancements: plausibilidad, GOP-Engine, GOÄ-Rechnung | P1 | ⏳ |
| M24 | Missing KBV Forms: Muster 2, 7, 13, 14, 26 | P1 | ⏳ |
| M25 | Appointment Enhancements: recurrentes, Warteliste, session tracking | P2 | ⚠️ parcial |
| M26 | Disease-Specific Documentation: Epilepsia, MS, Parkinson, Cefalea | P2 | ✅ |
| M27 | Suicidality Assessment & Safety Planning | P2 | ✅ |
| M28 | GDT Device Interface (EEG, EMG, potenciales evocados) | P2 | ⏳ |
| M29 | Videosprechstunde Integration | P3 | ⏳ |
| M30 | Text Modules & Documentation Automation (Textbausteine) | P3 | ⚠️ parcial |
| M31 | DMP Depression | P3 | ✅ |
| M32 | Involuntary Commitment & Legal Documentation (PsychKG, BGB 1906) | P3 | ⏳ |
| M33 | Private Billing GOÄ: Rechnungen, Mahnwesen | P3 | ⚠️ parcial |
| M34 | eAU Electronic Transmission | P3 | ⏳ |
| M35 | BtM Management & E-BtM-Rezept | P3 | ✅ |
| M36 | Automated Mahnwesen (recordatorios de pago en 3 etapas) | P3 | ⏳ |
| M37 | DATEV-Export Interface (para Steuerberater) | P3 | ⏳ |
| M38 | Umsatzstatistik / Revenue Analytics | P3 | ⏳ |
| M39 | Arztbrief Therapiebericht Vorlage | P2 | ⏳ |
| M40 | Sprechzeiten & Verfügbarkeit (horarios y ausencias de médicos) | P1 | ✅ |
| M41 | Intelligente Terminvergabe (sugerencia automática de slots) | P1 | ✅ |
| M42 | Automatische Diagnosevorschläge aus strukturierten Befunden | P1 | ⏳ |
| M43 | Structured Encounter Documentation (ABDTP-Felder) | P1 | ✅ |
| M44 | Advanced Diagnosis Management (Dauerdiagnosen, Metadaten, Vererbung) | P1 | ✅ |
| M45 | Encounter Types (Karteieintragstypen) | P2 | ⏳ |
| M46 | Action Chains (Aktionsketten / Behandlungskomplexe) | P1 | ⏳ |
| M47 | Enhanced Textbausteine with Inline Expansion | P2 | ⏳ |

**Resumen Fase 2:** ~18/31 completados. Pendientes prioritarios: **M23**, **M24**, **M42**, **M46** (P1); **M28**, **M39**, **M45**, **M47** (P2).

---

### Fases siguientes (infraestructura externa requerida)

| Fase | Contenido | Estado |
|------|-----------|--------|
| Fase 2 TI | Konnektor real, ePA, E-Rezept, KIM, ECC-256 | Simulado — pendiente acceso gematik |
| Fase 3 Abrechnung | EBM-Motor completo, KVDT validado, Hybrid-DRG | Parcialmente en M10/M23 |
| Fase 4 Zertifizierung | KBV, gematik KOB, DSGVO-Audit, migración Medistar, Go-Live | Pendiente |

---

## Comandos útiles

```bash
# Desarrollo local
docker compose up -d                          # Levanta PostgreSQL en puerto 5432
dotnet run --project src/MediPrax.Server      # Servidor en http://localhost:5116

# Base de datos
dotnet ef migrations add <Nombre> \
  --project src/MediPrax.Infrastructure \
  --startup-project src/MediPrax.Server       # Nueva migración
dotnet ef database update \
  --project src/MediPrax.Infrastructure \
  --startup-project src/MediPrax.Server       # Aplicar migraciones

# Tests
dotnet test                                   # Todos los tests
dotnet test tests/MediPrax.UnitTests          # Solo unitarios
dotnet test tests/MediPrax.IntegrationTests   # Solo integración (requiere Docker)

# Build & Docker
dotnet build                                  # Compilar solución
docker compose up --build                     # Build + deploy completo
```

**Conexión local:** `Host=localhost;Port=5432;Database=mediprax;Username=mediprax;Password=mediprax_dev`
**Puerto app:** `http://localhost:5116` (dev) / `http://localhost:8080` (Docker)

---

## Entornos de deployment

Estrategia de 3 entornos para software médico. Documentación completa en `docs/10-deployment.md`.

### Hetzner cloud (46.225.170.6)

| Entorno | URL | Directorio | Docker project | BD | Branch |
|---------|-----|-----------|----------------|-----|--------|
| **INT** | `http://46.225.170.6:8081` | `~/mediprax-int/` | `mediprax-int` | `mediprax_int` | `main` / feature |
| **VAL** | `http://46.225.170.6:8082` | `~/mediprax-val/` | `mediprax-val` | `mediprax_val` | tags release |

- SSH: `deployer@46.225.170.6` (grupo `docker`, sin sudo)
- Login demo: `meier@neuropsych-bremen.de` / `mediprax2026`
- Sin datos reales, sin TI

```bash
# Actualizar INT
ssh 46.225.170.6 "cd ~/mediprax-int && git pull && docker compose -p mediprax-int up --build -d"

# Desplegar release en VAL
ssh 46.225.170.6 "cd ~/mediprax-val && git fetch --tags && git checkout v1.1.0 && docker compose -p mediprax-val up --build -d"
```

### Producción (futuro) — Servidor local en consulta

- **OS:** Windows Server 2022 con Docker Desktop
- **Konnektor TI** en la misma LAN
- Datos reales, DSGVO, backups cifrados
- Misma configuración Docker, sin seed demo, migraciones explícitas

### Fixes necesarios para Docker deploy

Aplicados en servidor (no en repo Git):
1. Dockerfile: `dotnet restore` apunta al server project (no al `.slnx`)
2. Program.cs: eliminado `createScopeForStatusCodePages`, añadido `PendingModelChangesWarning` ignore
3. Program.cs: `EnsureCreated()` + seed en todos los entornos

---

## Convenciones de código

- **Idioma código:** TODO el código fuente en inglés — clases, métodos, variables, propiedades, comentarios, nombres de archivos, mensajes de log, documentación técnica, commit messages, PR descriptions, CLAUDE.md, docs/
- **Idioma UI:** TODO lo visible para el usuario final en alemán — labels, botones, mensajes de error/éxito, navegación, títulos de página, placeholders, tooltips, contenido de `userdocs/`
- **Dominio médico:** términos alemanes del dominio se mantienen como nombres propios en el código (`Encounter`, `Arztbrief`, `Sprechzeiten`, `Wiedervorlage`, `Termin`…) cuando no hay equivalente inglés claro o cuando es un concepto regulatorio alemán (KBV, GOP, EBM, KVDT)
- Nuevas entidades siempre en `MediPrax.Core/Entities/`, heredan `BaseEntity`
- Nuevos servicios: interfaz en `MediPrax.Application/Interfaces/`, implementación en `MediPrax.Application/Services/`
- Configuración EF en `MediPrax.Infrastructure/Persistence/Configurations/`
- Registro de DI en `src/MediPrax.Server/Program.cs`
- Cada milestone nuevo requiere: entidad + servicio + DTO + configuración EF + migración + página Blazor + tests
- Al completar un milestone, actualizar la documentación de usuario (`userdocs/`) describiendo la nueva funcionalidad para el usuario final

---

## Reglas obligatorias de testing

1. **Toda nueva funcionalidad DEBE incluir tests.** No se considera completa una feature sin sus tests unitarios y/o de integración correspondientes.
2. **Antes de hacer commit, ejecutar `dotnet test` y verificar que todos los tests pasan.** No commitear código que rompa tests existentes.
3. **Tipos de tests:**
   - **Unit tests** (`tests/MediPrax.UnitTests/`): para DTOs, catálogos, generadores de texto, lógica pura sin BD
   - **Integration tests** (`tests/MediPrax.IntegrationTests/`): para servicios que usan DbContext, requieren PostgreSQL via Testcontainers
4. **Convención de nombres:** `[ClaseTesteada]Tests.cs`, métodos: `[Método]_[Escenario]_[Resultado]` o descriptivo en inglés
5. **Al corregir un bug, añadir un test que reproduce el bug** antes del fix para evitar regresiones
6. **Al completar una funcionalidad, actualizar la documentación de usuario** (`userdocs/`) describiendo la nueva funcionalidad para el usuario final antes del commit

```bash
dotnet test                                   # Todos los tests (antes de commit)
dotnet test tests/MediPrax.UnitTests          # Solo unitarios (rápido)
dotnet test tests/MediPrax.IntegrationTests   # Solo integración (requiere Docker)
```

---

## Documentación adicional

| Archivo | Contenido |
|---------|-----------|
| `CHANGELOG.md` | Release notes v1.0.0 |
| `docs/06-entwicklungsplan.md` | Plan de fases y milestones (índice general M1–M42) |
| `docs/milestones/` | Especificaciones detalladas por milestone (M17–M42) |
| `docs/05-architektur.md` | Decisiones de arquitectura técnica |
| `docs/04-regulatorik.md` | Marco regulatorio KBV, gematik, DSGVO |
| `docs/10-deployment.md` | Estrategia de entornos, Hetzner, producción local |
| `userdocs/` | Guía de usuario en alemán |
