namespace Api.Catalog.Application.Contracts.Contexts;

public interface ITenantContext
{
    Guid TenantId { get; }
    bool HasContext { get; }
    bool IsPlatformContext { get; }
}
