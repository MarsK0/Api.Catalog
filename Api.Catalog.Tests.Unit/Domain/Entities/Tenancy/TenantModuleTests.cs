using Api.Catalog.Domain.Entities;
using Api.Catalog.Domain.Models;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Entities.Tenancy;

public class TenantModuleTests
{
    private static readonly Faker faker = new();

    [Fact]
    public void Create_WithExistingModuleCode_ShouldReturnTenantModuleWithExpectedValues()
    {
        //Arrange
        var expectedModuleCode = Modules.Tables;
        //Act
        var tenantModule = TenantModule.Create(expectedModuleCode).Value;
        //Assert
        tenantModule.ModuleCode.Should().Be(expectedModuleCode);
    }

    [Theory]
    [InlineData("tables")]
    [InlineData("NONEXISTENT")]
    [InlineData("")]
    [InlineData(null)]
    public void Create_WithNonExistingModuleCode_ShouldReturnFailureWithExpectedCodeAndMessage(string? moduleCode)
    {
        //Arrange
        //Act
        var result = TenantModule.Create(moduleCode!);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be($"Módulo de código '{moduleCode}' inexistente.");
    }
}