using MediPrax.Application.Services;
using MediPrax.Core.Entities;

namespace MediPrax.IntegrationTests;

public class SearchServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly SearchService _sut;

    public SearchServiceTests()
    {
        _factory = new TestDbContextFactory();
        _sut = new SearchService(_factory.Context);

        // Seed test data
        _factory.Context.Set<Patient>().Add(new Patient
        {
            FirstName = "Heinrich", LastName = "Bergmann",
            DateOfBirth = new DateOnly(1960, 4, 15), Kvnr = "X111222333"
        });
        _factory.Context.Set<Patient>().Add(new Patient
        {
            FirstName = "Maria", LastName = "Schneider",
            DateOfBirth = new DateOnly(1975, 9, 3), Phone = "0421-123456"
        });
        _factory.Context.SaveChanges();
    }

    [Fact]
    public async Task SearchAsync_FindsByLastName()
    {
        var result = await _sut.SearchAsync("Bergmann");
        Assert.True(result.TotalCount > 0);
        Assert.Contains(result.Results, r => r.Title.Contains("Bergmann"));
    }

    [Fact]
    public async Task SearchAsync_FindsByKvnr()
    {
        var result = await _sut.SearchAsync("X111222333");
        Assert.True(result.TotalCount > 0);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_ForShortQuery()
    {
        var result = await _sut.SearchAsync("B");
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_ForNoMatch()
    {
        var result = await _sut.SearchAsync("Zzzzzzz");
        Assert.Equal(0, result.TotalCount);
    }

    public void Dispose() => _factory.Dispose();
}
