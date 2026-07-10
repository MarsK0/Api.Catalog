using Api.Catalog.Application.Contracts;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public UnitOfWork(AppDbContext db)
    {
        _db = db;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public async Task<ITransaction> BeginTransactionAsync(
       CancellationToken ct
    )
    {
        var transaction = await _db.Database.BeginTransactionAsync(ct);
        return new EFTransaction(transaction);
    }
}
