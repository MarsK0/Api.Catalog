using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Mappers;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Entities;
using Mediator;

namespace Api.Catalog.Application.Handlers.Tenancy;

internal sealed class CreateTenantHandler(
    IUnitOfWork unitOfWork,
    ITenantRepo tenantRepo
) : IRequestHandler<CreateTenantCommand, AppResult<TenantDto>>
{
    public async ValueTask<AppResult<TenantDto>> Handle(CreateTenantCommand command, CancellationToken ct)
    {
        var slugTenant = await tenantRepo.GetBySlugAsync(command.Slug, ct);
        if (slugTenant is not null)
            return AppFailure.ApplicationValidation($"Slug '{command.Slug}' já em uso.");

        var tenantResult = Tenant.Create(command.Name, command.Slug, command.Modules);
        if (!tenantResult.IsSuccess)
            return tenantResult.Failure;

        var tenant = tenantResult.Value;
        tenantRepo.Add(tenant);
        await unitOfWork.SaveChangesAsync(ct);
        return tenant.Dto();
    }
}
