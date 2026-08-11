using System.Collections.Frozen;

namespace Api.Catalog.Domain.Models;

[AttributeUsage(AttributeTargets.Class)]
public class ModuleAttribute(string moduleName) : Attribute
{
    public string ModuleName { get; } = moduleName;
}

public static class Modules
{
    public static readonly FrozenSet<string> All = [
        Tables,
    ];
    public const string Tables = "TABLES";
    //public const string Catalog = "CATALOG";
    //public const string Comercial = "COMERCIAL";

    public static bool Exists(string module) => All.Contains(module);
}
