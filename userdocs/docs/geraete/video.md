# Videosprechstunde

MediPrax unterstuetzt die Durchfuehrung von Videosprechstunden fuer Kontakte, die nicht zwingend eine persoenliche Anwesenheit erfordern. Die KBV-Regelungen fuer Videosprechstunden in der Psychotherapie werden vom System ueberwacht.

## Termin als Videosprechstunde markieren

1. Erstellen Sie einen [neuen Termin](../termine/anlegen.md) oder oeffnen Sie einen bestehenden.
2. Aktivieren Sie die Option **Videosprechstunde**.
3. Fuegen Sie einen **Video-Link** hinzu (URL der Videoplattform).
4. Dokumentieren Sie die **Einwilligung** des Patienten.
5. Speichern Sie den Termin.

## Einwilligungspflicht

!!! warning "Einwilligung erforderlich"
    Vor der ersten Videosprechstunde muss die **Einwilligung des Patienten** zur Durchfuehrung von Videosprechstunden dokumentiert werden. Diese Einwilligung gilt bis auf Widerruf.

## KBV-Regelungen fuer Psychotherapie

Die KBV legt besondere Regeln fuer Videosprechstunden in der Psychotherapie fest:

| Regel | Beschreibung |
|---|---|
| **Erste Sprechstunde** | Die erste psychotherapeutische Sprechstunde muss **persoenlich** (nicht per Video) stattfinden |
| **Mindestens 50 Min. persoenlich** | Vor Beginn von Videositzungen muessen mindestens **50 Minuten** persoenliche Kontaktzeit dokumentiert sein |
| **Max. 75% Video-Anteil** | Pro Therapiefall duerfen maximal **75%** der Sitzungen per Video stattfinden |

MediPrax ueberwacht diese Regeln und warnt automatisch bei Verstoessen.

## Video-faehige GOPs

Folgende GOPs koennen per Videosprechstunde abgerechnet werden:

| GOP | Beschreibung |
|---|---|
| **35100** | Psychotherapeutische Sprechstunde (nicht fuer Erstkontakt) |
| **35110** | Probatorische Sitzung |
| **35150** | Verhaltenstherapie Einzelsitzung |
| **21220** | Psychiatrisches Gespraech |
| **16220** | Neurologisches Gespraech |

!!! info "Hinweis"
    Nicht alle GOPs sind fuer Videosprechstunden zugelassen. Pruefen Sie im Zweifelsfall die aktuelle KBV-Liste der videofaehigen Leistungen.

## Video-Link-Verwaltung

| Feld | Beschreibung |
|---|---|
| **Video-URL** | Link zur Videosprechstunde (wird dem Patienten mitgeteilt) |
| **Plattform** | Name des Videodienstanbieters |
| **Einwilligung** | Datum der Patienteneinwilligung |

## Durchfuehrung

1. Zum Terminzeitpunkt oeffnet der Patient den **Video-Link**.
2. Der Arzt startet die Videositzung ueber die Plattform.
3. Nach der Sitzung dokumentieren Sie den [Kontakt](../dokumentation/kontakte.md) wie gewohnt.
4. Markieren Sie den Kontakt als **Videosprechstunde**.

## Video-Quote ueberwachen

MediPrax berechnet automatisch den **Video-Anteil** pro Therapiefall:

- Aktuelle Anzahl Video-Sitzungen
- Aktuelle Anzahl Praesenz-Sitzungen
- Prozentualer Video-Anteil

!!! tip "Tipp"
    Behalten Sie die Video-Quote im Blick. Bei Ueberschreitung der 75%-Grenze werden die betroffenen GOPs moeglicherweise nicht verguetet.

!!! note "Hinweis"
    Der Videodienstanbieter muss KBV-zertifiziert sein und die Anforderungen an Datenschutz und Datensicherheit erfuellen.
