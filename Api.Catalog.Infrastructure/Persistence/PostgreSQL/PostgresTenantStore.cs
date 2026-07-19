using Api.Catalog.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class PostgresTenantStore(
    ICacheService cache,
    AppDbContext db
) : ITenantStore
{
    public async Task<Guid?> GetTenantIdBySlugAsync(string slug, CancellationToken ct)
    {
        return await cache.GetOrCreateAsync(
            $"TENANT_ID_BY_SLUG:{slug}",
            async (cacheCt) =>
            {
                return (await db.Tenants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Slug == slug, cacheCt))?
                    .Id;
            },
            ct
        );
    }

    public async Task<bool> TenantExistsAsync(Guid tenantId, CancellationToken ct)
    {
        return await cache.GetOrCreateAsync(
            $"TENANT_EXISTS:{tenantId}",
            async (cacheCt) =>
            {
                return (await db.Tenants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == tenantId, cacheCt)
                ) is not null;
            },
            ct
        );
    }
}
