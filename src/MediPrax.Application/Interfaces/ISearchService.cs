namespace MediPrax.Application.Interfaces;

public interface ISearchService
{
    Task<GlobalSearchResultDto> SearchAsync(string query, CancellationToken ct = default);
}

public class GlobalSearchResultDto
{
    public IReadOnlyList<SearchResultItemDto> Results { get; set; } = [];
    public int TotalCount { get; set; }
}

public class SearchResultItemDto
{
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string Url { get; set; } = string.Empty;
}
