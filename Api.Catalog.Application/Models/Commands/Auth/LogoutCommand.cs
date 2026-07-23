using Mediator;

namespace Api.Catalog.Application.Models;

public record LogoutCommand(string? TokenValue) : IRequest<Unit>;