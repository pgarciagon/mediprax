# 9. Zertifizierungsstrategie: Der günstigste Weg zur eigenen Praxissoftware

> **Zielgruppe:** Arzt/Ärztin, der/die ein eigenes PVS für die eigene Praxis entwickelt und als Geschäftspartner/Gesellschafter auftritt.

## 9.1 Übersicht: Was braucht man wirklich?

Um ein PVS (Praxisverwaltungssystem) in einer vertragsärztlichen Praxis legal einsetzen zu können, gibt es **zwei getrennte Zertifizierungspfade**, die beide erfüllt werden müssen:

| Stelle | Was wird geprüft | Pflicht? | Kosten |
|--------|-----------------|----------|--------|
| **KBV** (Kassenärztliche Bundesvereinigung) | Software-Zertifizierung: korrekte Abrechnung (ADT/KVDT), Formularbedruckung, Verordnungssoftware | **Ja** — ohne KBV-Zertifizierung keine KV-Abrechnung | ~1.170–1.750 € pro Prüfmodul |
| **gematik** | Konformitätsbewertung (KOB): TI-Anbindung, ePA, E-Rezept, VSDM, KIM | **Ja** — ab 01.01.2026 verpflichtend | 600 € + MwSt. pro Fachanwendung; KOB ePA-Medikationsliste: aktuell **kostenlos** |

Zusätzlich gibt es eine **freiwillige** Rahmenvereinbarung mit der KBV (§ 332b SGB V), die das Siegel „PVS mit KBV-Vertrag" verleiht. Diese ist für den Betrieb nicht zwingend erforderlich, aber für die spätere Kommerzialisierung sehr wertvoll.

## 9.2 Schritt-für-Schritt: Der effizienteste Pfad

### Phase 1: Registrierung als Primärsystemhersteller bei gematik

**Aufwand:** Minimal — eine E-Mail
**Kosten:** 0 €

1. E-Mail an **idp-registrierung@gematik.de** mit:
   - Name des Herstellers (z.B. „MediPrax GmbH" oder Ihr Name als Einzelunternehmer)
   - Name des Produkts: „MediPrax"
2. gematik vergibt eine **Client-ID** für den Zugang zu TI-Diensten
3. Zugang zum gematik-Fachportal und zu Testumgebungen

> **Tipp:** Dies kann sofort erfolgen, noch bevor eine einzige Zeile Code geschrieben ist.

### Phase 2: KBV-Zertifizierung (Pflicht für KV-Abrechnung)

**Aufwand:** Hoch — technische Implementierung und Testverfahren
**Kosten:** ca. 1.170–1.750 € pro Prüfmodul

Die KBV-Prüfstelle zertifiziert folgende Module:

| Modul | Beschreibung | Geschätzte Gebühr |
|-------|-------------|-------------------|
| **ADT** (Abrechnungsdatenträger) | KVDT-Export, EBM-Abrechnung, Plausibilitätsprüfung | ~1.750 € |
| **AVWG** (Arzneimittelverordnung) | Verordnungssoftware nach § 73 SGB V | ~1.170 € |
| **Formularbedruckung** | Blankoformularbedruckung (Muster 1, 6, 10, etc.) | ~1.000–1.500 € |
| **Labordatentransfer** (LDT) | Labordatenimport/-export | ~1.000–1.500 € |

**Vorgehensweise:**
1. Anforderungskataloge herunterladen von https://update.kbv.de/ita-update/
2. Zertifizierungsrichtlinie der KBV studieren
3. Software gemäß den Anforderungen implementieren
4. Prüfung über das KBV-Zertifizierungsportal beantragen
5. Testdurchläufe mit KBV-Prüfdatensätzen
6. Bei Bestehen: Aufnahme in das Verzeichnis zertifizierter Software

> **Wichtig für den Piloten:** Für die Phase 1 (Pilotbetrieb in der eigenen Praxis) ist **mindestens ADT** erforderlich, um über die KV Bremen abrechnen zu können.

### Phase 3: gematik-Bestätigungsverfahren (TI-Anbindung)

**Aufwand:** Mittel — hauptsächlich technische Integration
**Kosten:** 600 € + MwSt. pro Fachanwendung

Für jede TI-Fachanwendung muss ein separates Bestätigungsverfahren durchlaufen werden:

| Fachanwendung | Priorität für MVP | Gebühr |
|--------------|-------------------|--------|
| **VSDM** (Versichertenstammdatenmanagement) | Pflicht — eGK-Prüfung | 600 € + MwSt. |
| **E-Rezept** | Pflicht | 600 € + MwSt. |
| **KIM** (Kommunikation im Medizinwesen) | Pflicht — Arztbriefe | 600 € + MwSt. |
| **NFDM** (Notfalldatenmanagement) | Optional Phase 2 | 600 € + MwSt. |
| **AMTS** (Arzneimitteltherapiesicherheit) | Optional Phase 2 | 600 € + MwSt. |
| **ePA-Anbindung** | Pflicht ab 2026 | 600 € + MwSt. |

**Vorgehensweise:**
1. gematik-Spezifikationen (gemSpec) studieren und implementieren
2. Tests in der gematik-Referenzumgebung (RU) durchführen
3. Bestätigungsantrag einreichen
4. Gutachterverfahren nach gematik-Vorgaben bestehen

### Phase 4: Konformitätsbewertung (KOB) — ePA

**Aufwand:** Mittel
**Kosten:** Aktuell kostenlos (für ePA-Medikationsliste)

Die KOB für die ePA-Medikationsliste ist seit 2025 verpflichtend. Aktuell erhebt gematik **keine Gebühren** dafür. Man muss nachweisen:
1. Erfolgreiche Tests gegen mindestens ein Aktensystem in der Referenzumgebung
2. Screenshots der Medikationslisten-Anzeige in der eigenen Software
3. Testreports aus der KOB-Testsuite

> **2026 folgt:** KOB für den Medikationsplan — Gebühren werden noch bekanntgegeben.

### Phase 5 (Optional): KBV-Rahmenvereinbarung (§ 332b SGB V)

**Aufwand:** Hoch — erfordert ISO 27001-Zertifizierung
**Kosten:** ISO 27001-Audit ca. 5.000–15.000 € + jährliche Überwachung

Voraussetzungen:
- ISO 27001-zertifiziertes ISMS für Rechenzentrumsbetrieb, Softwareentwicklung, Deployment, Support
- Mindestens **100 Installationen** in der KBV-Installationsstatistik
- Kontakt: PVSmitVertrag@kbv.de

> **Für den Piloten nicht relevant.** Erst interessant, wenn MediPrax kommerzialisiert wird und > 100 Praxen bedient.

## 9.3 Kostenkalkulation: Minimalweg für den Piloten

### Minimum Viable Certification (nur für eigene Praxis)

| Position | Kosten |
|----------|--------|
| gematik-Registrierung als Hersteller | 0 € |
| KBV-Zertifizierung ADT (Abrechnung) | ~1.750 € |
| KBV-Zertifizierung AVWG (Verordnung) | ~1.170 € |
| gematik-Bestätigung VSDM | ~714 € (600 + MwSt.) |
| gematik-Bestätigung E-Rezept | ~714 € |
| gematik-Bestätigung KIM | ~714 € |
| gematik KOB ePA-Medikationsliste | 0 € (aktuell) |
| **Summe Zertifizierungsgebühren** | **~5.062 €** |

### Verglichen mit CGM MEDISTAR-Jahreskosten

| | Zertifizierung MediPrax | CGM MEDISTAR (Jahr) |
|---|---|---|
| Einmalkosten | ~5.062 € | 0 € (in Lizenz enthalten) |
| Jährliche Lizenz | 0 € (eigene Software) | 3.560–11.280 € |
| **Break-even** | **Sofort im ersten Jahr** | — |

### Volle Zertifizierung (für Kommerzialisierung)

| Position | Kosten |
|----------|--------|
| Alle KBV-Module (ADT, AVWG, Formular, LDT) | ~5.500–6.000 € |
| Alle gematik-Bestätigungen (6 Fachanwendungen) | ~4.300 € |
| ISO 27001-Zertifizierung (für Rahmenvereinbarung) | ~10.000 € |
| KOB ePA (aktuell + künftige Module) | TBD |
| **Summe geschätzt** | **~20.000–25.000 €** |

## 9.4 Strategische Vorteile als Arzt-Entwickler

Als Arzt, der sein eigenes PVS für die eigene Praxis entwickelt, hast du mehrere einzigartige Vorteile:

**1. Kein Henne-Ei-Problem:** Du bist gleichzeitig Hersteller und erster Kunde. Du brauchst keine 100 Installationen, um das System zu nutzen — die KBV-Zertifizierung (ADT) reicht für den eigenen Praxisbetrieb.

**2. KOB ohne Gebühren:** Die erste KOB-Runde (ePA-Medikationsliste) ist kostenlos. Das senkt die Einstiegshürde.

**3. Schrittweise Zertifizierung möglich:** Du musst nicht alles auf einmal zertifizieren. Der Minimalweg (ADT + VSDM + E-Rezept) reicht für den täglichen Praxisbetrieb.

**4. Domänenwissen:** Du kennst die Workflows, die GOPs, die Formulare — das spart enormen Abstimmungsaufwand.

**5. TI-Pauschale:** Die KV erstattet 263,62 €/Monat für TI-Kosten — das refinanziert die Zertifizierungskosten in unter 2 Jahren.

## 9.5 Risiken und Gegenmaßnahmen

| Risiko | Wahrscheinlichkeit | Gegenmaßnahme |
|--------|-------------------|---------------|
| KBV-Prüfung nicht bestanden | Mittel | Anforderungskataloge als Testspezifikation nutzen; KBV-Prüfdatensätze frühzeitig integrieren |
| gematik ändert Spezifikationen | Hoch | Modulare Architektur; TI-Schicht isoliert; regelmäßig gematik-Newsletter verfolgen |
| KOB wird kostenpflichtig | Wahrscheinlich | In Kostenplanung 3.000–5.000 € Reserve einplanen |
| Zeitdruck durch Fristen | Mittel | Zertifizierungen parallel zur Entwicklung vorbereiten; Testdaten ab Tag 1 |
| ISO 27001 zu teuer für Pilot | Gering (nicht nötig) | Erst bei Kommerzialisierung relevant; für Pilotbetrieb nicht erforderlich |

## 9.6 Empfohlener Zeitplan

| Monat | Aktivität |
|-------|-----------|
| 1 | gematik-Registrierung; KBV-Anforderungskataloge herunterladen |
| 1–6 | Kern-Entwicklung mit KBV-ADT-Anforderungen als Testbasis |
| 6–8 | KBV-ADT-Zertifizierung beantragen und durchlaufen |
| 7–12 | TI-Anbindung (Konnektor, VSDM, E-Rezept) |
| 10–12 | gematik-Bestätigungsverfahren VSDM + E-Rezept |
| 12–14 | gematik-Bestätigung KIM; KOB ePA |
| 14–16 | KBV-Zertifizierung AVWG (Verordnungssoftware) |
| 16+ | Pilotbetrieb in eigener Praxis |

## 9.7 Wichtige Kontakte und Links

| Stelle | Kontakt / URL |
|--------|--------------|
| gematik Hersteller-Registrierung | idp-registrierung@gematik.de |
| gematik Fachportal Primärsysteme | https://fachportal.gematik.de/hersteller-anbieter/primaersysteme |
| gematik Gebührenübersicht | https://fachportal.gematik.de/schnelleinstieg/downloadcenter/zulassungs-bestaetigungsantraege-verfahrensbeschreibungen/kosten |
| gematik Konformitätsbewertung (KOB) | https://www.ina.gematik.de/kig/konformitaetsbewertung |
| gematik Spezifikationen (gemSpec) | https://gemspec.gematik.de/ |
| KBV Zertifizierung | https://www.kbv.de/infothek/ita/zertifizierung |
| KBV Anforderungskataloge / Updates | https://update.kbv.de/ita-update/ |
| KBV Rahmenvereinbarung | PVSmitVertrag@kbv.de |
| KBV Verzeichnis zertifizierter Software | https://update.kbv.de/ita-update/Service-Informationen/Zulassungsverzeichnisse/ |
| KBV Zertifizierungsrichtlinie (PDF) | https://update.kbv.de/ita-update/Allgemein/KBV_ITA_RLEX_Zert.pdf |
