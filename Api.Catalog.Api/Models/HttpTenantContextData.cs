namespace Api.Catalog.Api.Models;

public record HttpTenantContextData(Guid? TenantId, bool IsPlatformContext);