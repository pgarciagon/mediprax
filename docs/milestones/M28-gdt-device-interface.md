# M28: GDT Device Interface -- AUSSTEHEND

> **Priority:** P2 | **Estimated Effort:** Large
> **Why:** Neurological practices depend on EEG, EMG, NLG, and Doppler devices. GDT (Geraetedatentransfer) is the standard interface for bidirectional data exchange with medical devices.

#### 28.1 GDT Protocol Implementation

```
MediPrax.Application/Services/GdtParser.cs
MediPrax.Application/Services/GdtWriter.cs
MediPrax.Application/Services/GdtService.cs
```

**GDT 2.1 format** (universally supported):
- Fixed-width text file
- Field identifiers (Feldkennungen): 3000 (Patient name), 3100 (DOB), 6200 (Result text), etc.
- Satzarten: 6310 (Request), 6311 (Response with results)

**Workflow:**
1. **PVS -> Device:** Write GDT file with patient data (Satzart 6310) to device's watch directory
2. **Device -> PVS:** Device writes results file (Satzart 6311) to PVS watch directory
3. **PVS reads:** Parse results, create `Document` entity with findings

#### 28.2 File Watcher Service

```
MediPrax.Server/Services/GdtFileWatcherService.cs : BackgroundService
```

- Monitor configured directories for incoming GDT files
- Parse and import automatically
- Create notification for reviewing clinician
- Configurable per device (EEG path, EMG path, etc.)

#### 28.3 Device Configuration

```
MediPrax.Application/DTOs/GdtDeviceConfig.cs
```

| Property | Description |
|----------|-------------|
| `DeviceName` | E.g., "Nihon Kohden EEG-1200" |
| `DeviceType` | EEG, EMG, NLG, Doppler, EP |
| `ImportDirectory` | Path where device writes results |
| `ExportDirectory` | Path where PVS writes requests |
| `GdtVersion` | "2.1" or "3.5" |
| `Encoding` | "ISO-8859-1" (standard for GDT) |

Store in `appsettings.json` under `GdtDevices` section.

#### 28.4 Structured Report Templates

When GDT results are imported, provide structured report templates per device type:

- **EEG Report:** Background activity, focal abnormalities, epileptiform discharges, HV response, PS response, conclusion
- **EMG Report:** Muscle examined, insertional activity, spontaneous activity, motor unit potentials, interference pattern, conclusion
- **NLG Report:** Nerve, DML, CMAP amplitude, MNCV, F-wave, SNAP amplitude, SNCV
- **Evoked Potentials:** Type (VEP/SEP/AEP/MEP), latencies, amplitudes, interpretation

#### 28.5 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Application/Services/GdtParser.cs` |
| Create | `MediPrax.Application/Services/GdtWriter.cs` |
| Create | `MediPrax.Application/Services/GdtService.cs` |
| Create | `MediPrax.Application/DTOs/GdtDeviceConfig.cs` |
| Create | `MediPrax.Application/DTOs/GdtResultDto.cs` |
| Create | `MediPrax.Server/Services/GdtFileWatcherService.cs` |
| Create | `MediPrax.Server/Components/Pages/Geraete/GeraeteUebersicht.razor` |
| Create | `MediPrax.Server/Components/Pages/Geraete/GeraeteErgebnisse.razor` |
| Modify | `MediPrax.Server/Program.cs` -- Register BackgroundService |
| Modify | `MediPrax.Server/appsettings.json` -- GDT device config section |
| Create | `tests/MediPrax.UnitTests/GdtParserTests.cs` |
| Create | `tests/MediPrax.UnitTests/GdtWriterTests.cs` |

---

