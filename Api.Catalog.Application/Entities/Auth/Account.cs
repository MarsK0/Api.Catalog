using Api.Catalog.Application.Enums;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Api.Catalog.Application.Entities;

public class Account : BaseEntity
{
    public Guid PersonId { get; private set; }
    public EAccountStatus Status { get; private set; }
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
            PasswordHash = passwordHash,
            Status = EAccountStatus.Enabled
        };
    }
};
