using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal sealed class AccountRepo(
    AppDbContext db
) : IAccountRepo
{
    public void Add(Account account) => db.Accounts.Add(account);
    public Task<Account?> FindByPersonIdAsync(Guid personId, CancellationToken ct, bool includes = true, bool track = false)
        => GetQuery(includes, track).FirstOrDefaultAsync(f => f.PersonId == personId, ct);
    public Task<Account?> FindByIdAsync(Guid id, CancellationToken ct, bool includes = true, bool track = false)
        => GetQuery(includes, track).FirstOrDefaultAsync(f => f.Id == id, ct);
    public Task<Account?> FindByLoginAsync(string email, CancellationToken ct, bool includes = true, bool track = false)
        => GetQuery(includes, track).FirstOrDefaultAsync(f => f.Person.Email.Equals(email), ct);

    private IQueryable<Account> GetQuery(bool includes = true, bool track = false)
    {
        var query = db.Accounts.AsQueryable();
        if (includes)
        {
            query = query
                .Include(i => i.Person)
                    .ThenInclude(i => i.Roles)
                        .ThenInclude(i => i.RoleInfo.Permissions)
                .AsSplitQuery();
        }
        if (!track)
            query = query.AsNoTracking();

        return query;
    }
}
