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

1. ✅ Projekt-Setup: Repository, CI/CD, Clean Architecture, Datenbankschema (EF Core + PostgreSQL)
2. ✅ Patientenverwaltung (CRUD, Suche, Versicherungsdaten, GKV/PKV)
3. ✅ Terminplanung (Wochenansicht, Terminformular, Wartezimmer)
4. ✅ Klinische Dokumentation (Encounters, ICD-10-GM-Suche, Neuro/Psych-Vorlagen)
5. ✅ Arztbrief-Generierung (QuestPDF, Vorlagen, PDF-Download)
6. ✅ Dashboard (KPIs, heutige Termine, letzte Aktivität, Schnellzugriffe)
7. ✅ Authentifizierung (Cookie-Login, BCrypt, Seitenprotection, Abmelden)
8. ✅ Branding (Logo-SVG/PNG, Favicons, Login-Screen)

### Nächste Meilensteine (Phase 1 Härtung)

#### Meilenstein 8: Benutzerverwaltung und Rollenbasierte Zugriffskontrolle

Verwaltung von Benutzerkonten und Durchsetzung rollenbasierter Zugriffsrechte.

- **User CRUD** — `IUserService` erweitern: GetAll, Create, Update, Deactivate, ResetPassword
- **Admin-Seiten** — `/verwaltung/benutzer` (Liste, Formular)
- **RBAC durchsetzen** — Arzt (voller klinischer Zugriff), MFA (Patienten/Termine), Empfang (Check-in), Admin (Benutzerverwaltung)
- **BaseEntity erweitern** — `CreatedById`, `UpdatedById` für Audit-Trail
- **Passwort ändern** — `/konto/passwort` für eingeloggte Benutzer
- **Sidebar** — Verwaltungsbereich nur für Admin sichtbar

#### Meilenstein 9: Audit-Logging und Datenschutz-Compliance

Pflicht für medizinische Software gemäß DSGVO, §203 StGB, KBV-Richtlinien.

- **AuditLog-Entity** — Timestamp, User, Action, EntityType, EntityId, OldValues/NewValues (JSON)
- **EF Core Interceptor** — automatische Protokollierung aller Create/Update/Delete-Operationen
- **Login/Logout-Audit** — inkl. IP-Adresse und Erfolg/Misserfolg
- **Patientenakten-Zugriff loggen** — jeder Zugriff auf Patientendaten nachvollziehbar (DSGVO)
- **Soft Delete** — `IsDeleted` + `DeletedAt` auf BaseEntity, Global Query Filter (Aufbewahrungspflicht 10 Jahre, §630f BGB)
- **Audit-Log Viewer** — `/verwaltung/audit-log` (nur Admin, Filterung, Paginierung)

#### Meilenstein 10: Abrechnung — EBM-Ziffernerfassung und GOÄ-Rechnungen

Die `BillingItem`-Entity existiert bereits. Geschäftslogik und UI aufbauen.

- **GOP-Katalog** — EBM Kap. 16/21 (Neurologie/Psychiatrie) + GOÄ-Ziffern als Stammdaten
- **IBillingService** — Ziffern erfassen, Quartalsübersicht, Plausibilitätsprüfung
- **Encounter-Formular erweitern** — Abrechnung-Fieldset mit GOP-Suche
- **Abrechnungsübersicht** — `/abrechnung` mit Quartalsfilter, Arztfilter, Exportstatus
- **GOÄ-Rechnung PDF** — QuestPDF-basierte Privatrechnung
- **Plausibilitätsprüfung** — Ausschlüsse, Mengenbegrenzung, Dauerdokumentation

#### Meilenstein 11: Globale Suche, Berichte und Praxisstatistiken

- **Globale Suche** — Ctrl+K Shortcut, Suche über Patienten/Termine/Encounters/Dokumente
- **Berichte** — Tagesbericht, Quartalsstatistik, Patientenstatistik, Abrechnungsbericht
- **Dashboard erweitern** — rollenspezifische Widgets, Trend-Diagramme
- **PDF-Berichte** — Quartalsübersicht als PDF für Praxisleitung
- **Input-Validierung** — DataAnnotations auf allen DTOs

#### Meilenstein 12: Tests, Docker-Deployment und Produktionshärtung

- **Unit Tests** — alle Application Services, Validierungsregeln, Audit-Interceptor
- **Integration Tests** — WebApplicationFactory + Testcontainers.PostgreSQL
- **Dockerfile** — Multi-Stage Build (SDK → Runtime)
- **docker-compose.yml** — mediprax-server + PostgreSQL + Health Checks
- **CI/CD erweitern** — Integration Tests, Docker Build, GHCR Push
- **Fehlerbehandlung** — Global Exception Handler, ILogger, Retry-Policies
- **Performance** — AsNoTracking, Index-Review, Response Compression
- **Konfiguration** — appsettings.json, Praxis-Stammdaten externalisieren

### Abhängigkeiten

```
M8: Benutzerverwaltung & RBAC
 └─→ M9: Audit-Logging & Datenschutz (benötigt Benutzer-Tracking)
      └─→ M10: Abrechnung EBM/GOÄ (benötigt Rollen + Audit)
           └─→ M11: Suche, Berichte, Statistiken (benötigt Abrechnungsdaten)
                └─→ M12: Tests, Docker, Produktion (benötigt alle vorherigen)
```

### Ergebnis Phase 1

Produktionsreifes System mit Patientenverwaltung, Terminplanung, klinischer Dokumentation, Arztbriefen, Abrechnung, Audit-Trail und Docker-Deployment. Bereit für Parallelbetrieb.

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
