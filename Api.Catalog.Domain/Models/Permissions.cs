using Api.Catalog.Domain.ValueObjects;
using System.Collections.Frozen;
using System.Reflection;

namespace Api.Catalog.Domain.Models;

public static class Permissions
{
    private static readonly FrozenSet<PermissionInfo> _all;
    private static readonly Dictionary<string, List<PermissionInfo>> _tenantModulesPermissions;
    public static FrozenSet<PermissionInfo> GetAll => _all;
    public static IEnumerable<PermissionInfo> GetAllForModules(IEnumerable<string> modules)
        => modules
            .Where(module => _tenantModulesPermissions.ContainsKey(module))
            .SelectMany(module => _tenantModulesPermissions[module]);
    static Permissions()
    {
        _all = [.. typeof(Permissions)
            .GetNestedTypes(BindingFlags.Public)
            .SelectMany(s => s.GetNestedTypes(BindingFlags.Public))
            .SelectMany(s => s.GetFields(BindingFlags.Public | BindingFlags.Static))
            .Where(w => w.FieldType == typeof(PermissionInfo))
            .Select(s => (PermissionInfo)s.GetValue(null)!)];

        _tenantModulesPermissions = [];

        var tenantResources = typeof(TenantPermissions)
            .GetNestedTypes(BindingFlags.Public);

        foreach (var resource in tenantResources)
        {
            var moduleAttr = resource.GetCustomAttribute<ModuleAttribute>();
            if (moduleAttr is null)
                continue;

            var resourcePerms = resource
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(w => w.FieldType == typeof(PermissionInfo))
                .Select(s => (PermissionInfo)s.GetValue(null)!)
                .ToList();

            if (!_tenantModulesPermissions.ContainsKey(moduleAttr.ModuleName))
                _tenantModulesPermissions[moduleAttr.ModuleName] = [];

            _tenantModulesPermissions[moduleAttr.ModuleName].AddRange(resourcePerms);
        }

    }

    public static class PlatformPermissions
    {
        public const string Scope = "SYSTEM";
        public static class Tenants
        {
            public const string Resource = "TENANTS";

            public static readonly PermissionInfo Manage = new(Scope, Resource, "MANAGE");
            public static readonly PermissionInfo Read = new(Scope, Resource, "READ");
            public static readonly PermissionInfo Create = new(Scope, Resource, "CREATE");
            public static readonly PermissionInfo Update = new(Scope, Resource, "UPDATE");
            public static readonly PermissionInfo Delete = new(Scope, Resource, "DELETE");
        }

        public static class Roles
        {
            public const string Resource = "ROLES";

            public static readonly PermissionInfo Manage = new(Scope, Resource, "MANAGE");
            public static readonly PermissionInfo Read = new(Scope, Resource, "READ");
            public static readonly PermissionInfo Create = new(Scope, Resource, "CREATE");
            public static readonly PermissionInfo Update = new(Scope, Resource, "UPDATE");
            public static readonly PermissionInfo Delete = new(Scope, Resource, "DELETE");
        }
    }
    public static class TenantPermissions
    {
        public const string Scope = "TENANT";
        [Module(Modules.Tables)]
        public static class Person
        {
            public const string Resource = "PERSON";
            public static readonly PermissionInfo Manage = new(Scope, Resource, "MANAGE");
            public static readonly PermissionInfo Read = new(Scope, Resource, "READ");
            public static readonly PermissionInfo Create = new(Scope, Resource, "CREATE");
            public static readonly PermissionInfo Update = new(Scope, Resource, "UPDATE");
            public static readonly PermissionInfo Delete = new(Scope, Resource, "DELETE");
        }
        [Module(Modules.Tables)]
        public static class Roles
        {
            public const string Resource = "ROLES";
            public static readonly PermissionInfo Manage = new(Scope, Resource, "MANAGE");
            public static readonly PermissionInfo Read = new(Scope, Resource, "READ");
            public static readonly PermissionInfo Create = new(Scope, Resource, "CREATE");
            public static readonly PermissionInfo Update = new(Scope, Resource, "UPDATE");
            public static readonly PermissionInfo Delete = new(Scope, Resource, "DELETE");
        }
    }
}
