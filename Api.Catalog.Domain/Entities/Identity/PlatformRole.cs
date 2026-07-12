using Api.Catalog.Domain.Models;
using Api.Catalog.Domain.ValueObjects;

namespace Api.Catalog.Domain.Entities;

public sealed class PlatformRole : BaseEntity
{
    private readonly RoleInfo _roleInfo = null!;
    public string Name => _roleInfo.Name;
    public string Description => _roleInfo.Description;
    public IReadOnlyCollection<PermissionInfo> Permissions => _roleInfo.Permissions;
    public RoleInfo RoleInfo => _roleInfo;
    private PlatformRole() { }
    private PlatformRole(RoleInfo roleInfo)
    {
        _roleInfo = roleInfo;
    }

    public static AppResult<PlatformRole> Create(RoleInfo roleInfo)
        => new PlatformRole(roleInfo);

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
