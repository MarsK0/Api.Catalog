namespace Api.Catalog.Domain.Entities;

public class Person : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? Phone { get; private set; }
    private readonly List<TenantRole> _tenantRoles = new();
    private readonly List<PlatformRole> _platformRoles = new();
    public IReadOnlyCollection<TenantRole> TenantRoles => _tenantRoles.AsReadOnly();
    public IReadOnlyCollection<PlatformRole> PlatformRoles => _platformRoles.AsReadOnly();

    private Person() { }

    public static AppResult<Person> Create(
        string name,
        string email,
        string? phone
    )
    {
        return new Person
        {
            Name = name,
            Email = email,
            Phone = phone
        };
    }

    public static AppResult<Person> Create(
        Guid id,
        string name,
        string email,
        string? phone
    )
    {
        return new Person
        {
            Id = id,
            Name = name,
            Email = email,
            Phone = phone
        };
    }
}
