using MediPrax.Core.Interfaces;

namespace MediPrax.Infrastructure.Persistence;

public class UnitOfWork(MediPraxDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => context.SaveChangesAsync(ct);

    public void Dispose() => context.Dispose();
}
