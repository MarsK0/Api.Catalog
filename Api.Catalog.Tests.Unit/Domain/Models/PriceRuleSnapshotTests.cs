using Api.Catalog.Domain.Enums;
using Api.Catalog.Domain.Models;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Models;

public class PriceRuleSnapshotTests
{
    private static readonly Faker faker = new();
    [Fact]
    public void Create_WithValidInput_ShouldReturnValidSnapshotWithExpectedValues()
    {
        //Arrange
        var expectedId = Guid.NewGuid();
        var expectedRuleType = EPriceRuleType.Direct;
        var expectedPrice = faker.Random.Decimal(min: 1m);
        //Act
        var rule = PriceRuleSnapshot.Create(expectedId, expectedRuleType, expectedPrice).Value;
        //Assert
        rule.PriceRuleId.Should().Be(expectedId);
        rule.RuleType.Should().Be(expectedRuleType);
        rule.Price.Should().Be(expectedPrice);
    }
    [Fact]
    public void Create_WithPriceZero_ShouldReturnValidSnapshotWithExpectedValues()
    {
        //Arrange
        var expectedId = Guid.NewGuid();
        var expectedRuleType = EPriceRuleType.Direct;
        var expectedPrice = 0;
        //Act
        var rule = PriceRuleSnapshot.Create(expectedId, expectedRuleType, expectedPrice).Value;
        //Assert
        rule.PriceRuleId.Should().Be(expectedId);
        rule.RuleType.Should().Be(expectedRuleType);
        rule.Price.Should().Be(expectedPrice);
    }
    [Fact]
    public void Create_WithInvalidId_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var rule = PriceRuleSnapshot.Create(Guid.Empty, EPriceRuleType.Direct, faker.Random.Decimal(min: 1));
        //Assert
        rule.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        rule.Failure.Message.Should().Be("Um id de regra de preço deve ser informada para a snapshot da regra de preço.");
    }
    [Fact]
    public void Create_WithInvalidRuleType_ShoudlReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var rule = PriceRuleSnapshot.Create(Guid.NewGuid(), (EPriceRuleType)(-1), faker.Random.Decimal(min: 1));
        //Assert
        rule.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        rule.Failure.Message.Should().Be("Tipo de regra de preço inválida.");
    }
    [Fact]
    public void Create_WithInvalidPrice_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var rule = PriceRuleSnapshot.Create(Guid.NewGuid(), EPriceRuleType.Direct, -1);
        //Assert
        rule.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        rule.Failure.Message.Should().Be("O preço informado deve ser maior ou igual a 0.");
    }
}
