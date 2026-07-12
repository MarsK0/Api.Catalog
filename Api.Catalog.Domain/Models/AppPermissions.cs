using Api.Catalog.Domain.ValueObjects;
using System.Reflection;

namespace Api.Catalog.Domain.Models;

public static class AppPermissions
{
    private static readonly HashSet<PermissionInfo> _all;
    public static IReadOnlySet<PermissionInfo> GetAll => _all;
    static AppPermissions()
    {
        _all = [.. typeof(AppPermissions)
            .GetNestedTypes(BindingFlags.Public | BindingFlags.Static)
            .SelectMany(s => s.GetNestedTypes(BindingFlags.Public | BindingFlags.Static))
            .SelectMany(s => s.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            .Where(w => w.IsStatic && !w.IsInitOnly && w.FieldType == typeof(PermissionInfo))
            .Select(s => (PermissionInfo)s.GetValue(null)!)];
    }
    
    public static class PlatformPermissions
    {
        public const string Scope = "PLATFORM";
        public static class Tenants
        {
            public const string Resource = "TENANTS";

            public static readonly PermissionInfo Manage = new(Scope, Resource, "MANAGE");
            public static readonly PermissionInfo Read   = new(Scope, Resource, "READ");
            public static readonly PermissionInfo Create = new(Scope, Resource, "CREATE");
            public static readonly PermissionInfo Update = new(Scope, Resource, "UPDATE");
            public static readonly PermissionInfo Delete = new(Scope, Resource, "DELETE");
        }

        public static class Roles
        {
            public const string Resource = "ROLES";

            public static readonly PermissionInfo Manage = new(Scope, Resource, "MANAGE");
            public static readonly PermissionInfo Read   = new(Scope, Resource, "READ");
            public static readonly PermissionInfo Create = new(Scope, Resource, "CREATE");
            public static readonly PermissionInfo Update = new(Scope, Resource, "UPDATE");
            public static readonly PermissionInfo Delete = new(Scope, Resource, "DELETE");
        }
    }
    public static class TenantPermissions
    {
        public const string Scope = "TENANT";
        public static class Roles
        {
            public const string Resource = "ROLES";
            public static readonly PermissionInfo Manage = new(Scope, Resource, "MANAGE");
            public static readonly PermissionInfo Read   = new(Scope, Resource, "READ");
            public static readonly PermissionInfo Create = new(Scope, Resource, "CREATE");
            public static readonly PermissionInfo Update = new(Scope, Resource, "UPDATE");
            public static readonly PermissionInfo Delete = new(Scope, Resource, "DELETE");
        }
    }
}
