using Api.Catalog.Domain.Models;

namespace Api.Catalog.Domain.Entities;

public class TenantModule : TenantScopedEntity
{
    public string ModuleCode { get; private set; } = string.Empty;
    private TenantModule() { }
    public static Result<TenantModule> Create(
        string moduleCode
    )
    {
        if (!Modules.Exists(moduleCode))
            return DomainResultFailures.Validation("Módulo não existente, verifique o código informado e tente novamente.");

        return new TenantModule
        {
            ModuleCode = moduleCode
        };
    }
}
