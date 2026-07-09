using System.Diagnostics.CodeAnalysis;

namespace Api.Catalog.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid PersonId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public Guid FamilyId { get; private set; }
    public DateTimeOffset Expires { get; private set; }
    public bool RememberMe { get; private set; }
    public bool IsUsed { get; private set; }
    public bool Revoked { get; private set; }
    [SuppressMessage("Compiler", "CS0649", Justification = "Populado na camada de infra")]
    private Person _person = null!;
    public Person Person => _person;

    private RefreshToken() { }

    public RefreshToken(Guid personId, string tokenHash, Guid familyId, DateTimeOffset expires, bool rememberMe)
    {
        PersonId = personId;
        TokenHash = tokenHash;
        FamilyId = familyId;
        Expires = expires;
        RememberMe = rememberMe;
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
        MarkAsUpdated();
    }
    public void Revoke()
    {
        Revoked = true;
        MarkAsUpdated();
    }
    public bool IsValid => !IsUsed && !Revoked && Expires > DateTimeOffset.UtcNow;
}
