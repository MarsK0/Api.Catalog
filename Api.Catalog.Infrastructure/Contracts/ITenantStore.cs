namespace Api.Catalog.Infrastructure.Contracts;

public interface ITenantStore
{
    Task<Guid?> GetTenantIdBySlugAsync(string slug, CancellationToken ct);
    Task<bool> TenantExistsAsync(Guid tenantId, CancellationToken ct);
}
