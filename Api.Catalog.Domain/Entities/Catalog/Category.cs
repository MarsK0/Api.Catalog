namespace Api.Catalog.Domain.Entities;

public class Category : TenantScopedEntity
{
    public Guid? ParentId { get; private set; }
    public string Description { get; private set; } = null!;
    private readonly List<Category> _subCategories = new();

    public IReadOnlyCollection<Category> SubCategories => _subCategories.AsReadOnly();

    public static AppResult<Category> Create(
        string description
    )
    {
        return new Category
        {
            Description = description,
        };
    }

    public AppResult<Category> CreateSubCategory(
        string description
    )
    {
        var _sub = new Category
        {
            ParentId = this.Id,
            Description = description
        };

        _subCategories.Add(_sub);

        return _sub;
    }
}
