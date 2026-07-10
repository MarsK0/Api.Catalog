using Api.Catalog.Domain;
using MediatR;

namespace Api.Catalog.Application.Models;

public record RefreshTokenCommand(string TokenValue) : IRequest<AppResult<LoginResponse>>;