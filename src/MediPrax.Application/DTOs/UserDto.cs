using MediPrax.Core.Enums;

namespace MediPrax.Application.DTOs;

public class DoctorListItemDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
}
