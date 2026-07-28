using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Entities;
using Api.Catalog.Domain.Models;
using Api.Catalog.Domain.ValueObjects;
using Api.Catalog.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal sealed class PostgresSeed(
    AppDbContext db,
    IConfiguration config,
    IPasswordHashService passwordHasher
) : IDataSeeder
{
    public async Task SeedAsync(CancellationToken ct)
    {
        await db.Database.MigrateAsync(ct);
        var platformOwner = await SeedPlatformOwner(ct);
        var platformOwnerRole = await SeedPlatformOwnerRole(ct);
        SeedPlatformOwnerPermissions(platformOwnerRole);
        platformOwner.AssignRole(platformOwnerRole);
        await db.SaveChangesAsync(ct);
    }

    private async Task<PlatformUser> SeedPlatformOwner(CancellationToken ct)
    {
        var platformOwnerLogin = config["PlatformOwner:Login"] ?? throw new InvalidOperationException("Login Owner não definido.");
        var platformOwner = await db.PlatformUsers
            .Include(i => i.Roles)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Login == platformOwnerLogin, ct);

        if (platformOwner is null)
        {
            var platformOwnerEmail = config["PlatformOwner:Email"] ?? throw new InvalidOperationException("Email Owner não definido.");
            var platformOwnerPassword = config["PlatformOwner:Password"] ?? throw new InvalidOperationException("Senha Owner não definida.");
            var platformOwnerHashedPassword = passwordHasher.GenerateHash(platformOwnerEmail);
            var platformOwnerCreateResult = PlatformUser.Create(platformOwnerLogin, platformOwnerLogin, platformOwnerEmail, platformOwnerHashedPassword);
            if (!platformOwnerCreateResult.IsSuccess)
                throw new ApplicationException($"Um erro ocorreu ao criar o usuário PlatformOwner: {platformOwnerCreateResult.Failure.Message}");

            platformOwner = platformOwnerCreateResult.Value;
            db.PlatformUsers.Add(platformOwner);
        }

        return platformOwner;
    }
    private async Task<PlatformRole> SeedPlatformOwnerRole(CancellationToken ct)
    {
        var platformOwnerRole = await db.PlatformRoles
            .FirstOrDefaultAsync(r => r.RoleInfo.Name == RootRoles.PlatformOwner, ct);
        if (platformOwnerRole is null)
        {
            var ownerRoleInfoCreateResult = RoleInfo.Create(RootRoles.PlatformOwner, "Owner");
            if (!ownerRoleInfoCreateResult.IsSuccess)
                throw new ApplicationException($"Um erro ocorreu ao criar as informações da role Owner: {ownerRoleInfoCreateResult.Failure.Message}");
            var platformOwnerRoleCreateResult = PlatformRole.Create(ownerRoleInfoCreateResult.Value);
            if (!platformOwnerRoleCreateResult.IsSuccess)
                throw new ApplicationException($"Um erro ocorreu ao criar a role Owner: {platformOwnerRoleCreateResult.Failure.Message}");

            platformOwnerRole = platformOwnerRoleCreateResult.Value;
            db.PlatformRoles.Add(platformOwnerRole);
        }

        return platformOwnerRole;
    }
    private static void SeedPlatformOwnerPermissions(PlatformRole role)
    {
        var assignedPermissions = role.RoleInfo.Permissions.ToHashSet();
        var unassignedPermissions = new HashSet<PermissionInfo>();
        foreach (var permission in AppPermissions.GetAll)
            if (!assignedPermissions.Contains(permission))
                unassignedPermissions.Add(permission);

        if (unassignedPermissions.Count != 0)
            role.AssignPermissions(unassignedPermissions);
    }
}
