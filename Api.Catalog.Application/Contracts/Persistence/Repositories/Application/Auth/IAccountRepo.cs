using Api.Catalog.Application.Entities;

namespace Api.Catalog.Application.Contracts;

public interface IAccountRepo
{
    void Add(Account account);
    Task<Account?> FindByPersonIdAsync(Guid personId, CancellationToken ct, bool includes = true, bool track = false);
    Task<Account?> FindByIdAsync(Guid id, CancellationToken ct, bool includes = true, bool track = false);
    Task<Account?> FindByLoginAsync(string email, CancellationToken ct, bool includes = true, bool track = false);
}
