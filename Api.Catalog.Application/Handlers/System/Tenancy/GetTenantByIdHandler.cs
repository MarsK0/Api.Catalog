using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Mappers;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Models;
using Mediator;

namespace Api.Catalog.Application.Handlers.Tenancy;

internal sealed class GetTenantByIdHandler(
    ITenantRepo tenantRepo
) : IRequestHandler<GetTenantByIdQuery, Result<TenantDto?>>
{
    public async ValueTask<Result<TenantDto?>> Handle(GetTenantByIdQuery query, CancellationToken ct)
    {
        var result = await tenantRepo.GetByIdAsync(query.Id, ct);
        if (result is null)
            return DomainResultFailures.EntityNotFound("Nenhuma empresa encontrada para o ID informado.");

        return result.Dto();
    }
}