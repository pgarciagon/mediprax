# 4. Regulatorischer Rahmen

> **Kritisch:** Ein PVS in Deutschland ist nicht nur Software — es ist ein reguliertes Produkt, das mehrere Pflicht-Zertifizierungen benötigt, um legal betrieben werden zu dürfen. Ohne diese Zertifizierungen darf die Software seit dem 01.01.2026 nicht zur Abrechnung mit den Kassenärztlichen Vereinigungen eingesetzt werden.

## 4.1 Pflicht-Zertifizierungen

| Zertifizierung | Institution | Status | Konsequenz bei Fehlen |
|---------------|------------|--------|----------------------|
| KBV-Zertifizierung | Kassenärztliche Bundesvereinigung | Pflicht | Keine elektronische Abrechnung mit der KV möglich |
| KOB (Konformitätsbewertung) | gematik | Pflicht seit 01.01.2026 | PVS darf nicht zur Abrechnung eingesetzt werden |
| ePA-Zertifizierung | gematik | Pflicht seit 01.10.2025 | 50% Kürzung der TI-Pauschale |
| DSGVO-Konformität | Landesdatenschutzbehörde Bremen | Pflicht | Bußgelder bis zu 20 Mio. € |

## 4.2 KBV-Zertifizierung

Die KBV ist der Bundesverband der Vertragsärzte. Jedes PVS, das zur Abrechnung mit den KVen eingesetzt wird, muss von der KBV zertifiziert sein.

- **Pflichtzertifizierung:** Ohne KBV-Zertifizierung kann die Praxis nicht elektronisch mit der KV Bremen (KVHB) abrechnen.
- **KVDT (KV-Datentransfer):** Standardisiertes Format für den Abrechnungsdatentransfer. MediPrax muss gültige KVDT-Dateien erzeugen.
- **Zertifizierungsprüfungen:** Die KBV veröffentlicht Prüfpakete (Prüfpaket KVDT), die die Software bestehen muss.
- **Offizielles Verzeichnis:** MediPrax muss im Verzeichnis zertifizierter Software der KBV gelistet sein.

### Nächste Schritte

1. KBV kontaktieren: kbv.de/infothek/ita
2. Prüfpakete beziehen
3. Zertifizierungskosten erfragen

## 4.3 Konformitätsbewertung (KOB) — gematik

Seit dem 01.01.2026 ist die KOB für alle PVS gemäß dem Digitalisierungsgesetz (DigiG) verpflichtend.

- **Rechtsgrundlage:** § 387 SGB V — Hersteller von Gesundheits-IT-Systemen müssen die KOB der gematik durchlaufen.
- **Ohne KOB = keine Abrechnung:** Ein PVS ohne KOB darf seit 01.01.2026 nicht zur Abrechnung über die KV eingesetzt werden.
- **Übergangsfrist:** Weitere 9 Monate für Hersteller, um die Zertifizierung abzuschließen.
- **Kryptographie:** Pflichtmigration von RSA-2048 auf ECC 256. RSA ist in der TI ab 2026 untersagt.

### Nächste Schritte

1. gematik kontaktieren: fachportal.gematik.de
2. Registrierung als Primärsystem-Hersteller
3. KOB-Kosten und -Prozess erfragen
4. Zugang zur Referenzumgebung beantragen

## 4.4 Telematikinfrastruktur (TI)

Die TI ist die nationale Gesundheits-Kommunikationsinfrastruktur Deutschlands.

| Komponente | Beschreibung |
|-----------|-------------|
| Konnektor | Hardware-/Software-Gerät, das die Praxis mit der TI verbindet. Mindestens PTV4+ |
| eGK | Elektronische Gesundheitskarte — Einlesen zur Datenprüfung (VSDM) |
| eHBA | Elektronischer Heilberufsausweis — digitale Signatur, Authentifizierung |
| SMC-B | Institutionelles Zertifikat der Praxis |
| KIM | Sicheres Nachrichtensystem zwischen Leistungserbringern |

### MediPrax TI-Integration

- Konnektor SOAP/REST API für VSDM, eGK-Einlesen
- ePA Lesen/Schreiben über FHIR (Firely SDK)
- E-Rezept-Anbindung
- KIM für sichere Arztbriefe
- ECC-256-Kryptographie nativ (kein Altlasten-RSA)

## 4.5 ePA (Elektronische Patientenakte)

Seit Oktober 2025 ist die ePA in allen medizinischen Einrichtungen Pflicht.

- **Pflichtinhalte:** Befunde, Arztbriefe, Bildgebung und Laborergebnisse (wenn intern erzeugt und digital verfügbar)
- **Technische Anforderung:** ePA PVS Update Level 3.0, Zugriff über FHIR oder gerendertes Dokument
- **Zertifikate:** eHBA und SMC-B Generation 2.0 gültig bis 30.06.2026

## 4.6 Interoperabilität (HL7 FHIR / ISiK / ISiP)

- **HL7 FHIR:** Internationaler Standard für den Austausch von Gesundheitsdaten
- **ISiK:** Informationstechnische Systeme in Krankenhäusern (§ 373 SGB V). RESTful-Schnittstellen auf FHIR-Basis
- **ISiP:** Äquivalent für ambulante Praxen
- **MediPrax-Ansatz:** Vollständige FHIR-Server-Implementierung mit Firely SDK, Validierung gegen ISiP-Profile

## 4.7 Fachspezifische Abrechnung: Psychiatrie und Neurologie

| GOP-Code | Beschreibung | Hinweise |
|---------|-------------|---------|
| 21213–21215 | Grundpauschalen für Nervenärzte | Für Ärzte mit Doppelfachgebiet Neuro+Psych |
| 16220 | Neurologisches Gespräch, Behandlung, Beratung | Kapitel 16 EBM |
| 21220 | Psychiatrisches Gespräch, Behandlung, Beratung | Kapitel 21 EBM |
| 35600 | Standardisierte psychologische Testverfahren | Pflichtdokumentation der Dauer |
| GOÄ | Abrechnung für Privatpatienten (PKV) | Separater Leistungskatalog |

## 4.8 Datenschutz

- **DSGVO:** EU-Datenschutz-Grundverordnung
- **BDSG:** Bundesdatenschutzgesetz
- **BremDSGVOAG:** Bremisches Ausführungsgesetz zur DSGVO
- **Ärztliche Schweigepflicht:** § 203 StGB, strafrechtlich geschützt
- **Verschlüsselung:** Daten at-rest und in-transit, Partitionsverschlüsselung mit TPM

## Quellen

- [KBV Praxisverwaltungssystem](https://www.kbv.de/praxis/digitalisierung/praxisverwaltungssystem)
- [KBV Rahmenvereinbarung](https://www.kbv.de/infothek/ita/rahmenvereinbarung-pvs-anbieter)
- [gematik Primärsysteme](https://fachportal.gematik.de/hersteller-anbieter/primaersysteme)
- [gematik KOB](https://www.ina.gematik.de/kig/konformitaetsbewertung)
- [KVHB Telematikinfrastruktur](https://www.kvhb.de/praxen/it-digitalisierung/telematik-infrastruktur-ti)
- [Gelbe Liste Digitalisierung 2026](https://www.gelbe-liste.de/allgemeinmedizin/digitalisierung-2026-ti-umstellung-epa-medikationsplan-pvs-uebergangsregeln)
