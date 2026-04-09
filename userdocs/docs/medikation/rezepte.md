# Rezeptdruck

MediPrax unterstuetzt die Erstellung und den Druck von Rezepten fuer die Verordnung von Medikamenten. Je nach Versicherungstyp des Patienten werden Kassenrezepte oder Privatrezepte erstellt.

## Rezept erstellen

1. Oeffnen Sie die [Patientenakte](../patienten/akte.md).
2. Navigieren Sie zum Bereich **Rezepte** oder klicken Sie auf **Neues Rezept**.
3. Waehlen Sie die **Rezeptart**:

| Rezeptart | Verwendung | Formular |
|---|---|---|
| **Kassenrezept** | GKV-Patienten | Muster 16 (rosa) |
| **Privatrezept** | PKV-Patienten oder IGeL | Blaues Rezept |
| **E-Rezept** | Elektronisches Rezept via TI | Siehe [E-Rezept](../telematik/erezept.md) |

4. Fuegen Sie **Medikamente** hinzu:

| Feld | Beschreibung |
|---|---|
| **Medikament** | Name des Praeparats |
| **PZN** | Pharmazentralnummer |
| **Dosierung** | Dosieranweisung (z. B. "1-0-0-1") |
| **Menge** | Packungsgroesse (N1, N2, N3) oder Stueckzahl |
| **Aut idem** | Austausch zulaessig (Ja/Nein) |

5. Klicken Sie auf **Speichern** und anschliessend auf **PDF erstellen**.

## Datenvorausfuellung

Folgende Daten werden automatisch aus der Patientenakte uebernommen:

- Patientenname, Geburtsdatum, Adresse
- Versicherungsstatus und Krankenkasse
- KVNR / Versichertennummer

## PDF-Generierung und Druck

1. Klicken Sie auf **PDF erstellen**.
2. Das Rezept wird als druckfertiges PDF generiert.
3. Klicken Sie auf **Drucken** oder **Herunterladen**.
4. Legen Sie das Kassenrezeptformular in den Drucker ein.

!!! info "Hinweis"
    Kassenrezepte werden auf die vorgedruckten rosa Formulare gedruckt. Stellen Sie sicher, dass der Drucker korrekt kalibriert ist, damit die Felder passgenau bedruckt werden.

## E-Rezept

Alternativ zum Papierrezept koennen Sie ein **E-Rezept** ueber die Telematikinfrastruktur erstellen. Siehe [E-Rezept](../telematik/erezept.md) fuer Details.

## Rezepthistorie

Alle ausgestellten Rezepte werden in der Patientenakte gespeichert und sind chronologisch einsehbar. Die Historie umfasst:

- Verordnungsdatum
- Verordnete Medikamente
- Rezeptart
- Verordnender Arzt

!!! tip "Tipp"
    Nutzen Sie die Funktion **Rezept wiederholen**, um ein vorheriges Rezept mit denselben Medikamenten schnell erneut auszustellen. Pruefen Sie dabei immer die Aktualitaet der Verordnung.

!!! warning "BtM-Rezepte"
    Fuer Betaeubungsmittel gelten besondere Vorschriften. Verwenden Sie die [BtM-Verwaltung](btm.md) fuer BtM-pflichtige Praeparate.
