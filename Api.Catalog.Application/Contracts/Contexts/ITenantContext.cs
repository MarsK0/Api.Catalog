namespace Api.Catalog.Application.Contracts.Contexts;

public interface ITenantContext
{
    Guid? TenantId { get; }
    bool IsPlatformContext { get; }
    bool AllowCrossTenancy { get; }
}

public interface IMutableTenantContext : ITenantContext
{
    void SetTenant(Guid? tenantId);
}