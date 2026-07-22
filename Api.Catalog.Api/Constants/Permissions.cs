namespace Api.Catalog.Api.Constants;

public static class Permissions
{
    private const string P = "PERM";
    public static class PlatformPermissions
    {
        private const string Scope = "PLATFORM";
        public static class Tenants
        {
            private const string Resource = "TENANTS";

            public const string Manage = $"{P}:{Scope}:{Resource}:MANAGE";
            public const string Read = $"{P}:{Scope}:{Resource}:READ";
            public const string Create = $"{P}:{Scope}:{Resource}:CREATE";
            public const string Update = $"{P}:{Scope}:{Resource}:UDPATE";
            public const string Delete = $"{P}:{Scope}:{Resource}:DELETE";
        }
        public static class Roles
        {
            private const string Resource = "ROLES";

            public const string Manage = $"{P}:{Scope}:{Resource}:MANAGE";
            public const string Read = $"{P}:{Scope}:{Resource}:READ";
            public const string Create = $"{P}:{Scope}:{Resource}:CREATE";
            public const string Update = $"{P}:{Scope}:{Resource}:UPDATE";
            public const string Delete = $"{P}:{Scope}:{Resource}:DELETE";
        }
    }
    public static class TenantPermissions
    {
        private const string Scope = "TENANT";
        public static class Roles
        {
            private const string Resource = "ROLES";

            public const string Manage = $"{P}:{Scope}:{Resource}:MANAGE";
            public const string Read = $"{P}:{Scope}:{Resource}:READ";
            public const string Create = $"{P}:{Scope}:{Resource}:CREATE";
            public const string Update = $"{P}:{Scope}:{Resource}:UPDATE";
            public const string Delete = $"{P}:{Scope}:{Resource}:DELETE";
        }
    }
}
