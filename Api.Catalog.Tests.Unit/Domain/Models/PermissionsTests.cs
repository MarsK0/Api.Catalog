using Api.Catalog.Domain.Models;
using Api.Catalog.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Models;

public class PermissionsTests
{
    [Theory]
    [InlineData(Modules.Tables)]
    public void GetAllForModules_WithValidModule_ShouldContainPermissions(string module)
    {
        //Arrange
        //Act
        var permissions = Permissions.GetAllForModules([module]);
        //Assert
        permissions.Should().NotBeEmpty();
        permissions.Should().AllBeAssignableTo(typeof(PermissionInfo));
    }
    [Theory]
    [InlineData("INVALID_MODULE")]
    [InlineData(" ")]
    [InlineData("")]
    public void GetAllForModules_WithInvalidModule_ShouldReturnEmptyList(string invalidModule)
    {
        //Arrange
        //Act
        var permissions = Permissions.GetAllForModules([invalidModule]);
        //Assert
        permissions.Should().BeEmpty();
    }
}
