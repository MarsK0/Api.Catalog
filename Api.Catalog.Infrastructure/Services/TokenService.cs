using Api.Catalog.Application.Contracts;
using Api.Catalog.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Catalog.Infrastructure.Services;

internal sealed class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly TimeProvider _timeProvider;

    public TokenService(
        IConfiguration config,
        TimeProvider timeProvider
    )
    {
        _config = config;
        _timeProvider = timeProvider;
    }


    public (string Token, DateTime Expires) GenerateToken(Person person)
    {
        var jwtConfig = _config.GetSection("Jwt") ?? throw new KeyNotFoundException("Sessão de cofiguração JWT não definida");
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtConfig["Key"] ?? throw new KeyNotFoundException("Configuração de chave JWT não definida"))
        );

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, person.Name),
            new(JwtRegisteredClaimNames.Sub, person.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, person.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        //var permissions = person.Roles
        //    .SelectMany(s => s.Permissions)
        //    .Distinct();
        //foreach (var permission in permissions)
        //    claims.Add(new Claim(TokenClaims.PermissionClaimName, permission.ToString()));

        var expires = _timeProvider.GetUtcNow().AddMinutes(int.Parse(jwtConfig["Expires"] ?? "15"));

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
