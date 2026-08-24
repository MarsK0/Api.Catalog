using Api.Catalog.Domain.Entities;
using Api.Catalog.Domain.Models;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Entities.MediaAssets;

public class AssetTests
{
    private static readonly Bogus.Faker faker = new();
    [Fact]
    public void Create_WithValidInputs_ShouldReturnAssetWithExpectedValues()
    {
        //Arrange
        var expectedMediaId = Guid.NewGuid();
        var expectedFileName = faker.Random.String2(20);
        //Act
        var asset = Asset.Create(expectedMediaId, expectedFileName).Value;
        //Assert
        asset.MediaId.Should().Be(expectedMediaId);
        asset.FileName.Should().Be(expectedFileName);
    }
    [Fact]
    public void Create_WithEmptyMediaId_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var result = Asset.Create(Guid.Empty, faker.Random.String2(20));
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Uma mídia deve ser informada para o recurso.");
    }
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithEmptyFileName_ShouldReturnFailureWithExpectedCodeAndMessage(string? fileName)
    {
        //Arrange
        //Act
        var result = Asset.Create(Guid.NewGuid(), fileName!);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Um nome de arquivo deve ser informado para o recurso.");
    }
}
