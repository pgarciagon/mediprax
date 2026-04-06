# 8. UX-Analyse: CGM MEDISTAR

> **Zweck:** Analyse der Benutzeroberfläche des Konkurrenzprodukts CGM MEDISTAR, um Schwächen zu identifizieren und MediPrax gezielt besser zu gestalten.

## 8.1 Übersicht CGM MEDISTAR

CGM MEDISTAR ist laut KBV-Installationsstatistiken die meistinstallierte Praxissoftware in Deutschland. Sie wird von CompuGroup Medical (CGM) entwickelt und ist modular aufgebaut — Praxen können aus einer Vielzahl von Modulen und Add-Ons wählen.

Seit einigen Jahren bietet CGM eine modernisierte Oberfläche namens **"MEDISTAR BLACK"** an, die als alternativer "Skin" über das bestehende System gelegt wird. Die darunterliegende Software bleibt identisch.

## 8.2 Benutzeroberfläche — Zwei Gesichter

### Klassische Oberfläche (Legacy)

- **Stil:** Windows-95/2000-Ästhetik mit beige/braunen Karteireitern
- **Layout:** Karteikarten-Metapher — Patientendaten werden wie physische Karteikarten dargestellt
- **Navigation:** Komplexe Menüstrukturen, viele verschachtelte Dialoge
- **Farbschema:** Beige, Braun, Grau — wirkt veraltet
- **Terminkalender:** Tabellarische Rasteransicht mit farbcodierten Blöcken (Rot, Gelb, Grün, Blau) für verschiedene Termintypen
- **Typografie:** Kleine Schriftgrößen, geringe Lesbarkeit

### MEDISTAR BLACK (Modernisiert)

- **Stil:** Dunkles Theme mit modernerem Look, aber gleiche Grundstruktur
- **Header:** Dunkle Titelleiste "CGM MEDISTAR BLACK" mit goldenen/gelben Akzenten
- **Tabs:** Oberer Bereich mit Reitern wie "PATIENT", "Spielwiese", "KRANKENKASSE"
- **Patientendaten:** Formularbasiert mit Seitennavigation (links: Abschnitte wie "Allgemein", "Stammdaten", "Versicherung", "Adressdaten", etc.)
- **Felder:** Standard-Windows-Formularfelder (Textfelder, Dropdowns)
- **Widgets:** Konfigurierbares Dashboard mit Kacheln (Allergien, CAVE-Hinweise, Laborwerte, Timer)
- **Problem:** BLACK ist nur ein optisches Update — die UX-Probleme der Grundstruktur bleiben bestehen

## 8.3 Hauptmodule und ihre Oberfläche

### Patientenverwaltung (Stammdaten)

- Formularbasiert mit vielen Feldern auf einer Seite
- Linke Seitenleiste mit Navigationsabschnitten
- Felder: Name, Vorname, Geburtsdatum, Adresse, Telefon, E-Mail
- Versicherungsdaten auf separatem Tab ("KRANKENKASSE")
- **Schwäche:** Informationsüberflutung — zu viele Felder gleichzeitig sichtbar

### Terminkalender

- Rasteransicht mit Zeitslots (typisch 5-15 Minuten)
- Farbcodierung: verschiedene Farben für Termintypen, Ärzte, Status
- Tages- und Wochenansicht verfügbar
- Freie Slots visuell erkennbar
- **Schwäche:** Starre Rasterdarstellung, wenig flexibel, keine moderne Drag-and-Drop-Interaktion

### Abrechnung

- EBM-GOPs werden pro Behandlungsfall erfasst
- KVDT-Export für die Kassenärztliche Vereinigung
- Plausibilitätsprüfung integriert
- **Schwäche:** Komplex und unübersichtlich für neue Nutzer

### Dokumentation

- Freitext-Befunddokumentation
- ICD-10-Kodierung
- Abschnittsbeschreibungen: Anamnese, Fragestellung, Befunde, Besondere Hinweise (Cave), Therapie/Behandlungsempfehlungen
- **Schwäche:** Vorlagenbasiert, aber wenig flexibel

### Verordnung / E-Rezept

- Medikamentenliste mit PZN (Pharmazentralnummer)
- Restmengenberechnung integriert
- BtM-Dokumentation
- **Schwäche:** E-Rezept als nachträgliche Integration, nicht nativ

### Widgets / Dashboard

- Konfigurierbare Kacheln auf dem Startbildschirm
- Widget-Store für zusätzliche Widgets
- Allergien/CAVE-Widget, Laborwerte, Timer, Nachrichten
- **Stärke:** Guter Ansatz für personalisierte Übersicht
- **Schwäche:** Widgets wirken aufgesetzt, nicht nahtlos integriert

## 8.4 Identifizierte UX-Schwächen

### 1. Legacy-Architektur sichtbar
MEDISTAR BLACK ist ein kosmetisches Update einer jahrzehntealten Architektur. Die Navigation, Dialogstruktur und Interaktionsmuster stammen aus den 1990er-Jahren. Moderne UX-Konzepte (Responsive Design, kontextbasierte Navigation, Inline-Editing) fehlen.

### 2. Informationsüberflutung
Zu viele Felder und Optionen sind gleichzeitig sichtbar. Es gibt keine progressive Disclosure — alles wird auf einmal angezeigt, unabhängig vom aktuellen Arbeitskontext.

### 3. Inkonsistente Navigation
Die Mischung aus Menüs, Tabs, Seitenleisten und Dialogfenstern schafft keine konsistente Navigationserfahrung. Nutzer müssen sich merken, wo Funktionen versteckt sind.

### 4. Starre Workflows
Die Software zwingt Nutzer in vorgegebene Abläufe, statt sich dem natürlichen Praxisworkflow anzupassen. Für eine Gemeinschaftspraxis mit Kurzterminen (5-10 Minuten) ist jeder unnötige Klick ein Produktivitätsverlust.

### 5. Modulare Komplexität
Die hohe Modularität führt zu einer fragmentierten Erfahrung. Module fühlen sich wie separate Anwendungen an, nicht wie ein integriertes System.

### 6. Veraltete Terminplanung
Der Terminkalender nutzt eine starre Rasterdarstellung ohne moderne Interaktionsmöglichkeiten wie Drag-and-Drop, Schnelltermine oder intelligente Vorschläge.

### 7. Kein modernes Responsive Design
Die Oberfläche ist für feste Bildschirmgrößen optimiert. Es gibt keine Anpassung an verschiedene Monitore oder Auflösungen.

## 8.5 MediPrax UX-Differenzierung

Basierend auf den identifizierten Schwächen sollte MediPrax folgende UX-Prinzipien verfolgen:

| MEDISTAR-Schwäche | MediPrax-Ansatz |
|-------------------|----------------|
| Legacy-Look trotz BLACK-Skin | Nativ modernes UI mit Blazor — kein Skin über altes System |
| Informationsüberflutung | Progressive Disclosure — nur relevante Informationen im aktuellen Kontext |
| Inkonsistente Navigation | Einheitliche Sidebar-Navigation mit klarer Hierarchie |
| Starre Workflows | Workflow-optimiert für Kurztermine (5-10 Min) — minimal Klicks |
| Fragmentierte Module | Ein integriertes System — nahtlose Übergänge zwischen Funktionen |
| Starrer Terminkalender | Moderner Kalender mit Drag-and-Drop, Schnellterminen, Wartezimmer-View |
| Kein Responsive Design | Blazor Server mit responsivem CSS — funktioniert auf jedem Bildschirm |
| Widgets als Aufsetzer | Dashboard als zentraler Einstiegspunkt, nativ integriert |
| Komplexe Abrechnung | Geführte Abrechnungsworkflows mit Plausibilitätsprüfung in Echtzeit |
| E-Rezept nachträglich | E-Rezept nativ in den Verordnungsworkflow integriert |

## 8.6 UX-Designprinzipien für MediPrax

1. **Workflow-First:** Die UI folgt dem tatsächlichen Praxisablauf (Empfang → Konsultation → Verordnung → Dokumentation → Abrechnung)
2. **Minimal Clicks:** Jede häufige Aktion muss in maximal 2-3 Klicks erreichbar sein
3. **Kontext-Sensitivität:** Die Oberfläche zeigt nur das, was im aktuellen Arbeitsschritt relevant ist
4. **Schnellsuche überall:** Globale Suche für Patienten, Diagnosen, Medikamente, GOPs
5. **Tastaturfreundlich:** Wichtige Aktionen über Tastenkürzel erreichbar (Ärzte tippen schnell)
6. **Klare Typografie:** Gut lesbare Schriftgrößen, ausreichend Kontrast, Platz zwischen Elementen
7. **Farbsystem mit Bedeutung:** Farben für Status (Wartezimmer, In Behandlung, Fertig), Termintypen, Dringlichkeit
8. **Sofortiges Feedback:** Speichern, Validierung und Statusänderungen sofort sichtbar
9. **Mobile-Ready:** Responsive Design für verschiedene Bildschirmgrößen (Empfangstresen vs. Arztzimmer)
10. **Barrierefreiheit:** WCAG-konform, auch für ältere Ärzte mit eingeschränkter Sicht nutzbar

## 8.7 Quellen

- [CGM MEDISTAR — Offizielle Produktseite](https://www.cgm.com/deu_de/loesungen/praxissoftware/healthcare-software/cgm-medistar.html)
- [CGM MEDISTAR Kundenbereich](https://www.cgm.com/deu_de/loesungen/praxissoftware/cgm-medistar-kundenseite.html)
- [CGM Knowledge Base — Handbuch](https://support.cgm.com/CGM_MEDISTAR/Handbuch_(Version_404.115.....))
- [CGM MEDISTAR Module — TPS Eisleben](https://www.tps-eisleben.de/praxissoftware/arztinformationssystem-medistar/module)
- [CGM Systemhaus — Videos MEDISTAR](https://www.cgm-systemhaus.org/kim-videos)
- [MediStar Black Tutorial | Stammdaten — YouTube (Tilman Kappe)](https://www.youtube.com/results?search_query=MediStar+Black+Tutorial+Stammdaten)
- [CGM MEDISTAR Terminkalender — CGM Systemhaus](https://www.cgm-systemhaus.org/training-details-reader?tid=6185)
- [MEDISTAR — Kanzlei und Praxis Computer](https://www.kanzleiundpraxis.com/medistar)
- [medxsmart: CGM MEDISTAR](https://www.medxsmart.de/product/cgm-medistar/)
