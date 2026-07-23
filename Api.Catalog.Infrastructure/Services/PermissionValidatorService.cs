using Api.Catalog.Application.Contracts.Contexts;
using Api.Catalog.Domain.ValueObjects;
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
    public async Task<bool> HasPermission(PermissionInfo permission, CancellationToken ct)
    {
        if (personContext.PersonId is null || (!tenantContext.IsPlatformContext && tenantContext.TenantId is null))
            return false;

        if (tenantContext.IsPlatformContext)
            return await ValidatePlatformPermission(permission, ct);

        return await ValidateTenantPermission(permission, ct);
    }
    private async Task<bool> ValidatePlatformPermission(PermissionInfo permission, CancellationToken ct)
    {
        var personId = personContext.PersonId;
        var personRoles = await cache.GetOrCreateAsync(
            $"PLATFORM_PERSON:{personId}:ROLES",
            (cacheCt) => PlatformUserRoles(db, personId, cacheCt),
            ct
        );
        foreach (var role in personRoles ?? [])
        {
            var rolePermissions = await cache.GetOrCreateAsync(
                $"",
                (cacheCt) => PlatformRolePermissions(role, cacheCt),
                ct
            ) ?? [];
            if (rolePermissions.Contains(permission))
                return true;
        }
        return false;
    }
    private async Task<bool> ValidateTenantPermission(PermissionInfo permission, CancellationToken ct)
    {
        var tenantId = tenantContext.TenantId;
        var personId = personContext.PersonId;
        var personRoles = await cache.GetOrCreateAsync(
            $"TENANT:{tenantId}:PERSON:{personId}:ROLES",
            (cacheCt) => TenantUserRoles(db, personId, cacheCt),
            ct
        );
        foreach (var role in personRoles ?? [])
        {
            var rolePermissions = await cache.GetOrCreateAsync(
                $"TENANT:{tenantContext.TenantId}:ROLE:{role}:PERMISSIONS",
                (cacheCt) => TenantRolePermissions(role, cacheCt),
                ct
            ) ?? [];
            if (rolePermissions.Contains(permission))
                return true;
        }
        return false;
    }
    private Task<List<Guid>> PlatformUserRoles(AppDbContext db, Guid? personId, CancellationToken cacheCt)
    {
        return db.Persons
            .AsNoTracking()
            .Where(p => p.Id == personId)
            .SelectMany(p => p.PlatformRoles.Select(pr => pr.Id))
            .ToListAsync(cacheCt);
    }
    private Task<List<Guid>> TenantUserRoles(AppDbContext db, Guid? personId, CancellationToken cacheCt)
    {
        return db.Persons
            .AsNoTracking()
            .Where(p => p.Id == personId)
            .SelectMany(p => p.TenantRoles.Select(pr => pr.Id))
            .ToListAsync(cacheCt);
    }
    private Task<IReadOnlyCollection<PermissionInfo>?> PlatformRolePermissions(Guid roleId, CancellationToken cacheCt)
    {
        return db.PlatformRoles
            .AsNoTracking()
            .Where(r => r.Id == roleId)
            .Select(r => r.RoleInfo.Permissions)
            .FirstOrDefaultAsync(cacheCt);
    }
    private Task<IReadOnlyCollection<PermissionInfo>?> TenantRolePermissions(Guid roleId, CancellationToken cacheCt)
    {
        return db.TenantRoles
            .AsNoTracking()
            .Where(r => r.Id == roleId)
            .Select(r => r.RoleInfo.Permissions)
            .FirstOrDefaultAsync(cacheCt);
    }
}
