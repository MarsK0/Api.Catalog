using Api.Catalog.Domain.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Models;

public class RootRolesTests
{
    [Theory]
    [InlineData("PlatformOwner", "PLATFORM_OWNER")]
    [InlineData("PlatformAdmin", "PLATFORM_ADMIN")]
    [InlineData("TenantOwner", "TENANT_OWNER")]
    [InlineData("TenantAdmin", "TENANT_ADMIN")]
    public void Consts_Values_ShouldMaintainExactContractValues(string fieldName, string expectedValue)
    {
        //Arrange
        var field = typeof(RootRoles).GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
        //Act
        var actual = field?.GetValue(null);
        //Assert
        field.Should().NotBeNull($"O campo {fieldName} não deve ser removido das roles raíz");
        actual.Should().Be(expectedValue);
    }
    [Fact]
    public void Consts_Values_ShouldBeInNamesFrozenSet()
    {
        //Arrange
        var values = typeof(RootRoles)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(w => w.IsLiteral && !w.IsInitOnly && w.FieldType == typeof(string))
            .Select(s => (string)s.GetValue(null)!)
            .ToList();
        //Act
        //Assert
        RootRoles.Names.Should().HaveCount(values.Count);
        RootRoles.Names.Should().BeEquivalentTo(values);
    }
}
