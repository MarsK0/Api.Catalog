namespace Api.Catalog.Application.Models;

public record LoginResponse(LoginResult Result, string RefreshTokenValue, DateTime RefreshTokenExpires, bool RememberMe);
public record LoginResult(
    string Token,
    DateTime Expires,
    Guid PersonId,
    string Name,
    string Email
);