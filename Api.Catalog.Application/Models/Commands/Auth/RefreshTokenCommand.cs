using Api.Catalog.Domain;
using Mediator;

namespace Api.Catalog.Application.Models;

public record RefreshTokenCommand(string TokenValue) : IRequest<Result<LoginResponseDto>>;