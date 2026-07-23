using Api.Catalog.Domain;
using Mediator;

namespace Api.Catalog.Application.Models;

public record GetTenantByIdQuery(Guid Id) : IRequest<AppResult<TenantResponse?>>;
