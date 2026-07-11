using System.Reflection;

namespace Api.Catalog.Domain.Models;

public static class AppPermissions
{
    public static string[] GetAll => [..PlatformPermissions.GetAll, ..TenantPermissions.GetAll];
    public static class PlatformPermissions
    {
        private static readonly string[] _all;
        public static string[] GetAll => _all;
        static PlatformPermissions()
        {
            _all = typeof(PlatformPermissions)
                .GetNestedTypes(BindingFlags.Public | BindingFlags.Static)
                .SelectMany(s => s.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                .Where(w => w.IsLiteral && !w.IsInitOnly && w.FieldType == typeof(string))
                .Select(s => (string)s.GetRawConstantValue()!)
                .ToArray();
        }
        public static class Tenants
        {
            public const string Base   = "PLATFORM.TENANTS";
            public const string Read   = "PLATFORM.TENANTS.READ";
            public const string Create = "PLATFORM.TENANTS.CREATE";
            public const string Update = "PLATFORM.TENANTS.UPDATE";
            public const string Delete = "PLATFORM.TENANTS.DELETE";
        }

        public static class Roles
        {
            public const string Base   = "PLATFORM.ROLES";
            public const string Read   = "PLATFORM.ROLES.READ";
            public const string Create = "PLATFORM.ROLES.CREATE";
            public const string Update = "PLATFORM.ROLES.UPDATE";
            public const string Delete = "PLATFORM.ROLES.DELETE";
        }
    }
    public static class TenantPermissions
    {
        private static readonly string[] _all;
        public static string[] GetAll => _all;
        static TenantPermissions()
        {
            _all = typeof(TenantPermissions)
                .GetNestedTypes(BindingFlags.Public | BindingFlags.Static)
                .SelectMany(s => s.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                .Where(w => w.IsLiteral && !w.IsInitOnly && w.FieldType == typeof(string))
                .Select(s => (string)s.GetRawConstantValue()!)
                .ToArray();
        }
        public static class Roles
        {
            public const string Base   = "TENANT.ROLES";
            public const string Read   = "TENANT.ROLES.READ";
            public const string Create = "TENANT.ROLES.CREATE";
            public const string Update = "TENANT.ROLES.UPDATE";
            public const string Delete = "TENANT.ROLES.DELETE";
        }
    }
}
