using Api.Catalog.Domain.ValueObjects;

namespace Api.Catalog.Infrastructure.Contracts;

public interface IPermissionValidator
{
    Task<bool> HasPermission(PermissionInfo permission, CancellationToken ct);
}
