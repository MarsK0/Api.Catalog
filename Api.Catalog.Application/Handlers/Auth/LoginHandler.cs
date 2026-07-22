using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Entities;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Enums;
using MediatR;

namespace Api.Catalog.Application.Handlers;

internal sealed class LoginHandler(
    TimeProvider timeProvider,
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

        if (!passwordHashService.Matches(command.Password, account.PasswordHash))
            return AppFailure.InvalidRequest("Credenciais iválidas.");

        if (account.Status is EStatus.Disabled)
            return AppFailure.InvalidRequest("Credenciais iválidas.");

        if (account.Person.Status is EStatus.Disabled)
            return AppFailure.InvalidRequest("Credenciais iválidas.");

        return await Login(timeProvider, tokenService, unitOfWork, refreshTokenRepo, account, command.RememberMe, ct);
    }
    public static async Task<LoginResponse> Login(
        TimeProvider timeProvider,
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

        var (rtValue, rtHash) = tokenService.GenerateRefreshToken();
        var utcNow = timeProvider.GetUtcNow();
        var rtExpires = rememberMe
            ? utcNow.AddDays(30)
            : utcNow.AddHours(8);

        var refreshToken = new RefreshToken(account.Person.Id, rtHash, Guid.NewGuid(), rtExpires, rememberMe);
        await refreshTokenRepo.AddAsync(refreshToken, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new LoginResponse(loginResult, rtValue, rtExpires, rememberMe);
    }
}
