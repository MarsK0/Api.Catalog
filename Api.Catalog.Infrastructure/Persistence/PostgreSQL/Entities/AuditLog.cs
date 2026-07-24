using Api.Catalog.Domain.Entities;

namespace Api.Catalog.Infrastructure.Persistence.PostgreSQL;

internal enum EAuditAction
{
    Created = 0,
    Updated = 1,
    Deleted = 2,
    SoftDeleted = 3,
}
internal sealed class AuditLog : BaseEntity
{
    public string EntityName { get; init; } = null!;
    public Guid? EntityId { get; init; }
    public EAuditAction Action { get; init; }
    public string? Changes { get; init; }
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
    public Guid? TenantId { get; init; }
    public Guid? PersonId { get; init; }
    private Tenant _tenant = null!;
    private Person _person = null!;
    public Tenant Tenant => _tenant;
    public Person Person => _person;
}