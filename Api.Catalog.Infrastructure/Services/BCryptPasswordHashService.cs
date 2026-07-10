using Api.Catalog.Application.Contracts;

namespace Api.Catalog.Infrastructure.Services;

internal sealed class BCryptPasswordHashService : IPasswordHashService
{
    public string GenerateHash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    public bool Matches(string password, string hashPassword) => BCrypt.Net.BCrypt.Verify(password, hashPassword);
}
