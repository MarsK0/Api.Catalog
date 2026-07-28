using Api.Catalog.Domain.Entities;

namespace Api.Catalog.Application.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public Guid FamilyId { get; private set; }
    public DateTimeOffset Expires { get; private set; }
    public bool RememberMe { get; private set; }
    public bool IsUsed { get; private set; }
    public bool Revoked { get; private set; }

    private RefreshToken() { }
    public RefreshToken(Guid userId, string tokenHash, Guid familyId, DateTimeOffset expires, bool rememberMe)
    {
        UserId = userId;
        TokenHash = tokenHash;
        FamilyId = familyId;
        Expires = expires;
        RememberMe = rememberMe;
    }
    public void MarkAsUsed() => IsUsed = true;
    public void Revoke() => Revoked = true;
    public bool IsValid => !IsUsed && !Revoked && Expires > DateTimeOffset.UtcNow;

}
