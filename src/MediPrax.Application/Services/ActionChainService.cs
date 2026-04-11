using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class ActionChainService(DbContext context) : IActionChainService
{
    private DbSet<ActionChain> ActionChains => context.Set<ActionChain>();

    public async Task<IReadOnlyList<ActionChainDto>> GetAllAsync(string? category = null, CancellationToken ct = default)
    {
        var query = ActionChains
            .Include(c => c.CreatedBy)
            .Include(c => c.Steps.OrderBy(s => s.SortOrder))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(c => c.Category == category);

        var chains = await query
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Title)
            .ToListAsync(ct);

        return chains.Select(MapToDto).ToList();
    }

    public async Task<ActionChainDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var chain = await ActionChains
            .Include(c => c.CreatedBy)
            .Include(c => c.Steps.OrderBy(s => s.SortOrder))
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        return chain is null ? null : MapToDto(chain);
    }

    public async Task<ActionChainDto?> GetByShortcutAsync(string shortcut, CancellationToken ct = default)
    {
        var chain = await ActionChains
            .Include(c => c.CreatedBy)
            .Include(c => c.Steps.OrderBy(s => s.SortOrder))
            .FirstOrDefaultAsync(c => c.Shortcut == shortcut && c.IsActive, ct);

        return chain is null ? null : MapToDto(chain);
    }

    public async Task<IReadOnlyList<ActionChainDto>> SearchAsync(string term, CancellationToken ct = default)
    {
        var normalized = term.TrimStart('#').ToLowerInvariant();

        var chains = await ActionChains
            .Include(c => c.CreatedBy)
            .Include(c => c.Steps.OrderBy(s => s.SortOrder))
            .Where(c => c.IsActive && (
                c.Shortcut.ToLower().StartsWith(normalized) ||
                c.Title.ToLower().Contains(normalized)))
            .OrderBy(c => c.SortOrder)
            .Take(10)
            .ToListAsync(ct);

        return chains.Select(MapToDto).ToList();
    }

    public async Task<ActionChainDto> CreateAsync(CreateActionChainDto dto, CancellationToken ct = default)
    {
        var chain = new ActionChain
        {
            Shortcut = dto.Shortcut.TrimStart('#').ToLowerInvariant(),
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            CreatedById = dto.CreatedById,
            IsGlobal = dto.IsGlobal,
            SortOrder = dto.SortOrder,
            IsActive = dto.IsActive
        };

        foreach (var stepDto in dto.Steps)
        {
            chain.Steps.Add(new ActionChainStep
            {
                StepType = stepDto.StepType,
                SortOrder = stepDto.SortOrder,
                Configuration = stepDto.Configuration
            });
        }

        ActionChains.Add(chain);
        await context.SaveChangesAsync(ct);

        var created = await ActionChains
            .Include(c => c.CreatedBy)
            .Include(c => c.Steps.OrderBy(s => s.SortOrder))
            .FirstAsync(c => c.Id == chain.Id, ct);

        return MapToDto(created);
    }

    public async Task<ActionChainDto> UpdateAsync(UpdateActionChainDto dto, CancellationToken ct = default)
    {
        var chain = await ActionChains
            .Include(c => c.Steps)
            .FirstOrDefaultAsync(c => c.Id == dto.Id, ct)
            ?? throw new InvalidOperationException("Aktionskette nicht gefunden.");

        chain.Shortcut = dto.Shortcut.TrimStart('#').ToLowerInvariant();
        chain.Title = dto.Title;
        chain.Description = dto.Description;
        chain.Category = dto.Category;
        chain.IsGlobal = dto.IsGlobal;
        chain.SortOrder = dto.SortOrder;
        chain.IsActive = dto.IsActive;

        // Replace steps: remove existing, add new
        context.Set<ActionChainStep>().RemoveRange(chain.Steps);
        chain.Steps.Clear();

        foreach (var stepDto in dto.Steps)
        {
            chain.Steps.Add(new ActionChainStep
            {
                ActionChainId = chain.Id,
                StepType = stepDto.StepType,
                SortOrder = stepDto.SortOrder,
                Configuration = stepDto.Configuration
            });
        }

        await context.SaveChangesAsync(ct);

        var updated = await ActionChains
            .Include(c => c.CreatedBy)
            .Include(c => c.Steps.OrderBy(s => s.SortOrder))
            .FirstAsync(c => c.Id == chain.Id, ct);

        return MapToDto(updated);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var chain = await ActionChains.FindAsync([id], ct)
            ?? throw new InvalidOperationException("Aktionskette nicht gefunden.");

        context.Remove(chain);
        await context.SaveChangesAsync(ct);
    }

    private static ActionChainDto MapToDto(ActionChain chain) => new()
    {
        Id = chain.Id,
        Shortcut = chain.Shortcut,
        Title = chain.Title,
        Description = chain.Description,
        Category = chain.Category,
        CreatedById = chain.CreatedById,
        CreatedByName = chain.CreatedBy?.FullName ?? "",
        IsGlobal = chain.IsGlobal,
        SortOrder = chain.SortOrder,
        IsActive = chain.IsActive,
        StepCount = chain.Steps.Count,
        Steps = chain.Steps.OrderBy(s => s.SortOrder).Select(s => new ActionChainStepDto
        {
            Id = s.Id,
            StepType = s.StepType,
            SortOrder = s.SortOrder,
            Configuration = s.Configuration
        }).ToList()
    };
}
