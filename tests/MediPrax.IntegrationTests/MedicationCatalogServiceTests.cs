using MediPrax.Application.Services;
using MediPrax.Core.Entities;
using MediPrax.Core.Enums;

namespace MediPrax.IntegrationTests;

[Collection("Postgres")]
public class MedicationCatalogServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly MedicationCatalogService _sut;

    public MedicationCatalogServiceTests(PostgresFixture postgres)
    {
        _factory = new TestDbContextFactory(postgres.ConnectionString);
        _sut = new MedicationCatalogService(_factory.Context);

        // Seed test catalog entries
        _factory.Context.Set<MedicationCatalogEntry>().AddRange(
            new MedicationCatalogEntry
            {
                Id = Guid.NewGuid(), Pzn = "99900001", Handelsname = "Sertralin HEXAL 50mg",
                Wirkstoff = "Sertralin", AtcCode = "N06AB06", Staerke = "50 mg",
                Darreichungsform = "Filmtabletten", Packungsgroesse = "20 Stk",
                NormPackungsgroesse = "N1", Hersteller = "HEXAL",
                Category = MedicationCategory.Antidepressivum, IsActive = true
            },
            new MedicationCatalogEntry
            {
                Id = Guid.NewGuid(), Pzn = "99900002", Handelsname = "Sertralin HEXAL 100mg",
                Wirkstoff = "Sertralin", AtcCode = "N06AB06", Staerke = "100 mg",
                Darreichungsform = "Filmtabletten", Packungsgroesse = "50 Stk",
                NormPackungsgroesse = "N2", Hersteller = "HEXAL",
                Category = MedicationCategory.Antidepressivum, IsActive = true
            },
            new MedicationCatalogEntry
            {
                Id = Guid.NewGuid(), Pzn = "99900003", Handelsname = "Tavor 1mg",
                Wirkstoff = "Lorazepam", AtcCode = "N05BA06", Staerke = "1 mg",
                Darreichungsform = "Tabletten", Packungsgroesse = "20 Stk",
                NormPackungsgroesse = "N1", Hersteller = "Pfizer",
                IsBtm = true, Category = MedicationCategory.Anxiolytikum, IsActive = true
            },
            new MedicationCatalogEntry
            {
                Id = Guid.NewGuid(), Pzn = "99900004", Handelsname = "Leponex 100mg",
                Wirkstoff = "Clozapin", AtcCode = "N05AH02", Staerke = "100 mg",
                Darreichungsform = "Tabletten", Packungsgroesse = "50 Stk",
                NormPackungsgroesse = "N2", Hersteller = "Novartis",
                Category = MedicationCategory.Antipsychotikum, IsActive = true
            },
            new MedicationCatalogEntry
            {
                Id = Guid.NewGuid(), Pzn = "99900005", Handelsname = "Keppra 500mg",
                Wirkstoff = "Levetiracetam", AtcCode = "N03AX14", Staerke = "500 mg",
                Darreichungsform = "Filmtabletten", Packungsgroesse = "100 Stk",
                NormPackungsgroesse = "N3", Hersteller = "UCB",
                Category = MedicationCategory.Antikonvulsivum, IsActive = true
            },
            new MedicationCatalogEntry
            {
                Id = Guid.NewGuid(), Pzn = "99900006", Handelsname = "Madopar 125",
                Wirkstoff = "Levodopa/Benserazid", AtcCode = "N04BA02", Staerke = "100/25 mg",
                Darreichungsform = "Kapseln", Packungsgroesse = "30 Stk",
                NormPackungsgroesse = "N1", Hersteller = "Roche",
                Category = MedicationCategory.Parkinsonmittel, IsActive = true
            },
            new MedicationCatalogEntry
            {
                Id = Guid.NewGuid(), Pzn = "99900007", Handelsname = "InactiveOld 10mg",
                Wirkstoff = "OldSubstance", AtcCode = "N99XX99", Staerke = "10 mg",
                Darreichungsform = "Tabletten", Packungsgroesse = "20 Stk",
                Hersteller = "OldPharma",
                Category = MedicationCategory.Sonstiges, IsActive = false
            }
        );
        _factory.Context.SaveChanges();
    }

    [Fact]
    public async Task SearchAsync_ByHandelsname_ReturnsMatches()
    {
        var results = await _sut.SearchAsync("Sertralin");
        Assert.Equal(2, results.Count);
        Assert.All(results, r => Assert.Contains("Sertralin", r.Handelsname));
    }

    [Fact]
    public async Task SearchAsync_ByWirkstoff_ReturnsMatches()
    {
        var results = await _sut.SearchAsync("Lorazepam");
        Assert.Single(results);
        Assert.Equal("Tavor 1mg", results[0].Handelsname);
    }

    [Fact]
    public async Task SearchAsync_ByPzn_ReturnsMatches()
    {
        var results = await _sut.SearchAsync("99900003");
        Assert.Single(results);
        Assert.Equal("Tavor 1mg", results[0].Handelsname);
    }

    [Fact]
    public async Task SearchAsync_ShortTerm_ReturnsEmpty()
    {
        var results = await _sut.SearchAsync("S");
        Assert.Empty(results);
    }

    [Fact]
    public async Task SearchAsync_ExcludesInactive()
    {
        var results = await _sut.SearchAsync("InactiveOld");
        Assert.Empty(results);
    }

    [Fact]
    public async Task GetByPznAsync_ExistingPzn_ReturnsEntry()
    {
        var result = await _sut.GetByPznAsync("99900004");
        Assert.NotNull(result);
        Assert.Equal("Leponex 100mg", result.Handelsname);
        Assert.Equal("Clozapin", result.Wirkstoff);
    }

    [Fact]
    public async Task GetByPznAsync_NonExistentPzn_ReturnsNull()
    {
        var result = await _sut.GetByPznAsync("00000000");
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByWirkstoffAsync_ReturnsAllMatches()
    {
        var results = await _sut.GetByWirkstoffAsync("Sertralin");
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task GetByAtcPrefixAsync_ReturnsMatches()
    {
        var results = await _sut.GetByAtcPrefixAsync("N06AB");
        Assert.Equal(2, results.Count); // Sertralin 50mg + 100mg
    }

    [Fact]
    public async Task GetByAtcPrefixAsync_BroadPrefix_ReturnsMore()
    {
        var results = await _sut.GetByAtcPrefixAsync("N0");
        // Should match all active entries with N0x ATC codes
        Assert.True(results.Count >= 5);
    }

    [Fact]
    public async Task GetByCategoryAsync_ReturnsCorrectCategory()
    {
        var results = await _sut.GetByCategoryAsync(MedicationCategory.Antidepressivum);
        Assert.Equal(2, results.Count);
        Assert.All(results, r => Assert.Equal(MedicationCategory.Antidepressivum, r.Category));
    }

    [Fact]
    public async Task GetByCategoryAsync_ExcludesInactive()
    {
        var results = await _sut.GetByCategoryAsync(MedicationCategory.Sonstiges);
        Assert.Empty(results); // Only inactive entry is Sonstiges
    }

    [Fact]
    public async Task GetStatisticsAsync_ReturnsCorrectCounts()
    {
        var stats = await _sut.GetStatisticsAsync();
        Assert.Equal(7, stats.TotalEntries);
        Assert.Equal(6, stats.ActiveEntries);
        Assert.Equal(1, stats.BtmCount);
        Assert.True(stats.ByCategory.ContainsKey("Antidepressivum"));
        Assert.Equal(2, stats.ByCategory["Antidepressivum"]);
    }

    public void Dispose() => _factory.Dispose();
}
