using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Entities;
using Mediator;

namespace Api.Catalog.Application.Handlers;

internal sealed class GetTenantPaginatedListHandler(
    ITenantRepo tenantRepo
) : IRequestHandler<GetTenantPaginatedListQuery, Result<PaginatedResponseDto<Tenant>>>
{
    public async ValueTask<Result<PaginatedResponseDto<Tenant>>> Handle(GetTenantPaginatedListQuery query, CancellationToken ct)
    {
        return await tenantRepo.GetPaginatedListAsync(query, ct, includes: false);
    }
}
