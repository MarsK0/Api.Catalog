namespace Api.Catalog.Api.Models;

public record TenantHttpContextData(Guid? TenantId, bool IsPlatformContext);