using Hl7.Fhir.Model;
using FhirPatient = Hl7.Fhir.Model.Patient;
using PatientEntity = MediPrax.Core.Entities.Patient;

namespace MediPrax.Application.Fhir;

/// <summary>
/// Maps between MediPrax Patient entity and FHIR R4 Patient resource.
/// Used for ePA integration and FHIR-based data exchange.
/// </summary>
public static class FhirPatientMapper
{
    public static FhirPatient ToFhir(PatientEntity patient)
    {
        var fhir = new FhirPatient
        {
            Id = patient.Id.ToString(),
            Meta = new Meta { LastUpdated = patient.UpdatedAt },
            Active = !patient.IsDeleted,
            BirthDate = patient.DateOfBirth.ToString("yyyy-MM-dd"),
        };

        // Name
        fhir.Name.Add(new HumanName
        {
            Family = patient.LastName,
            Given = [patient.FirstName],
            Use = HumanName.NameUse.Official
        });

        // Gender
        fhir.Gender = patient.Gender?.ToUpper() switch
        {
            "M" => AdministrativeGender.Male,
            "W" or "F" => AdministrativeGender.Female,
            "D" => AdministrativeGender.Other,
            _ => AdministrativeGender.Unknown
        };

        // Address
        if (!string.IsNullOrEmpty(patient.Street) || !string.IsNullOrEmpty(patient.City))
        {
            fhir.Address.Add(new Address
            {
                Line = string.IsNullOrEmpty(patient.Street) ? [] : [patient.Street],
                City = patient.City,
                PostalCode = patient.PostalCode,
                Country = "DE"
            });
        }

        // Phone
        if (!string.IsNullOrEmpty(patient.Phone))
        {
            fhir.Telecom.Add(new ContactPoint
            {
                System = ContactPoint.ContactPointSystem.Phone,
                Value = patient.Phone,
                Use = ContactPoint.ContactPointUse.Home
            });
        }

        // Email
        if (!string.IsNullOrEmpty(patient.Email))
        {
            fhir.Telecom.Add(new ContactPoint
            {
                System = ContactPoint.ContactPointSystem.Email,
                Value = patient.Email
            });
        }

        // KVNR as identifier
        if (!string.IsNullOrEmpty(patient.Kvnr))
        {
            fhir.Identifier.Add(new Identifier
            {
                System = "http://fhir.de/sid/gkv/kvid-10",
                Value = patient.Kvnr,
                Type = new CodeableConcept("http://fhir.de/CodeSystem/identifier-type-de-basis", "GKV")
            });
        }

        // Insurance number
        if (!string.IsNullOrEmpty(patient.InsuranceNumber))
        {
            fhir.Identifier.Add(new Identifier
            {
                System = "http://fhir.de/sid/gkv/versicherten-id",
                Value = patient.InsuranceNumber
            });
        }

        return fhir;
    }

    public static PatientEntity FromFhir(FhirPatient fhir)
    {
        var name = fhir.Name.FirstOrDefault();
        var address = fhir.Address.FirstOrDefault();
        var phone = fhir.Telecom.FirstOrDefault(t => t.System == ContactPoint.ContactPointSystem.Phone);
        var email = fhir.Telecom.FirstOrDefault(t => t.System == ContactPoint.ContactPointSystem.Email);
        var kvnr = fhir.Identifier.FirstOrDefault(i => i.System == "http://fhir.de/sid/gkv/kvid-10");

        return new PatientEntity
        {
            FirstName = name?.Given?.FirstOrDefault() ?? "",
            LastName = name?.Family ?? "",
            DateOfBirth = DateOnly.TryParse(fhir.BirthDate, out var dob) ? dob : default,
            Gender = fhir.Gender switch
            {
                AdministrativeGender.Male => "M",
                AdministrativeGender.Female => "W",
                AdministrativeGender.Other => "D",
                _ => null
            },
            Street = address?.Line?.FirstOrDefault(),
            City = address?.City,
            PostalCode = address?.PostalCode,
            Phone = phone?.Value,
            Email = email?.Value,
            Kvnr = kvnr?.Value,
            InsuranceType = kvnr is not null ? Core.Enums.InsuranceType.GKV : null
        };
    }
}
