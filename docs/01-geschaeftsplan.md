# 1. Geschäftsplan

## 1.1 Zusammenfassung

MediPrax ist ein modernes Praxisverwaltungssystem (PVS) als Alternative zu CGM Medistar. Das Projekt verfolgt einen zweistufigen Ansatz:

1. **Pilotbetrieb** in der Gemeinschaftspraxis Neuropsychiatricum Bremen, deren Ärzteteam gleichzeitig als Gesellschafter der neuen Firma fungiert.
2. **Kommerzialisierung** — Vertrieb an weitere Praxen.

### Kernstrategie

- **Minimale Funktionalität (MVP)**, maximale Anpassungsfähigkeit
- **KI-gestützte Entwicklung** mit Claude Code: Ein einzelner Entwickler erreicht die Produktivität eines kleinen Teams
- **Ärzte als Gesellschafter**: Domänenwissen ist kostenlos und jederzeit verfügbar — kein externes Consulting nötig
- **Schnelle Iterationszyklen** statt monatelanger Planungsphasen

## 1.2 Alleinstellungsmerkmale (USPs)

- **Kosten:** Ein Entwickler + Claude Code statt 3–5 Entwickler-Team. Personalkosten um 60–70% reduziert.
- **Ärzte als Gesellschafter:** Domänenwissen ist kostenlos und jederzeit verfügbar. Keine teuren Fachberater nötig.
- **Anpassungsgeschwindigkeit:** Claude Code ermöglicht Änderungen in Stunden statt Wochen. Direkte Rückmeldung der Praxis → sofortige Umsetzung.
- **MVP-Ansatz:** Nur die wirklich benötigten Funktionen. Keine aufgeblähte Software mit hunderten ungenutzten Modulen.
- **Skalierbarkeit:** Vom Pilotprojekt zur kommerziellen Lösung — die Architektur wird von Anfang an darauf ausgelegt.

## 1.3 Unternehmensstruktur

Gründung einer GmbH mit den Ärzten des Neuropsychiatricum Bremen als Gesellschafter.

| Rolle | Beitrag | Vorteil |
|-------|---------|---------|
| Ärzte (Gesellschafter) | Domänenwissen, Pilotpraxis, klinische Validierung, Marktzugang | Kein Beratungshonorar, direkte Feedback-Schleife |
| Entwickler + Claude Code | Gesamte Softwareentwicklung, Architektur, TI-Integration | 1 Person statt Team, drastisch reduzierte Kosten |
| Externer Berater (TI) | Unterstützung bei gematik-Zertifizierung (bei Bedarf) | Nur punktuell, nicht permanent |

## 1.4 Stufenplan

| Stufe | Zeitraum | Ziel |
|-------|----------|------|
| Stufe 1: Pilot | Monate 1–20 | Funktionsfähiges MediPrax im Neuropsychiatricum Bremen, alle Zertifizierungen |
| Stufe 2: Stabilisierung | Monate 21–24 | Parallelbetrieb, Optimierung, Schulung, Medistar-Ablösung |
| Stufe 3: Kommerzialisierung | Ab Monat 25 | Vertrieb an andere Praxen, Marketing, Support-Strukturen aufbauen |

## 1.5 Claude Code als Entwicklungswerkzeug

Claude Code ist ein agentenbasiertes KI-Entwicklungstool von Anthropic, das direkt im Terminal arbeitet. Es versteht die gesamte Codebasis, kann autonom Code schreiben, refactoren, testen und debuggen.

### Vorteile für MediPrax

| Vorteil | Auswirkung |
|---------|-----------|
| Ein-Personen-Entwicklung möglich | Produktivität eines kleinen Teams, keine Koordinationskosten |
| Schnelle Prototypentwicklung | UI-Komponenten, API-Endpunkte in Stunden statt Tagen |
| Codequalität | Konsistenter, gut strukturierter Code mit Tests |
| Technologie-Vielseitigkeit | C#/.NET, TypeScript, SQL, FHIR und mehr |
| Testabdeckung | Automatische Generierung von Unit-/Integrationstests |

### Grenzen

- **Zertifizierungsprozesse:** KBV- und gematik-Zertifizierungen erfordern formale Anträge und persönliche Kommunikation.
- **Konnektor-Zugang:** Zugriff auf die TI-Referenzumgebung erfordert Registrierung als Primärsystem-Hersteller.
- **Medizinrecht:** Rechtsfragen erfordern juristische Beratung.
- **Domänenwissen:** Klinische Workflows und EBM-Abrechnung erfordern enge Zusammenarbeit mit den Ärzten.
- **Hardware-Integration:** Kartenlesegeräte, Konnektor und eHBA/SMC-B erfordern physischen Zugang.

### Empfohlener Workflow

1. **Anforderungen definieren:** Entwickler analysiert den Bedarf mit der Praxis, dokumentiert User Stories.
2. **Architekturentscheidungen:** Entwickler trifft strategische Entscheidungen.
3. **Implementierung mit Claude Code:** Umsetzung der Anforderungen in Code — Module, Tests, Dokumentation.
4. **Review und QA:** Entwickler prüft den generierten Code, manuelle und automatisierte Tests.
5. **Iteration:** Feedback-Schleifen zwischen Praxis, Entwickler und Claude Code.

## 1.6 Kommerzialisierung

### Preismodell (Vorschlag)

| Paket | Preis/Monat | Enthält |
|-------|-------------|---------|
| Basis (Einzelpraxis) | 89–149 € | Kernfunktionen, EBM/GOÄ, TI-Anbindung, E-Mail-Support |
| Standard (Gemeinschaftspraxis) | 149–249 € | Basis + Mehrplatz, erweiterte Vorlagen, Telefon-Support |
| Premium (MVZ) | 249–449 € | Standard + Multi-Standort, erweiterte Analytik, Priority-Support |
| Einrichtungspauschale (einmalig) | 1.500–3.000 € | Installation, Datenmigration, Erstschulung |

### Wettbewerbsvorteil: Entwicklungsgeschwindigkeit

| Änderung | CGM Medistar (geschätzt) | MediPrax + Claude Code |
|----------|-------------------------|----------------------|
| Neue EBM-Ziffer integrieren | Quartals-Update (3–6 Monate) | 1–2 Tage |
| Neues Formular/Template | Feature-Request (Monate) | Stunden |
| Workflow-Anpassung | Oft nicht möglich | Sofort |
| Bugfix (kritisch) | Hotfix nach Meldung (Tage–Wochen) | Stunden |
| Neue TI-Anforderung (gematik) | Quartals-Update | 1–2 Wochen |
| Fachspezifische Erweiterung | Feature-Request (Jahre oder nie) | Tage–Wochen |
