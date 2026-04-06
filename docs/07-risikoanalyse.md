# 7. Risikoanalyse

## 7.1 Risikomatrix

| Risiko | W. | A. | Maßnahme |
|--------|---|----|---------|
| Zertifizierungskosten unerwartet hoch | Mittel | Hoch | Sofort gematik + KBV kontaktieren; Budget-Reserve |
| KBV-/KOB-Zertifizierung abgelehnt | Mittel | Kritisch | Früh gematik-Referenzumgebung nutzen, iterativ testen |
| TI-Komplexität unterschätzt | Hoch | Hoch | TI-Berater für Phase 2 einplanen |
| Medistar-Daten nicht exportierbar | Mittel | Hoch | Export-Möglichkeiten in Phase 1 analysieren |
| Claude Code Halluzinationen | Mittel | Hoch | Strikte Tests, ärztliche Validierung, Code-Reviews |
| Ein-Personen-Risiko (Bus-Faktor) | Hoch | Kritisch | Dokumentation, Clean Code, Ärzte als Fachtester |
| Kein Markt für Kommerzialisierung | Mittel | Hoch | Marktanalyse in Phase 1; Fokus erst auf Pilotpraxis |
| Regulatorische Änderungen | Mittel | Hoch | gematik-/KBV-Newsletter abonnieren, flexible Architektur |

**Legende:** W. = Wahrscheinlichkeit, A. = Auswirkung

## 7.2 Spezifische Risiken bei Claude-Code-Entwicklung

### Halluzinationen

Claude Code kann plausibel aussehenden, aber fehlerhaften Code generieren. Bei medizinischer Software ist dies besonders gefährlich.

**Maßnahme:** Jede Funktion muss durch Unit-Tests und manuelle Überprüfung validiert werden. Besonders kritisch: Abrechnungslogik, Medikamenten-Interaktionen, Kryptographie.

### Abhängigkeit von einem Anbieter

Claude Code ist ein Produkt von Anthropic. Änderungen an Preisen, Verfügbarkeit oder Funktionen könnten das Projekt beeinflussen.

**Maßnahme:** Saubere Codestruktur, die auch ohne KI wartbar ist. Claude Code ist ein Produktivitätstool, keine Abhängigkeit.

### Kontextfenster-Grenzen

Bei sehr großen Codebasen kann Claude Code den Überblick verlieren.

**Maßnahme:** Modulare Architektur mit klaren Grenzen zwischen Modulen. Jedes Modul sollte eigenständig verständlich sein.

### Datenschutz bei Code-Generierung

Patientendaten dürfen niemals an Claude Code übermittelt werden.

**Maßnahme:** Ausschließlich Testdaten verwenden. Produktivdaten bleiben lokal. Keine echten Patientendaten in Prompts, Logs oder Code-Kommentaren.

## 7.3 Sofortige nächste Schritte

Diese Schritte sollten innerhalb der nächsten 2–4 Wochen erfolgen:

1. **gematik kontaktieren:** Registrierung als Primärsystem-Hersteller, KOB-Kosten erfragen, Zugang zur Referenzumgebung beantragen.
2. **KBV kontaktieren:** Zertifizierungsprozess und -kosten erfragen, aktuelle Prüfpakete (KVDT) beziehen.
3. **CGM-Vertrag prüfen:** Aktuelle Vertragskosten dokumentieren, Kündigungsfristen prüfen, Datenexport-Möglichkeiten klären.
4. **GmbH-Gründung vorbereiten:** Gesellschaftsvertrag entwerfen, Notar konsultieren, Rollen und Anteile definieren.
5. **Medizinrechtliche Beratung:** Haftungsfragen, Datenschutz, Zulassungsanforderungen für Bremen klären.
6. **Claude Code einrichten:** Entwicklungsumgebung konfigurieren, Repository erstellen, ersten Prototyp bauen.
7. **Praxis-Workshop:** Alle aktuellen Workflows mit dem Ärzteteam dokumentieren, Pain Points mit Medistar erfassen, MVP-Anforderungen priorisieren.

> **Höchste Priorität:** Die wichtigsten unbekannten Variablen sind die Zertifizierungskosten (gematik KOB + KBV). Erst nach deren Klärung kann ein belastbares Gesamtbudget erstellt werden.
