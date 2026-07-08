using Api.Catalog.Application.Contracts;
using Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal sealed class RefreshTokenRepo(
    AppDbContext db
) : IRefreshTokenRepo
{
    public async Task AddAsync(RefreshToken refreshToken, CancellationToken ct)
    {
        await db.RefreshTokens.AddAsync(refreshToken, ct);
    }
    public async Task<RefreshToken?> GetByHashAsync(string hash, CancellationToken ct)
    {
        return await db.RefreshTokens
            .Include("Person")
            .FirstOrDefaultAsync(f => f.TokenHash.Equals(hash), ct);
    }
    public async Task<List<RefreshToken>> GetByFamilyIdAsync(Guid familyId, CancellationToken ct)
    {
        return await db.RefreshTokens
            .Where(w => w.FamilyId == familyId)
            .ToListAsync();
    }
}
