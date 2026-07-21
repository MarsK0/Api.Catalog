using Api.Catalog.Domain.ValueObjects;

namespace Api.Catalog.Domain.Entities;

public sealed class TenantRole : TenantScopedEntity
{
    private readonly RoleInfo _roleInfo = null!;
    public RoleInfo RoleInfo => _roleInfo;
    private TenantRole() { }
    private TenantRole(RoleInfo roleInfo)
    {
        _roleInfo = roleInfo;
    }

    public static AppResult<TenantRole> Create(RoleInfo roleInfo)
        => new TenantRole(roleInfo);

    public AppResult AssignPermissions(IEnumerable<PermissionInfo> permissions)
    {
        var uniquePermissions = permissions?.ToHashSet() ?? [];
        if (uniquePermissions.Count == 0)
            return AppFailure.DomainValidation("Ao menos uma permissão deve ser informada");

        foreach (var permission in uniquePermissions)
            if (!_roleInfo.Permissions.Contains(permission))
                _roleInfo.AssignPermission(permission);

        return AppResult.Success;
    }
}
