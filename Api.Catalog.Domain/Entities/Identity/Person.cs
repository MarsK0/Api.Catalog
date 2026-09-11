using Api.Catalog.Domain.Enums;
using Api.Catalog.Domain.Models;

namespace Api.Catalog.Domain.Entities;

public class Person : TenantScopedEntity
{
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? Phone { get; private set; }
    public EPersonStatus Status { get; private set; }
    private readonly List<Role> _roles = [];
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    private Person() { }

    public static Result<Person> Create(
        string name,
        string email,
        string? phone = null
    )
    {
        if (string.IsNullOrWhiteSpace(name))
            return DomainResultFailures.Validation("Um nome deve ser informado para a pessoa.");

        if (name.Length < 3 || name.Length > 60)
            return DomainResultFailures.Validation("O nome deve conter entre 3 e 60 caracteres.");

        if (string.IsNullOrWhiteSpace(email))
            return DomainResultFailures.Validation("Um e-mail deve ser informado para a pessoa.");

        return new Person
        {
            Name = name,
            Email = email,
            Phone = phone
        };
    }
    public void AssignRole(Role role)
    {
        if (!_roles.Any(a => a.RoleInfo.Name == role.RoleInfo.Name))
            _roles.Add(role);
    }

    public void Disable() => Status = EPersonStatus.Disabled;
    public void Enable() => Status = EPersonStatus.Enabled;
}
