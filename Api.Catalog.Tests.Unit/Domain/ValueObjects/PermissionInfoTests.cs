using Api.Catalog.Domain.ValueObjects;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.ValueObjects;

public class PermissionInfoTests
{
    [Theory]
    [InlineData("tenant", "person", "read", "TENANT:PERSON:READ")]
    [InlineData("SYSTEM", "ROLES", "MANAGE", "SYSTEM:ROLES:MANAGE")]
    [InlineData("Tenant", "Person", "Create", "TENANT:PERSON:CREATE")]
    public void Value_DeveRetornarFormatoFormatadoEMaiusculo(
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
