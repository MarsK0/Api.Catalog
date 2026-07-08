using Api.Catalog.Domain;
using MediatR;

namespace Api.Catalog.Application.Models;

public record LoginCommand(string Email, string Password, bool RememberMe) : IRequest<AppResult<LoginResponse>>;