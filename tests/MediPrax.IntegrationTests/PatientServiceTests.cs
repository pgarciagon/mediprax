using MediPrax.Application.DTOs;
using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;

namespace MediPrax.IntegrationTests;

[Collection("Postgres")]
public class PatientServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly PatientService _sut;

    public PatientServiceTests(PostgresFixture postgres)
    {
        _factory = new TestDbContextFactory(postgres.ConnectionString);
        _sut = new PatientService(_factory.Context);
    }

    [Fact]
    public async Task CreateAsync_CreatesPatient()
    {
        var dto = new CreatePatientDto
        {
            FirstName = "Max",
            LastName = "Mustermann",
            DateOfBirth = new DateOnly(1985, 3, 15),
            InsuranceType = InsuranceType.GKV
        };

        var result = await _sut.CreateAsync(dto);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Max", result.FirstName);
        Assert.Equal("Mustermann", result.LastName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsPatient()
    {
        var created = await _sut.CreateAsync(new CreatePatientDto
        {
            FirstName = "Anna",
            LastName = "Schmidt",
            DateOfBirth = new DateOnly(1990, 7, 20)
        });

        var found = await _sut.GetByIdAsync(created.Id);

        Assert.NotNull(found);
        Assert.Equal("Anna", found.FirstName);
    }

    [Fact]
    public async Task SearchAsync_FindsByName()
    {
        await _sut.CreateAsync(new CreatePatientDto
        {
            FirstName = "Klaus",
            LastName = "Weber",
            DateOfBirth = new DateOnly(1970, 1, 1)
        });

        var result = await _sut.SearchAsync(new PatientSearchDto
        {
            SearchTerm = "Weber",
            Page = 1,
            PageSize = 10
        });

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Weber", result.Items[0].LastName);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletes()
    {
        var created = await _sut.CreateAsync(new CreatePatientDto
        {
            FirstName = "Test",
            LastName = "Delete",
            DateOfBirth = new DateOnly(2000, 1, 1)
        });

        await _sut.DeleteAsync(created.Id);

        // Soft deleted: not found via search (query filter applies)
        var result = await _sut.SearchAsync(new PatientSearchDto
        {
            SearchTerm = "Delete", Page = 1, PageSize = 10
        });
        Assert.Equal(0, result.TotalCount);
    }

    public void Dispose() => _factory.Dispose();
}
