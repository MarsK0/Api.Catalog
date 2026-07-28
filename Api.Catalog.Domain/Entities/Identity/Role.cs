using Api.Catalog.Domain.ValueObjects;

namespace Api.Catalog.Domain.Entities;

public sealed class Role : TenantScopedEntity
{
    private readonly RoleInfo _roleInfo = null!;
    public RoleInfo RoleInfo => _roleInfo;
    private Role() { }
    private Role(RoleInfo roleInfo)
    {
        _roleInfo = roleInfo;
    }

    public static AppResult<Role> Create(RoleInfo roleInfo)
        => new Role(roleInfo);

    public AppResult AssignPermissions(HashSet<PermissionInfo> permissions)
    {
        if (permissions.Count == 0)
            return AppFailure.DomainValidation("Ao menos uma permissão deve ser informada");

        foreach (var permission in permissions)
            if (!_roleInfo.Permissions.Contains(permission))
                _roleInfo.AssignPermission(permission);

        return AppResult.Success;
    }
}
