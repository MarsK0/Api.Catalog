using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Contracts.Contexts;
using Microsoft.AspNetCore.Authorization;

namespace Api.Catalog.Api.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
internal sealed class RequireTenantModuleAttribute : AuthorizeAttribute
{
    public RequireTenantModuleAttribute(string module) : base(TenantModulesPolicies.Name(module)) { }
}

public static class TenantModulesPolicies
{
    public const string Prefix = "Module";
    public static string Name(string module) => $"{Prefix}:{module}";
}
internal sealed class TenantModuleRequirement(string module) : IAuthorizationRequirement
{
    public string Module { get; } = module;
}
internal sealed class TenantModulesAuthorizationHandler(
    ITenantRepo tenantRepo,
    ITenantContext tenantContext,
    IHttpContextAccessor httpContextAccessor
) : AuthorizationHandler<TenantModuleRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TenantModuleRequirement requirement
    )
    {
        var cancellationToken = httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;

        var tenantId = tenantContext.TenantId;
        var unlockedModules = await tenantRepo.GetModulesAsync(cancellationToken);
        if (unlockedModules.Contains(requirement.Module))
            context.Succeed(requirement);
    }
}