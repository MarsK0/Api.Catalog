namespace Api.Catalog.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;

    private readonly List<TenantModule> _modules = new();
    public IReadOnlyCollection<TenantModule> Modules => _modules.AsReadOnly();

    private Tenant() { }
    public static AppResult<Tenant> Create(
        string name,
        string slug,
        IReadOnlyList<string> modules
    )
    {
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
    public AppResult UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return AppFailure.DomainValidation("Um nome deve ser informado para o tenant.");

        Name = name;
        return AppResult.Success;
    }
    public AppResult UnlockModules(IReadOnlyList<string> modules)
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
            return AppFailure.DomainValidation(string.Join(" | ", modulesFailureMessages));

        foreach (var module in modulesToUnlock)
            if (!_modules.Any(a => a.ModuleCode == module.ModuleCode))
                _modules.Add(module);

        return AppResult.Success;
    }
}
