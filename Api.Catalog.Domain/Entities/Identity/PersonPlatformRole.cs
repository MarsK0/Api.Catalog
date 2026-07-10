namespace Api.Catalog.Domain.Entities;

public class PersonPlatformRole : BaseEntity
{
    public Guid PersonId { get; private set; }
    public Guid PlatformRoleId { get; private set; }
    private PersonPlatformRole() { }
}
