# BtM-Verwaltung (Betaeubungsmittel)

Die BtM-Verwaltung dokumentiert die Verordnung von Betaeubungsmitteln gemaess der Betaeubungsmittel-Verschreibungsverordnung (BtMVV). Sie stellt die Einhaltung der gesetzlichen Hoechstmengen und Aufbewahrungspflichten sicher.

## BtM-Rezept erstellen

1. Oeffnen Sie die [Patientenakte](../patienten/akte.md).
2. Navigieren Sie zum Bereich **BtM-Verordnungen**.
3. Klicken Sie auf **Neues BtM-Rezept**.
4. Fuellen Sie die Pflichtfelder aus:

| Feld | Beschreibung | Pflicht |
|---|---|---|
| **Medikament** | BtM-pflichtiges Praeparat | Ja |
| **Dosierung** | Dosierung und Einnahmeschema | Ja |
| **Menge** | Verordnete Menge | Ja |
| **BtM-Rezeptnummer** | Fortlaufende Nummer des BtM-Rezepts | Ja |
| **BtM-Nummer des Arztes** | Persoenliche BtM-Nummer des verordnenden Arztes | Ja |
| **Datum** | Verordnungsdatum | Ja |

5. MediPrax prueft automatisch die **Hoechstmengen**.
6. Klicken Sie auf **Speichern**.

## Hoechstmengen (30-Tage-Versorgung)

Die BtMVV legt Hoechstmengen fuer einen 30-Tage-Zeitraum fest:

| Wirkstoff | Hoechstmenge (30 Tage) |
|---|---|
| **Methylphenidat** | 2.640 mg |
| **Lisdexamfetamin** | 1.890 mg |
| **Cannabis (Blueten)** | 100 g |
| **Dronabinol** | 1.000 mg |
| **Buprenorphin** | 800 mg |
| **Fentanyl** | 500 mg (TTS) |
| **Morphin** | 20.000 mg |
| **Oxycodon** | 15.000 mg |
| **Tilidin** | 18.000 mg |

!!! warning "Hoechstmengenprüefung"
    MediPrax warnt automatisch, wenn die verordnete Menge die **30-Tage-Hoechstmenge** gemaess BtMVV ueberschreitet. Eine Ueberschreitung ist nur mit Kennzeichnung ("A") auf dem Rezept zulaessig.

## Compliance-Pruefung

MediPrax ueberprueft bei jeder BtM-Verordnung:

- Einhaltung der 30-Tage-Hoechstmengen
- Zeitabstand zur letzten Verordnung
- Vollstaendigkeit der Rezeptangaben (BtM-Rezeptnummer, Arzt-BtM-Nummer)

## Aufbewahrungspflicht

!!! info "3-Jahres-Aufbewahrung"
    Gemaess BtMVV muessen alle BtM-Verordnungen **3 Jahre** lang aufbewahrt werden. MediPrax speichert alle BtM-Rezepte dauerhaft und markiert sie entsprechend.

## BtM-Uebersicht

Die BtM-Uebersicht zeigt alle Verordnungen eines Patienten chronologisch:

- Verordnungsdatum
- Medikament und Menge
- BtM-Rezeptnummer
- Verordnender Arzt

!!! tip "Tipp"
    Fuehren Sie die BtM-Dokumentation sorgfaeltig. Bei BtM-Prüefungen durch das Gesundheitsamt muss die lueckenlose Dokumentation nachgewiesen werden.

!!! note "Hinweis"
    BtM-Rezepte werden auf speziellen BtM-Rezeptformularen (dreiteilig) ausgestellt. MediPrax unterstuetzt die Dokumentation, ersetzt aber nicht das physische BtM-Rezeptformular.
