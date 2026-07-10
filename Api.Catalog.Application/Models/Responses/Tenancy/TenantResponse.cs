namespace Api.Catalog.Application.Models;

public record TenantResponse(Guid TenantId, string Name, string Slug, IReadOnlyList<string> Modules);
