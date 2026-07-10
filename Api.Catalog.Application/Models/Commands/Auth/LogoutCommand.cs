using MediatR;

namespace Api.Catalog.Application.Models.Commands.Auth;

public record LogoutCommand(string? TokenValue) : IRequest<Unit>;