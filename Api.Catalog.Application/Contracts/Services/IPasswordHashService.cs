namespace Api.Catalog.Application.Contracts;

public interface IPasswordHashService
{
    string GenerateHash(string password);

    bool Matches(string passsword, string hashedPassowrd);
}
