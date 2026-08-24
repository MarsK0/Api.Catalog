using Api.Catalog.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.ValueObjects;

public class PermissionInfoTests
{
    private static readonly Bogus.Faker faker = new();
    public static PermissionInfo CreateValid() => new(faker.Random.String2(8), faker.Random.String2(8), faker.Random.String2(8));
    [Theory]
    [InlineData("tenant", "person", "read", "TENANT:PERSON:READ")]
    [InlineData("SYSTEM", "ROLES", "MANAGE", "SYSTEM:ROLES:MANAGE")]
    [InlineData("Tenant", "Person", "Create", "TENANT:PERSON:CREATE")]
    public void Value_ShouldReturnFormattedAndUppercase(
            string scope,
            string resource,
            string action,
            string expectedValue)
    {
        //Arrange
        var permissionInfo = new PermissionInfo(scope, resource, action);
        //Act
        //Assert
        permissionInfo.Value.Should().Be(expectedValue);
    }
}
