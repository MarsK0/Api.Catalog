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
        var expectedProduct = Api.Catalog.Domain.Entities.Product.Create(faker.Random.String2(10), faker.Random.String2(10)).Value;
        //Act
        var product = ProductSnapshot.Create(expectedProduct).Value;
        //Assert
        product.Description.Should().Be(expectedProduct.Description);
        product.Reference.Should().Be(expectedProduct.Reference);
    }
    [Fact]
    public void Create_WithNullProduct_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var product = ProductSnapshot.Create(null!);
        //Assert
        product.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        product.Failure.Message.Should().Be("Informe um produto para a snapshot.");
    }
}
