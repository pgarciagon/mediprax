# Telematikinfrastruktur -- Uebersicht

Die Telematikinfrastruktur (TI) ist das sichere Netzwerk des deutschen Gesundheitswesens. MediPrax ist an die TI angebunden und nutzt deren Dienste fuer eGK-Lesung, E-Rezept, eAU und KIM-Kommunikation.

## TI-Komponenten

Fuer die TI-Anbindung sind folgende Komponenten in Ihrer Praxis erforderlich:

| Komponente | Beschreibung |
|---|---|
| **Konnektor** | Hardware-Gateway, das die Praxis mit der TI verbindet |
| **eGK-Kartenlesegeraet** | Liest die elektronische Gesundheitskarte der Patienten |
| **SMC-B-Karte** | Institutionskarte (Praxisausweis) zur Authentifizierung |
| **HBA** | Heilberufsausweis des Arztes (fuer qualifizierte elektronische Signatur) |

## Verbindungsstatus pruefen

Navigieren Sie zu **Telematik** > **Status** oder direkt zu `/telematik/status`, um den aktuellen TI-Status einzusehen:

| Statusanzeige | Bedeutung |
|---|---|
| **Gruen -- Verbunden** | Konnektor ist erreichbar, alle Dienste verfuegbar |
| **Gelb -- Eingeschraenkt** | Teilweise Verbindung, einige Dienste moeglicherweise nicht verfuegbar |
| **Rot -- Getrennt** | Keine Verbindung zum Konnektor |

## TI-Dienste in MediPrax

| Dienst | Beschreibung | Siehe |
|---|---|---|
| **eGK-Lesung** | Patientenstammdaten von der eGK lesen | [eGK](egk.md) |
| **E-Rezept** | Elektronische Rezepte erstellen und uebermitteln | [E-Rezept](erezept.md) |
| **KIM** | Sichere Kommunikation zwischen Leistungserbringern | [KIM](kim.md) |
| **eAU** | Elektronische Arbeitsunfaehigkeitsbescheinigung | [eAU](../formulare/eau.md) |

## Fehlerbehebung

| Problem | Moegliche Ursache | Massnahme |
|---|---|---|
| Status "Getrennt" | Konnektor nicht erreichbar | Netzwerkverbindung pruefen, Konnektor neu starten |
| eGK-Lesung fehlgeschlagen | Kartenlesegeraet nicht erkannt | USB-Verbindung pruefen, Geraet neu anschliessen |
| E-Rezept nicht moeglich | SMC-B oder HBA nicht gesteckt | Karten im Kartenlesegeraet pruefen |
| KIM-Versand fehlerhaft | KIM-Dienst nicht konfiguriert | Administrator kontaktieren |

!!! warning "SMC-B und HBA"
    Stellen Sie sicher, dass die **SMC-B-Karte** (Praxisausweis) und der **HBA** (Arztausweis) stets im Kartenlesegeraet gesteckt sind. Ohne diese Karten sind E-Rezept und eAU nicht moeglich.

!!! tip "Tipp"
    Pruefen Sie den TI-Status zu Beginn jedes Arbeitstages. So stellen Sie sicher, dass alle TI-Dienste verfuegbar sind, bevor der Praxisbetrieb beginnt.

!!! info "Hinweis"
    Bei TI-Stoerungen koennen Sie weiterhin mit MediPrax arbeiten. Nur die TI-spezifischen Funktionen (eGK, E-Rezept, KIM, eAU) sind dann eingeschraenkt.
