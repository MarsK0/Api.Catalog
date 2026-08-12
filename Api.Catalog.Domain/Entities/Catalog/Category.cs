using Api.Catalog.Domain.Models;

namespace Api.Catalog.Domain.Entities;

public class Category : TenantScopedEntity
{
    public Guid? ParentId { get; private set; }
    public string Description { get; private set; } = null!;
    private readonly List<Category> _subCategories = [];

    public IReadOnlyCollection<Category> SubCategories => _subCategories.AsReadOnly();
    private Category() { }
    public static Result<Category> Create(
        string description
    )
    {
        if (string.IsNullOrWhiteSpace(description))
            return DomainResultFailures.Validation("Uma descrição deve ser fornecida para a categoria.");

        return new Category
        {
            Description = description,
        };
    }

    public Result<Category> CreateSubCategory(
        string description
    )
    {
        var _find = _subCategories.FirstOrDefault(f => f.Description == description);
        if (_find is not null)
            return _find;

        var result = Create(description);
        if (!result.IsSuccess)
            return result.Failure;

        _subCategories.Add(result.Value);
        return result.Value;
    }
}
