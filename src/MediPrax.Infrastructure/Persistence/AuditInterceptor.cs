using System.Security.Claims;
using System.Text.Json;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MediPrax.Infrastructure.Persistence;

public class AuditInterceptor(IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not MediPraxDbContext db)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = user?.FindFirst(ClaimTypes.Name)?.Value;
        var userRole = user?.FindFirst(ClaimTypes.Role)?.Value;
        var ip = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        var auditEntries = new List<AuditLog>();

        foreach (var entry in db.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Unchanged || entry.State == EntityState.Detached)
                continue;

            var entityType = entry.Entity.GetType().Name;
            var entityId = entry.Entity.Id;

            var action = entry.State switch
            {
                EntityState.Added => AuditAction.Create,
                EntityState.Modified when entry.Entity.IsDeleted => AuditAction.Delete,
                EntityState.Modified => AuditAction.Update,
                EntityState.Deleted => AuditAction.Delete,
                _ => (AuditAction?)null
            };

            if (action is null) continue;

            string? oldValues = null;
            string? newValues = null;

            if (action == AuditAction.Update || action == AuditAction.Delete)
            {
                var changed = entry.Properties
                    .Where(p => p.IsModified)
                    .Where(p => p.Metadata.Name != nameof(BaseEntity.UpdatedAt))
                    .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue?.ToString());
                if (changed.Count > 0)
                    oldValues = JsonSerializer.Serialize(changed);
            }

            if (action == AuditAction.Create || action == AuditAction.Update)
            {
                var props = action == AuditAction.Update
                    ? entry.Properties.Where(p => p.IsModified)
                        .Where(p => p.Metadata.Name != nameof(BaseEntity.UpdatedAt))
                    : entry.Properties;
                var values = props.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue?.ToString());
                if (values.Count > 0)
                    newValues = JsonSerializer.Serialize(values);
            }

            auditEntries.Add(new AuditLog
            {
                UserId = userId is not null ? Guid.Parse(userId) : null,
                UserName = userName,
                UserRole = userRole,
                Action = action.Value,
                EntityType = entityType,
                EntityId = entityId,
                OldValues = oldValues,
                NewValues = newValues,
                IpAddress = ip
            });
        }

        foreach (var audit in auditEntries)
            db.AuditLogs.Add(audit);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
