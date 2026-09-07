using Api.Catalog.Application;
using Api.Catalog.Application.Entities;
using Api.Catalog.Application.Enums;
using Api.Catalog.Domain.ValueObjects;
using Api.Catalog.Tests.Unit.Domain.ValueObjects;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Application.Entities.Auth;

public class PlatformUserTests
{
    private static readonly Faker faker = new();
    public static PlatformUser CreateValid()
        => PlatformUser.Create(faker.Random.String2(10), faker.Random.String2(10), faker.Internet.Email(), faker.Random.String2(10)).Value;

    [Fact]
    public void Create_WithValidInputs_ShouldReturnPlatformUserWithExpectedValues()
    {
        //Arrange
        var expectedLogin = faker.Random.String2(10);
        var expectedName = faker.Random.String2(10);
        var expectedEmail = faker.Internet.Email();
        var expectedPasswordHash = faker.Random.String2(10);
        //Act
        var platformUser = PlatformUser.Create(expectedLogin, expectedName, expectedEmail, expectedPasswordHash).Value;
        //Assert
        platformUser.Login.Should().Be(expectedLogin);
        platformUser.Name.Should().Be(expectedName);
        platformUser.Email.Should().Be(expectedEmail);
        platformUser.PasswordHash.Should().Be(expectedPasswordHash);
        platformUser.Status.Should().Be(EPlatformUserStatus.Enabled);
        platformUser.Roles.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithEmptyLogin_ShouldReturnFailureWithExpectedCodeAndMessage(string? login)
    {
        //Arrange
        //Act
        var result = PlatformUser.Create(login!, faker.Random.String2(10), faker.Internet.Email(), faker.Random.String2(10));
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.Validation);
        result.Failure.Message.Should().Be("Um login deve ser informado para o usuário.");
    }

    [Theory]
    [InlineData("a")]
    [InlineData("ab")]
    public void Create_WithLoginBelowMinLength_ShouldReturnFailureWithExpectedCodeAndMessage(string login)
    {
        //Arrange
        //Act
        var result = PlatformUser.Create(login, faker.Random.String2(10), faker.Internet.Email(), faker.Random.String2(10));
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.Validation);
        result.Failure.Message.Should().Be("O login deve ter entre 3 e 30 caracteres.");
    }

    [Fact]
    public void Create_WithLoginAboveMaxLength_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        var login = faker.Random.String2(31);
        //Act
        var result = PlatformUser.Create(login, faker.Random.String2(10), faker.Internet.Email(), faker.Random.String2(10));
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.Validation);
        result.Failure.Message.Should().Be("O login deve ter entre 3 e 30 caracteres.");
    }

    [Theory]
    [InlineData(3)]
    [InlineData(30)]
    public void Create_WithLoginAtBounds_ShouldReturnPlatformUserSuccessfully(int loginLength)
    {
        //Arrange
        var login = faker.Random.String2(loginLength);
        //Act
        var result = PlatformUser.Create(login, faker.Random.String2(10), faker.Internet.Email(), faker.Random.String2(10));
        //Assert
        result.Value.Login.Should().Be(login);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ShouldReturnFailureWithExpectedCodeAndMessage(string? name)
    {
        //Arrange
        //Act
        var result = PlatformUser.Create(faker.Random.String2(10), name!, faker.Internet.Email(), faker.Random.String2(10));
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.Validation);
        result.Failure.Message.Should().Be("Um nome deve ser informado para o usuário.");
    }

    [Fact]
    public void Create_WithNameBelowMinLength_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var result = PlatformUser.Create(faker.Random.String2(10), "a", faker.Internet.Email(), faker.Random.String2(10));
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.Validation);
        result.Failure.Message.Should().Be("O nome deve ter entre 2 e 60 caracteres");
    }

    [Fact]
    public void Create_WithNameAboveMaxLength_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        var name = faker.Random.String2(61);
        //Act
        var result = PlatformUser.Create(faker.Random.String2(10), name, faker.Internet.Email(), faker.Random.String2(10));
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.Validation);
        result.Failure.Message.Should().Be("O nome deve ter entre 2 e 60 caracteres");
    }

    [Theory]
    [InlineData(2)]
    [InlineData(60)]
    public void Create_WithNameAtBounds_ShouldReturnPlatformUserSuccessfully(int nameLength)
    {
        //Arrange
        var name = faker.Random.String2(nameLength);
        //Act
        var result = PlatformUser.Create(faker.Random.String2(10), name, faker.Internet.Email(), faker.Random.String2(10));
        //Assert
        result.Value.Name.Should().Be(name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithEmptyPasswordHash_ShouldReturnFailureWithExpectedCodeAndMessage(string? passwordHash)
    {
        //Arrange
        //Act
        var result = PlatformUser.Create(faker.Random.String2(10), faker.Random.String2(10), faker.Internet.Email(), passwordHash!);
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.Validation);
        result.Failure.Message.Should().Be("Uma senha deve ser informada para o usuário");
    }

    [Fact]
    public void AssignRole_WithNewRole_ShouldAddRoleToRoles()
    {
        //Arrange
        var platformUser = CreateValid();
        var role = PlatformRole.Create(RoleInfoTests.CreateValid()).Value;
        //Act
        platformUser.AssignRole(role);
        //Assert
        platformUser.Roles.Should().Contain(role);
    }

    [Fact]
    public void AssignRole_WithSameRoleInstanceTwice_ShouldNotAddDuplicate()
    {
        //Arrange
        var platformUser = CreateValid();
        var role = PlatformRole.Create(RoleInfoTests.CreateValid()).Value;
        platformUser.AssignRole(role);
        //Act
        platformUser.AssignRole(role);
        //Assert
        platformUser.Roles.Where(w => w == role).Should().HaveCount(1);
    }

    [Fact]
    public void AssignRole_WithDifferentRoleInstanceSameName_ShouldNotAddDuplicate()
    {
        //Arrange
        var platformUser = CreateValid();
        var roleName = faker.Random.String2(10);
        var firstRole = PlatformRole.Create(RoleInfo.Create(roleName, faker.Random.String2(10)).Value).Value;
        var secondRole = PlatformRole.Create(RoleInfo.Create(roleName, faker.Random.String2(10)).Value).Value;
        //Act
        platformUser.AssignRole(firstRole);
        platformUser.AssignRole(secondRole);
        //Assert
        platformUser.Roles.Should().ContainSingle();
        platformUser.Roles.Should().Contain(firstRole);
        platformUser.Roles.Should().NotContain(secondRole);
    }

    [Fact]
    public void AssignRole_WithMultipleDistinctRoles_ShouldAddAll()
    {
        //Arrange
        var platformUser = CreateValid();
        var firstRole = PlatformRole.Create(RoleInfoTests.CreateValid()).Value;
        var secondRole = PlatformRole.Create(RoleInfoTests.CreateValid()).Value;
        //Act
        platformUser.AssignRole(firstRole);
        platformUser.AssignRole(secondRole);
        //Assert
        platformUser.Roles.Should().Contain([firstRole, secondRole]);
    }
}