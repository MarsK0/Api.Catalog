using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Contracts.Contexts;
using Api.Catalog.Application.Mappers;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Mediator;

namespace Api.Catalog.Application.Handlers;

internal sealed class RefreshHandler(
    TimeProvider timeProvider,
    ITenantContext tenantContext,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    IPlatformUserRepo platformUserRepo,
    IRefreshTokenRepo refreshTokenRepo,
    IAccountRepo accountRepo
) : IRequestHandler<RefreshTokenCommand, Result<LoginResponseDto>>
{
    public async ValueTask<Result<LoginResponseDto>> Handle(RefreshTokenCommand command, CancellationToken ct)
    {
        var hash = tokenService.HashToken(command.TokenValue);
        var token = await refreshTokenRepo.GetByHashAsync(hash, ct);

        if (token is null)
            return AppResultFailures.Unauthorized("Sessão inválida. Faça login novamente.");

        if (token.IsUsed)
        {
            var tokenFamily = await refreshTokenRepo.GetByFamilyIdAsync(token.FamilyId, ct, track: true);
            foreach (var t in tokenFamily)
                t.Revoke();

            await unitOfWork.SaveChangesAsync(ct);
            return AppResultFailures.Unauthorized("Sessão inválida. Faça login novamente.");
        }

        if (!token.IsValid(timeProvider))
            return AppResultFailures.Unauthorized("Sessão inválida. Faça login novamente.");

        var userResult = await GetUser(token.UserId, ct);
        if (!userResult.IsSuccess)
            return userResult.Failure;

        var user = userResult.Value;

        token.MarkAsUsed();

        return await LoginHandler.Login(timeProvider, tokenService, unitOfWork, refreshTokenRepo, user, token.RememberMe, ct);
    }

    private async Task<Result<UserDto>> GetUser(Guid userId, CancellationToken ct)
    {
        if (tenantContext.IsPlatformContext)
        {
            var user = await platformUserRepo.FindByUserIdAsync(userId, ct);
            if (user is null)
                return AppResultFailures.Unauthorized("Credenciais inválidas.");
            return user.Dto();
        }

        var account = await accountRepo.FindByPersonIdAsync(userId, ct);
        if (account is null)
            return AppResultFailures.Unauthorized("Credenciais inválidas.");
        return account.Dto();
    }
}
