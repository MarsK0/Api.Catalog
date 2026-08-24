using Api.Catalog.Domain.Models;

namespace Api.Catalog.Domain.Entities;

public class TenantModule : TenantScopedEntity
{
    public string ModuleCode { get; private set; } = null!;
    private TenantModule() { }
    public static Result<TenantModule> Create(
        string moduleCode
    )
    {
        if (!Modules.Exists(moduleCode))
            return DomainResultFailures.Validation($"Módulo de código '{moduleCode}' inexistente.");

        return new TenantModule
        {
            ModuleCode = moduleCode
        };
    }
}
