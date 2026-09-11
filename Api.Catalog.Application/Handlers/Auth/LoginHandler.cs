using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Contracts.Contexts;
using Api.Catalog.Application.Entities;
using Api.Catalog.Application.Mappers;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Mediator;

namespace Api.Catalog.Application.Handlers;

internal sealed class LoginHandler(
    TimeProvider timeProvider,
    ITenantContext tenantContext,
    IPasswordHashService passwordHashService,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    IPlatformUserRepo platformUserRepo,
    IAccountRepo accountRepo,
    IRefreshTokenRepo refreshTokenRepo
) : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    public async ValueTask<Result<LoginResponseDto>> Handle(LoginCommand command, CancellationToken ct)
    {
        var userResult = await GetUser(command, ct);
        if (!userResult.IsSuccess)
            return userResult.Failure;

        var user = userResult.Value;
        if (!passwordHashService.Matches(command.Password, user.PasswordHash))
            return AppResultFailures.InvalidRequest("Credenciais iválidas.");
        if (!user.Enabled)
            return AppResultFailures.InvalidRequest("Credenciais iválidas.");

        return await Login(timeProvider, tokenService, unitOfWork, refreshTokenRepo, user, command.RememberMe, ct);
    }
    private async Task<Result<UserDto>> GetUser(LoginCommand command, CancellationToken ct)
    {
        if (tenantContext.IsPlatformContext)
        {
            var user = await platformUserRepo.FindByLoginAsync(command.Login, ct);
            if (user is null)
                return AppResultFailures.InvalidRequest("Credenciais inválidas.");
            return user.Dto();
        }

        var account = await accountRepo.FindByLoginAsync(command.Login, ct);
        if (account is null)
            return AppResultFailures.InvalidRequest("Credenciais inválidas.");
        return account.Dto();
    }
    public static async Task<Result<LoginResponseDto>>Login(
        TimeProvider timeProvider,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        IRefreshTokenRepo refreshTokenRepo,
        UserDto user,
        bool rememberMe,
        CancellationToken ct
    )
    {
        var (Token, Expires) = tokenService.GenerateToken(user);
        var loginResult = new LoginResult(
            Token,
            Expires,
            user.UserId,
            user.Name,
            user.Email,
            user.Roles
        );

        var (rtValue, rtHash) = tokenService.GenerateRefreshToken();
        var utcNow = timeProvider.GetUtcNow();
        var rtExpires = rememberMe
            ? utcNow.AddDays(30)
            : utcNow.AddHours(8);

        var refreshTokenResult = RefreshToken.Create(user.UserId, rtHash, Guid.NewGuid(), rtExpires, rememberMe, timeProvider);
        if (!refreshTokenResult.IsSuccess)
            return refreshTokenResult.Failure;

        refreshTokenRepo.Add(refreshTokenResult.Value);
        await unitOfWork.SaveChangesAsync(ct);

        return new LoginResponseDto(loginResult, rtValue, rtExpires, rememberMe);
    }
}
