using Api.Catalog.Domain.Models;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Models;

public class ProductSnapshotTests
{
    private static readonly Faker faker = new();
    [Fact]
    public void Create_WithValidInputs_ShouldReturnValidSnapshotWithExpectedValues()
    {
        //Arrange
        var expectedDescription = faker.Random.String2(10);
        var expectedReference = faker.Random.String2(10);
        //Act
        var product = ProductSnapshot.Create(expectedDescription, expectedReference).Value;
        //Assert
        product.Description.Should().Be(expectedDescription);
        product.Reference.Should().Be(expectedReference);
    }
    [Fact]
    public void Create_WithValidInputsAndNullablesNull_ShouldReturnValidSnapshotWithExpectedValues()
    {
        //Arrange
        var expectedDescription = faker.Random.String2(10);
        //Act
        var product = ProductSnapshot.Create(expectedDescription, null).Value;
        //Assert
        product.Reference.Should().BeNull();
    }
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithValidInputsAndEmptyReference_ShouldReturnValidSnapshotWithReferenceNull(string reference)
    {
        //Arrange
        var expectedDescription = faker.Random.String2(10);
        //Act
        var product = ProductSnapshot.Create(expectedDescription, reference).Value;
        //Assert
        product.Reference.Should().BeNull();
    }
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithInvalidDescription_ShouldReturnFailureWithExpectedCodeAndMessage(string? description)
    {
        //Arrange
        //Act
        var result = ProductSnapshot.Create(description!, string.Empty);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Uma descrição deve ser informada para a snapshot do produto.");
    }
}
