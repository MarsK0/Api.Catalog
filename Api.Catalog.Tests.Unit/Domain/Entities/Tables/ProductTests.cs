using Api.Catalog.Domain.Entities;
using Api.Catalog.Domain.Models;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Entities.Catalog;

public class ProductTests
{
    private static readonly Faker faker = new();

    [Fact]
    public void Create_WithValidInputs_ShouldReturnProductWithExpectedValues()
    {
        //Arrange
        var expectedDescription = faker.Random.String2(10);
        var expectedReference = faker.Random.String2(10);
        //Act
        var product = Product.Create(expectedDescription, expectedReference).Value;
        //Assert
        product.Description.Should().Be(expectedDescription);
        product.Reference.Should().Be(expectedReference);
    }

    [Fact]
    public void Create_WithValidInputsAndNullableNull_ShouldReturnProductWithExpectedValues()
    {
        //Arrange
        var expectedDescription = faker.Random.String2(10);
        //Act
        var product = Product.Create(expectedDescription, null).Value;
        //Assert
        product.Reference.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithEmptyDescription_ShouldReturnFailureWithExpectedCodeAndMessage(string? description)
    {
        //Arrange
        //Act
        var result = Product.Create(description!, faker.Random.String2(10));
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Uma descrição deve ser informada para o produto.");
    }
}