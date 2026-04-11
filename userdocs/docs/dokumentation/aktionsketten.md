# Aktionsketten

## Überblick

**Aktionsketten** sind vordefinierte Abläufe, die mehrere klinische Aktionen mit einem einzigen Kürzel ausführen. Statt manuell Diagnose, Abrechnungsziffer, Befundvorlage und Wiedervorlage einzeln hinzuzufügen, genügt ein Kürzel wie `#dep` — alle Schritte werden automatisch ausgeführt.

**Zeitersparnis:** 2-3 Minuten pro Patient, bei 40-60 Patienten/Tag = 1-2 Stunden täglich.

## Verfügbare Aktionsketten

Folgende Ketten sind vorinstalliert:

| Kürzel | Titel | Schritte |
|--------|-------|----------|
| `#dep` | Depression Standard | F32.1 + GOP 21220 + Psych. Befundvorlage |
| `#angst` | Angststörung | F41.1 + GOP 21220 + Psych. Befundvorlage |
| `#epi` | Epilepsie Kontrolle | G40.9 + GOP 16220 + EEG (16311) + Neuro. Vorlage |
| `#erstgespraech` | Psychiatrisches Erstgespräch | GOP 21210 + 21220 + Psych. Vorlage + 50 min |
| `#schmerz` | Chronischer Schmerz | R52.2 + GOP 16220 + Neuro. Vorlage |
| `#demenz` | Demenz Kontrolle | F00.1 + GOP 21220 + Psych. Vorlage + Wiedervorlage 6 Monate |
| `#park` | Parkinson Kontrolle | G20 + GOP 16220 + Neuro. Vorlage |
| `#ms` | Multiple Sklerose | G35 + GOP 16220 + Neuro. Vorlage + Wiedervorlage 3 Monate |

## Anwendung in der Konsultation

1. Öffnen Sie eine **bestehende Konsultation** (Bearbeiten-Modus)
2. Im Abschnitt **Aktionskette** geben Sie `#` gefolgt vom Kürzel ein, z.B. `#dep`
3. Wählen Sie die gewünschte Kette aus der Vorschlagsliste
4. Drücken Sie **Enter** oder klicken Sie auf den Eintrag
5. Die Aktionskette wird ausgeführt und ein Ergebnisbanner zeigt, was passiert ist

!!! tip "Schnellauswahl"
    Bei nur einem Suchergebnis können Sie direkt **Enter** drücken, um die Kette auszuführen.

## Was eine Aktionskette tun kann

Jede Kette besteht aus beliebig vielen Schritten der folgenden Typen:

| Schritttyp | Aktion |
|-----------|--------|
| **Diagnose hinzufügen** | ICD-10-Code mit Sicherheit (G/V/Z/A) und Typ (Dauer/Encounter) |
| **Abrechnungsziffer** | GOP-Code (EBM/GOÄ) mit Menge |
| **Befundvorlage** | ABDTP-Sektionen mit Vorlage vorausfüllen |
| **Rezept erstellen** | Entwurf-Rezept mit Medikament und Dosierung |
| **Dauer setzen** | Konsultationsdauer in Minuten |
| **Wiedervorlage** | Automatische Wiedervorlage nach X Tagen |

## Eigene Aktionsketten erstellen

1. Navigieren Sie zu **Verwaltung > Aktionsketten**
2. Klicken Sie auf **+ Neue Aktionskette**
3. Vergeben Sie ein **Kürzel** (z.B. `kopf`) und einen **Titel**
4. Wählen Sie eine **Kategorie** (Psychiatrie, Neurologie, Allgemein)
5. Fügen Sie **Schritte** hinzu und konfigurieren Sie diese
6. Speichern

!!! info "Global vs. persönlich"
    Aktivieren Sie **"Für alle Benutzer sichtbar"**, damit alle Ärzte die Kette nutzen können. Deaktivieren Sie die Option für persönliche Ketten.

## Aktionsketten verwalten

Auf der Seite **Verwaltung > Aktionsketten** können Sie:

- Alle Ketten nach Kategorie filtern
- Bestehende Ketten **bearbeiten** (Schritte hinzufügen/entfernen)
- Ketten **deaktivieren** (ohne Löschen, jederzeit reaktivierbar)
- Ketten **löschen**
