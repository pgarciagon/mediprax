using MediPrax.Application.Fhir;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;

namespace MediPrax.UnitTests;

public class FhirMapperTests
{
    [Fact]
    public void PatientToFhir_MapsBasicFields()
    {
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "Max", LastName = "Mustermann",
            DateOfBirth = new DateOnly(1985, 3, 15),
            Gender = "M", Kvnr = "A123456789",
            Street = "Musterstr. 1", PostalCode = "28195", City = "Bremen",
            Phone = "0421-12345", Email = "max@test.de"
        };

        var fhir = FhirPatientMapper.ToFhir(patient);

        Assert.Equal("Mustermann", fhir.Name[0].Family);
        Assert.Equal("Max", fhir.Name[0].Given.First());
        Assert.Equal("1985-03-15", fhir.BirthDate);
        Assert.Equal(Hl7.Fhir.Model.AdministrativeGender.Male, fhir.Gender);
        Assert.Equal("Bremen", fhir.Address[0].City);
        Assert.Contains(fhir.Identifier, i => i.Value == "A123456789");
    }

    [Fact]
    public void PatientFromFhir_MapsBasicFields()
    {
        var fhir = new Hl7.Fhir.Model.Patient
        {
            BirthDate = "1990-07-20",
            Gender = Hl7.Fhir.Model.AdministrativeGender.Female
        };
        fhir.Name.Add(new Hl7.Fhir.Model.HumanName { Family = "Schmidt", Given = ["Anna"] });
        fhir.Identifier.Add(new Hl7.Fhir.Model.Identifier
        {
            System = "http://fhir.de/sid/gkv/kvid-10", Value = "B987654321"
        });

        var patient = FhirPatientMapper.FromFhir(fhir);

        Assert.Equal("Anna", patient.FirstName);
        Assert.Equal("Schmidt", patient.LastName);
        Assert.Equal(new DateOnly(1990, 7, 20), patient.DateOfBirth);
        Assert.Equal("W", patient.Gender);
        Assert.Equal("B987654321", patient.Kvnr);
    }

    [Fact]
    public void EncounterToFhir_MapsFields()
    {
        var encounter = new Encounter
        {
            Id = Guid.NewGuid(),
            PatientId = Guid.NewGuid(), DoctorId = Guid.NewGuid(),
            EncounterDate = new DateOnly(2026, 4, 7),
            Icd10Codes = ["F32.1", "G43.0"],
            DurationMinutes = 25,
            Status = EncounterStatus.Completed
        };

        var fhir = FhirEncounterMapper.ToFhir(encounter);

        Assert.Equal(Hl7.Fhir.Model.Encounter.EncounterStatus.Finished, fhir.Status);
        Assert.Equal(2, fhir.Diagnosis.Count);
        Assert.Equal(25, fhir.Length.Value);
    }

    [Fact]
    public void MedicationRequestToFhir_MapsFields()
    {
        var med = new Medication
        {
            Id = Guid.NewGuid(),
            PatientId = Guid.NewGuid(), PrescribedById = Guid.NewGuid(),
            Name = "Sertralin", Staerke = "50mg", Pzn = "01234567",
            Dosierung = "1-0-0-0", Hinweis = "zur Mahlzeit",
            SeitDatum = new DateOnly(2026, 1, 1), IsBtm = false, IsActive = true
        };

        var fhir = FhirMedicationRequestMapper.ToFhir(med);

        Assert.Equal(Hl7.Fhir.Model.MedicationRequest.MedicationrequestStatus.Active, fhir.Status);
        Assert.Equal("1-0-0-0", fhir.DosageInstruction[0].Text);
        Assert.Contains("Sertralin", ((Hl7.Fhir.Model.CodeableConcept)fhir.Medication).Text);
    }
}
