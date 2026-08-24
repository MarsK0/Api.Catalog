using Api.Catalog.Domain.Entities;
using Api.Catalog.Domain.Models;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Entities.Pricing;

public class PriceListTests
{
    private static readonly Faker faker = new();

    [Fact]
    public void Create_WithValidInputs_ShouldReturnPriceListWithExpectedValues()
    {
        //Arrange
        var expectedName = faker.Random.String2(10);
        var expectedValidFrom = faker.Date.PastOffset();
        var expectedValidUntil = faker.Date.FutureOffset();
        //Act
        var priceList = PriceList.Create(expectedName, expectedValidFrom, expectedValidUntil).Value;
        //Assert
        priceList.Name.Should().Be(expectedName);
        priceList.ValidFrom.Should().Be(expectedValidFrom);
        priceList.ValidUntil.Should().Be(expectedValidUntil);
    }

    [Fact]
    public void Create_WithValidInputsAndNullablesNull_ShouldReturnPriceListWithExpectedValues()
    {
        //Arrange
        var expectedName = faker.Random.String2(10);
        //Act
        var priceList = PriceList.Create(expectedName, null, null).Value;
        //Assert
        priceList.Name.Should().Be(expectedName);
        priceList.ValidFrom.Should().BeNull();
        priceList.ValidUntil.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ShouldReturnFailureWithExpectedCodeAndMessage(string? name)
    {
        //Arrange
        //Act
        var result = PriceList.Create(name!, faker.Date.PastOffset(), faker.Date.FutureOffset());
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Um nome deve ser informado para a lista de preços.");
    }
}