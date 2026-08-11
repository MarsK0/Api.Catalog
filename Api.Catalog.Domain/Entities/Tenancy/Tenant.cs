using Api.Catalog.Domain.Models;

namespace Api.Catalog.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;

    private readonly List<TenantModule> _modules = [];
    public IReadOnlyCollection<TenantModule> Modules => _modules.AsReadOnly();

    private Tenant() { }
    public static Result<Tenant> Create(
        string name,
        string slug,
        IReadOnlyList<string> modules
    )
    {
        if (string.IsNullOrWhiteSpace(slug) || slug.Length < 3)
            return DomainResultFailures.Validation("O slug deve conter ao menos 3 caracteres.");

        if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
            return DomainResultFailures.Validation("O nome deve conter ao menos 3 caracteres.");

        var tenant = new Tenant
        {
            Name = name,
            Slug = slug,
        };

        var unlockResult = tenant.UnlockModules(modules);
        if (!unlockResult.IsSuccess)
            return unlockResult.Failure;

        return tenant;
    }
    public Result UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return DomainResultFailures.Validation("Um nome deve ser informado para o tenant.");

        Name = name;
        return Result.Success;
    }
    public Result UnlockModules(IReadOnlyList<string> modules)
    {
        var uniqueModules = modules
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var modulesToUnlock = new List<TenantModule>();
        var modulesFailureMessages = new List<string>();
        foreach (var module in uniqueModules)
        {
            var tModuleResult = TenantModule.Create(module);
            if (tModuleResult.IsSuccess)
                modulesToUnlock.Add(tModuleResult.Value);
            else
                modulesFailureMessages.Add(tModuleResult.Failure.Message);
        }
        if (modulesFailureMessages.Count > 0)
            return DomainResultFailures.Validation(string.Join(" | ", modulesFailureMessages));

        foreach (var module in modulesToUnlock)
            if (!_modules.Any(a => a.ModuleCode == module.ModuleCode))
                _modules.Add(module);

        return Result.Success;
    }
}
