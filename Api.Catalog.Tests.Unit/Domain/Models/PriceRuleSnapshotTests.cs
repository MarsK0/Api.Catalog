using Api.Catalog.Domain.Entities;
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
        var expectedPriceRule = DirectPriceRule.Create(Guid.NewGuid(), Guid.NewGuid(), faker.Random.Decimal(min: 0)).Value;
        //Act
        var rule = PriceRuleSnapshot.Create(expectedPriceRule).Value;
        //Assert
        rule.PriceRuleId.Should().Be(expectedPriceRule.Id);
        rule.Price.Should().Be(expectedPriceRule.Price);
    }
    [Fact]
    public void Create_WithNullPriceRule_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var rule = PriceRuleSnapshot.Create(null!);
        //Assert
        rule.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        rule.Failure.Message.Should().Be("Informe uma regra de preço para a snapshot.");
    }
}
