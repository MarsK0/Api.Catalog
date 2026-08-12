using Api.Catalog.Domain.Entities;
using Api.Catalog.Domain.Models;
using Api.Catalog.Domain.ValueObjects;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Entities.Budget;

public class BudgetItemTests
{
    private static readonly Faker faker = new();
    private static Product ValidProduct()
        => Product.Create(faker.Random.String2(10), faker.Random.String2(10)).Value;
    private static ProductSnapshot ValidProductSnapshot()
        => ProductSnapshot.Create(ValidProduct()).Value;
    private static DirectPriceRule ValidPriceRule()
        => DirectPriceRule.Create(Guid.NewGuid(), Guid.NewGuid(), faker.Random.Decimal(min:0)).Value;
    private static PriceRuleSnapshot ValidPriceRuleSnapshot()
        => PriceRuleSnapshot.Create(ValidPriceRule()).Value;
    public static BudgetItem CreateValid() => BudgetItem.Create(Guid.NewGuid(), 1, ValidProductSnapshot(), ValidPriceRuleSnapshot()).Value;

    [Fact]
    public void Create_WithValidInputs_ShouldReturnBudgetItemWithExpectedFields()
    {
        //Arrange
        var expectedBudgetId = Guid.NewGuid();
        var expectedQuantity = faker.Random.Decimal(min:1);
        var expectedProductSnapshot = ValidProductSnapshot();
        var expectedPriceRuleSnapshot = ValidPriceRuleSnapshot();
        //Act
        var budgetItem = BudgetItem.Create(
            expectedBudgetId,
            expectedQuantity,
            expectedProductSnapshot,
            expectedPriceRuleSnapshot
        ).Value;
        //Assert
        budgetItem.Id.Should().NotBeEmpty();
        budgetItem.BudgetId.Should().Be(expectedBudgetId);
        budgetItem.Quantity.Should().Be(expectedQuantity);
        budgetItem.ProductSnapshot.Should().Be(expectedProductSnapshot);
        budgetItem.PriceRuleSnapshot.Should().Be(expectedPriceRuleSnapshot);
    }
    [Fact]
    public void Create_WithEmptyBudgetId_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var result = BudgetItem.Create(
            Guid.Empty,
            faker.Random.Decimal(min: 1),
            ValidProductSnapshot(),
            ValidPriceRuleSnapshot()
        );
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Um orçamento deve ser informado para a inclusão do item.");
    }
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Create_WithInvalidQuantity_ShouldReturnFailureWithExpectedCodeAndMessage(int quantity)
    {
        //Arrange
        //Act
        var result = BudgetItem.Create(
            Guid.NewGuid(),
            quantity,
            ValidProductSnapshot(),
            ValidPriceRuleSnapshot()
        );
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("O item deve possuir uma quantidade maior que zero informada.");
    }
    [Fact]
    public void Create_WithNullProductSnapshot_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var result = BudgetItem.Create(
            Guid.NewGuid(),
            faker.Random.Decimal(min: 1),
            null!,
            ValidPriceRuleSnapshot()
        );
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Uma snapshot do produto deve ser informada para o item.");
    }
    [Fact]
    public void Create_WithNullPriceRuleSnapshot_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var result = BudgetItem.Create(
            Guid.NewGuid(),
            faker.Random.Decimal(min: 1),
            ValidProductSnapshot(),
            null!
        );
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Uma snapshot da regra de preço deve ser informada para o item.");
    }
}
