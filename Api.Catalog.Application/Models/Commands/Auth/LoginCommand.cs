using Api.Catalog.Domain;
using Mediator;

namespace Api.Catalog.Application.Models;

public record LoginCommand(string Login, string Password, bool RememberMe) : IRequest<Result<LoginResponseDto>>;