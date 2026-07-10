using Api.Catalog.Domain.Models;

namespace Api.Catalog.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    private readonly List<TenantMembership> _membership = new();

    private readonly List<TenantModule> _modules = new();
    public IReadOnlyCollection<TenantMembership> Membership => _membership.AsReadOnly();
    public IReadOnlyCollection<TenantModule> Modules => _modules.AsReadOnly();

    private Tenant() { }

    public static AppResult<Tenant> Create(
        string name,
        string slug
    )
    {
        return new Tenant
        {
            Name = name,
            Slug = slug
        };
    }
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
    public static AppResult<Tenant> Create(
        Guid id,
        string name,
        string slug
    )
    {
        return new Tenant
        {
            Id = id,
            Name = name,
            Slug = slug
        };
    }
    public AppResult UnlockModule(string moduleCode)
    {
        var tModuleResult = TenantModule.Create(moduleCode);
        if (!tModuleResult.IsSuccess)
            return tModuleResult.Failure;

        RegisterModule(tModuleResult.Value);
        return AppResult.Success;
    }
    public AppResult UnlockModules(IReadOnlyList<string> modules)
    {
        var invalidModules = modules
            .Where(m => !AppModules.Exists(m))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (invalidModules.Count > 0)
            return AppFailure.DomainValidation($"Módulos inválidos: {string.Join(", ", invalidModules)}");

        var uniqueModules = modules
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var modulesFailureMessages = new List<string>();
        foreach (var module in uniqueModules)
        {
            var tModuleResult = TenantModule.Create(module);
            if (tModuleResult.IsSuccess)
                _modules.Add(tModuleResult.Value);
            else
                modulesFailureMessages.Add(tModuleResult.Failure.Message);
        }
        if (modulesFailureMessages.Count > 0)
            return AppFailure.DomainValidation(string.Join(" | ", modulesFailureMessages));

        return AppResult.Success;
    }
    public AppResult RegisterUser(Guid personId)
    {
        var tPersonResult = TenantMembership.Create(personId);
        if (!tPersonResult.IsSuccess)
            return tPersonResult.Failure;

        RegisterMemeber(tPersonResult.Value);
        return AppResult.Success;
    }
    private void RegisterMemeber(TenantMembership person)
    {
        var _user = _membership.FirstOrDefault(p => p.Id == person.Id);
        if (_user is null)
            _membership.Add(person);
    }
    private void RegisterModule(TenantModule module)
    {
        var _module = _modules.FirstOrDefault(m => m.Id == module.Id);
        if (_module is null)
            _modules.Add(module);
    }
}
