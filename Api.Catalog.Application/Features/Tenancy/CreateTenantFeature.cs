using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Mappers;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Entities;
using MediatR;

namespace Api.Catalog.Application.Features.Tenancy;

internal sealed class CreateTenantFeature(
    ITenantRepo tenantRepo
) : IRequestHandler<CreateTenantCommand, AppResult<TenantResponse>>
{
    public async Task<AppResult<TenantResponse>> Handle(CreateTenantCommand command, CancellationToken ct)
    {
        var slugTenant = tenantRepo.GetBySlugAsync(command.Slug, ct);
        if (slugTenant is not null)
            return AppFailure.ApplicationValidation($"Slug '{command.Slug}' já em uso.");

        var tenantResult = Tenant.Create(command.Name, command.Slug, command.Modules);
        if (!tenantResult.IsSuccess)
            return tenantResult.Failure;

        var tenant = tenantResult.Value;
        await tenantRepo.CreateAsync(tenant, ct);
        return tenant.ToResponse();
    }
}
