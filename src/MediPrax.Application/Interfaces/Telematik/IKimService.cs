namespace MediPrax.Application.Interfaces.Telematik;

/// <summary>
/// KIM — Kommunikation im Medizinwesen.
/// Secure email between healthcare providers via TI infrastructure.
/// In production: SMTP/POP3 via KIM-Fachdienst.
/// </summary>
public interface IKimService
{
    Task<KimSendResultDto> SendAsync(KimMessageDto message, CancellationToken ct = default);
    Task<IReadOnlyList<KimInboxItemDto>> GetInboxAsync(CancellationToken ct = default);
    Task<KimMessageDetailDto?> GetMessageAsync(string messageId, CancellationToken ct = default);
    Task<IReadOnlyList<KimDirectoryEntryDto>> SearchDirectoryAsync(string query, CancellationToken ct = default);
}

public class KimMessageDto
{
    public string RecipientAddress { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Guid? AttachmentDocumentId { get; set; }
}

public class KimSendResultDto
{
    public bool Success { get; set; }
    public string? MessageId { get; set; }
    public string? ErrorMessage { get; set; }
}

public class KimInboxItemDto
{
    public string MessageId { get; set; } = string.Empty;
    public string SenderAddress { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
    public bool IsRead { get; set; }
    public bool HasAttachment { get; set; }
}

public class KimMessageDetailDto
{
    public string MessageId { get; set; } = string.Empty;
    public string SenderAddress { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
    public bool HasAttachment { get; set; }
    public string? AttachmentName { get; set; }
}

public class KimDirectoryEntryDto
{
    public string KimAddress { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Organization { get; set; }
    public string? Specialty { get; set; }
    public string? City { get; set; }
}
