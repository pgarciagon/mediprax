# Patient anlegen und bearbeiten

Hier erfahren Sie, wie Sie einen neuen Patienten in MediPrax erfassen oder bestehende Patientendaten bearbeiten.

## Neuen Patienten anlegen

1. Navigieren Sie zu **Patienten** in der Seitenleiste.
2. Klicken Sie auf **Neuer Patient**.
3. Fuellen Sie die Pflichtfelder aus (siehe unten).
4. Ergaenzen Sie optionale Angaben nach Bedarf.
5. Klicken Sie auf **Speichern**.

## Pflichtfelder

| Feld | Beschreibung | Beispiel |
|---|---|---|
| **Vorname** | Vorname des Patienten | Maria |
| **Nachname** | Nachname des Patienten | Mueller |
| **Geburtsdatum** | Geburtsdatum (TT.MM.JJJJ) | 15.03.1980 |

## Optionale Felder

| Feld | Beschreibung |
|---|---|
| **Geschlecht** | Maennlich, Weiblich, Divers |
| **Versicherungstyp** | GKV (gesetzlich) oder PKV (privat) |
| **Versicherungsnummer** | Versichertennummer der Krankenkasse |
| **KVNR** | Krankenversichertennummer (10-stellig) |
| **Strasse** | Strasse und Hausnummer |
| **PLZ / Ort** | Postleitzahl und Wohnort |
| **Telefon** | Telefonnummer |
| **E-Mail** | E-Mail-Adresse |

## Betreuer / Gesetzlicher Vertreter

Falls der Patient unter gesetzlicher Betreuung steht, koennen Sie die Betreuerangaben erfassen:

| Feld | Beschreibung |
|---|---|
| **Betreuer-Name** | Name des gesetzlichen Betreuers |
| **Betreuer-Telefon** | Telefonnummer des Betreuers |
| **Betreuer-Adresse** | Anschrift des Betreuers |
| **Betreuungsumfang** | Art der Betreuung (z. B. Gesundheitssorge, Aufenthaltsbestimmung) |

!!! info "Hinweis"
    Die Betreuerinformationen sind besonders in der Psychiatrie relevant, wenn Patienten einwilligungsunfaehig sind oder eine gesetzliche Betreuung besteht.

## Patienten bearbeiten

1. Oeffnen Sie die [Patientenakte](akte.md) des gewuenschten Patienten.
2. Klicken Sie auf **Bearbeiten** bei den Stammdaten.
3. Aendern Sie die gewuenschten Felder.
4. Klicken Sie auf **Speichern**.

!!! warning "Wichtig"
    Aenderungen an Stammdaten werden im [Audit-Protokoll](../verwaltung/audit.md) protokolliert. Stellen Sie sicher, dass die eingegebenen Daten korrekt sind.

!!! tip "Tipp"
    Wenn Sie eine eGK einlesen, werden die Patientenstammdaten automatisch befuellt. Siehe [eGK-Lesung](../telematik/egk.md).
