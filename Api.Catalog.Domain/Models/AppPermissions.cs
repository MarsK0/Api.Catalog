using System.Reflection;

namespace Api.Catalog.Domain.Models;

public static class AppPermissions
{
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
            public const string Base = "TENANTS";
            public const string Read = "TENANTS.READ";
            public const string Create = "TENANTS.CREATE";
            public const string Update = "TENANTS.UPDATE";
            public const string Delete = "TENANTS.DELETE";
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
            public const string Base = "ROLES";
            public const string Read = "ROLES.READ";
            public const string Create = "ROLES.CREATE";
            public const string Update = "ROLES.UPDATE";
            public const string Delete = "ROLES.DELETE";
        }
    }
}
