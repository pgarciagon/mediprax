using MediPrax.Core.Enums;
using MediPrax.Core.ValueObjects;

namespace MediPrax.Application.DTOs;

public class PsychometricTestDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid? EncounterId { get; set; }
    public PsychometricTestType TestType { get; set; }
    public string TestTypeName { get; set; } = string.Empty;
    public DateOnly TestDate { get; set; }
    public int TotalScore { get; set; }
    public int MaxScore { get; set; }
    public string Interpretation { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public TestStatus Status { get; set; }
    public List<TestResponse> Responses { get; set; } = [];
    public string AdministeredByName { get; set; } = string.Empty;
}

public class PsychometricTestListItemDto
{
    public Guid Id { get; set; }
    public PsychometricTestType TestType { get; set; }
    public string TestTypeName { get; set; } = string.Empty;
    public DateOnly TestDate { get; set; }
    public int TotalScore { get; set; }
    public int MaxScore { get; set; }
    public string Interpretation { get; set; } = string.Empty;
    public TestStatus Status { get; set; }
}

public class CreatePsychometricTestDto
{
    public Guid PatientId { get; set; }
    public Guid? EncounterId { get; set; }
    public Guid AdministeredById { get; set; }
    public PsychometricTestType TestType { get; set; }
    public DateOnly TestDate { get; set; }
}

public class SubmitTestResponsesDto
{
    public Guid TestId { get; set; }
    public List<TestResponse> Responses { get; set; } = [];
}

public class ScoreHistoryPointDto
{
    public DateOnly Date { get; set; }
    public int Score { get; set; }
    public string Interpretation { get; set; } = string.Empty;
}
