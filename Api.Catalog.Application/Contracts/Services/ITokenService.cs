using Api.Catalog.Domain.Entities;

namespace Api.Catalog.Application.Contracts;

public interface ITokenService
{
    string HashToken(string token);
    (string Value, string Hash) GenerateRefreshToken();
    (string Token, DateTime Expires) GenerateToken(Person person);
}
public static class TokenClaims
{
    public const string TenantClaimName = "tenant_id";
    public const string PermissionClaimName = "permission";
}