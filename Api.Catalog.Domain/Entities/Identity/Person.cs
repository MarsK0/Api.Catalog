using Api.Catalog.Domain.Enums;

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

    public static AppResult<Person> Create(
        string name,
        string email,
        string? phone = null
    )
    {
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
}
