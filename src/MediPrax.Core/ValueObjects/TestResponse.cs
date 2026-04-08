namespace MediPrax.Core.ValueObjects;

public class TestResponse
{
    public int ItemNumber { get; set; }
    public string ItemText { get; set; } = string.Empty;
    public int ResponseValue { get; set; }
    public string? ResponseText { get; set; }
}
