using Api.Catalog.Domain.ValueObjects;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.ValueObjects;

public class PermissionInfoTests
{
    private readonly Faker faker = new();
    [Fact]
    public void PermissionInfo_Instance_ShouldHaveExpectedValue()
    {
        //Arrange
        var scope = faker.Lorem.Word();
        var resource = faker.Lorem.Word();
        var action = faker.Lorem.Word();
        //Act
        var permission = new PermissionInfo(scope, resource, action);
        //Assert
        permission.Value.Should().Be($"{scope}:{resource}:{action}");
    }
}
