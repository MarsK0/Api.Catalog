using Api.Catalog.Domain.Models;
using Api.Catalog.Tests.Unit.Domain.ValueObjects;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Models;

public class PricingContextTests
{
    private static readonly Faker faker = new();

    [Fact]
    public void Create_WithValidInputs_ShouldReturnValidSnapshotWithExpectedValues()
    {
        //Arrange
        var expectedPersonSnapshot = PersonSnapshotTests.CreateValidSnapshot();
        var expectedQuantity = faker.Random.Decimal(min: 1);
        //Act
        var context = PricingContext.Create(expectedPersonSnapshot, expectedQuantity).Value;
        //Assert
        context.Person.Should().Be(expectedPersonSnapshot);
        context.Quantity.Should().Be(expectedQuantity);
    }
    [Fact]
    public void Create_WithValidInputsAndNullablesNull_ShouldReturnValidSnapshotWithExpectedValues()
    {
        //Arrange
        var expectedQuantity = faker.Random.Decimal(min: 1);
        //Act
        var context = PricingContext.Create(null, expectedQuantity).Value;
        //Assert
        context.Person.Should().BeNull();
        context.Quantity.Should().Be(expectedQuantity);
    }
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithQuantityZeroOrLess_ShouldReturnFailureWithExpectedCodeAndMessage(int quantity)
    {
        //Arrange
        //Act
        var result = PricingContext.Create(null, quantity);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Uma quantidade maior que zero deve ser informada.");
    }
}
