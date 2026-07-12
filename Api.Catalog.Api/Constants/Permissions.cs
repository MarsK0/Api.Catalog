namespace Api.Catalog.Api.Constants;

public static class Permissions
{
    public static class PlatformPermissions
    {
        private const string Scope = "PLATFORM";
        public static class Tenants
        {
            private const string Resource = "TENANTS";

            public const string Manage = $"{Scope}:{Resource}:MANAGE";
            public const string Read   = $"{Scope}:{Resource}:READ";
            public const string Create = $"{Scope}:{Resource}:CREATE";
            public const string Update = $"{Scope}:{Resource}:UDPATE";
            public const string Delete = $"{Scope}:{Resource}:DELETE";
        }
        public static class Roles
        {
            private const string Resource = "ROLES";

            public const string Manage = $"{Scope}:{Resource}:MANAGE";
            public const string Read   = $"{Scope}:{Resource}:READ";
            public const string Create = $"{Scope}:{Resource}:CREATE";
            public const string Update = $"{Scope}:{Resource}:UPDATE";
            public const string Delete = $"{Scope}:{Resource}:DELETE";
        }
    }
    public static class TenantPermissions
    {
        private const string Scope = "TENANT";
        public static class Roles
        {
            private const string Resource = "ROLES";

            public const string Manage = $"{Scope}:{Resource}:MANAGE";
            public const string Read = $"{Scope}:{Resource}:READ";
            public const string Create = $"{Scope}:{Resource}:CREATE";
            public const string Update = $"{Scope}:{Resource}:UPDATE";
            public const string Delete = $"{Scope}:{Resource}:DELETE";
        }
    }
}
