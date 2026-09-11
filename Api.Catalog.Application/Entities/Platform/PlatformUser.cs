using Api.Catalog.Application.Enums;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Entities;

namespace Api.Catalog.Application.Entities;

public class PlatformUser : BaseEntity
{
    public string Login { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public EPlatformUserStatus Status { get; private set; }
    private readonly List<PlatformRole> _roles = [];
    public IReadOnlyCollection<PlatformRole> Roles => _roles.AsReadOnly();
    private PlatformUser() { }
    public static Result<PlatformUser> Create(
        string login,
        string name,
        string email,
        string passwordHash
    )
    {
        if (string.IsNullOrWhiteSpace(login))
            return AppResultFailures.Validation("Um login deve ser informado para o usuário.");

        if (login.Length < 3 || login.Length > 30)
            return AppResultFailures.Validation("O login deve ter entre 3 e 30 caracteres.");

        if (string.IsNullOrWhiteSpace(name))
            return AppResultFailures.Validation("Um nome deve ser informado para o usuário.");

        if(name.Length < 2 || name.Length > 60)
            return AppResultFailures.Validation("O nome deve ter entre 2 e 60 caracteres");

        if (string.IsNullOrWhiteSpace(passwordHash))
            return AppResultFailures.Validation("Uma senha deve ser informada para o usuário");
        
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

    public void Disable() => Status = EPlatformUserStatus.Disabled;
    public void Enable() => Status = EPlatformUserStatus.Enabled;

}
