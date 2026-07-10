using Microsoft.AspNetCore.Authorization;

namespace Api.Catalog.Api.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
internal sealed class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permission) : base(policy: permission) { }
}
public static class PermissionPolicies
{
    public const string Prefix = "Perm";
    public static string PlatformPolicy(string permission) => $"{Prefix}:Platform:{permission}";
    public static string TenantPolicy(string permission) => $"{Prefix}:Tenant:{permission}";
}