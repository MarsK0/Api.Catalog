using Api.Catalog.Application.Models;

namespace Api.Catalog.Application.Contracts;

public interface ITokenService
{
    string HashToken(string token);
    (string Value, string Hash) GenerateRefreshToken();
    (string Token, DateTime Expires) GenerateToken(UserDto user);
}
public static class TokenClaims
{
    public const string TenantClaimName = "tenant_id";
    public const string PermissionClaimName = "permission";
}