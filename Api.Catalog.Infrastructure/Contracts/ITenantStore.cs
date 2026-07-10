namespace Api.Catalog.Infrastructure.Contracts;

public interface ITenantStore
{
    Task<Guid?> GetTenantIdBySlugAsync(string slug);
    Task<bool> TenantExistsAsync(Guid tenantId);
}
