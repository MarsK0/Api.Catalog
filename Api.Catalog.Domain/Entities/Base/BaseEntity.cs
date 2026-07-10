using Api.Catalog.Domain.Enums;

namespace Api.Catalog.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public EStatus Status { get; protected set; } = EStatus.Enabled;
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    protected void Enable()
    {
        Status = EStatus.Enabled;
        MarkAsUpdated();
    }
    protected void Disable()
    {
        Status = EStatus.Disabled;
        MarkAsUpdated();
    }

    protected void MarkAsUpdated()
        => UpdatedAt = DateTime.UtcNow;
}
