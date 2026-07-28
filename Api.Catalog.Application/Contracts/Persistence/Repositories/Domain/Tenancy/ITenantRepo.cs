using Api.Catalog.Domain.Entities;

namespace Api.Catalog.Application.Contracts;

public interface ITenantRepo
{
    void Add(Tenant tenant);
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct, bool includes = true, bool track = false);
    Task<Tenant?> GetBySlugAsync(string slug, CancellationToken ct, bool includes = true, bool track = false);
    Task<List<string>> GetModulesAsync(CancellationToken ct);
}
