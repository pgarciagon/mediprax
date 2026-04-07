using MediPrax.Application.DTOs;

namespace MediPrax.Application.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<DoctorListItemDto>> GetDoctorsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<UserListItemDto>> GetAllAsync(CancellationToken ct = default);
    Task<UserDetailDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserDetailDto> CreateAsync(CreateUserDto dto, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpdateUserDto dto, CancellationToken ct = default);
    Task ResetPasswordAsync(Guid id, ResetPasswordDto dto, CancellationToken ct = default);
    Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken ct = default);
    Task DeactivateAsync(Guid id, CancellationToken ct = default);
}
