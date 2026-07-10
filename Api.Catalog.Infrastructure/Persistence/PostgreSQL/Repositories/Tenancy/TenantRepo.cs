using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Contracts.Contexts;
using Api.Catalog.Domain.Entities;
using Api.Catalog.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal sealed class TenantRepo(
    AppDbContext db,
    ITenantContext tenantContext,
    ICacheService cache
) : ITenantRepo
{
    public async Task CreateAsync(Tenant tenant, CancellationToken ct)
    {
        await db.Tenants.AddAsync(tenant, ct);
    }
    public async Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct, bool includes = true, bool track = false)
    {
        return await GetQuery(includes, track).FirstOrDefaultAsync(t => t.Id == id, ct);
    }
    public async Task<Tenant?> GetBySlugAsync(string slug, CancellationToken ct, bool includes = true, bool track = false)
    {
        return await GetQuery(includes, track).FirstOrDefaultAsync(t => t.Slug.Equals(slug));
    }
    public async Task<List<string>> GetModulesAsync(CancellationToken ct)
    {
        var tenantId = tenantContext.TenantId;

        string cacheKey = $"tenant:{tenantId}:Modules";
        return await cache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                return await db.TenantModules
                    .Select(s => s.ModuleCode)
                    .ToListAsync(ct);
            }
        ) ?? [];
    }
    private IQueryable<Tenant> GetQuery(bool includes = true, bool track = false)
    {
        var query = db.Tenants.AsQueryable();
        if (includes)
        {
            query = query
                .Include("Persons")
                .Include("Modules");
        }
        if (!track)
            query = query.AsNoTracking();

        return query;
    }
}
