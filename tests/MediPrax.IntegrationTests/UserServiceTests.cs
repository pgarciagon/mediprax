using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Enums;

namespace MediPrax.IntegrationTests;

[Collection("Postgres")]
public class UserServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly UserService _sut;

    public UserServiceTests(PostgresFixture postgres)
    {
        _factory = new TestDbContextFactory(postgres.ConnectionString);
        _sut = new UserService(_factory.Context);
    }

    [Fact]
    public async Task CreateAsync_CreatesUser()
    {
        var dto = new CreateUserDto
        {
            FirstName = "Test",
            LastName = "Arzt",
            Email = "test@praxis.de",
            Password = "password123",
            Role = UserRole.Arzt
        };

        var result = await _sut.CreateAsync(dto);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Test", result.FirstName);
        Assert.Equal(UserRole.Arzt, result.Role);
    }

    [Fact]
    public async Task CreateAsync_ThrowsOnDuplicateEmail()
    {
        await _sut.CreateAsync(new CreateUserDto
        {
            FirstName = "A", LastName = "B", Email = "dup@praxis.de",
            Password = "pass", Role = UserRole.MFA
        });

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.CreateAsync(new CreateUserDto
            {
                FirstName = "C", LastName = "D", Email = "dup@praxis.de",
                Password = "pass", Role = UserRole.MFA
            }));
    }

    [Fact]
    public async Task GetDoctorsAsync_ReturnsOnlyActiveDoctors()
    {
        await _sut.CreateAsync(new CreateUserDto
        {
            FirstName = "Dr", LastName = "House", Email = "house@praxis.de",
            Password = "pass", Role = UserRole.Arzt
        });
        await _sut.CreateAsync(new CreateUserDto
        {
            FirstName = "Nurse", LastName = "Joy", Email = "joy@praxis.de",
            Password = "pass", Role = UserRole.MFA
        });

        var doctors = await _sut.GetDoctorsAsync();

        Assert.Single(doctors);
        Assert.Equal("House", doctors[0].FullName.Split(' ').Last());
    }

    [Fact]
    public async Task DeactivateAsync_SetsInactive()
    {
        var user = await _sut.CreateAsync(new CreateUserDto
        {
            FirstName = "Deact", LastName = "Test", Email = "deact@praxis.de",
            Password = "pass", Role = UserRole.MFA
        });

        await _sut.DeactivateAsync(user.Id);

        var found = await _sut.GetByIdAsync(user.Id);
        Assert.NotNull(found);
        Assert.False(found.IsActive);
    }

    [Fact]
    public async Task ChangePasswordAsync_ThrowsOnWrongCurrent()
    {
        var user = await _sut.CreateAsync(new CreateUserDto
        {
            FirstName = "Pw", LastName = "Test", Email = "pw@praxis.de",
            Password = "correct", Role = UserRole.MFA
        });

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.ChangePasswordAsync(user.Id, new ChangePasswordDto
            {
                CurrentPassword = "wrong",
                NewPassword = "newpass"
            }));
    }

    public void Dispose() => _factory.Dispose();
}
