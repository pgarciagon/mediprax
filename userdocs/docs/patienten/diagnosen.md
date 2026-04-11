# Diagnosenverwaltung

## Überblick

Jeder Patient verfügt über eine eigene **Diagnosenliste**, in der alle ICD-10-Diagnosen mit Metadaten verwaltet werden. Diagnosen werden unterschieden in:

- **Dauerdiagnosen** — chronische oder langfristige Diagnosen, die automatisch in jede neue Konsultation übernommen werden
- **Encounterdiagnosen** — einmalige oder situationsbezogene Diagnosen

## Diagnosen aufrufen

1. Öffnen Sie die Patientenakte
2. Im Abschnitt **Diagnosen** sehen Sie die aktiven Dauerdiagnosen als farbige Chips
3. Klicken Sie auf **„Alle verwalten"** für die vollständige Diagnosenverwaltung

## Neue Diagnose hinzufügen

1. Klicken Sie auf **„+ Neue Diagnose"**
2. Geben Sie einen ICD-10-Code oder Suchbegriff ein (z.B. „F32" oder „Depression")
3. Wählen Sie den passenden Code aus der Vorschlagsliste
4. Konfigurieren Sie die Metadaten:

| Feld | Optionen | Bedeutung |
|------|----------|-----------|
| **Sicherheit** | G, V, Z, A | G=Gesichert, V=Verdacht, Z=Zustand nach, A=Ausschluss |
| **Seitenlokalisation** | R, L, B | R=Rechts, L=Links, B=Beidseitig (optional) |
| **Typ** | Dauerdiagnose, Encounterdiagnose | Bestimmt, ob die Diagnose automatisch vererbt wird |
| **Status** | Aktiv, Anamnestisch, Inaktiv | Nur aktive Diagnosen werden vererbt |

5. Klicken Sie auf **„Speichern"**

## Diagnosesicherheit (KBV-konform)

Die Sicherheitskennzeichen entsprechen den KBV-Vorgaben für die KVDT-Abrechnung:

- **G** (grün) — Gesicherte Diagnose
- **V** (gelb) — Verdachtsdiagnose
- **Z** (violett) — Zustand nach (frühere, nicht mehr bestehende Erkrankung)
- **A** (grau) — Ausschlussdiagnose

## Dauerdiagnosen-Vererbung

Wenn Sie eine **neue Konsultation** für einen Patienten anlegen, werden alle aktiven Dauerdiagnosen automatisch als anklickbare Chips angezeigt:

- Standardmäßig sind alle Dauerdiagnosen **aktiviert** (Häkchen gesetzt)
- Entfernen Sie das Häkchen bei Diagnosen, die für diese Konsultation nicht relevant sind
- Die ausgewählten Diagnosen werden mit der Konsultation verknüpft

## Arztbrief-Integration

Beim Erstellen eines neuen Arztbriefs werden die **aktiven Dauerdiagnosen** automatisch in das Diagnosen-Feld übernommen. Im Brieftext erscheinen die Diagnosen mit Sicherheitskennzeichen und ggf. Seitenlokalisation.

## KVDT-Export

Bei der Quartalsabrechnung werden die strukturierten Diagnosedaten automatisch in das KVDT-Format exportiert:

- Feld `6001` — ICD-10-Code
- Feld `6003` — Diagnosesicherheit (G/V/Z/A)
- Feld `6004` — Seitenlokalisation (R/L/B), falls angegeben

## Aktionen

| Aktion | Beschreibung |
|--------|-------------|
| **Bearbeiten** | Sicherheit, Seitenlokalisation, Typ oder Status ändern |
| **Zu Dauerdiagnose hochstufen** | Encounterdiagnose → Dauerdiagnose umwandeln |
| **Deaktivieren** | Status auf „Inaktiv" setzen (Diagnose bleibt in der Historie) |
| **Löschen** | Diagnose endgültig entfernen |

!!! tip "Automatische Hochstufung"
    Bei der Migration von Altdaten werden Diagnosen, die in 3 oder mehr Konsultationen vorkommen, automatisch als Dauerdiagnose vorgeschlagen.
