# Wartezimmer

Die Wartezimmerverwaltung zeigt den aktuellen Status aller heutigen Termine in Echtzeit. Navigieren Sie zu **Wartezimmer** in der Seitenleiste oder direkt zu `/wartezimmer`.

## Uebersicht

Die Ansicht gruppiert alle heutigen Termine nach ihrem aktuellen Status:

| Status | Beschreibung | Aktion |
|---|---|---|
| **In Behandlung** | Patient wird gerade behandelt | Fertig |
| **Im Wartezimmer** | Patient ist eingetroffen und wartet | Aufrufen |
| **Erwartet** | Termin geplant, Patient noch nicht da | Einchecken |
| **Fertig** | Behandlung abgeschlossen | -- |

## Arztfilter

Das Wartezimmer bietet einen **Arztfilter** als Dropdown oberhalb der Terminliste:

- **Aerzte**: Der Filter ist standardmaessig auf den **eigenen Namen** gesetzt -- Sie sehen nur Ihre Patienten.
- **MFA / Empfang / Admin**: Der Filter steht standardmaessig auf "Alle Aerzte".
- Sie koennen jederzeit den Filter aendern, um die Patienten eines bestimmten Arztes oder aller Aerzte zu sehen.

## Statuswechsel durchfuehren

### Patient einchecken

Wenn ein Patient in der Praxis eintrifft:

1. Finden Sie den Termin im Bereich **Erwartet**.
2. Klicken Sie auf **Einchecken**.
3. Der Termin wechselt in den Status **Im Wartezimmer**.

!!! tip "Check-in vom Dashboard"
    Aerzte koennen Patienten auch direkt vom Dashboard aus einchecken: Klicken Sie auf **Erwartet** neben dem Termin. Der Patient wird eingecheckt und Sie werden zur Wartezimmerseite weitergeleitet, wo der neue Eintrag hervorgehoben wird.

### Patient aufrufen

Wenn der Arzt den Patienten sehen moechte:

1. Finden Sie den Termin im Bereich **Im Wartezimmer**.
2. Klicken Sie auf **Aufrufen**.
3. Der Termin wechselt in den Status **In Behandlung**.

### Behandlung abschliessen

Nach Ende der Behandlung:

1. Finden Sie den Termin im Bereich **In Behandlung**.
2. Klicken Sie auf **Fertig**.
3. Der Termin wechselt in den Status **Fertig**.

!!! info "Automatischer Statuswechsel"
    Wenn ein Arzt eine **Konsultation** ueber das Dashboard oder den Termin startet, wechselt der Termin automatisch auf **In Behandlung**. Beim Abschliessen der Konsultation (Status "Abgeschlossen") wechselt der Termin automatisch auf **Fertig**. Ein manueller Statuswechsel im Wartezimmer ist dann nicht mehr noetig.

### Patient nicht erschienen

Wenn ein Patient nicht zum Termin kommt:

1. Finden Sie den Termin im Bereich **Erwartet**.
2. Klicken Sie auf **Nicht erschienen**.
3. Der Termin wird entsprechend markiert.

## Anzeige

Fuer jeden Termin werden folgende Informationen angezeigt:

- Patientenname
- Uhrzeit des Termins
- Behandlungsdauer
- Behandelnder Arzt

!!! info "Hinweis"
    Die Wartezimmeransicht zeigt nur Termine des **heutigen Tages**. Vergangene oder zukuenftige Tage koennen ueber die [Wochenansicht](wochenansicht.md) eingesehen werden.
