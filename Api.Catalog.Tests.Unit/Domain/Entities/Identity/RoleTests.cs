using Api.Catalog.Domain.Entities;
using Api.Catalog.Domain.Models;
using Api.Catalog.Domain.ValueObjects;
using Api.Catalog.Tests.Unit.Domain.ValueObjects;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Entities.Identity;

public class RoleTests
{
    private static readonly Faker faker = new();
    public static Role CreateValid() => Role.Create(RoleInfoTests.CreateValid()).Value;

    [Fact]
    public void Create_WithValidInputs_ShouldReturnRoleWithExpectedRoleInfo()
    {
        //Arrange
        var expectedRoleInfo = RoleInfoTests.CreateValid();
        //Act
        var role = Role.Create(expectedRoleInfo).Value;
        //Assert
        role.RoleInfo.Should().Be(expectedRoleInfo);
    }

    [Fact]
    public void AssignPermissions_WithEmptyList_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        var role = CreateValid();
        //Act
        var result = role.AssignPermissions([]);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Ao menos uma permissão deve ser informada");
    }

    [Fact]
    public void AssignPermissions_WithPermissions_ShouldAddPermissionsToRoleInfo()
    {
        //Arrange
        var role = CreateValid();
        var permission = PermissionInfoTests.CreateValid();
        //Act
        var result = role.AssignPermissions([permission]);
        //Assert
        result.IsSuccess.Should().BeTrue();
        role.RoleInfo.Permissions.Should().Contain(permission);
    }

    [Fact]
    public void AssignPermissions_WithAlreadyAssignedPermission_ShouldNotAddDuplicate()
    {
        //Arrange
        var role = CreateValid();
        var permission = PermissionInfoTests.CreateValid();
        //Act
        role.AssignPermissions([permission]);
        role.AssignPermissions([permission]);
        //Assert
        role.RoleInfo.Permissions.Where(w => w == permission).Should().HaveCount(1);
    }

    [Fact]
    public void AssignPermissions_WithDuplicatePermissionsInSameCall_ShouldNotAddDuplicate()
    {
        //Arrange
        var role = CreateValid();
        var permission = PermissionInfoTests.CreateValid();
        //Act
        role.AssignPermissions([permission, permission]);
        //Assert
        role.RoleInfo.Permissions.Where(w => w == permission).Should().HaveCount(1);
    }

    [Fact]
    public void AssignPermissions_WithMultipleDistinctPermissions_ShouldAddAll()
    {
        //Arrange
        var role = CreateValid();
        var firstPermission = PermissionInfoTests.CreateValid();
        var secondPermission = PermissionInfoTests.CreateValid();
        //Act
        role.AssignPermissions([firstPermission, secondPermission]);
        //Assert
        role.RoleInfo.Permissions.Should().Contain([firstPermission, secondPermission]);
    }
}