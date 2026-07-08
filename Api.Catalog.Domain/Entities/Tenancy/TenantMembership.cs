namespace Api.Catalog.Domain.Entities;

public class TenantMembership : TenantScopedEntity
{
    public Guid PersonId { get; private set; }
    private TenantMembership() { }
    public static AppResult<TenantMembership> Create(
        Guid personId
    )
    {
        return new TenantMembership
        {
            PersonId = personId
        };
    }
}
