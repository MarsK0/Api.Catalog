using Api.Catalog.Domain.Models;
using Api.Catalog.Domain.ValueObjects;
using Bogus;
using Bogus.DataSets;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.ValueObjects;

public class RoleInfoTests
{
    private readonly Faker faker = new();
    private RoleInfo CreateValidRole() => RoleInfo.Create(faker.Random.String2(10), faker.Random.String2(10)).Value;
    private PermissionInfo CreateValidPermission() => new(faker.Random.String2(10), faker.Random.String2(10), faker.Random.String2(10));
    [Fact]
    public void Create_ValidRole_ShouldReturnSuccessResultWithValidRole()
    {
        //Arrange
        var expectedName = faker.Random.String2(10);
        var expectedDescription = faker.Random.String2(10);
        //Act
        var result = RoleInfo.Create(expectedName, expectedDescription);
        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(expectedName);
        result.Value.Description.Should().Be(expectedDescription);
    }
    [Theory]
    [InlineData("ABC")]
    [InlineData("H4iZaPd9bcod1XZc0iziRFeN75K4Jz")]
    public void Create_WithNameAtBounds_ShouldReturnSuccessResult(string name)
    {
        //Arrange
        var description = faker.Random.String2(10);
        //Act
        var result = RoleInfo.Create(name, description);
        //Assert
        result.IsSuccess.Should().BeTrue();
    }
    [Theory]
    [InlineData("ABC")]
    [InlineData("r8cmkLdnkwTVFvPFTJRiciciTTTAUzXr8cmkLdnkwTVFvPFTJRiciciTTTAU")]
    public void Create_WithDescriptionAtBounds_ShouldReturnSuccessResult(string description)
    {
        //Arrange
        var name = faker.Random.String2(10);
        //Act
        var result = RoleInfo.Create(name, description);
        //Assert
        result.IsSuccess.Should().BeTrue();
    }
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithEmptyName_ShouldReturnExpectedFailure(string? name)
    {
        //Arrange
        var description = faker.Random.String2(10);
        //Act
        var result = RoleInfo.Create(name!, description);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Informe um nome para o papel.");
    }
    [Theory]
    [InlineData("AB")]
    [InlineData("H4iZaPd9bcod1XZc0iziRFeN75K4Jzc")]//31 caracteres
    public void Create_WithOutOfBoundsName_ShouldReturnExpectedFailure(string name)
    {
        //Arrange
        var description = faker.Random.String2(10);
        //Act
        var result = RoleInfo.Create(name, description);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("O nome do papel deve ter entre 3 e 30 caracteres.");
    }
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithEmptyDescription_ShouldReturnExpectedFailure(string? description)
    {
        //Arrange
        var name = faker.Random.String2(10);
        //Act
        var result = RoleInfo.Create(name, description!);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Forneça uma descrição para o papel.");
    }
    [Theory]
    [InlineData("AB")]
    [InlineData("r8cmkLdnkwTVFvPFTJRiciciTTTAUzXr8cmkLdnkwTVFvPFTJRiciciTTTAUz")]//61 caracteres
    public void Create_WithOutOfBoundsDescription_ShouldReturnExpectedFailure(string description)
    {
        //Arrange
        var name = faker.Random.String2(10);
        //Act
        var result = RoleInfo.Create(name, description);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("A descrição do papel deve ter entre 3 e 60 caracteres.");
    }
    [Fact]
    public void AssignPermission_ValidPermission_ShouldAddPermission()
    {
        //Arrange
        var role = CreateValidRole();
        var permission = CreateValidPermission();
        //Act
        role.AssignPermission(permission);
        //Assert
        role.Permissions.Should().HaveCount(1);
        role.Permissions.Should().Contain(permission);
    }
    [Fact]
    public void AssignPermission_MultiplePermissions_ShouldAccumulateInPermissions()
    {
        //Arrange
        var role = CreateValidRole();
        var permission1 = CreateValidPermission();
        var permission2 = CreateValidPermission();
        //Act
        role.AssignPermission(permission1);
        role.AssignPermission(permission2);
        //Assert
        role.Permissions.Should().HaveCount(2);
        role.Permissions.Should().ContainInOrder(permission1, permission2);
    }
}

