using Api.Catalog.Domain;
using Api.Catalog.Domain.Entities;

namespace Api.Catalog.Application.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public Guid FamilyId { get; private set; }
    public DateTimeOffset Expires { get; private set; }
    public bool RememberMe { get; private set; }
    public bool IsUsed { get; private set; }
    public bool Revoked { get; private set; }

    private RefreshToken() { }
    public static Result<RefreshToken> Create(Guid userId, string tokenHash, Guid familyId, DateTimeOffset expires, bool rememberMe, TimeProvider timeProvider)
    {
        if (userId == Guid.Empty)
            return AppResultFailures.Validation("Deve ser informado um usuário para a criação do Refresh Token.");

        if (string.IsNullOrWhiteSpace(tokenHash))
            return AppResultFailures.Validation("Uma hash deve ser informada para a criação do Token.");

        if (familyId == Guid.Empty)
            return AppResultFailures.Validation("Deve ser informado um id para a família do RefreshToken.");

        if (expires <= timeProvider.GetUtcNow())
            return AppResultFailures.Validation("A data de expiração deve ser maior do que o momento atual.");

        return new RefreshToken()
        {
            UserId = userId,
            TokenHash = tokenHash,
            FamilyId = familyId,
            Expires = expires,
            RememberMe = rememberMe,
        };
    }
    public void MarkAsUsed() => IsUsed = true;
    public void Revoke() => Revoked = true;
    public bool IsValid(TimeProvider timeProvider) => !IsUsed && !Revoked && Expires > timeProvider.GetUtcNow();

}
