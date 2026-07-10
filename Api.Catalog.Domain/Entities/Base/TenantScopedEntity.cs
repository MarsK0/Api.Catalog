namespace Api.Catalog.Domain.Entities;

public abstract class TenantScopedEntity : BaseEntity
{
    public Guid TenantId { get; private set; } = Guid.Empty;
    protected Tenant _tenant = null!;
    public Tenant Tenant => _tenant;

    public void SetTenant(Guid tenantId) => TenantId = tenantId;
}
