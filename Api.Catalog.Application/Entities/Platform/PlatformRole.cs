using Api.Catalog.Domain;
using Api.Catalog.Domain.Entities;
using Api.Catalog.Domain.Models;
using Api.Catalog.Domain.ValueObjects;

namespace Api.Catalog.Application.Entities;

public sealed class PlatformRole : BaseEntity
{
    private readonly RoleInfo _roleInfo = null!;
    public RoleInfo RoleInfo => _roleInfo;
    private PlatformRole() { }
    private PlatformRole(RoleInfo roleInfo)
    {
        _roleInfo = roleInfo;
    }

    public static Result<PlatformRole> Create(RoleInfo roleInfo)
        => new PlatformRole(roleInfo);

    public Result AssignPermissions(HashSet<PermissionInfo> permissions)
    {
        if (permissions.Count == 0)
            return DomainResultFailures.Validation("Ao menos uma permissão deve ser informada");

        foreach (var permission in permissions)
            if (!_roleInfo.Permissions.Contains(permission))
                _roleInfo.AssignPermission(permission);

        return Result.Success;
    }
}
