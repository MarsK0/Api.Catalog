using Api.Catalog.Domain.Models;
using System.Diagnostics.CodeAnalysis;

namespace Api.Catalog.Domain.Entities;

public class Budget : TenantScopedEntity
{
    public DateTime ValidUntil { get; private set; }
    public string UserEmail { get; private set; } = null!;

    private readonly List<BudgetItem> _items = new();
    public IReadOnlyCollection<BudgetItem> Items => _items.AsReadOnly();

    public Guid? PersonId { get; private set; }
    [SuppressMessage("Compiler", "CS0649", Justification = "Populado na camada de infra")]
    private Person? _person = null!;
    public Person? Person => _person;

    private Budget() { }

    public static AppResult<Budget> Create(
        DateTime validUntil,
        string userEmail,
        Guid? userId
    )
    {
        return new Budget
        {
            ValidUntil = validUntil,
            UserEmail = userEmail,
            PersonId = userId,
        };
    }
    public AppResult AddItem(
        decimal quantity,
        Guid productID,
        ProductSnapshot productSnapshot,
        PriceRuleSnapshot priceRuleSnapshot
    )
    {
        var bItemResult = BudgetItem.Create(
            this.Id,
            quantity,
            productID,
            productSnapshot,
            priceRuleSnapshot
        );

        if (!bItemResult.IsSuccess)
            return bItemResult.Failure;

        _items.Add(bItemResult.Value);
        return AppResult.Success;
    }
}
