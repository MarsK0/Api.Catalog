using Api.Catalog.Application.Contracts;
using Api.Catalog.Domain.Entities;
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
        var platformOwne = await SeedPlatformOwner(ct);
        await SeedPlatformOwnerAccount(platformOwne, ct);
        var platformOwnerRole = await SeedPlatformOwnerRole(ct);
        SeedPlatformOwnerPermissions(platformOwnerRole);
        await db.SaveChangesAsync(ct);
    }

    private async Task<Person> SeedPlatformOwner(CancellationToken ct)
    {
        var platformOwnerEmail = config["PlatformOwner:Email"] ?? throw new InvalidOperationException("Email PlatformOwner não definido.");
        var platformOwner = await db.Persons
            .FirstOrDefaultAsync(p => p.Email == platformOwnerEmail, ct);

        if (platformOwner is null)
        {
            var platformOwnerName = config["PlatformOwner:Name"] ?? throw new InvalidOperationException("Nome PlatformOwner não definido.");
            var platformOwnerCreateResult = Person.Create(platformOwnerName, platformOwnerEmail);
            if (!platformOwnerCreateResult.IsSuccess)
                throw new ApplicationException($"Um erro ocorreu ao criar o usuário PlatformOwner: {platformOwnerCreateResult.Failure.Message}");

            platformOwner = platformOwnerCreateResult.Value;
            db.Persons.Add(platformOwner);
        }

        return platformOwner;
    }
    private async Task SeedPlatformOwnerAccount(Person owner, CancellationToken ct)
    {
        var platformOwnerAccount = await db.Accounts
            .FirstOrDefaultAsync(a => a.PersonId == owner.Id, ct);
        if (platformOwnerAccount is null)
        {
            var platformOwnerPassword = config["Owner:Password"] ?? throw new InvalidOperationException("Senha Owner não definida.");
            var platformOwnerHashedPassword = passwordHasher.GenerateHash(platformOwnerPassword);
            var ownerAccountCreateResult = Account.Create(owner.Id, platformOwnerHashedPassword);
            if (!ownerAccountCreateResult.IsSuccess)
                throw new ApplicationException($"Um erro ocorreu ao criar a conta de Owner: {ownerAccountCreateResult.Failure.Message}");

            platformOwnerAccount = ownerAccountCreateResult.Value;
            db.Accounts.Add(platformOwnerAccount);
        }
    }
    private async Task<PlatformRole> SeedPlatformOwnerRole(CancellationToken ct)
    {
        var platformOwnerRole = await db.PlatformRoles
            .FirstOrDefaultAsync(r => r.Name == RootRoles.PlatformOwner, ct);
        if (platformOwnerRole is null)
        {
            var ownerRoleInfoCreateResult = RoleInfo.Create(RootRoles.PlatformOwner, "Absolute Unit :o");
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
        var assignedPermissions = role.Permissions.ToHashSet();
        var unassignedPermissions = new HashSet<PermissionInfo>();
        foreach (var permission in AppPermissions.GetAll)
            if (!assignedPermissions.Contains(permission))
                unassignedPermissions.Add(permission);

        if (unassignedPermissions.Count != 0)
            role.AssignPermissions(unassignedPermissions);
    }
}
