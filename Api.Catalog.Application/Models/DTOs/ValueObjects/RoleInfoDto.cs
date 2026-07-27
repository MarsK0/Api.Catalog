namespace Api.Catalog.Application.Models;

public record RoleInfoDto(
    string Name,
    string Description,
    IEnumerable<string> Permissions
);