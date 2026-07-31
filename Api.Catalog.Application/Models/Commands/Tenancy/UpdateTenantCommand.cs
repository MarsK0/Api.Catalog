using Api.Catalog.Application.Helpers;
using Api.Catalog.Domain;
using Mediator;

namespace Api.Catalog.Application.Models;

public record UpdateTenantCommand(
    Guid TenantId,
    OptionalField<string> Name,
    OptionalField<IReadOnlyList<string>> Modules
) : IRequest<AppResult<TenantDto>>;