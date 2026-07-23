using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Enums;
using Mediator;

namespace Api.Catalog.Application.Handlers;

internal sealed class RefreshHandler(
    TimeProvider timeProvider,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    IRefreshTokenRepo refreshTokenRepo,
    IAccountRepo accountRepo
) : IRequestHandler<RefreshTokenCommand, AppResult<LoginResponse>>
{
    public async ValueTask<AppResult<LoginResponse>> Handle(RefreshTokenCommand command, CancellationToken ct)
    {
        var hash = tokenService.HashToken(command.TokenValue);
        var token = await refreshTokenRepo.GetByHashAsync(hash, ct);

        if (token is null)
            return AppFailure.AuthValidation("Sessão inválida. Faça login novamente.");

        if (token.IsUsed)
        {
            var tokenFamily = await refreshTokenRepo.GetByFamilyIdAsync(token.FamilyId, ct);
            foreach (var t in tokenFamily)
                t.Revoke();

            await unitOfWork.SaveChangesAsync(ct);
            return AppFailure.AuthValidation("Sessão inválida. Faça login novamente.");
        }

        if (!token.IsValid)
            return AppFailure.AuthValidation("Sessão inválida. Faça login novamente.");

        var account = await accountRepo.FindByPersonIdAsync(token.PersonId, ct);
        if (account is null || account.Status is EStatus.Disabled || account.Person.Status is EStatus.Disabled)
            return AppFailure.AuthValidation("Sessão inválida. Faça login novamente");

        token.MarkAsUsed();

        return await LoginHandler.Login(timeProvider, tokenService, unitOfWork, refreshTokenRepo, account, token.RememberMe, ct);
    }
}
