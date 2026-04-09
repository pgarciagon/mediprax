# Benutzerverwaltung

Die Benutzerverwaltung ermoeglicht Administratoren das Erstellen, Bearbeiten und Verwalten von Benutzerkonten. Navigieren Sie zu **Verwaltung** > **Benutzer**.

!!! warning "Nur fuer Administratoren"
    Die Benutzerverwaltung ist ausschliesslich fuer Benutzer mit der Rolle **Admin** zugaenglich.

## Benutzeruebersicht

Die Uebersicht zeigt alle Benutzerkonten mit:

| Spalte | Beschreibung |
|---|---|
| **Name** | Vor- und Nachname des Benutzers |
| **E-Mail** | Login-E-Mail-Adresse |
| **Rolle** | Zugewiesene Benutzerrolle |
| **Status** | Aktiv oder Deaktiviert |
| **Letzte Anmeldung** | Datum der letzten Anmeldung |

## Neuen Benutzer anlegen

1. Klicken Sie auf **Neuer Benutzer**.
2. Fuellen Sie die Felder aus:

| Feld | Beschreibung | Pflicht |
|---|---|---|
| **Vorname** | Vorname des Benutzers | Ja |
| **Nachname** | Nachname des Benutzers | Ja |
| **E-Mail** | E-Mail-Adresse (wird als Login verwendet) | Ja |
| **Passwort** | Initiales Passwort | Ja |
| **Rolle** | Admin, Arzt, MFA oder Empfang | Ja |

3. Klicken Sie auf **Speichern**.
4. Teilen Sie dem neuen Benutzer die Zugangsdaten mit.

## Benutzerrollen

| Rolle | Berechtigungen |
|---|---|
| **Admin** | Vollzugriff: Alle Funktionen inkl. Benutzerverwaltung, KVDT-Export, Systemkonfiguration |
| **Arzt** | Klinische Funktionen: Dokumentation, Abrechnung, Formulare, PsychKG, Medikation |
| **MFA** | Organisatorische Funktionen: Termine, Patientenaufnahme, Rezeptdruck, Formulare |
| **Empfang** | Basisfunktionen: Terminvergabe, Wartezimmer, eGK-Lesung |

## Benutzer bearbeiten

1. Klicken Sie auf den gewuenschten Benutzer in der Liste.
2. Aendern Sie die gewuenschten Felder (Name, E-Mail, Rolle).
3. Klicken Sie auf **Speichern**.

## Benutzer deaktivieren / aktivieren

Statt Benutzer zu loeschen, koennen sie deaktiviert werden:

1. Oeffnen Sie den Benutzer.
2. Klicken Sie auf **Deaktivieren** (bzw. **Aktivieren**).
3. Deaktivierte Benutzer koennen sich nicht mehr anmelden.

!!! info "Deaktivierung statt Loeschung"
    Benutzer werden nicht geloescht, sondern deaktiviert. So bleiben alle historischen Zuordnungen (dokumentierte Kontakte, erstellte Rezepte, Audit-Eintraege) erhalten.

## Passwort zuruecksetzen

1. Oeffnen Sie den Benutzer.
2. Klicken Sie auf **Passwort zuruecksetzen**.
3. Vergeben Sie ein neues temporaeres Passwort.
4. Der Benutzer sollte das Passwort bei der naechsten [Anmeldung](../erste-schritte/anmeldung.md) aendern.

!!! tip "Tipp"
    Legen Sie fuer jeden Mitarbeiter einen eigenen Benutzer an. Gemeinsam genutzte Konten erschweren die Nachvollziehbarkeit im [Audit-Protokoll](audit.md) und verstoessen gegen die DSGVO.
