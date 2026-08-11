using Api.Catalog.Domain;
using Mediator;

namespace Api.Catalog.Application.Models;

public record CreateTenantCommand(
    string Name,
    string Slug,
    string OwnerName,
    string OwnerEmail,
    string OwnerLogin,
    string OwnerPassword,
    IReadOnlyList<string> Modules,
    string? OwnerPhone = null
    ) : IRequest<Result<TenantDto>>;
