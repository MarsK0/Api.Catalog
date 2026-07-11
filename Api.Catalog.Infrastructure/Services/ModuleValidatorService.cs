using Api.Catalog.Application.Contracts.Contexts;
using Api.Catalog.Infrastructure.Contracts;
using Api.Catalog.Infrastructure.Persistence.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace Api.Catalog.Infrastructure.Services;

internal sealed class ModuleValidatorService(
    ITenantContext tenantContext,
    ICacheService cache,
    AppDbContext db
) : IModuleValidator
{
    public async Task<bool> IsModuleUnlocked(string module, CancellationToken ct)
    {
        if (tenantContext.TenantId is null)
            return false;

        return (await cache.GetOrCreateAsync(
            $"{tenantContext.TenantId}:MODULES",
            async (cacheCt) => await db.TenantModules
                .Select(s => s.ModuleCode)
                .ToListAsync(cacheCt),
            ct
        ))
        .Any(a => a.Equals(module, StringComparison.OrdinalIgnoreCase));
    }
}
