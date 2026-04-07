using System.Security.Claims;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using MediPrax.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Infrastructure.Services;

public class AuditService(MediPraxDbContext db, IHttpContextAccessor httpContextAccessor) : IAuditService
{
    public async Task LogAsync(AuditAction action, string? entityType = null, Guid? entityId = null, string? details = null, CancellationToken ct = default)
    {
        var user = httpContextAccessor.HttpContext?.User;
        var ip = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        db.AuditLogs.Add(new AuditLog
        {
            UserId = Guid.TryParse(user?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var uid) ? uid : null,
            UserName = user?.FindFirst(ClaimTypes.Name)?.Value,
            UserRole = user?.FindFirst(ClaimTypes.Role)?.Value,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            IpAddress = ip,
            Details = details
        });

        await db.SaveChangesAsync(ct);
    }

    public async Task<AuditLogResult> SearchAsync(AuditLogSearchDto search, CancellationToken ct = default)
    {
        var query = db.AuditLogs.AsQueryable();

        if (search.From.HasValue)
            query = query.Where(a => a.Timestamp >= search.From.Value);
        if (search.To.HasValue)
            query = query.Where(a => a.Timestamp <= search.To.Value);
        if (search.UserId.HasValue)
            query = query.Where(a => a.UserId == search.UserId.Value);
        if (!string.IsNullOrEmpty(search.EntityType))
            query = query.Where(a => a.EntityType == search.EntityType);
        if (search.Action.HasValue)
            query = query.Where(a => a.Action == search.Action.Value);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((search.Page - 1) * search.PageSize)
            .Take(search.PageSize)
            .Select(a => new AuditLogItemDto
            {
                Id = a.Id,
                Timestamp = a.Timestamp,
                UserName = a.UserName,
                UserRole = a.UserRole,
                Action = a.Action,
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                Details = a.Details,
                IpAddress = a.IpAddress
            })
            .ToListAsync(ct);

        return new AuditLogResult
        {
            Items = items,
            TotalCount = totalCount,
            Page = search.Page,
            PageSize = search.PageSize
        };
    }
}
