using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class UserService(DbContext context) : IUserService
{
    public async Task<IReadOnlyList<DoctorListItemDto>> GetDoctorsAsync(CancellationToken ct = default)
    {
        return await context.Set<User>()
            .Where(u => u.Role == UserRole.Arzt && u.IsActive)
            .OrderBy(u => u.LastName)
            .Select(u => new DoctorListItemDto
            {
                Id = u.Id,
                FullName = u.FirstName + " " + u.LastName
            })
            .ToListAsync(ct);
    }
}
