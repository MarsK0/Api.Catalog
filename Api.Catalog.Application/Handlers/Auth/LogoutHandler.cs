using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Models;
using Mediator;

namespace Api.Catalog.Application.Handlers;

internal sealed class LogoutHandler(
    IUnitOfWork unitOfWork,
    IRefreshTokenRepo refreshTokenRepo,
    ITokenService tokenService
) : IRequestHandler<LogoutCommand, Unit>
{
    public async ValueTask<Unit> Handle(LogoutCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.TokenValue))
            return Unit.Value;

        var hash = tokenService.HashToken(command.TokenValue);
        var token = await refreshTokenRepo.GetByHashAsync(hash, ct);

        if (token is not null)
        {
            token.Revoke();
            await unitOfWork.SaveChangesAsync(ct);
        }

        return Unit.Value;
    }
}
