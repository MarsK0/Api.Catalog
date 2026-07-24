using Api.Catalog.Domain;
using Mediator;

namespace Api.Catalog.Application.Models;

public record CreateTenantCommand(string Name, string Slug, IReadOnlyList<string> Modules) : IRequest<AppResult<TenantResponse>>;
