namespace Api.Catalog.Domain.Models;

public static class Modules
{
    public const string Tables    = "TABLES";
    public const string Catalog   = "CATALOG";
    public const string Comercial = "COMERCIAL";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        Tables, Catalog, Comercial
    };

    public static bool Exists(string module) => All.Contains(module);
}
