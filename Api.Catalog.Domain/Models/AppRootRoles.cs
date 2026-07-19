using System.Collections.Frozen;

namespace Api.Catalog.Domain.Models;

public static class RootRoles
{
    public static readonly FrozenSet<string> Names =
    [
        PlatformOwner,
        PlatformAdmin,
        TenantOwner,
        TenantAdmin
    ];

    public static bool IsRoot(string roleName) => Names.Contains(roleName);

    public const string PlatformOwner = "PLATFORM_OWNER";
    public const string PlatformAdmin = "PLATFORM_ADMIN";
    public const string TenantOwner = "TENANT_OWNER";
    public const string TenantAdmin = "TENANT_ADMIN";
}
