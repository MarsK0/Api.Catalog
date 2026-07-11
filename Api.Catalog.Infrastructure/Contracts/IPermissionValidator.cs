namespace Api.Catalog.Infrastructure.Contracts;

public interface IPermissionValidator
{
    Task<bool> HasPermission(string permission, CancellationToken ct);
}
