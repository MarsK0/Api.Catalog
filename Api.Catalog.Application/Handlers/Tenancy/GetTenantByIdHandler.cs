using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Mappers;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using MediatR;

namespace Api.Catalog.Application.Handlers.Tenancy;

internal sealed class GetTenantByIdHandler(
    ITenantRepo tenantRepo
) : IRequestHandler<GetTenantByIdQuery, AppResult<TenantResponse?>>
{
    public async Task<AppResult<TenantResponse?>> Handle(GetTenantByIdQuery query, CancellationToken ct)
        => (await tenantRepo.GetByIdAsync(query.Id, ct))?.ToResponse();
}