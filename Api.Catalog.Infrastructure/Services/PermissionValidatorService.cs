using Api.Catalog.Application.Contracts.Contexts;
using Api.Catalog.Domain.ValueObjects;
using Api.Catalog.Infrastructure.Contracts;
using Api.Catalog.Infrastructure.Persistence.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace Api.Catalog.Infrastructure.Services;

internal sealed class PermissionValidatorService(
    IUserContext personContext,
    ITenantContext tenantContext,
    ICacheService cache,
    AppDbContext db
) : IPermissionValidator
{
    public async Task<bool> HasPermission(PermissionInfo permission, CancellationToken ct)
    {
        if (personContext.UserId is null || (!tenantContext.IsPlatformContext && tenantContext.TenantId is null))
            return false;

        if (tenantContext.IsPlatformContext)
            return await ValidatePlatformPermission(permission, ct);

        return await ValidateTenantPermission(permission, ct);
    }
    private async Task<bool> ValidatePlatformPermission(PermissionInfo permission, CancellationToken ct)
    {
        var userId = personContext.UserId;
        var personRoles = await cache.GetOrCreateAsync(
            $"PLATFORM_USER:{userId}:ROLES",
            (cacheCt) => PlatformUserRoles(db, userId, cacheCt),
            ct
        );
        foreach (var role in personRoles ?? [])
        {
            var rolePermissions = await cache.GetOrCreateAsync(
                $"PLATFORM:{role}:PERMISSIONS",
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
        var personId = personContext.UserId;
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
    private static Task<List<Guid>> PlatformUserRoles(AppDbContext db, Guid? userId, CancellationToken cacheCt)
    {
        return db.Persons
            .AsNoTracking()
            .Where(p => p.Id == userId)
            .SelectMany(p => p.Roles.Select(pr => pr.Id))
            .ToListAsync(cacheCt);
    }
    private static Task<List<Guid>> TenantUserRoles(AppDbContext db, Guid? personId, CancellationToken cacheCt)
    {
        return db.Persons
            .AsNoTracking()
            .Where(p => p.Id == personId)
            .SelectMany(p => p.Roles.Select(pr => pr.Id))
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
