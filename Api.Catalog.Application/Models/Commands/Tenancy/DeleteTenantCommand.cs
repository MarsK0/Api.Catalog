using Api.Catalog.Domain;
using Mediator;

namespace Api.Catalog.Application.Models;

public record DeleteTenantCommand(Guid TenantId) : IRequest<AppResult>;