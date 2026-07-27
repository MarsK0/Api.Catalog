namespace Api.Catalog.Application.Models;

public record TenantDto(Guid TenantId, string Name, string Slug, IReadOnlyList<string> Modules);