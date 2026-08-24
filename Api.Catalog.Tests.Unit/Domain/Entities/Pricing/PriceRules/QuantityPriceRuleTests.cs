using Api.Catalog.Domain.Entities;
using Api.Catalog.Domain.Enums;
using Api.Catalog.Domain.Models;
using Api.Catalog.Tests.Unit.Domain.Models;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Entities.Pricing.PriceRules;

public class QuantityPriceRuleTests
{
    private static readonly Faker faker = new();
    public static QuantityPriceRule CreateValid(decimal? min = null, decimal? max = null, decimal? price = null)
        => QuantityPriceRule.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            price ?? faker.Random.Decimal(min: 0),
            new ProductQuantityCondition(min ?? 0, max ?? faker.Random.Decimal(min: 1, max: 1000))
        ).Value;

    [Fact]
    public void Create_WithValidInputs_ShouldReturnQuantityPriceRuleWithExpectedValues()
    {
        //Arrange
        var expectedPriceListId = Guid.NewGuid();
        var expectedProductId = Guid.NewGuid();
        var expectedPrice = faker.Random.Decimal(min: 0);
        var expectedCondition = new ProductQuantityCondition(1, 10);
        //Act
        var priceRule = QuantityPriceRule.Create(expectedPriceListId, expectedProductId, expectedPrice, expectedCondition).Value;
        //Assert
        priceRule.PriceListId.Should().Be(expectedPriceListId);
        priceRule.ProductId.Should().Be(expectedProductId);
        priceRule.Price.Should().Be(expectedPrice);
        priceRule.Min.Should().Be(expectedCondition.Min);
        priceRule.Max.Should().Be(expectedCondition.Max);
        priceRule.RuleType.Should().Be(EPriceRuleType.ProductQuantity);
    }
    [Fact]
    public void Create_WithConditionsMinAndMaxEqualZero_ShouldBeValid()
    {
        //Arrange
        var condition = new ProductQuantityCondition(0, 0);
        //Act
        var result = QuantityPriceRule.Create(Guid.NewGuid(), Guid.NewGuid(), faker.Random.Decimal(min: 0), condition);
        //Assert
        result.IsSuccess.Should().BeTrue();
    }
    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(-1, -1)]
    public void Create_WithConditionsLessThanZero_ShouldReturnFailureWithExpectedCodeAndMessage(decimal min, decimal max)
    {
        //Arrange
        var condition = new ProductQuantityCondition(min, max);
        //Act
        var result = QuantityPriceRule.Create(Guid.NewGuid(), Guid.NewGuid(), faker.Random.Decimal(min: 0), condition);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("As quantidades mínima e máxima devem ser maiores ou iguais a zero.");
    }
    [Fact]
    public void Create_WithEmptyPriceListId_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var result = QuantityPriceRule.Create(Guid.Empty, Guid.NewGuid(), faker.Random.Decimal(min: 0), new ProductQuantityCondition(1, 10));
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Uma lista de preço deve ser informada para a regra de preço.");
    }

    [Fact]
    public void Create_WithEmptyProductId_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var result = QuantityPriceRule.Create(Guid.NewGuid(), Guid.Empty, faker.Random.Decimal(min: 0), new ProductQuantityCondition(1, 10));
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
        var result = QuantityPriceRule.Create(Guid.NewGuid(), Guid.NewGuid(), price, new ProductQuantityCondition(1, 10));
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("O valor do preço do produto deve ser maior ou igual a zero");
    }

    [Theory]
    [InlineData(-1, 10)]
    [InlineData(0, -1)]
    [InlineData(-0.01, -0.01)]
    public void Create_WithNegativeMinOrMax_ShouldReturnFailureWithExpectedCodeAndMessage(decimal min, decimal max)
    {
        //Arrange
        //Act
        var result = QuantityPriceRule.Create(Guid.NewGuid(), Guid.NewGuid(), faker.Random.Decimal(min: 0), new ProductQuantityCondition(min, max));
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("As quantidades mínima e máxima devem ser maiores ou iguais a zero.");
    }

    [Fact]
    public void Create_WithMinGreaterThanMax_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var result = QuantityPriceRule.Create(Guid.NewGuid(), Guid.NewGuid(), faker.Random.Decimal(min: 0), new ProductQuantityCondition(10, 5));
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("A quantidade mínima não deve ser maior do que a quantidade máxima");
    }

    [Fact]
    public void Create_WithMinEqualToMax_ShouldReturnQuantityPriceRuleWithExpectedValues()
    {
        //Arrange
        var expectedMin = 5;
        var expectedMax = 5;
        //Act
        var result = QuantityPriceRule.Create(Guid.NewGuid(), Guid.NewGuid(), faker.Random.Decimal(min: 0), new ProductQuantityCondition(expectedMin, expectedMax));
        //Assert
        result.Value.Min.Should().Be(expectedMin);
        result.Value.Max.Should().Be(expectedMax);
    }

    [Fact]
    public void AppliesTo_WithQuantityBelowMin_ShouldReturnFalse()
    {
        //Arrange
        var priceRule = CreateValid(min: 5, max: 10);
        var context = PricingContextTests.CreateValid(quantity: 4);
        //Act
        var result = priceRule.AppliesTo(context);
        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void AppliesTo_WithQuantityAtMin_ShouldReturnTrue()
    {
        //Arrange
        var priceRule = CreateValid(min: 5, max: 10);
        var context = PricingContextTests.CreateValid(quantity: 5);
        //Act
        var result = priceRule.AppliesTo(context);
        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void AppliesTo_WithQuantityBetweenMinAndMax_ShouldReturnTrue()
    {
        //Arrange
        var priceRule = CreateValid(min: 5, max: 10);
        var context = PricingContextTests.CreateValid(quantity: 7);
        //Act
        var result = priceRule.AppliesTo(context);
        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void AppliesTo_WithQuantityAtMax_ShouldReturnTrue()
    {
        //Arrange
        var priceRule = CreateValid(min: 5, max: 10);
        var context = PricingContextTests.CreateValid(quantity: 10);
        //Act
        var result = priceRule.AppliesTo(context);
        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void AppliesTo_WithQuantityAboveMax_ShouldReturnFalse()
    {
        //Arrange
        var priceRule = CreateValid(min: 5, max: 10);
        var context = PricingContextTests.CreateValid(quantity: 11);
        //Act
        var result = priceRule.AppliesTo(context);
        //Assert
        result.Should().BeFalse();
    }
}