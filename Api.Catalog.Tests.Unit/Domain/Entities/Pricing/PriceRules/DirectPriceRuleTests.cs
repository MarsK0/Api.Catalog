using Api.Catalog.Domain.Entities;
using Api.Catalog.Domain.Enums;
using Api.Catalog.Domain.Models;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Entities.Pricing;

public class DirectPriceRuleTests
{
    private static readonly Bogus.Faker faker = new();
    public static DirectPriceRule CreateValid()
        => DirectPriceRule.Create(Guid.NewGuid(), Guid.NewGuid(), faker.Random.Decimal(min: 0)).Value;

    [Fact]
    public void Create_WithValidInputs_ShouldReturnDirectPriceRuleWithExpectedValues()
    {
        //Arrange
        var expectedPriceListId = Guid.NewGuid();
        var expectedProductId = Guid.NewGuid();
        var expectedPrice = faker.Random.Decimal(min: 0);
        //Act
        var priceRule = DirectPriceRule.Create(expectedPriceListId, expectedProductId, expectedPrice).Value;
        //Assert
        priceRule.PriceListId.Should().Be(expectedPriceListId);
        priceRule.ProductId.Should().Be(expectedProductId);
        priceRule.Price.Should().Be(expectedPrice);
        priceRule.RuleType.Should().Be(EPriceRuleType.Direct);
    }

    [Fact]
    public void Create_WithEmptyPriceListId_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var result = DirectPriceRule.Create(Guid.Empty, Guid.NewGuid(), faker.Random.Decimal(min: 0));
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Uma lista de preço deve ser informada para a regra de preço.");
    }

    [Fact]
    public void Create_WithEmptyProductId_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var result = DirectPriceRule.Create(Guid.NewGuid(), Guid.Empty, faker.Random.Decimal(min: 0));
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Um produto deve ser informado para a regra de preço.");
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1)]
    public void Create_WithNegativePrice_ShouldReturnFailureWithExpectedCodeAndMessage(decimal price)
    {
        //Arrange
        //Act
        var result = DirectPriceRule.Create(Guid.NewGuid(), Guid.NewGuid(), price);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("O valor do preço do produto deve ser maior ou igual a zero");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.01)]
    public void Create_WithPriceAtOrAboveZero_ShouldBeValid(decimal price)
    {
        //Arrange
        //Act
        var result = DirectPriceRule.Create(Guid.NewGuid(), Guid.NewGuid(), price);
        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void AppliesTo_WithAnyContext_ShouldAlwaysReturnTrue()
    {
        //Arrange
        var priceRule = CreateValid();
        //Act
        var result = priceRule.AppliesTo(null!);
        //Assert
        result.Should().BeTrue();
    }
}