using MediPrax.Core.Entities;
using MediPrax.Core.Enums;

namespace MediPrax.Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(AuditAction action, string? entityType = null, Guid? entityId = null, string? details = null, CancellationToken ct = default);
    Task<AuditLogResult> SearchAsync(AuditLogSearchDto search, CancellationToken ct = default);
}

public class AuditLogSearchDto
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public Guid? UserId { get; set; }
    public string? EntityType { get; set; }
    public AuditAction? Action { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class AuditLogItemDto
{
    public long Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string? UserName { get; set; }
    public string? UserRole { get; set; }
    public AuditAction Action { get; set; }
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }

    public string ActionDisplay => Action switch
    {
        AuditAction.Create => "Erstellt",
        AuditAction.Read => "Gelesen",
        AuditAction.Update => "Geändert",
        AuditAction.Delete => "Gelöscht",
        AuditAction.Login => "Anmeldung",
        AuditAction.LoginFailed => "Anmeldung fehlgeschlagen",
        AuditAction.Logout => "Abmeldung",
        AuditAction.Export => "Exportiert",
        AuditAction.Print => "Gedruckt",
        _ => Action.ToString()
    };
}

public class AuditLogResult
{
    public IReadOnlyList<AuditLogItemDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
