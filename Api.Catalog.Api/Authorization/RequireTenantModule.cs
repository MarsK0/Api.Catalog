using Api.Catalog.Infrastructure.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace Api.Catalog.Api.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
internal sealed class RequireTenantModuleAttribute(string module) : AuthorizeAttribute(TenantModulesPolicies.Name(module)) { }

public static class TenantModulesPolicies
{
    public static string Name(string module) => $"Module:{module}";
}
internal sealed record TenantModuleRequirement(string Module) : IAuthorizationRequirement;
internal sealed class TenantModulesAuthorizationHandler(
    IModuleValidator moduleValidator,
    IHttpContextAccessor httpContextAccessor
) : AuthorizationHandler<TenantModuleRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TenantModuleRequirement requirement
    )
    {
        var cancellationToken = httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;

        var moduleIsUnlocked = await moduleValidator.IsModuleUnlocked(requirement.Module, cancellationToken);
        if (moduleIsUnlocked)
            context.Succeed(requirement);
    }
}