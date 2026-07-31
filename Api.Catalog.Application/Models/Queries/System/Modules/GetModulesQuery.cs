using Api.Catalog.Domain;
using Mediator;

namespace Api.Catalog.Application.Models;

public record GetModulesQuery() : IRequest<AppResult<IEnumerable<string>>>;
