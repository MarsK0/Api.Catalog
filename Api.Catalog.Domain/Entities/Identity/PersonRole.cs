namespace Api.Catalog.Domain.Entities;

public class PersonRole : TenantScopedEntity
{
    public Guid PersonId { get; private set; }
    public Guid RoleId { get; private set; }
    private PersonRole() { }
}
