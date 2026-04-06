using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<DoctorListItemDto>> GetDoctorsAsync(CancellationToken ct = default);
}
