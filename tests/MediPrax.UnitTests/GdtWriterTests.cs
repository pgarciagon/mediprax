using MediPrax.Application.DTOs;
using MediPrax.Application.Services;

namespace MediPrax.UnitTests;

public class GdtWriterTests
{
    [Fact]
    public void WriteRequest_ProducesNonEmptyOutput()
    {
        var request = new GdtRequestDto
        {
            PatientId = Guid.NewGuid(),
            PatientName = "Mustermann, Max",
            PatientDob = new DateOnly(1990, 1, 1),
            DeviceType = "EEG"
        };

        var content = GdtWriter.WriteRequest(request);
        Assert.False(string.IsNullOrWhiteSpace(content));
    }

    [Fact]
    public void WriteRequest_ContainsSatzart6310()
    {
        var request = new GdtRequestDto
        {
            PatientId = Guid.NewGuid(),
            PatientName = "Mustermann, Max",
            PatientDob = new DateOnly(1990, 1, 1),
            DeviceType = "EEG"
        };

        var content = GdtWriter.WriteRequest(request);
        Assert.Contains("80006310", content);
    }

    [Fact]
    public void WriteRequest_ContainsPatientData()
    {
        var request = new GdtRequestDto
        {
            PatientId = Guid.NewGuid(),
            PatientName = "Mustermann, Max",
            PatientDob = new DateOnly(1990, 1, 1),
            DeviceType = "EEG"
        };

        var content = GdtWriter.WriteRequest(request);
        Assert.Contains("3101Mustermann", content);
        Assert.Contains("3103", content);
        Assert.Contains("01011990", content);
    }

    [Fact]
    public void WriteRequest_ContainsDeviceType()
    {
        var request = new GdtRequestDto
        {
            PatientId = Guid.NewGuid(),
            PatientName = "Test, Patient",
            PatientDob = new DateOnly(1985, 6, 15),
            DeviceType = "EMG"
        };

        var content = GdtWriter.WriteRequest(request);
        Assert.Contains("6302EMG", content);
    }

    [Fact]
    public void WriteRequest_IncludesOptionalNotes()
    {
        var request = new GdtRequestDto
        {
            PatientId = Guid.NewGuid(),
            PatientName = "Test, Patient",
            PatientDob = new DateOnly(1985, 6, 15),
            DeviceType = "NLG",
            RequestNotes = "Bitte N. medianus prüfen"
        };

        var content = GdtWriter.WriteRequest(request);
        Assert.Contains("6220", content);
        Assert.Contains("N. medianus", content);
    }
}
