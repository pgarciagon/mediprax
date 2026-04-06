# 5. Technische Architektur

## 5.1 Technologie-Stack

Optimiert für Claude Code + Ein-Personen-Entwicklung:

| Schicht | Technologie | Begründung |
|---------|------------|-----------|
| Backend + Frontend | C# / .NET 8+ mit Blazor Server | Gleiche Sprache für alles → Claude Code bearbeitet den gesamten Stack |
| Datenbank | PostgreSQL 16+ | Kostenlos, robust, FHIR-JSON-Unterstützung |
| API | ASP.NET Core Web API (REST) | FHIR-Basis, Interoperabilität |
| FHIR | Firely SDK (.NET) | Zertifizierte HL7-FHIR-Bibliothek |
| TI | Konnektor SOAP/REST API | gematik-Spezifikation |
| PDF-Erzeugung | QuestPDF | Rezepte, Befunde, Arztbriefe (Open Source) |
| Tests | xUnit + Playwright | Claude Code generiert Tests automatisch |
| CI/CD | GitHub Actions | Automatisierte Builds und Deployments |
| Server | Windows Server 2022 | Vorhandene Infrastruktur des Kunden |

## 5.2 Architekturprinzipien

- **Ein Stack, eine Sprache:** C# für Frontend, Backend und Tests. Keine Fragmentierung, Claude Code arbeitet effizienter.
- **Modularer Monolith:** Klare Modul-Grenzen, aber ein Deployment. Keine Microservice-Komplexität für eine Einzelpraxis.
- **API-First:** REST-API als Kern. Ermöglicht spätere mobile Apps, Web-Portale oder Integrationen.
- **Vorbereitet für Multi-Tenancy:** Architektur berücksichtigt von Anfang an mehrere Praxen (Kommerzialisierung).

## 5.3 Architekturübersicht

```
┌──────────────────────────────────────────────────────────┐
│                    MediPrax Server                        │
│                  (Windows Server 2022)                    │
│                                                          │
│  ┌────────────────┐  ┌────────────────┐  ┌────────────┐ │
│  │  Blazor Server  │  │  REST API      │  │  Background│ │
│  │  (Frontend)     │  │  (FHIR, KVDT)  │  │  Services  │ │
│  └───────┬────────┘  └───────┬────────┘  └─────┬──────┘ │
│          │                   │                  │        │
│  ┌───────┴───────────────────┴──────────────────┴──────┐ │
│  │              Business Logic Layer                    │ │
│  │  ┌──────┐ ┌──────┐ ┌────────┐ ┌──────┐ ┌────────┐ │ │
│  │  │Patien│ │Termin│ │Dokumen-│ │Abrech│ │Verord- │ │ │
│  │  │ten   │ │e     │ │tation  │ │nung  │ │nung    │ │ │
│  │  └──────┘ └──────┘ └────────┘ └──────┘ └────────┘ │ │
│  └───────────────────────┬────────────────────────────┘ │
│                          │                               │
│  ┌───────────────────────┴────────────────────────────┐ │
│  │              Data Access Layer                      │ │
│  │         Entity Framework Core + PostgreSQL          │ │
│  └────────────────────────────────────────────────────┘ │
└──────────────────────────┬───────────────────────────────┘
                           │
            ┌──────────────┼──────────────┐
            │              │              │
    ┌───────▼──────┐ ┌────▼─────┐ ┌──────▼──────┐
    │  Konnektor   │ │PostgreSQL│ │   KIM       │
    │  (TI/eGK/   │ │  16+     │ │  (Messaging)│
    │   ePA/eRx)  │ │          │ │             │
    └──────────────┘ └──────────┘ └─────────────┘


┌─────────────┐  ┌─────────────┐  ┌─────────────┐
│  Client PC  │  │  Client PC  │  │  Client PC  │
│  Win 11     │  │  Win 11     │  │  Win 11     │
│  (Browser)  │  │  (Browser)  │  │  (Browser)  │
└─────────────┘  └─────────────┘  └─────────────┘
```

## 5.4 Modulstruktur

```
MediPrax/
├── src/
│   ├── MediPrax.Server/           # Blazor Server Host + Startup
│   ├── MediPrax.Core/             # Domain Models, Interfaces
│   ├── MediPrax.Application/      # Business Logic, Use Cases
│   ├── MediPrax.Infrastructure/   # DB, External Services
│   │   ├── Persistence/           # EF Core, Migrations
│   │   ├── TI/                    # Konnektor, eGK, ePA, eRx
│   │   ├── FHIR/                  # Firely SDK Integration
│   │   ├── KIM/                   # Messaging
│   │   └── KVDT/                  # Abrechnungsdaten-Export
│   ├── MediPrax.UI/               # Blazor Components, Pages
│   │   ├── Pages/
│   │   ├── Components/
│   │   └── Shared/
│   └── MediPrax.Reporting/        # QuestPDF Templates
│       ├── Arztbrief/
│       ├── Rezept/
│       └── Befund/
├── tests/
│   ├── MediPrax.UnitTests/
│   ├── MediPrax.IntegrationTests/
│   └── MediPrax.E2ETests/         # Playwright
├── docs/                           # Dieses Dokumentationsverzeichnis
├── .github/
│   └── workflows/                  # CI/CD
└── README.md
```

## 5.5 Datenbank-Design (Kerntabellen)

```sql
-- Kerntabellen (vereinfacht)

CREATE TABLE patients (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    date_of_birth DATE NOT NULL,
    gender VARCHAR(10),
    insurance_type VARCHAR(3) CHECK (insurance_type IN ('GKV', 'PKV')),
    insurance_number VARCHAR(30),
    insurance_provider VARCHAR(200),
    kvnr VARCHAR(10),          -- Krankenversichertennummer
    address JSONB,
    phone VARCHAR(50),
    email VARCHAR(200),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE appointments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id UUID REFERENCES patients(id),
    doctor_id UUID REFERENCES users(id),
    start_time TIMESTAMPTZ NOT NULL,
    duration_minutes INT DEFAULT 10,
    status VARCHAR(20) DEFAULT 'scheduled',
    notes TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE encounters (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id UUID REFERENCES patients(id),
    doctor_id UUID REFERENCES users(id),
    appointment_id UUID REFERENCES appointments(id),
    encounter_date DATE NOT NULL,
    notes TEXT,
    icd10_codes JSONB,         -- Array of ICD-10-GM codes
    gops JSONB,                -- Gebührenordnungspositionen
    duration_minutes INT,      -- Pflichtdokumentation für Gespräche
    status VARCHAR(20) DEFAULT 'open',
    created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE prescriptions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id UUID REFERENCES patients(id),
    doctor_id UUID REFERENCES users(id),
    encounter_id UUID REFERENCES encounters(id),
    medication_name VARCHAR(300),
    medication_pzn VARCHAR(20),  -- Pharmazentralnummer
    dosage TEXT,
    is_btm BOOLEAN DEFAULT FALSE, -- Betäubungsmittel
    e_rezept_id VARCHAR(100),
    status VARCHAR(20) DEFAULT 'draft',
    created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE documents (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id UUID REFERENCES patients(id),
    encounter_id UUID REFERENCES encounters(id),
    doc_type VARCHAR(50),       -- 'arztbrief', 'befund', 'labor', etc.
    title VARCHAR(300),
    content TEXT,
    pdf_data BYTEA,
    kim_message_id VARCHAR(200),
    created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE billing_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    encounter_id UUID REFERENCES encounters(id),
    patient_id UUID REFERENCES patients(id),
    gop_code VARCHAR(10) NOT NULL,
    gop_description VARCHAR(300),
    quantity INT DEFAULT 1,
    billing_type VARCHAR(3) CHECK (billing_type IN ('EBM', 'GOA')),
    quarter VARCHAR(7),         -- z.B. '2026-Q2'
    kvdt_exported BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMPTZ DEFAULT NOW()
);
```

## 5.6 Deployment

### Pilotpraxis (On-Premise)

- MediPrax Server auf vorhandenem Windows Server 2022
- PostgreSQL auf demselben Server
- Blazor Server über LAN erreichbar (keine Internetexposition)
- Clients: jeder Windows-11-PC mit Browser (Chrome/Edge)
- Konnektor-Zugang über lokales Netzwerk

### Zukünftig (Kommerzialisierung)

- Option 1: On-Premise (wie Pilot) — für datenschutzbewusste Praxen
- Option 2: Managed Hosting — Server in deutschem Rechenzentrum (DSGVO-konform)
- Option 3: Hybrid — Server lokal, Backup/Updates remote

## 5.7 Sicherheit

- HTTPS/TLS für alle Verbindungen
- Rollenbasierte Zugriffskontrolle (Arzt, MFA, Empfang, Admin)
- Audit-Logging aller datenschutzrelevanten Zugriffe
- Datenverschlüsselung at-rest (PostgreSQL + TPM)
- Regelmäßige automatische Backups
- Keine Patientendaten an externe Dienste (inkl. Claude Code)
