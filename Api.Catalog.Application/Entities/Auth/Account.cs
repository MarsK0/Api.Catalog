using Api.Catalog.Application.Enums;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Entities;

namespace Api.Catalog.Application.Entities;

public class Account : TenantScopedEntity
{
    public Guid PersonId { get; private set; }
    public EAccountStatus Status { get; private set; }
    public string Login { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    private readonly Person _person = null!;
    public Person Person => _person;

    private Account() { }
    public static Result<Account> Create(Guid personId, string login, string passwordHash)
    {
        if (personId == Guid.Empty)
            return AppResultFailures.Validation("Deve ser informada uma pessoa para vínculo com a conta.");

        if (string.IsNullOrWhiteSpace(login) || login.Length < 3)
            return AppResultFailures.Validation("O login deve conter ao menos 3 caracteres.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            return AppResultFailures.Validation("Uma senha deve ser informada");

        return new Account
        {
            PersonId = personId,
            Login = login,
            PasswordHash = passwordHash,
            Status = EAccountStatus.Enabled
        };
    }
};
