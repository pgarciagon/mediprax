# M34: eAU Electronic Transmission -- AUSSTEHEND

> **Priority:** P3 | **Estimated Effort:** Small
> **Why:** eAU (elektronische Arbeitsunfaehigkeitsbescheinigung) electronic transmission is mandatory. Currently we only generate PDFs.

#### 34.1 Enhancement

Extend existing AU functionality:

1. **After PDF generation,** transmit eAU data via TI (KIM) to the patient's health insurance
2. **Track transmission status:** Draft -> Transmitted -> Confirmed
3. **Erst-AU vs. Folge-AU** tracking per patient
4. **AU history** per patient with date ranges

#### 34.2 TI Integration

```
MediPrax.Application/Interfaces/Telematik/IEauService.cs
MediPrax.Server/Services/Telematik/MockEauService.cs
```

Methods:
- `TransmitAsync(EauData data)` -> eAU submission to KK
- `GetStatusAsync(string eauId)` -> Check transmission status
- `CancelAsync(string eauId)` -> Cancel a transmitted eAU

#### 34.3 Files to Create/Modify

| Action | File |
|--------|------|
| Create | `MediPrax.Application/Interfaces/Telematik/IEauService.cs` |
| Create | `MediPrax.Application/DTOs/EauDto.cs` |
| Create | `MediPrax.Server/Services/Telematik/MockEauService.cs` |
| Modify | `MediPrax.Server/Components/Pages/Formulare/AuFormular.razor` -- Add transmit button |
| Modify | `MediPrax.Server/Program.cs` -- Register service |

---

