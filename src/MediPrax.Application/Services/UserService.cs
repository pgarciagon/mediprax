using MediPrax.Application.DTOs;
using MediPrax.Application.Interfaces;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPrax.Application.Services;

public class UserService(DbContext context) : IUserService
{
    private DbSet<User> Users => context.Set<User>();

    public async Task<IReadOnlyList<DoctorListItemDto>> GetDoctorsAsync(CancellationToken ct = default)
    {
        return await Users
            .Where(u => u.Role == UserRole.Arzt && u.IsActive)
            .OrderBy(u => u.LastName)
            .Select(u => new DoctorListItemDto
            {
                Id = u.Id,
                FullName = u.FirstName + " " + u.LastName
            })
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<UserListItemDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await Users
            .OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
            .Select(u => new UserListItemDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                FullName = u.FirstName + " " + u.LastName,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync(ct);
    }

    public async Task<UserDetailDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await Users
            .Where(u => u.Id == id)
            .Select(u => new UserDetailDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<UserDetailDto> CreateAsync(CreateUserDto dto, CancellationToken ct = default)
    {
        if (await Users.AnyAsync(u => u.Email == dto.Email, ct))
            throw new InvalidOperationException("Ein Benutzer mit dieser E-Mail existiert bereits.");

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role
        };

        Users.Add(user);
        await context.SaveChangesAsync(ct);

        return new UserDetailDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task UpdateAsync(Guid id, UpdateUserDto dto, CancellationToken ct = default)
    {
        var user = await Users.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"Benutzer {id} nicht gefunden.");

        if (await Users.AnyAsync(u => u.Email == dto.Email && u.Id != id, ct))
            throw new InvalidOperationException("Ein anderer Benutzer mit dieser E-Mail existiert bereits.");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;
        user.Role = dto.Role;
        user.IsActive = dto.IsActive;
        user.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(ct);
    }

    public async Task ResetPasswordAsync(Guid id, ResetPasswordDto dto, CancellationToken ct = default)
    {
        var user = await Users.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"Benutzer {id} nicht gefunden.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(ct);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken ct = default)
    {
        var user = await Users.FindAsync([userId], ct)
            ?? throw new KeyNotFoundException("Benutzer nicht gefunden.");

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            throw new InvalidOperationException("Das aktuelle Passwort ist falsch.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(ct);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken ct = default)
    {
        var user = await Users.FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"Benutzer {id} nicht gefunden.");

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(ct);
    }
}
