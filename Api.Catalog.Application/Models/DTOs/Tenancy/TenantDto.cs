namespace Api.Catalog.Application.Models;

public record TenantDto(Guid? Id, string Name, string Slug, IReadOnlyList<string> Modules);