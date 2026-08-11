using Api.Catalog.Domain.Models;

namespace Api.Catalog.Domain.ValueObjects;

public sealed class RoleInfo
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    private readonly List<PermissionInfo> _permissions = [];
    public IReadOnlyCollection<PermissionInfo> Permissions => _permissions;
    private RoleInfo() { }
    public static Result<RoleInfo> Create(
        string name,
        string description
    )
    {
        if (name is null || string.IsNullOrWhiteSpace(name))
            return DomainResultFailures.Validation("Informe um nome para o papel.");

        if (name.Length < 3 || name.Length > 30)
            return DomainResultFailures.Validation("O nome do papel deve ter entre 3 e 30 caracteres.");

        if (description is null || string.IsNullOrWhiteSpace(description))
            return DomainResultFailures.Validation("Forneça uma descrição para o papel.");

        if (description.Length < 3 || description.Length > 60)
            return DomainResultFailures.Validation("A descrição do papel deve ter entre 3 e 60 caracteres.");

        return new RoleInfo
        {
            Name = name,
            Description = description
        };
    }

    public void AssignPermission(PermissionInfo permission)
        => _permissions.Add(permission);
}
