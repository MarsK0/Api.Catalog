namespace Api.Catalog.Infrastructure.Contracts;

public interface IModuleValidator
{
    Task<bool> IsModuleUnlocked(string module, CancellationToken ct);
}
