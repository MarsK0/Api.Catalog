using Api.Catalog.Application.Contracts;
using Microsoft.EntityFrameworkCore.Storage;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal sealed class EFTransaction : ITransaction
{
    private readonly IDbContextTransaction _transaction;

    public EFTransaction(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public Task CommitAsync(CancellationToken ct = default)
        => _transaction.CommitAsync(ct);

    public Task RollbackAsync(CancellationToken ct = default)
        => _transaction.RollbackAsync(ct);

    public ValueTask DisposeAsync()
        => _transaction.DisposeAsync();
}
