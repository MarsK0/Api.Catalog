using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Entities;
using Api.Catalog.Domain.Enums;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace Api.Catalog.Application.Auth;

internal sealed class LoginHandler(
    IPasswordHashService passwordHashService,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    IAccountRepo accountRepo,
    IRefreshTokenRepo refreshTokenRepo
) : IRequestHandler<LoginCommand, AppResult<LoginResponse>>
{
    public async Task<AppResult<LoginResponse>> Handle(LoginCommand command, CancellationToken ct)
    {
        var account = await accountRepo.FindByEmailAsync(command.Email, ct);
        if (account is null)
            return AppFailure.InvalidRequest("Credenciais inválidas.");

        var hashedPassword = passwordHashService.GenerateHash(account.PasswordHash);
        if (!passwordHashService.Matches(account.PasswordHash, hashedPassword))
            return AppFailure.InvalidRequest("Credenciais iválidas.");

        if (account.Status is EStatus.Disabled)
            return AppFailure.InvalidRequest("Credenciais iválidas.");

        if (account.Person.Status is EStatus.Disabled)
            return AppFailure.InvalidRequest("Credenciais iválidas.");

        return await Login(tokenService, unitOfWork, refreshTokenRepo, account, command.RememberMe, ct);
    }
    public static async Task<LoginResponse> Login(
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        IRefreshTokenRepo refreshTokenRepo,
        Account account,
        bool rememberMe,
        CancellationToken ct
    )
    {
        var (Token, Expires) = tokenService.GenerateToken(account.Person);
        var loginResult = new LoginResult(
            Token,
            Expires,
            account.PersonId,
            account.Person.Name,
            account.Person.Email
        );

        var (rtValue, rtHash) = GenerateRefreshToken();
        var rtExpires = rememberMe
            ? DateTime.UtcNow.AddDays(30)
            : DateTime.UtcNow.AddHours(8);

        var refreshToken = new RefreshToken(account.Person.Id, rtHash, Guid.NewGuid(), rtExpires, rememberMe);
        await refreshTokenRepo.AddAsync(refreshToken, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new LoginResponse(loginResult, rtValue, rtExpires, rememberMe);
    }
    internal static (string Value, string Hash) GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var value = Convert.ToBase64String(bytes);
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

        return (value, hash);
    }

    internal static string HashToken(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
