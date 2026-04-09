# Warteliste

Die Warteliste verwaltet Patienten, die auf einen Therapieplatz oder Termin warten. Navigieren Sie zu **Termine** > **Warteliste** oder direkt zu `/warteliste`.

## Patient zur Warteliste hinzufuegen

1. Klicken Sie auf **Zur Warteliste hinzufuegen**.
2. Waehlen Sie den **Patienten** ueber die Suchfunktion.
3. Legen Sie die **Prioritaet** fest:
    - **Normal** -- Standardpriorisierung
    - **Dringend** -- Bevorzugte Vermittlung
4. Waehlen Sie optional einen **bevorzugten Therapeuten**.
5. Geben Sie die **bevorzugte Zeit** an:
    - **Vormittags** -- Termine am Vormittag
    - **Nachmittags** -- Termine am Nachmittag
6. Klicken Sie auf **Speichern**.

## Statusverlauf

Jeder Wartelisteneintrag durchlaeuft folgenden Workflow:

| Status | Beschreibung |
|---|---|
| **Wartend** | Patient steht auf der Warteliste |
| **Angeboten** | Dem Patienten wurde ein Termin angeboten |
| **Eingeplant** | Der Patient hat den Termin angenommen und wurde eingeplant |

Um den Status zu aendern, klicken Sie auf den aktuellen Status und waehlen den neuen Wert.

## Statistiken

Im oberen Bereich der Warteliste sehen Sie zusammenfassende Kennzahlen:

| Kennzahl | Beschreibung |
|---|---|
| **Gesamt wartend** | Anzahl der Patienten mit Status "Wartend" |
| **Durchschnittliche Wartezeit** | Mittlere Wartezeit in Tagen |
| **Dringende Faelle** | Anzahl der Eintraege mit Prioritaet "Dringend" |

## Filtern und Sortieren

Sie koennen die Warteliste filtern nach:

- **Status**: Wartend, Angeboten, Eingeplant
- **Therapeut**: Nur Patienten anzeigen, die einen bestimmten Therapeuten bevorzugen

!!! warning "Dringende Faelle"
    Eintraege mit Prioritaet **Dringend** werden optisch hervorgehoben. Pruefen Sie diese regelmaessig und bieten Sie zeitnah Termine an.

!!! tip "Tipp"
    Nutzen Sie die Warteliste, um den Ueberblick ueber die Therapieplatznachfrage zu behalten. Die Statistiken helfen bei der Kapazitaetsplanung.

!!! note "Hinweis"
    Die Warteliste ersetzt keine Terminvergabe. Sobald ein Patient eingeplant ist, erstellen Sie einen regulaeren [Termin](anlegen.md) oder eine [Terminserie](serien.md).
