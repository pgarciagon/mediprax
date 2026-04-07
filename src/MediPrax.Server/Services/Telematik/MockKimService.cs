using MediPrax.Application.Interfaces.Telematik;

namespace MediPrax.Server.Services.Telematik;

public class MockKimService : IKimService
{
    private static readonly List<MockKimMessage> _inbox =
    [
        new()
        {
            MessageId = "kim-001", SenderAddress = "dr.weber@klinikum-bremen.kim.telematik",
            SenderName = "Dr. Annette Weber", Subject = "Befundbericht — Patient Müller, Hans",
            Body = "Sehr geehrte Kollegin, sehr geehrter Kollege,\n\nanbei übersende ich Ihnen den Befundbericht zu o.g. Patienten.\nMRT-Schädel vom 15.03.2026: kein pathologischer Befund.\n\nMit freundlichen kollegialen Grüßen\nDr. Annette Weber\nAbteilung Radiologie, Klinikum Bremen-Mitte",
            ReceivedAt = DateTime.UtcNow.AddDays(-2), IsRead = true, HasAttachment = true, AttachmentName = "Befundbericht_Mueller.pdf"
        },
        new()
        {
            MessageId = "kim-002", SenderAddress = "praxis.hoffmann@neurologie-oldenburg.kim.telematik",
            SenderName = "Dr. Stefan Hoffmann", Subject = "Überweisungsanfrage — Frau Schmidt",
            Body = "Sehr geehrte Kollegin, sehr geehrter Kollege,\n\nich überweise Ihnen Frau Maria Schmidt (geb. 12.05.1968) zur psychiatrischen Mitbeurteilung.\nAnamnese: seit 3 Monaten zunehmende Schlafstörungen und Antriebslosigkeit.\nBisherige Medikation: keine psychotrope Medikation.\n\nMit freundlichen Grüßen\nDr. Stefan Hoffmann\nFacharzt für Neurologie, Oldenburg",
            ReceivedAt = DateTime.UtcNow.AddHours(-6), IsRead = false, HasAttachment = false
        },
        new()
        {
            MessageId = "kim-003", SenderAddress = "labor@laborbremen.kim.telematik",
            SenderName = "Labor Bremen", Subject = "Laborbefund — Auftrag 2026-04-B2847",
            Body = "Laborbefund zu Auftrag 2026-04-B2847\nPatient: Meier, Wolfgang (geb. 03.11.1955)\n\nTSH: 2.4 mU/l (Ref: 0.4-4.0) — normal\nVitamin B12: 289 pg/ml (Ref: 200-900) — normal\nFolsäure: 7.2 ng/ml (Ref: >3.0) — normal\nBlutsenkung: 12 mm/h (Ref: <15) — normal",
            ReceivedAt = DateTime.UtcNow.AddHours(-1), IsRead = false, HasAttachment = true, AttachmentName = "Laborbefund_2026-04-B2847.pdf"
        }
    ];

    private static readonly List<MockKimMessage> _sent = [];

    public async Task<KimSendResultDto> SendAsync(KimMessageDto message, CancellationToken ct = default)
    {
        await Task.Delay(500, ct);
        var id = $"kim-sent-{Guid.NewGuid():N}"[..16];
        _sent.Add(new MockKimMessage
        {
            MessageId = id,
            SenderAddress = "praxis@neuropsych-bremen.kim.telematik",
            SenderName = "Neuropsychiatricum Bremen",
            Subject = message.Subject,
            Body = message.Body,
            ReceivedAt = DateTime.UtcNow
        });

        return new KimSendResultDto { Success = true, MessageId = id };
    }

    public async Task<IReadOnlyList<KimInboxItemDto>> GetInboxAsync(CancellationToken ct = default)
    {
        await Task.Delay(300, ct);
        return _inbox
            .OrderByDescending(m => m.ReceivedAt)
            .Select(m => new KimInboxItemDto
            {
                MessageId = m.MessageId,
                SenderAddress = m.SenderAddress,
                SenderName = m.SenderName,
                Subject = m.Subject,
                ReceivedAt = m.ReceivedAt,
                IsRead = m.IsRead,
                HasAttachment = m.HasAttachment
            })
            .ToList();
    }

    public async Task<KimMessageDetailDto?> GetMessageAsync(string messageId, CancellationToken ct = default)
    {
        await Task.Delay(200, ct);
        var msg = _inbox.FirstOrDefault(m => m.MessageId == messageId);
        if (msg is null) return null;
        msg.IsRead = true;

        return new KimMessageDetailDto
        {
            MessageId = msg.MessageId,
            SenderAddress = msg.SenderAddress,
            SenderName = msg.SenderName,
            Subject = msg.Subject,
            Body = msg.Body,
            ReceivedAt = msg.ReceivedAt,
            HasAttachment = msg.HasAttachment,
            AttachmentName = msg.AttachmentName
        };
    }

    public async Task<IReadOnlyList<KimDirectoryEntryDto>> SearchDirectoryAsync(string query, CancellationToken ct = default)
    {
        await Task.Delay(300, ct);
        var lower = query.ToLower();
        return Directory
            .Where(d => d.DisplayName.ToLower().Contains(lower) || d.Organization?.ToLower().Contains(lower) == true)
            .ToList();
    }

    private static readonly KimDirectoryEntryDto[] Directory =
    [
        new() { KimAddress = "dr.weber@klinikum-bremen.kim.telematik", DisplayName = "Dr. Annette Weber", Organization = "Klinikum Bremen-Mitte", Specialty = "Radiologie", City = "Bremen" },
        new() { KimAddress = "praxis.hoffmann@neurologie-oldenburg.kim.telematik", DisplayName = "Dr. Stefan Hoffmann", Organization = "Praxis Hoffmann", Specialty = "Neurologie", City = "Oldenburg" },
        new() { KimAddress = "labor@laborbremen.kim.telematik", DisplayName = "Labor Bremen", Organization = "Labor Bremen GmbH", Specialty = "Labormedizin", City = "Bremen" },
        new() { KimAddress = "dr.peters@psychiatrie-hb.kim.telematik", DisplayName = "Dr. Claudia Peters", Organization = "Klinikum Bremen-Ost", Specialty = "Psychiatrie", City = "Bremen" },
        new() { KimAddress = "rehazentrum@reha-bremen.kim.telematik", DisplayName = "Rehazentrum Bremen", Organization = "Rehazentrum Bremen GmbH", Specialty = "Rehabilitation", City = "Bremen" },
    ];

    private class MockKimMessage
    {
        public string MessageId { get; set; } = string.Empty;
        public string SenderAddress { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime ReceivedAt { get; set; }
        public bool IsRead { get; set; }
        public bool HasAttachment { get; set; }
        public string? AttachmentName { get; set; }
    }
}
