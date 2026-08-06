using Api.Catalog.Domain.Models;

namespace Api.Catalog.Domain.ValueObjects;

public sealed class RoleInfo
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    private readonly List<PermissionInfo> _permissions = new();
    public IReadOnlyCollection<PermissionInfo> Permissions => _permissions;
    private RoleInfo() { }
    public static Result<RoleInfo> Create(
        string name,
        string description
    )
    {
        if (name is null || string.IsNullOrWhiteSpace(name))
            return DomainResultFailures.Validation("Informe um nome para o papel.");

        if (name.Length > 30)
            return DomainResultFailures.Validation("O nome do papel não deve exceder 60 caracteres.");

        if (description is null || string.IsNullOrWhiteSpace(description))
            return DomainResultFailures.Validation("Forneça uma descrição para o papel.");

        if (description.Length > 60)
            return DomainResultFailures.Validation("A descrição do papel não deve exceder 60 caracteres.");

        return new RoleInfo
        {
            Name = name,
            Description = description
        };
    }

    public void AssignPermission(PermissionInfo permission)
        => _permissions.Add(permission);
}
