using Api.Catalog.Application;
using Api.Catalog.Application.Entities;
using Api.Catalog.Application.Enums;
using Api.Catalog.Tests.Unit.Domain.Entities.Identity;
using FluentAssertions;
using System.Reflection;
using Xunit;

namespace Api.Catalog.Tests.Unit.Application.Entities.Auth;

public class AccountTests
{
    private static readonly Bogus.Faker faker = new();
    public static Account CreateValid()
        => Account.Create(Guid.NewGuid(), faker.Random.String2(10), faker.Random.String2(10)).Value;
    public static Account CreateValidWithPerson(bool personEnabled = true)
    {
        var person = PersonTests.CreateValid();
        if (!personEnabled)
            person.Disable();

        var account = CreateValid();

        // Atribui o campo privado _person sem expor a propriedade no domínio
        typeof(Account)
            .GetField("_person", BindingFlags.Instance | BindingFlags.NonPublic)?
            .SetValue(account, person);

        return account;
    }
    [Fact]
    public void Create_WithValidInputs_ShouldReturnAccountWithExpectedValues()
    {
        //Arrange
        var expectedPersonId = Guid.NewGuid();
        var expectedLogin = faker.Random.String2(10);
        var expectedPasswordHash = faker.Random.String2(10);
        //Act
        var account = Account.Create(expectedPersonId, expectedLogin, expectedPasswordHash).Value;
        //Assert
        account.PersonId.Should().Be(expectedPersonId);
        account.Login.Should().Be(expectedLogin);
        account.PasswordHash.Should().Be(expectedPasswordHash);
        account.Status.Should().Be(EAccountStatus.Enabled);
    }

    [Fact]
    public void Create_WithEmptyPersonId_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var result = Account.Create(Guid.Empty, faker.Random.String2(10), faker.Random.String2(10));
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.Validation);
        result.Failure.Message.Should().Be("Deve ser informada uma pessoa para vínculo com a conta.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithEmptyLogin_ShouldReturnFailureWithExpectedCodeAndMessage(string? login)
    {
        //Arrange
        //Act
        var result = Account.Create(Guid.NewGuid(), login!, faker.Random.String2(10));
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.Validation);
        result.Failure.Message.Should().Be("O login deve conter ao menos 3 caracteres.");
    }

    [Theory]
    [InlineData("a")]
    [InlineData("ab")]
    public void Create_WithLoginBelowMinLength_ShouldReturnFailureWithExpectedCodeAndMessage(string login)
    {
        //Arrange
        //Act
        var result = Account.Create(Guid.NewGuid(), login, faker.Random.String2(10));
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.Validation);
        result.Failure.Message.Should().Be("O login deve conter ao menos 3 caracteres.");
    }

    [Fact]
    public void Create_WithLoginAtMinLength_ShouldReturnAccountSuccessfully()
    {
        //Arrange
        //Act
        var result = Account.Create(Guid.NewGuid(), "abc", faker.Random.String2(10));
        //Assert
        result.Value.Login.Should().Be("abc");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithEmptyPasswordHash_ShouldReturnFailureWithExpectedCodeAndMessage(string? passwordHash)
    {
        //Arrange
        //Act
        var result = Account.Create(Guid.NewGuid(), faker.Random.String2(10), passwordHash!);
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.Validation);
        result.Failure.Message.Should().Be("Uma senha deve ser informada");
    }
}