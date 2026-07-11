using Api.Catalog.Application.Contracts.Contexts;
using Api.Catalog.Infrastructure.Contracts;
using Api.Catalog.Infrastructure.Persistence.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace Api.Catalog.Infrastructure.Services;

internal sealed class PermissionValidatorService(
    IPersonContext personContext,
    ITenantContext tenantContext,
    ICacheService cache,
    AppDbContext db

) : IPermissionValidator
{
    public async Task<bool> HasPermission(string permission, CancellationToken ct)
    {
        if (personContext.PersonId is null || (!tenantContext.IsPlatformContext && tenantContext.TenantId is null))
            return false;

        if (tenantContext.IsPlatformContext)
            return await ValidatePlatformPermission(permission, cache, db, personContext, ct);

        return await ValidateTenantPermission(permission, cache, db, personContext, tenantContext, ct);
    }
    private static async Task<bool> ValidatePlatformPermission(string permission, ICacheService cache, AppDbContext db, IPersonContext personContext, CancellationToken ct)
    {
        var personRoles = await cache.GetOrCreateAsync(
            $"PLATFORM_PERSON:{personContext.PersonId}:ROLES",
            (cacheCt) => PlatformUserRoles(db, personContext, cacheCt),
            ct
        );
        foreach(var role in personRoles ?? [])
        {
            var rolePermissions = await cache.GetOrCreateAsync(
                $"",
                (cacheCt) => PlatformRolePermissions(role, db, cacheCt),
                ct
            ) ?? [];
            if (rolePermissions.Contains(permission))
                return true;
        }
        return false;
    }
    private static async Task<bool> ValidateTenantPermission(string permission, ICacheService cache, AppDbContext db, IPersonContext personContext, ITenantContext tenantContext, CancellationToken ct)
    {
        var personRoles = await cache.GetOrCreateAsync(
            $"TENANT:{tenantContext.TenantId}:PERSON:{personContext.PersonId}:ROLES",
            (cacheCt) => TenantUserRoles(db, personContext, cacheCt),
            ct
        );
        foreach (var role in personRoles ?? [])
        {
            var rolePermissions = await cache.GetOrCreateAsync(
                $"TENANT:{tenantContext.TenantId}:ROLE:{role}:PERMISSIONS",
                (cacheCt) => TenantRolePermissions(role, db, cacheCt),
                ct
            ) ?? [];
            if (rolePermissions.Contains(permission))
                return true;
        }
        return false;
    }
    private static Task<List<Guid>> PlatformUserRoles(AppDbContext db, IPersonContext personContext, CancellationToken cacheCt)
    {
        return db.Persons
            .AsNoTracking()
            .Where(p => p.Id == personContext.PersonId)
            .SelectMany(p => p.PlatformRoles.Select(pr => pr.Id))
            .ToListAsync(cacheCt);
    }
    private static Task<List<Guid>> TenantUserRoles(AppDbContext db, IPersonContext personContext, CancellationToken cacheCt)
    {
        return db.Persons
            .AsNoTracking()
            .Where(p => p.Id == personContext.PersonId)
            .SelectMany(p => p.TenantRoles.Select(pr => pr.Id))
            .ToListAsync(cacheCt);
    }
    private static Task<IReadOnlyList<string>?> PlatformRolePermissions(Guid roleId, AppDbContext db, CancellationToken cacheCt)
    {
        return db.PlatformRoles
            .AsNoTracking()
            .Where(r => r.Id == roleId)
            .Select(r => r.RoleInfo.Permissions)
            .FirstOrDefaultAsync(cacheCt);
    }
    private static Task<IReadOnlyList<string>?> TenantRolePermissions(Guid roleId, AppDbContext db, CancellationToken cacheCt)
    {
        return db.TenantRoles
            .AsNoTracking()
            .Where(r => r.Id == roleId)
            .Select(r => r.RoleInfo.Permissions)
            .FirstOrDefaultAsync(cacheCt);
    }
}
