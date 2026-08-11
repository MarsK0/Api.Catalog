using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Contracts.Contexts;
using Api.Catalog.Application.Entities;
using Api.Catalog.Application.Mappers;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Entities;
using Api.Catalog.Domain.Models;
using Api.Catalog.Domain.ValueObjects;
using Mediator;

namespace Api.Catalog.Application.Handlers.Tenancy;

internal sealed class CreateTenantHandler(
    IUnitOfWork unitOfWork,
    ITenantRepo tenantRepo,
    IPersonRepo personRepo,
    IAccountRepo accountRepo,
    IRoleRepo roleRepo,
    IPasswordHashService passwordHashService,
    IMutableTenantContext tenantContext
) : IRequestHandler<CreateTenantCommand, Result<TenantDto>>
{
    public async ValueTask<Result<TenantDto>> Handle(CreateTenantCommand command, CancellationToken ct)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var tenantResult = await CreateTenantAsync(command, ct);
            if (!tenantResult.IsSuccess)
            {
                await transaction.RollbackAsync(ct);
                return tenantResult.Failure;
            }
            var tenant = tenantResult.Value;
            tenantContext.SetTenant(tenant.Id);

            var ownerSeedResult = SeedOwner(command, tenant.Id);
            if (!ownerSeedResult.IsSuccess)
            {
                await transaction.RollbackAsync(ct);
                return ownerSeedResult.Failure;
            }

            await unitOfWork.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return tenant.Dto();
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    private async Task<Result<Tenant>> CreateTenantAsync(CreateTenantCommand command, CancellationToken ct)
    {
        var slugTenant = await tenantRepo.GetBySlugAsync(command.Slug, ct);
        if (slugTenant is not null)
            return AppResultFailures.Validation($"Slug '{command.Slug}' já em uso.");

        var tenantResult = Tenant.Create(command.Name, command.Slug, command.Modules);
        if (!tenantResult.IsSuccess)
            return tenantResult.Failure;

        tenantRepo.Add(tenantResult.Value);
        return tenantResult;
    }

    private Result SeedOwner(CreateTenantCommand command, Guid tenantId)
    {
        var (_, _, name, email, login, password, unlockedModules, phone) = command;

        var roleInfoResult = RoleInfo.Create(RootRoles.TenantOwner, "Owner");
        if (!roleInfoResult.IsSuccess)
            return roleInfoResult.Failure;

        var roleResult = Role.Create(roleInfoResult.Value);
        if (!roleResult.IsSuccess)
            return roleResult.Failure;

        var role = roleResult.Value;
        var unlockedPermissions = Permissions.GetAllForModules(unlockedModules);
        role.AssignPermissions(unlockedPermissions);
        roleRepo.Add(role);

        var personResult = Person.Create(name, email, phone);
        if (!personResult.IsSuccess)
            return personResult.Failure;

        var person = personResult.Value;
        person.AssignRole(role);
        personRepo.Add(person);

        var passwordHash = passwordHashService.GenerateHash(password);
        var accountResult = Account.Create(person.Id, login, passwordHash);
        if (!accountResult.IsSuccess)
            return accountResult.Failure;

        var account = accountResult.Value;
        accountRepo.Add(account);

        return Result.Success;
    }
}
