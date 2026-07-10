using Api.Catalog.Application.Models;
using Api.Catalog.Domain.Entities;

namespace Api.Catalog.Application.Mappers;

public static class TenantMapper
{
    public static TenantResponse ToResponse(this Tenant tenant)
    {
        return new(
            tenant.Id,
            tenant.Name,
            tenant.Slug,
            [.. tenant.Modules.Select(s => s.ModuleCode)]
        );
    }
}
