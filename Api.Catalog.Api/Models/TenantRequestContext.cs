namespace Api.Catalog.Api.Models;

public record TenantRequestContext(Guid? TenantId, bool IsPlatformContext)
{
    public static readonly TenantRequestContext Empty = new(null, false);
};
