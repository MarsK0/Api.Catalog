using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Entities;
using Api.Catalog.Application.Enums;
using Api.Catalog.Application.Mappers;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Enums;
using Mediator;

namespace Api.Catalog.Application.Handlers;

internal sealed class LoginHandler(
    TimeProvider timeProvider,
    IPasswordHashService passwordHashService,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    IAccountRepo accountRepo,
    IRefreshTokenRepo refreshTokenRepo
) : IRequestHandler<LoginCommand, AppResult<LoginResponseDto>>
{
    public async ValueTask<AppResult<LoginResponseDto>> Handle(LoginCommand command, CancellationToken ct)
    {
        var account = await accountRepo.FindByEmailAsync(command.Email, ct);
        if (account is null)
            return AppFailure.InvalidRequest("Credenciais inválidas.");

        if (!passwordHashService.Matches(command.Password, account.PasswordHash))
            return AppFailure.InvalidRequest("Credenciais iválidas.");

        if (account.Status is EAccountStatus.Disabled)
            return AppFailure.InvalidRequest("Credenciais iválidas.");

        if (account.Person.Status is EPersonStatus.Disabled)
            return AppFailure.InvalidRequest("Credenciais iválidas.");

        return await Login(timeProvider, tokenService, unitOfWork, refreshTokenRepo, account, command.RememberMe, ct);
    }
    public static async Task<LoginResponseDto> Login(
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
            account.Person.Email,
            account.Person.PlatformRoles.Select(s => s.RoleInfo).Dto(),
            account.Person.TenantRoles.Select(s => s.RoleInfo).Dto()
        );

        var (rtValue, rtHash) = tokenService.GenerateRefreshToken();
        var utcNow = timeProvider.GetUtcNow();
        var rtExpires = rememberMe
            ? utcNow.AddDays(30)
            : utcNow.AddHours(8);

        var refreshToken = new RefreshToken(account.Person.Id, rtHash, Guid.NewGuid(), rtExpires, rememberMe);
        await refreshTokenRepo.AddAsync(refreshToken, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new LoginResponseDto(loginResult, rtValue, rtExpires, rememberMe);
    }
}
