namespace Api.Catalog.Application.Models;

public record PermissionInfoDto(string Scope, string Resource, string Action);