using Api.Catalog.Domain;
using MediatR;

namespace Api.Catalog.Application.Models;

public record GetTenantByIdQuery(Guid Id) : IRequest<AppResult<TenantResponse?>>;
