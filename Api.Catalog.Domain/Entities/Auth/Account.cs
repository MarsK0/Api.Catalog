using System.Diagnostics.CodeAnalysis;

namespace Api.Catalog.Domain.Entities;

public class Account : BaseEntity
{
    public Guid PersonId { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    [SuppressMessage("Compiler", "CS0649", Justification = "Populado na camada de infra")]
    private Person _person = null!;
    public Person Person => _person;

    private Account() { }
    public static AppResult<Account> Create(Guid personId, string passwordHash)
    {
        return new Account
        {
            PersonId = personId,
            PasswordHash = passwordHash
        };
    }
};
