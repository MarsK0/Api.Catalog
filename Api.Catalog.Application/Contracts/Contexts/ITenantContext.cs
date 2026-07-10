namespace Api.Catalog.Application.Contracts.Contexts;

public interface ITenantContext
{
    Guid TenantId { get; }
    bool HasHttpContext { get; }
    bool IsPlatformContext { get; }
}
