using Api.Catalog.Domain.Models;
using FluentAssertions;
using System.Reflection;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Models;

public class ModulesTests
{
    public static TheoryData<string> AllModules => [.. Modules.All];
    [Fact]
    public void All_Call_ShouldReturnAllModulesAndOnlyDeclaredOnes()
    {
        //Arrange
        var all = typeof(Modules)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(w => w.FieldType == typeof(string) && w.IsPublic && w.IsLiteral && !w.IsInitOnly)
            .Select(s => (string)s.GetValue(null)!);
        //Act
        //Assert
        Modules.All.Should().BeEquivalentTo(all);
    }
    [Theory]
    [MemberData(nameof(AllModules))]
    public void Exists_ForModules_ShouldReturnTrue(string module)
    {
        //Arrange
        //Act
        var actual = Modules.Exists(module);
        //Assert
        actual.Should().BeTrue();
    }
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("ABC")]
    public void Existis_ForOtherStrings_ShouldReturnFalse(string value)
    {
        //Arrange
        //Act
        var actual = Modules.Exists(value);
        //Assert
        actual.Should().BeFalse();
    }
}
