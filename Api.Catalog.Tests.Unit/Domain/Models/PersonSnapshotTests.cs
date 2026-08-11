using Api.Catalog.Domain.Models;
using Api.Catalog.Domain.ValueObjects;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Models;

public class PersonSnapshotTests
{
    private static readonly Faker faker = new();
    public static Api.Catalog.Domain.Entities.Person CreateValidPerson()
        => Api.Catalog.Domain.Entities.Person.Create(
            faker.Person.FullName,
            faker.Internet.Email(),
            faker.Phone.ToString()
        ).Value;
    public static PersonSnapshot CreateValidSnapshot() => PersonSnapshot.Create(CreateValidPerson()).Value;
    [Fact]
    public void Create_WithValidInput_ShouldReturnValidSnapshotWithExpectedValues()
    {
        //Arrange
        var expectedPerson = CreateValidPerson();
        //Act
        var person = PersonSnapshot.Create(expectedPerson).Value;
        //Assert
        person.Id.Should().Be(expectedPerson.Id);
        person.Name.Should().Be(expectedPerson.Name);
    }
    [Fact]
    public void Create_WithNullInput_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var result = PersonSnapshot.Create(null!);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Informe uma pessoa para a snapshot.");
    }
}
