using Hl7.Fhir.Model;

namespace MediPrax.Application.Fhir;

/// <summary>
/// Maps MediPrax Medication to FHIR R4 MedicationRequest.
/// Used for E-Rezept integration (KBV_PR_ERP_Prescription profile).
/// </summary>
public static class FhirMedicationRequestMapper
{
    public static MedicationRequest ToFhir(Core.Entities.Medication med)
    {
        var fhir = new MedicationRequest
        {
            Id = med.Id.ToString(),
            Meta = new Meta
            {
                Profile = ["https://fhir.kbv.de/StructureDefinition/KBV_PR_ERP_Prescription|1.1.0"],
                LastUpdated = med.UpdatedAt
            },
            Status = med.IsActive ? MedicationRequest.MedicationrequestStatus.Active : MedicationRequest.MedicationrequestStatus.Stopped,
            Intent = MedicationRequest.MedicationRequestIntent.Order,
            Subject = new ResourceReference($"Patient/{med.PatientId}"),
            Requester = new ResourceReference($"Practitioner/{med.PrescribedById}"),
            AuthoredOn = med.SeitDatum.ToString("yyyy-MM-dd"),
            Medication = new CodeableConcept
            {
                Text = $"{med.Name} {med.Staerke}".Trim()
            }
        };

        // PZN as medication coding
        if (!string.IsNullOrEmpty(med.Pzn))
        {
            ((CodeableConcept)fhir.Medication).Coding.Add(new Coding
            {
                System = "http://fhir.de/CodeSystem/ifa/pzn",
                Code = med.Pzn,
                Display = med.Name
            });
        }

        // Dosage
        if (!string.IsNullOrEmpty(med.Dosierung))
        {
            fhir.DosageInstruction.Add(new Dosage
            {
                Text = med.Dosierung,
                PatientInstruction = med.Hinweis
            });
        }

        // BtM extension
        if (med.IsBtm)
        {
            fhir.Extension.Add(new Extension
            {
                Url = "https://fhir.kbv.de/StructureDefinition/KBV_EX_ERP_BVG",
                Value = new FhirBoolean(true)
            });
        }

        return fhir;
    }
}
