using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Models;
using MediatR;

namespace Api.Catalog.Application.Features;

internal sealed class LogoutFeature(
    IUnitOfWork unitOfWork,
    IRefreshTokenRepo refreshTokenRepo
) : IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Unit> Handle(LogoutCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.TokenValue))
            return Unit.Value;

        var hash = LoginHandler.HashToken(command.TokenValue);
        var token = await refreshTokenRepo.GetByHashAsync(hash, ct);

        if (token is not null)
        {
            token.Revoke();
            await unitOfWork.SaveChangesAsync(ct);
        }

        return Unit.Value;
    }
}
