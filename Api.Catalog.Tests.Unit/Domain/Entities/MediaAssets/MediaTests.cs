using Api.Catalog.Domain.Entities;
using Api.Catalog.Domain.Models;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Entities.MediaAssets;

public class MediaTests
{
    private static readonly Bogus.Faker faker = new();
    public static Media CreateValid()
        => Media.Create(faker.Random.Long(min: 1), $".{faker.Random.String2(3)}", faker.Random.String2(20), faker.Random.Bytes(32)).Value;

    [Fact]
    public void Create_WithValidInputs_ShouldReturnMediaWithExpectedValues()
    {
        //Arrange
        var expectedSize = faker.Random.Long(min: 1);
        var expectedExtension = $".{faker.Random.String2(3)}";
        var expectedContentType = faker.Random.String2(20);
        var expectedHash = faker.Random.Bytes(32);
        //Act
        var media = Media.Create(expectedSize, expectedExtension, expectedContentType, expectedHash).Value;
        //Assert
        media.Size.Should().Be(expectedSize);
        media.Extension.Should().Be(expectedExtension);
        media.ContentType.Should().Be(expectedContentType);
        media.Hash.Should().BeEquivalentTo(expectedHash);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithSizeZeroOrNegative_ShouldReturnFailureWithExpectedCodeAndMessage(long size)
    {
        //Arrange
        //Act
        var result = Media.Create(size, $".{faker.Random.String2(3)}", faker.Random.String2(20), faker.Random.Bytes(32));
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("O tamanho da mídia deve ser superior a zero.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithEmptyExtension_ShouldReturnFailureWithExpectedCodeAndMessage(string? extension)
    {
        //Arrange
        //Act
        var result = Media.Create(faker.Random.Long(min: 1), extension!, faker.Random.String2(20), faker.Random.Bytes(32));
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Uma extensão deve ser informado para a mídia.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithEmptyContentType_ShouldReturnFailureWithExpectedCodeAndMessage(string? contentType)
    {
        //Arrange
        //Act
        var result = Media.Create(faker.Random.Long(min: 1), $".{faker.Random.String2(3)}", contentType!, faker.Random.Bytes(32));
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Um tipo de conteúdo deve ser informado para a mídia");
    }

    [Fact]
    public void Create_WithEmptyHash_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        var hash = Array.Empty<byte>();
        //Act
        var result = Media.Create(faker.Random.Long(min: 1), $".{faker.Random.String2(3)}", faker.Random.String2(20), hash);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("A hash da mídia deve ser informada");
    }
}