using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal sealed class AccountRepo(
    AppDbContext db
) : IAccountRepo
{
    public async Task CreateAsync(Account account, CancellationToken ct)
    {
        await db.Accounts.AddAsync(account, ct);
    }
    public async Task<Account?> FindByPersonIdAsync(Guid personId, CancellationToken ct, bool includes = true, bool track = false)
    {
        return await GetQuery(includes, track).FirstOrDefaultAsync(f => f.PersonId == personId, ct);
    }
    public async Task<Account?> FindByIdAsync(Guid id, CancellationToken ct, bool includes = true, bool track = false)
    {
        return await GetQuery(includes, track).FirstOrDefaultAsync(f => f.Id == id, ct);
    }
    public async Task<Account?> FindByEmailAsync(string email, CancellationToken ct, bool includes = true, bool track = false)
    {
        return await GetQuery(includes, track).FirstOrDefaultAsync(f => f.Person.Email.Equals(email), ct);
    }

    private IQueryable<Account> GetQuery(bool includes = true, bool track = false)
    {
        var query = db.Accounts.AsQueryable();
        if (includes)
        {
            query = query
                .Include(i => i.Person);
        }
        if (!track)
            query = query.AsNoTracking();

        return query;
    }
}
