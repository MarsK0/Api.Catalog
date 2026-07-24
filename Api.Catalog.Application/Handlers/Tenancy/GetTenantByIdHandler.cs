using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Mappers;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Mediator;

namespace Api.Catalog.Application.Handlers.Tenancy;

internal sealed class GetTenantByIdHandler(
    ITenantRepo tenantRepo
) : IRequestHandler<GetTenantByIdQuery, AppResult<TenantResponse?>>
{
    public async ValueTask<AppResult<TenantResponse?>> Handle(GetTenantByIdQuery query, CancellationToken ct)
    {
        var result = await tenantRepo.GetByIdAsync(query.Id, ct);
        if (result is null)
            return AppFailure.EntityNotFound("Nenhuma empresa encontrada para o ID informado.");

        return result.ToResponse();
    }
}