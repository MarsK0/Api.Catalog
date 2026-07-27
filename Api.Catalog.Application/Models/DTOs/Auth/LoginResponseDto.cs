namespace Api.Catalog.Application.Models;

public record LoginResponseDto(LoginResult Result, string RefreshTokenValue, DateTimeOffset RefreshTokenExpires, bool RememberMe);
public record LoginResult(
    string Token,
    DateTimeOffset Expires,
    Guid PersonId,
    string Name,
    string Email,
    IEnumerable<RoleInfoDto> PlatformRoles,
    IEnumerable<RoleInfoDto> TenantRoles
);