using Api.Catalog.Domain.Models;
using Api.Catalog.Domain.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace Api.Catalog.Domain.Entities;

public class Budget : TenantScopedEntity
{
    public DateTimeOffset ValidUntil { get; private set; }
    public string? UserEmail { get; private set; } = null!;

    private readonly List<BudgetItem> _items = [];
    public IReadOnlyCollection<BudgetItem> Items => _items.AsReadOnly();

    public Guid? PersonId { get; private set; }
    private readonly Person? _person = null;
    public Person? Person => _person;

    private Budget() { }

    public static Result<Budget> Create(
        DateTimeOffset validUntil,
        string? userEmail,
        Guid? personId,
        TimeProvider? timeProvider = null
    )
    {
        timeProvider ??= TimeProvider.System;

        if (validUntil < timeProvider.GetUtcNow())
            return DomainResultFailures.Validation("A data de validade do orçamento deve ser superior a agora.");

        return new Budget
        {
            ValidUntil = validUntil,
            UserEmail = userEmail,
            PersonId = personId,
        };
    }
    public void AddItem(BudgetItem item) => _items.Add(item);
}
