using Api.Catalog.Infrastructure.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace Api.Catalog.Api.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
internal sealed class RequirePermissionAttribute(string permission) : AuthorizeAttribute(policy: permission) { }
public static class PermissionPolicies
{
    public static string Name(string permission) => $"PERM:{permission}";
}
internal sealed record PermissionRequirement(string Permission) : IAuthorizationRequirement;
internal sealed class PermissionAuthorizationHandler(
    IPermissionValidator permissionValidator,
    IHttpContextAccessor httpContextAccessor
) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement
    )
    {
        var cancellationToken = httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;

        var hasPermission = await permissionValidator.HasPermission(requirement.Permission, cancellationToken);
        if(hasPermission)
            context.Succeed(requirement);
    }
}