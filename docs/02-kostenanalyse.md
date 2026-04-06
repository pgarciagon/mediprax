# 2. Kostenanalyse

> **Hinweis:** CGM veröffentlicht keine offiziellen Preislisten. Die folgenden Werte basieren auf Branchenrecherchen, Vergleichsportalen und Erfahrungsberichten. Die tatsächlichen Kosten der Praxis können abweichen und sollten anhand der aktuellen Verträge verifiziert werden.

## 2.1 Ist-Zustand: Geschätzte jährliche Kosten CGM Medistar

| Kostenposition | Monatlich (gesch.) | Jährlich (gesch.) | Quelle / Anmerkung |
|---------------|-------------------|-------------------|-------------------|
| Softwarelizenz (Basis) | 80–200 € | 960–2.400 € | Medizinio, Branchenvergleiche |
| Wartungsvertrag / Support | 50–150 € | 600–1.800 € | CGM Systemhaus Servicevertrag |
| Zusätzliche Module (KIM, ClickDoc etc.) | 45–260 € | 540–3.120 € | KIM: 14,90 €/Mon; ClickDoc: 29 €/Mon; AmbulApps: 229 €/Mon |
| Quartals-Updates / Versionen | inkl. Wartung | inkl. Wartung | Im Wartungsvertrag enthalten |
| Ersteinrichtung (einmalig, umgelegt) | ~80 € | ~960 € | Ca. 5.000–10.000 € einmalig, auf 5–10 Jahre verteilt |
| Schulung / Anpassungen | variabel | 500–1.000 € | Bei Versionsänderungen oder neuen Mitarbeitern |
| **GESCHÄTZTE GESAMTKOSTEN CGM** | | **3.560–11.280 €/Jahr** | |

## 2.2 TI-Infrastrukturkosten (unabhängig vom PVS)

Diese Kosten fallen unabhängig vom eingesetzten PVS an und werden größtenteils durch die TI-Pauschale der KV erstattet:

| Komponente | Kosten | KV-Erstattung | Anmerkung |
|-----------|--------|---------------|-----------|
| TI-Pauschale (bis 3 Ärzte) | — | 263,62 €/Monat | Automatische Auszahlung durch KV ab Q3/2025 |
| SMC-B (Praxisausweis) | ~200–400 €/5 Jahre | 23,25 €/Quartal | Pro BSNR |
| eHBA (Arztausweis) | ~300–500 €/5 Jahre | 11,63 €/Quartal | Pro Arzt |
| Konnektor (Austausch) | ~2.300 € (einmalig) | Voll erstattet | Durch Krankenkassen finanziert |
| KIM-Dienst | 14,90 €/Monat | Max. 23,40 €/Quartal | Pro Arzt, transaktionsbasiert |
| **JÄHRLICHE TI-ERSTATTUNG (3 Ärzte)** | | **~3.163 €/Jahr netto** | TI-Pauschale deckt Kosten weitgehend ab |

> **Wichtig:** Die TI-Infrastrukturkosten fallen bei jedem PVS an — egal ob CGM Medistar oder MediPrax. Die KV-Erstattung deckt diese Kosten größtenteils ab. Beim Kostenvergleich sind daher nur die PVS-spezifischen Kosten relevant.

## 2.3 Einmalige Entwicklungskosten MediPrax (Pilotphase)

| Kostenposition | Geschätzte Kosten | Anmerkung |
|---------------|-------------------|-----------|
| Entwickler (20 Monate, Vollzeit) | 80.000–120.000 € | 1 Entwickler, 4.000–6.000 €/Monat |
| Claude Code (API/Subscription) | 3.600–7.200 € | Max Team Plan: ~300 €/Monat × 20 Monate |
| gematik-Registrierung als Hersteller | **Zu klären** | Kosten nicht öffentlich; direkte Anfrage erforderlich |
| KBV-Zertifizierung (Prüfverfahren) | **Zu klären** | Kosten nicht öffentlich; direkte Anfrage erforderlich |
| gematik KOB (Konformitätsbewertung) | **Zu klären** | Pflicht seit 01.01.2026; Kosten anfragen |
| Testumgebung (Hardware) | 2.000–5.000 € | Testkonnektor, Testkarten, Testserver |
| Medizinrechtliche Beratung | 3.000–5.000 € | Datenschutz, Haftung, Zulassung (punktuell) |
| DSGVO-Gutachten / Datenschutzaudit | 2.000–4.000 € | Einmalig für Pilotbetrieb |
| GmbH-Gründung + Notar | 1.500–3.000 € | Stammkapital 25.000 € separat |
| Kontingenz (30%) | ~28.000–43.000 € | Für unvorhergesehene Kosten |
| **GESAMTE PILOTPHASE** | **120.000–190.000 €** | Ohne GmbH-Stammkapital und Zertifizierungsgebühren |

## 2.4 Laufende Kosten nach Go-Live (Pilotpraxis)

| Kostenposition | Monatlich | Jährlich | Anmerkung |
|---------------|----------|---------|-----------|
| Wartung & Weiterentwicklung | 1.000–2.000 € | 12.000–24.000 € | Entwickler in Teilzeit |
| Claude Code | 100–300 € | 1.200–3.600 € | Für laufende Anpassungen |
| Server-Hosting / Infrastruktur | 0 € | 0 € | Vorhandener Windows Server 2022 |
| **LAUFENDE GESAMTKOSTEN** | | **13.200–27.600 €/Jahr** | |

## 2.5 Vergleich: CGM Medistar vs. MediPrax

> **Ehrliche Einschätzung:** Die laufenden Kosten von MediPrax sind zunächst höher als die CGM-Lizenzkosten, da kontinuierliche Weiterentwicklung benötigt wird. MediPrax wird erst durch Skalierung auf weitere Praxen wirtschaftlich. Für die Pilotpraxis ist der Hauptvorteil die perfekte Anpassung an den eigenen Workflow, nicht die Kostenersparnis.

| Vergleich | CGM Medistar | MediPrax | Differenz |
|----------|-------------|----------|-----------|
| Jährliche Kosten | 3.560–11.280 € | 13.200–27.600 € | +9.640–16.320 €/Jahr (teurer) |
| Anpassungsfähigkeit | Eingeschränkt | Unbegrenzt | **Hauptvorteil** |
| Abhängigkeit | Vollständig von CGM | Eigenes IP, volle Kontrolle | Strategischer Vorteil |
| Kommerzialisierungspotenzial | Null | Unbegrenzt | Langfristiger Wert |
| **Break-Even** | — | **Ab 15–25 zahlenden Praxen** | Abhängig vom Preismodell |

## 2.6 Break-Even-Analyse

Annahmen: Durchschnittlicher Umsatz pro Praxis: 170 €/Monat. Laufende Kosten: ca. 3.000 €/Monat nach Skalierung.

| Anzahl Praxen | Monatlicher Umsatz | Monatliche Kosten | Ergebnis |
|--------------|-------------------|-------------------|---------|
| 5 Praxen | 850 € | 3.000 € | -2.150 € (Verlust) |
| 10 Praxen | 1.700 € | 3.000 € | -1.300 € (Verlust) |
| **18 Praxen** | **3.060 €** | **3.000 €** | **+60 € (Break-Even)** |
| 30 Praxen | 5.100 € | 3.500 € | +1.600 € (Gewinn) |
| 50 Praxen | 8.500 € | 4.500 € | +4.000 € (Gewinn) |
| 100 Praxen | 17.000 € | 7.000 € | +10.000 € (Gewinn) |

## Quellen

- [CGM Medistar (Medizinio)](https://medizinio.de/hersteller/cgm-medistar-praxissoftware)
- [CGM Systemhaus Preisliste](https://www.cgm-systemhaus.org/preisliste)
- [TI-Pauschale 2026 (Telekonnekt)](https://www.telekonnekt.de/artikel/ti-pauschale)
- [KV Baden-Württemberg TI-Pauschalen](https://www.kvbawue.de/praxis/unternehmen-praxis/it-online-dienste/telematikinfrastruktur-ti-e-health/pauschalen-auszahlung-sanktionen)
- [Virchow-Bund PVS-Vergleich](https://www.virchowbund.de/praxisaerzte-blog/praxissoftware-vergleich-das-sind-die-besten-anbieter-2024)
