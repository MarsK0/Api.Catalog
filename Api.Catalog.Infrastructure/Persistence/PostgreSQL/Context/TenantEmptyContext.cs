using Api.Catalog.Application.Contracts.Contexts;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL.Context;

internal sealed record TenantEmptyContext : ITenantContext
{
    public Guid? TenantId => null;
    public bool IsPlatformContext => false;
    public bool AllowCrossTenancy => false;
}
