using Api.Catalog.Domain.Models;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Models;

public class PersonSnapshotTests
{
    private static readonly Faker faker = new();
    public static PersonSnapshot CreateValid() => PersonSnapshot.Create(Guid.NewGuid(), faker.Name.FullName()).Value;
    [Fact]
    public void Create_WithValidInput_ShouldReturnValidSnapshotWithExpectedValues()
    {
        //Arrange
        var expectedGuid = Guid.NewGuid();
        var expectedName = faker.Name.FullName();
        //Act
        var person = PersonSnapshot.Create(expectedGuid, expectedName).Value;
        //Assert
        person.Id.Should().Be(expectedGuid);
        person.Name.Should().Be(expectedName);
    }
    [Fact]
    public void Create_WithInvalidId_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var result = PersonSnapshot.Create(Guid.Empty, faker.Name.FullName());
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Um id de pessoa deve ser informada para a snapshot da pessoa.");
    }
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithInvalidName_ShouldReturnFailureWithExpectedCodeAndMessage(string? name)
    {
        //Arrange
        //Act
        var result = PersonSnapshot.Create(Guid.NewGuid(), name!);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("O nome da pessoa deve ser informado na snapshot.");
    }
}
