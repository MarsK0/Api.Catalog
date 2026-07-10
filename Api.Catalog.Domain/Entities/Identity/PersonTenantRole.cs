namespace Api.Catalog.Domain.Entities;

public class PersonTenantRole : TenantScopedEntity
{
    public Guid PersonId { get; private set; }
    public Guid TenantRoleId { get; private set; }
    private PersonTenantRole() { }
}
