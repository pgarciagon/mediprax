using MediPrax.Core.Entities;

namespace MediPrax.Application.Interfaces;

public interface IAuthService
{
    Task<User?> ValidateCredentialsAsync(string email, string password, CancellationToken ct = default);
}
