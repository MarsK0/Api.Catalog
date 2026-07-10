namespace Api.Catalog.Domain.ValueObjects;

public sealed class RoleInfo
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    private readonly List<string> _permissions = new();
    public IReadOnlyList<string> Permissions => _permissions.AsReadOnly();
    private RoleInfo() { }
    public static AppResult<RoleInfo> Create(
        string name,
        string description
    )
    {
        return new RoleInfo
        {
            Name = name,
            Description = description
        };
    }

    public void AssignPermission(string permission)
        => _permissions.Add(permission);
}
