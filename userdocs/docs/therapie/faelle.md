# Therapiefaelle

Die Therapiefallverwaltung bildet den gesamten Therapieverlauf ab -- von der Sprechstunde ueber die Antragstellung bis zum Abschluss der bewilligten Sitzungen. Sie finden Therapiefaelle in der [Patientenakte](../patienten/akte.md) im Bereich **Therapiefaelle**.

## Therapiefall erstellen

1. Oeffnen Sie die Patientenakte.
2. Navigieren Sie zu **Therapiefaelle**.
3. Klicken Sie auf **Neuer Therapiefall**.
4. Waehlen Sie die **Therapieart**:

| Therapieart | Abkuerzung | Beschreibung |
|---|---|---|
| Verhaltenstherapie | VT | Kognitive Verhaltenstherapie |
| Tiefenpsychologisch fundierte Psychotherapie | TP | Psychodynamische Therapie |
| Analytische Psychotherapie | AP | Langzeit-Psychoanalyse |
| Systemische Therapie | ST | Systemische Einzeltherapie |

5. Legen Sie die **Sitzungsdauer** fest (25 oder 50 Minuten).
6. Geben Sie an, ob es sich um **Gruppentherapie** handelt.
7. Klicken Sie auf **Speichern**.

## Status-Workflow

Ein Therapiefall durchlaeuft folgende Phasen:

| Status | Beschreibung |
|---|---|
| **Sprechstunde** | Psychotherapeutische Sprechstunde (Erstgespraech) |
| **Probatorik** | Probatorische Sitzungen (max. 2-4) |
| **Antrag** | Antrag bei der Krankenkasse gestellt |
| **Gutachter** | Antrag beim Gutachter zur Pruefung |
| **Bewilligt** | Therapie von der Krankenkasse bewilligt |
| **In Behandlung** | Therapie laeuft, Sitzungen werden durchgefuehrt |
| **Abgeschlossen** | Alle bewilligten Sitzungen absolviert oder Therapie beendet |

Aendern Sie den Status ueber die Statusauswahl im Therapiefall.

## Sitzungsverwaltung

Nach Bewilligung erfassen Sie die bewilligten Sitzungen:

- **Bewilligte Sitzungen**: Anzahl der von der Krankenkasse genehmigten Sitzungen
- **Absolvierte Sitzungen**: Wird automatisch gezaehlt

Der aktuelle Stand wird als **"Sitzung X von Y"** angezeigt (z. B. "Sitzung 12 von 24").

## Gutachterverfahren

Bei Langzeittherapien ist ein Gutachterverfahren erforderlich:

| Feld | Beschreibung |
|---|---|
| **Gutachter-Status** | Ausstehend, Eingereicht, Bewilligt, Abgelehnt |
| **PTV-Formulare** | Verknuepfte [PTV-Dokumente](ptv.md) (PTV3, PTV10) |

## Verlaengerung

Wenn die bewilligten Sitzungen zur Neige gehen:

1. Aendern Sie den Status auf **Antrag** (Verlaengerung).
2. Erstellen Sie ein [PTV12-Formular](ptv.md).
3. Nach Bewilligung aktualisieren Sie die bewilligte Sitzungszahl.

!!! tip "Tipp"
    Behalten Sie den Sitzungszaehler im Blick. Planen Sie die Verlaengerung rechtzeitig (ca. 5 Sitzungen vor Ende), um Unterbrechungen zu vermeiden.

!!! warning "Wichtig"
    Die Sitzungsdokumentation ist abrechnungsrelevant. Jede Therapiesitzung muss als [Kontakt](../dokumentation/kontakte.md) mit der entsprechenden GOP dokumentiert werden.

!!! info "Hinweis"
    Fuer Videosprechstunden gelten besondere Regeln. Siehe [Videosprechstunde](../geraete/video.md) fuer KBV-Vorgaben zur Psychotherapie per Video.
