# Textbausteine

Textbausteine sind wiederverwendbare Textvorlagen, die Ihnen die Dokumentation erheblich erleichtern. Sie verwalten Textbausteine unter **Verwaltung** > **Textbausteine** oder direkt unter `/verwaltung/textbausteine`.

## Uebersicht

Textbausteine koennen in der gesamten klinischen Dokumentation verwendet werden -- in Kontaktnotizen, Befunden und Arztbriefen. Jeder Baustein hat ein **Kuerzel** (mit #-Praefix), das als Schnellzugriff dient.

## Textbaustein erstellen

1. Navigieren Sie zu **Verwaltung** > **Textbausteine**.
2. Klicken Sie auf **Neuer Textbaustein**.
3. Fuellen Sie die Felder aus:

| Feld | Beschreibung | Beispiel |
|---|---|---|
| **Kuerzel** | Schnellzugriff mit #-Praefix | `#normalbefund` |
| **Kategorie** | Zuordnung zur Fachkategorie | Psychopathologie |
| **Titel** | Beschreibender Name | Unauffaelliger psychopathologischer Befund |
| **Text** | Der eigentliche Textinhalt | Bewusstseinsklar, allseits orientiert... |
| **Typ** | Global oder Persoenlich | Global |

4. Klicken Sie auf **Speichern**.

## Kategorien

| Kategorie | Typische Verwendung |
|---|---|
| **Psychopathologie** | Standardbefunde, typische Befundkonstellationen |
| **Neurologie** | Neurologische Normalbefunde, haeufige Befundmuster |
| **Arztbrief** | Standardtexte fuer Therapieempfehlungen, Einleitungen |

## Variablen

Textbausteine koennen **Variablen** enthalten, die beim Einfuegen automatisch mit Patientendaten ersetzt werden:

| Variable | Wird ersetzt durch | Beispiel |
|---|---|---|
| `{Patient.Name}` | Vollstaendiger Patientenname | Maria Mueller |
| `{Patient.Geburtsdatum}` | Geburtsdatum | 15.03.1980 |
| `{Patient.Alter}` | Berechnetes Alter | 46 Jahre |
| `{Datum}` | Aktuelles Datum | 09.04.2026 |

**Beispiel-Textbaustein:**

> Ich berichte ueber {Patient.Name}, geb. am {Patient.Geburtsdatum} ({Patient.Alter}), der/die sich am {Datum} in meiner Sprechstunde vorstellte.

## Global vs. Persoenlich

| Typ | Sichtbarkeit | Verwendung |
|---|---|---|
| **Global** | Fuer alle Benutzer sichtbar | Praxisweite Standardtexte |
| **Persoenlich** | Nur fuer den Ersteller | Individuelle Dokumentationsvorlieben |

## Textbaustein verwenden

In jedem Freitextfeld koennen Sie Textbausteine einfuegen:

1. Tippen Sie das **#-Kuerzel** (z. B. `#normalbefund`).
2. Eine Vorschlagsliste erscheint.
3. Waehlen Sie den gewuenschten Baustein aus.
4. Der Text wird eingefuegt und Variablen automatisch aufgeloest.

!!! tip "Tipp"
    Legen Sie fuer haeufig verwendete Befundtexte Kuerzel mit kurzen, einpraegsamen Namen an (z. B. `#psyunauff`, `#neurounauff`). Das spart erheblich Zeit bei der taeglichen Dokumentation.

!!! info "Hinweis"
    Globale Textbausteine koennen nur von Nutzern mit der Rolle **Admin** oder **Arzt** erstellt und bearbeitet werden.
