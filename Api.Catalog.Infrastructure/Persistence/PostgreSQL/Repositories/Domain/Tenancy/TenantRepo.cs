using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Contracts.Contexts;
using Api.Catalog.Domain.Entities;
using Api.Catalog.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal sealed class TenantRepo(
    AppDbContext db,
    ITenantContext tenantContext,
    ICacheService cache
) : ITenantRepo
{
    public void Add(Tenant tenant) => db.Tenants.Add(tenant);
    public Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct, bool includes = true, bool track = false)
        => GetQuery(includes, track).FirstOrDefaultAsync(t => t.Id == id, ct);
    public Task<Tenant?> GetBySlugAsync(string slug, CancellationToken ct, bool includes = true, bool track = false)
        => GetQuery(includes, track).FirstOrDefaultAsync(t => t.Slug.Equals(slug), ct);
    public async Task<List<string>> GetModulesAsync(CancellationToken ct)
    {
        var tenantId = tenantContext.TenantId;

        string cacheKey = $"TENANT:{tenantId}:MODULES";
        return await cache.GetOrCreateAsync(
            cacheKey,
            (cacheCt) => db.TenantModules
                    .Select(s => s.ModuleCode)
                    .ToListAsync(cacheCt),
            ct
        ) ?? [];
    }
    private IQueryable<Tenant> GetQuery(bool includes = true, bool track = false)
    {
        var query = db.Tenants.AsQueryable();
        if (includes)
            query = query.Include(i => i.Modules);
        if (!track)
            query = query.AsNoTracking();

        return query;
    }
}
