using Api.Catalog.Application.Enums;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Entities;

namespace Api.Catalog.Application.Entities;

public class PlatformUser : BaseEntity
{
    public string Login { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public EPlatformUserStatus Status { get; private set; }
    private readonly List<PlatformRole> _roles = [];
    public IReadOnlyCollection<PlatformRole> Roles => _roles.AsReadOnly();
    private PlatformUser() { }
    public static AppResult<PlatformUser> Create(
        string login,
        string name,
        string email,
        string passwordHash
    )
    {
        return new PlatformUser
        {
            Login = login,
            Name = name,
            Email = email,
            PasswordHash = passwordHash,
            Status = EPlatformUserStatus.Enabled
        };
    }
    public void AssignRole(PlatformRole role)
    {
        if (!_roles.Any(a => a.RoleInfo.Name == role.RoleInfo.Name))
            _roles.Add(role);
    }

}
