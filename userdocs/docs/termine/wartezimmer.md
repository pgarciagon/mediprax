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

## Statuswechsel durchfuehren

### Patient einchecken

Wenn ein Patient in der Praxis eintrifft:

1. Finden Sie den Termin im Bereich **Erwartet**.
2. Klicken Sie auf **Einchecken**.
3. Der Termin wechselt in den Status **Im Wartezimmer**.

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

### Patient nicht erschienen

Wenn ein Patient nicht zum Termin kommt:

1. Finden Sie den Termin im Bereich **Erwartet**.
2. Klicken Sie auf **Nicht erschienen**.
3. Der Termin wird entsprechend markiert.

!!! tip "Tipp"
    Die Wartezimmerverwaltung ist besonders fuer den **Empfang** und die **MFA** nuetzlich. Halten Sie den Status aktuell, damit Aerzte sehen koennen, welche Patienten warten.

## Anzeige

Fuer jeden Termin werden folgende Informationen angezeigt:

- Patientenname
- Uhrzeit des Termins
- Terminart
- Wartedauer (seit Einchecken)
- Behandelnder Arzt

!!! info "Hinweis"
    Die Wartezimmeransicht zeigt nur Termine des **heutigen Tages**. Vergangene oder zukuenftige Tage koennen ueber die [Wochenansicht](wochenansicht.md) eingesehen werden.
