using Api.Catalog.Domain.Enums;

namespace Api.Catalog.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public EStatus Status { get; protected set; } = EStatus.Enabled;
    public DateTimeOffset CreatedAt { get; protected set; }
    public DateTimeOffset? UpdatedAt { get; protected set; }

    protected void Enable() => Status = EStatus.Enabled;
    protected void Disable() => Status = EStatus.Disabled;
}
