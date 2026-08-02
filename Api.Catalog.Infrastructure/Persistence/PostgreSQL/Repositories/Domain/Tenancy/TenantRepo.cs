using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Contracts.Contexts;
using Api.Catalog.Application.Models;
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
    public async Task<PaginatedResponseDto<Tenant>> GetPaginatedListAsync(
        GetTenantPaginatedListQuery query,
        CancellationToken ct,
        bool includes = true,
        bool track = false
    )
    {
        var (index, size, search) = query;
        var q = GetQuery(includes, track)
            .Where(
                w =>
                    search == null ||
                    EF.Functions.ILike(w.Slug, $"%{search}%") ||
                    EF.Functions.ILike(w.Name, $"%{search}%")
            );
        var result = await q
                .OrderByDescending(o => EF.Property<DateTimeOffset>(o, TrackingMetadata.CreatedAt))
                .Select(
                    s => new
                    {
                        Item = s,
                        TotalCount = q.Count()
                    }
                )
                .Skip(index * size)
                .Take(size)
                .ToListAsync(ct);

        return new PaginatedResponseDto<Tenant>(
            Items: result.Select(s => s.Item),
            TotalCount: result.Count > 0 ? result[0].TotalCount : await q.CountAsync(ct)
        );
    }
    public Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct, bool includes = true, bool track = false)
        => GetQuery(includes, track).FirstOrDefaultAsync(t => t.Id == id, ct);
    public Task<Tenant?> GetBySlugAsync(string slug, CancellationToken ct, bool includes = true, bool track = false)
        => GetQuery(includes, track).FirstOrDefaultAsync(t => t.Slug.Equals(slug), ct);
    public void Delete(Tenant tenant)
        => db.Tenants.Remove(tenant);
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
