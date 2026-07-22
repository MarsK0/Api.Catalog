using Api.Catalog.Application.Entities;

namespace Api.Catalog.Application.Contracts;

public interface IAccountRepo
{
    Task CreateAsync(Account account, CancellationToken ct);
    Task<Account?> FindByPersonIdAsync(Guid personId, CancellationToken ct, bool includes = true, bool track = false);
    Task<Account?> FindByIdAsync(Guid id, CancellationToken ct, bool includes = true, bool track = false);
    Task<Account?> FindByEmailAsync(string email, CancellationToken ct, bool includes = true, bool track = false);
}
