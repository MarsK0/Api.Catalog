using Api.Catalog.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal class PostgresTenantStore : ITenantStore
{
    private readonly ICacheService _cache;
    private readonly AppDbContext _db;
    public PostgresTenantStore(
        ICacheService cache,
        AppDbContext db
    )
    {
        _cache = cache;
        _db = db;
    }
    public async Task<Guid?> GetTenantIdBySlugAsync(string slug)
    {
        return await _cache.GetOrCreateAsync(
            $"tenant:{slug}",
            async () => { 
                return await _db.Tenants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Slug == slug)?
                    .Id;
            }
        );
    }

    public async Task<bool> TenantExistsAsync(Guid tenantId)
    {
        return await _cache.GetOrCreateAsync(
            $"tenant_exists:{tenantId}",
            async () =>
            {
                return (await _db.Tenants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == tenantId)
                ) is not null;
            }
        );
    }
}
