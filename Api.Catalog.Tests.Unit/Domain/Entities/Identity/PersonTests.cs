using Api.Catalog.Domain.Entities;
using Api.Catalog.Domain.Models;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Entities.Identity;

public class PersonTests
{
    private static readonly Bogus.Faker faker = new();
    public static Person CreateValid() => Person.Create(faker.Random.String2(10), faker.Internet.Email(), faker.Phone.PhoneNumber()).Value;
    [Fact]
    public void Create_WithValidInputs_ShouldReturnPersonWithExpectedValues()
    {
        //Arrange
        var expectedName = faker.Random.String2(10);
        var expectedEmail = faker.Internet.Email();
        var expectedPhone = faker.Phone.PhoneNumber();
        //Act
        var person = Person.Create(expectedName, expectedEmail, expectedPhone).Value;
        //Assert
        person.Name.Should().Be(expectedName);
        person.Email.Should().Be(expectedEmail);
        person.Phone.Should().Be(expectedPhone);
    }
    [Fact]
    public void Create_WithValidInputsAnNullableNulls_ShouldReturnPersonWithExpectedValues()
    {
        //Arrange
        //Act
        var person = Person.Create(faker.Random.String2(10), faker.Internet.Email(), null).Value;
        //Assert
        person.Phone.Should().BeNull();
    }
    [Theory]
    [InlineData("ABC")]
    [InlineData("Rp4pjbYsCo3VANwPTAnHaWTrsA4En7UHujOP2LHTXUVdNov7FM2OMMDVRU8m")]
    public void Create_WithNameAtBounds_ShouldBeValid(string name)
    {
        //Arrange
        //Act
        var result = Person.Create(name, faker.Internet.Email(), faker.Phone.PhoneNumber());
        //Assert
        result.IsSuccess.Should().BeTrue();
    }
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ShouldReturnFailureWithExpectedCodeAndMessage(string? name)
    {
        //Arrange
        //Act
        var result = Person.Create(name!, faker.Internet.Email(), faker.Phone.PhoneNumber());
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Um nome deve ser informado para a pessoa.");
    }
    [Theory]
    [InlineData("AB")]
    [InlineData("Rp4pjbYsCo3VANwPTAnHaWTrsA4En7UHujOP2LHTXUVdNov7FM2OMMDVRU8ma")]
    public void Create_WithNameOutOfBounds_ShouldReturnFailureWithExpectedCodeAndMessage(string name)
    {
        //Arrange
        //Act
        var result = Person.Create(name, faker.Internet.Email(), faker.Phone.PhoneNumber());
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("O nome deve conter entre 3 e 60 caracteres.");
    }
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithEmptyEmail_ShouldReturnFailureWithExpectedCodeAndMessage(string? email)
    {
        //Arrange
        //Act
        var result = Person.Create(faker.Random.String2(10), email!, faker.Phone.PhoneNumber());
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Um e-mail deve ser informado para a pessoa.");
    }
    [Fact]
    public void AssignRole_Once_ShouldInsertRole()
    {
        //Arrange
        var person = CreateValid();
        var role = RoleTests.CreateValid();
        //Act
        person.AssignRole(role);
        //Assert
        person.Roles.Should().Contain(role);
    }
    [Fact]
    public void AssignRole_MultipleTimesWithSameRole_ShouldInsertOnlyOnce()
    {
        //Arrange
        var person = CreateValid();
        var role = RoleTests.CreateValid();
        //Act
        person.AssignRole(role);
        person.AssignRole(role);
        //Assert
        person.Roles.Where(w => w == role).Should().HaveCount(1);
    }
}
