using Api.Catalog.Domain.Models;
using Api.Catalog.Domain.ValueObjects;

namespace Api.Catalog.Domain.Entities;

public sealed class TenantRole : TenantScopedEntity
{
    private readonly RoleInfo _roleInfo = null!;
    public string Name => _roleInfo.Name;
    public string Description => _roleInfo.Description;
    public IReadOnlyCollection<string> Permissions => _roleInfo.Permissions;
    public RoleInfo RoleInfo => _roleInfo;
    private TenantRole() { }
    private TenantRole(RoleInfo roleInfo)
    {
        _roleInfo = roleInfo;
    }

    public static AppResult<TenantRole> Create(RoleInfo roleInfo)
        => new TenantRole(roleInfo);

    public AppResult AssignPermissions(IEnumerable<string> permissions)
    {
        var uniquePermissions = permissions?.ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>();
        if (uniquePermissions.Count == 0)
            return AppFailure.DomainValidation("Ao menos uma permissão deve ser informada");

        var validPermissions = AppPermissions.TenantPermissions.GetAll;
        var invalidPermissions = uniquePermissions.Where(p => !validPermissions.Contains(p));

        if (invalidPermissions.Any())
            return AppFailure.DomainValidation(
                $"As seguintes permissões são inválidas: {string.Join(", ", invalidPermissions)}."
            );

        foreach (var permission in uniquePermissions)
            if (!_roleInfo.Permissions.Contains(permission))
                _roleInfo.AssignPermission(permission);

        return AppResult.Success;
    }
}
