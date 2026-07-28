using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Api.Catalog.Infrastructure.Services;

internal sealed class TokenService(
    IConfiguration config,
    TimeProvider timeProvider
) : ITokenService
{
    public (string Value, string Hash) GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var value = Convert.ToBase64String(bytes);
        var hash = HashToken(value);

        return (value, hash);
    }
    public string HashToken(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    public (string Token, DateTime Expires) GenerateToken(UserDto user)
    {
        var jwtConfig = config.GetSection("Jwt") ?? throw new KeyNotFoundException("Sessão de cofiguração JWT não definida");
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtConfig["Key"] ?? throw new KeyNotFoundException("Configuração de chave JWT não definida"))
        );

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Name),
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expires = timeProvider.GetUtcNow().AddMinutes(int.Parse(jwtConfig["Expires"] ?? "15"));

        var token = new JwtSecurityToken(
            issuer: jwtConfig["Issuer"],
            audience: jwtConfig["Audience"],
            claims: claims,
            expires: expires.UtcDateTime,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expires.UtcDateTime);
    }
}
