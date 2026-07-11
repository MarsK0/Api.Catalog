namespace Api.Catalog.Domain.ValueObjects;

public sealed record PermissionInfo(string Scope, string Resource, string Action)
{
    public string Value => $"{Scope}:{Resource}:{Action}";
}
