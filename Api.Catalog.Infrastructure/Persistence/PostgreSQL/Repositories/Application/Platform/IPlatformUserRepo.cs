using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal sealed class PlatformUserRepo(
    AppDbContext db
) : IPlatformUserRepo
{
    public Task<PlatformUser?> FindByLoginAsync(string login, CancellationToken ct, bool includes = true, bool track = false)
        => GetQuery(includes, track).FirstOrDefaultAsync(f => f.Login == login, ct);

    public Task<PlatformUser?> FindByUserIdAsync(Guid userId, CancellationToken ct, bool includes = true, bool track = false)
        => GetQuery(includes, track).FirstOrDefaultAsync(f => f.Id == userId, ct);

    private IQueryable<PlatformUser> GetQuery(bool includes = true, bool track = false)
    {
        var query = db.PlatformUsers.AsQueryable();
        if (includes)
        {
            query = query
                .Include(i => i.Roles)
                    .ThenInclude(i => i.RoleInfo.Permissions)
                .AsSplitQuery();
        }
        if (!track)
            query = query.AsNoTracking();

        return query;
    }
}
