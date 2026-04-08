using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface IIcd10CodeService
{
    Task<IReadOnlyList<Icd10CodeDto>> SearchAsync(string term, int maxResults = 15, CancellationToken ct = default);
    Task<IReadOnlyList<Icd10CodeDto>> GetByCategoryAsync(string category, CancellationToken ct = default);
    Task<IReadOnlyList<Icd10CodeDto>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken ct = default);
    Task<string?> GetDescriptionAsync(string code, CancellationToken ct = default);
    Task<Icd10CodeDto> CreateAsync(CreateIcd10CodeDto dto, CancellationToken ct = default);
    Task<Icd10CodeDto> UpdateAsync(UpdateIcd10CodeDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<int> SeedFromCatalogAsync(CancellationToken ct = default);
}
