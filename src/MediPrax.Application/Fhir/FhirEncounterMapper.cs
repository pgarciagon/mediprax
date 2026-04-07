using Hl7.Fhir.Model;
using MediPrax.Core.Enums;
using FhirEncounter = Hl7.Fhir.Model.Encounter;
using EncounterEntity = MediPrax.Core.Entities.Encounter;

namespace MediPrax.Application.Fhir;

/// <summary>
/// Maps between MediPrax Encounter entity and FHIR R4 Encounter resource.
/// </summary>
public static class FhirEncounterMapper
{
    public static FhirEncounter ToFhir(EncounterEntity encounter)
    {
        var fhir = new FhirEncounter
        {
            Id = encounter.Id.ToString(),
            Meta = new Meta { LastUpdated = encounter.UpdatedAt },
            Status = encounter.Status switch
            {
                EncounterStatus.Open => FhirEncounter.EncounterStatus.InProgress,
                EncounterStatus.InProgress => FhirEncounter.EncounterStatus.InProgress,
                EncounterStatus.Completed => FhirEncounter.EncounterStatus.Finished,
                _ => FhirEncounter.EncounterStatus.Unknown
            },
            Class = new Coding("http://terminology.hl7.org/CodeSystem/v3-ActCode", "AMB", "ambulatory"),
            Subject = new ResourceReference($"Patient/{encounter.PatientId}"),
            Period = new Period
            {
                Start = encounter.EncounterDate.ToString("yyyy-MM-dd")
            }
        };

        // Participant (doctor)
        fhir.Participant.Add(new FhirEncounter.ParticipantComponent
        {
            Individual = new ResourceReference($"Practitioner/{encounter.DoctorId}")
        });

        // Diagnoses (ICD-10)
        foreach (var code in encounter.Icd10Codes)
        {
            fhir.Diagnosis.Add(new FhirEncounter.DiagnosisComponent
            {
                Condition = new ResourceReference { Display = code },
                Use = new CodeableConcept("http://fhir.de/CodeSystem/KBV_CS_SFHIR_ICD_DIAGNOSESICHERHEIT", "G", "Gesicherte Diagnose")
            });
        }

        // Duration as extension
        if (encounter.DurationMinutes.HasValue)
        {
            fhir.Length = new Duration
            {
                Value = encounter.DurationMinutes.Value,
                Unit = "min",
                System = "http://unitsofmeasure.org",
                Code = "min"
            };
        }

        return fhir;
    }
}
