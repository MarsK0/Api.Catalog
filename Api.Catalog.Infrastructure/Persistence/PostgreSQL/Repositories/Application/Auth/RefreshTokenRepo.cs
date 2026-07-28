using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal sealed class RefreshTokenRepo(
    AppDbContext db
) : IRefreshTokenRepo
{
    public void Add(RefreshToken refreshToken) => db.RefreshTokens.Add(refreshToken);
    public Task<RefreshToken?> GetByHashAsync(string hash, CancellationToken ct, bool track = false)
        => GetQuery(track).FirstOrDefaultAsync(f => f.TokenHash.Equals(hash), ct);
    public Task<List<RefreshToken>> GetByFamilyIdAsync(Guid familyId, CancellationToken ct, bool track = false)
        => GetQuery(track).ToListAsync(ct);

    private IQueryable<RefreshToken> GetQuery(bool track = false)
    {
        var query = db.RefreshTokens.AsQueryable();

        if (!track)
            query = query.AsNoTracking();

        return query;
    }
}
