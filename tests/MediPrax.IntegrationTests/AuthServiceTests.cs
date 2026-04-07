using MediPrax.Core.Entities;
using MediPrax.Core.Enums;
using MediPrax.Server.Services;

namespace MediPrax.IntegrationTests;

public class AuthServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _factory = new TestDbContextFactory();
        _sut = new AuthService(_factory.Context);

        _factory.Context.Set<User>().Add(new User
        {
            FirstName = "Auth", LastName = "Test",
            Email = "auth@test.de",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct123"),
            Role = UserRole.Arzt
        });
        _factory.Context.SaveChanges();
    }

    [Fact]
    public async Task ValidateCredentialsAsync_ReturnsUser_OnCorrectPassword()
    {
        var user = await _sut.ValidateCredentialsAsync("auth@test.de", "correct123");

        Assert.NotNull(user);
        Assert.Equal("Auth", user.FirstName);
        Assert.Equal(UserRole.Arzt, user.Role);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_ReturnsNull_OnWrongPassword()
    {
        var user = await _sut.ValidateCredentialsAsync("auth@test.de", "wrong");
        Assert.Null(user);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_ReturnsNull_OnUnknownEmail()
    {
        var user = await _sut.ValidateCredentialsAsync("nobody@test.de", "correct123");
        Assert.Null(user);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_ReturnsNull_ForInactiveUser()
    {
        _factory.Context.Set<User>().Add(new User
        {
            FirstName = "Inactive", LastName = "User",
            Email = "inactive@test.de",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"),
            Role = UserRole.MFA,
            IsActive = false
        });
        _factory.Context.SaveChanges();

        var user = await _sut.ValidateCredentialsAsync("inactive@test.de", "pass");
        Assert.Null(user);
    }

    public void Dispose() => _factory.Dispose();
}
