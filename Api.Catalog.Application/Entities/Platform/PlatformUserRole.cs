using Api.Catalog.Domain.Entities;

namespace Api.Catalog.Application.Entities.Platform;

public class PlatformUserRole : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    private PlatformUserRole() { }
}
