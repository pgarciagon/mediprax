# Changelog

All notable changes to MediPrax will be documented in this file.

## [1.0.0] — 2026-04-08

### First Release — MVP Complete (Milestones 1–16)

MediPrax v1.0 is a fully functional Praxisverwaltungssystem (PVS) for psychiatric and
neurological practices in Germany, built with .NET 10 / Blazor Server and PostgreSQL.

#### Patient Management
- Full CRUD with search (name, KVNR, phone) and pagination
- GKV/PKV insurance data management
- Contact and address tracking
- CSV import for patient migration from legacy systems

#### Appointment Scheduling
- Weekly calendar view (Wochenansicht) with multi-doctor support
- Waiting room management (Wartezimmer) with real-time status workflow
- Appointment statuses: Scheduled, CheckedIn, InProgress, Completed, Cancelled, NoShow

#### Clinical Documentation
- Encounter management with ICD-10-GM coding (autocomplete catalog)
- Documentation templates: Psychiatrisch, Neurologisch, Kurzkonsultation
- Duration tracking for time-based billing (GOPs 16220/21220)

#### Physician Letters (Arztbrief)
- Full Arztbrief generation with QuestPDF
- Auto-populated patient data and diagnoses
- PDF storage and download

#### Medication Management
- Active medication plan with Wirkstoff, PZN, dosage schedule
- BTM (controlled substance) flagging
- Medication history tracking (active/inactive)

#### Prescription Management
- Prescription creation linked to encounters
- Status tracking: Draft, Signed, Sent, Dispensed, Cancelled
- E-Rezept ID support for digital prescriptions

#### Billing (Abrechnung)
- EBM and GOÄ billing with GOP catalog
- Quarter-based billing overview with patient aggregation
- KVDT export for submission to KV (Kassenärztliche Vereinigung)
- Billing item management per encounter

#### Forms & Documents (Formulare)
- Rezept (prescription form) — PDF generation
- AU-Bescheinigung (work incapacity certificate) — PDF generation
- Überweisung (referral form) — PDF generation

#### Follow-up Management (Wiedervorlagen)
- Recall/follow-up scheduling with due dates
- Status tracking: Open, Scheduled, Completed
- Overdue indicator

#### Telematikinfrastruktur (TI) — Simulated
- eGK card reading (VSDM) — mock service
- E-Rezept creation and status tracking — mock service
- KIM secure messaging (send, inbox, search) — mock service
- ePA document management (consent, upload, retrieve) — mock service
- Konnektor status monitoring — mock client
- FHIR R4 SDK integrated (Firely)
- All TI services are interface-based, ready for real TI implementation

#### Dashboard & Reporting
- Home dashboard with KPIs (patients, appointments today, docs this week)
- Upcoming appointments and recent activity feed
- Daily reports (Tagesbericht) with per-doctor breakdown
- Quarterly reports (Quartalsbericht) with billing summary
- Global search across patients, appointments, encounters, ICD-10 codes

#### User Management & Security
- Role-based access control: Admin, Arzt, MFA, Empfang
- Cookie-based authentication with BCrypt password hashing
- Rate limiting (5 login attempts/minute)
- 12-hour session timeout with sliding expiration
- Full DSGVO-compliant audit trail (create, read, update, delete, login, export)
- Soft delete on all entities with 10-year retention

#### UI/UX
- Dark mode support
- Responsive design
- German-language interface throughout
- Print-optimized form layouts

#### Infrastructure
- Docker + docker-compose deployment
- PostgreSQL 16+ with EF Core 10
- Health check endpoint (/health)
- 76 automated tests (26 unit + 50 integration)
- GitHub Actions CI/CD pipeline

### Known Limitations (v1.0)
- TI services are mocked (no real Konnektor connection)
- No structured psychopathological assessment (AMDP) — planned for v2.0
- No psychometric testing (PHQ-9, GAD-7, etc.) — planned for v2.0
- No PTV forms / therapy approval management — planned for v2.0
- No structured neurological examination — planned for v2.0
- No lab integration (LDT) — planned for v2.0
- No GDT device interface — planned for v2.0
- No video consultation integration — planned for v2.0
- See IMPLEMENTATION_PLAN_PSYCHIATRY_NEUROLOGY.md for the full v2.0 roadmap
