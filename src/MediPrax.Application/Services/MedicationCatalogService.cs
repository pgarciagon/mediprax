using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class MedicationCatalogService(DbContext context) : IMedicationCatalogService
{
    private DbSet<MedicationCatalogEntry> Catalog => context.Set<MedicationCatalogEntry>();

    public async Task<IReadOnlyList<MedicationCatalogEntryDto>> SearchAsync(
        string term, int maxResults = 15, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            return [];

        var normalizedTerm = term.Trim();

        // Check if term looks like a PZN (all digits)
        if (normalizedTerm.All(char.IsDigit))
        {
            return await Catalog
                .Where(e => e.IsActive && EF.Functions.ILike(e.Pzn, $"{normalizedTerm}%"))
                .OrderBy(e => e.Pzn)
                .Take(maxResults)
                .Select(e => ToDto(e))
                .ToListAsync(ct);
        }

        // Search by Handelsname or Wirkstoff (case-insensitive)
        var pattern = $"%{normalizedTerm}%";
        return await Catalog
            .Where(e => e.IsActive &&
                (EF.Functions.ILike(e.Handelsname, pattern) ||
                 EF.Functions.ILike(e.Wirkstoff, pattern)))
            .OrderBy(e => e.Handelsname)
            .Take(maxResults)
            .Select(e => ToDto(e))
            .ToListAsync(ct);
    }

    public async Task<MedicationCatalogEntryDto?> GetByPznAsync(string pzn, CancellationToken ct = default)
    {
        var entry = await Catalog.FirstOrDefaultAsync(e => e.Pzn == pzn, ct);
        return entry is null ? null : MapToDto(entry);
    }

    public async Task<IReadOnlyList<MedicationCatalogEntryDto>> GetByWirkstoffAsync(
        string wirkstoff, CancellationToken ct = default)
    {
        var pattern = $"%{wirkstoff.Trim()}%";
        return await Catalog
            .Where(e => e.IsActive && EF.Functions.ILike(e.Wirkstoff, pattern))
            .OrderBy(e => e.Handelsname)
            .Select(e => ToDto(e))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<MedicationCatalogEntryDto>> GetByAtcPrefixAsync(
        string atcPrefix, CancellationToken ct = default)
    {
        return await Catalog
            .Where(e => e.IsActive && e.AtcCode != null && EF.Functions.ILike(e.AtcCode, $"{atcPrefix}%"))
            .OrderBy(e => e.AtcCode).ThenBy(e => e.Handelsname)
            .Select(e => ToDto(e))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<MedicationCatalogEntryDto>> GetByCategoryAsync(
        MedicationCategory category, int maxResults = 50, CancellationToken ct = default)
    {
        return await Catalog
            .Where(e => e.IsActive && e.Category == category)
            .OrderBy(e => e.Wirkstoff).ThenBy(e => e.Handelsname)
            .Take(maxResults)
            .Select(e => ToDto(e))
            .ToListAsync(ct);
    }

    public async Task<CatalogStatisticsDto> GetStatisticsAsync(CancellationToken ct = default)
    {
        var total = await Catalog.CountAsync(ct);
        var active = await Catalog.CountAsync(e => e.IsActive, ct);
        var btm = await Catalog.CountAsync(e => e.IsBtm && e.IsActive, ct);
        var tRezept = await Catalog.CountAsync(e => e.IsTRezeptPflichtig && e.IsActive, ct);

        var byCategory = await Catalog
            .Where(e => e.IsActive && e.Category != null)
            .GroupBy(e => e.Category!.Value)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return new CatalogStatisticsDto
        {
            TotalEntries = total,
            ActiveEntries = active,
            BtmCount = btm,
            TRezeptCount = tRezept,
            ByCategory = byCategory.ToDictionary(x => x.Category.ToString(), x => x.Count)
        };
    }

    // Expression-tree compatible projection for EF Core queries
    private static MedicationCatalogEntryDto ToDto(MedicationCatalogEntry e) => new()
    {
        Id = e.Id,
        Pzn = e.Pzn,
        Handelsname = e.Handelsname,
        Wirkstoff = e.Wirkstoff,
        WirkstoffAsk = e.WirkstoffAsk,
        AtcCode = e.AtcCode,
        Staerke = e.Staerke,
        Darreichungsform = e.Darreichungsform,
        Packungsgroesse = e.Packungsgroesse,
        NormPackungsgroesse = e.NormPackungsgroesse,
        Hersteller = e.Hersteller,
        IsBtm = e.IsBtm,
        IsTRezeptPflichtig = e.IsTRezeptPflichtig,
        IsVerschreibungspflichtig = e.IsVerschreibungspflichtig,
        Festbetrag = e.Festbetrag,
        Avp = e.Avp,
        Category = e.Category,
        IsActive = e.IsActive,
        DataSource = e.DataSource
    };

    // In-memory mapping (for single entity lookups)
    private static MedicationCatalogEntryDto MapToDto(MedicationCatalogEntry e) => ToDto(e);
}
