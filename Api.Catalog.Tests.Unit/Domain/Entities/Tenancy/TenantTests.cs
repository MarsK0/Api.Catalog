using Api.Catalog.Domain.Entities;
using Api.Catalog.Domain.Models;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Entities.Tenancy;

public class TenantTests
{
    private static readonly Faker faker = new();
    public static Tenant CreateValid(IReadOnlyList<string>? modules = null)
        => Tenant.Create(faker.Random.String2(10), faker.Random.String2(10), modules ?? []).Value;

    [Fact]
    public void Create_WithValidInputs_ShouldReturnTenantWithExpectedValues()
    {
        //Arrange
        var expectedName = faker.Random.String2(10);
        var expectedSlug = faker.Random.String2(10);
        IReadOnlyList<string> expectedModules = [Modules.Tables];
        //Act
        var tenant = Tenant.Create(expectedName, expectedSlug, expectedModules).Value;
        //Assert
        tenant.Name.Should().Be(expectedName);
        tenant.Slug.Should().Be(expectedSlug);
        tenant.Modules.Select(s => s.ModuleCode).Should().BeEquivalentTo(expectedModules);
    }

    [Fact]
    public void Create_WithEmptyModulesList_ShouldReturnTenantWithNoModules()
    {
        //Arrange
        //Act
        var tenant = Tenant.Create(faker.Random.String2(10), faker.Random.String2(10), []).Value;
        //Assert
        tenant.Modules.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ShouldReturnFailureWithExpectedCodeAndMessage(string? name)
    {
        //Arrange
        //Act
        var result = Tenant.Create(name!, faker.Random.String2(10), []);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("O nome deve conter ao menos 3 caracteres.");
    }

    [Theory]
    [InlineData("a")]
    [InlineData("ab")]
    public void Create_WithNameBelowMinLength_ShouldReturnFailureWithExpectedCodeAndMessage(string name)
    {
        //Arrange
        //Act
        var result = Tenant.Create(name, faker.Random.String2(10), []);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("O nome deve conter ao menos 3 caracteres.");
    }

    [Fact]
    public void Create_WithNameAtMinLength_ShouldReturnTenantSuccessfully()
    {
        //Arrange
        //Act
        var result = Tenant.Create("abc", faker.Random.String2(10), []);
        //Assert
        result.Value.Name.Should().Be("abc");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithEmptySlug_ShouldReturnFailureWithExpectedCodeAndMessage(string? slug)
    {
        //Arrange
        //Act
        var result = Tenant.Create(faker.Random.String2(10), slug!, []);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("O slug deve conter ao menos 3 caracteres.");
    }

    [Theory]
    [InlineData("a")]
    [InlineData("ab")]
    public void Create_WithSlugBelowMinLength_ShouldReturnFailureWithExpectedCodeAndMessage(string slug)
    {
        //Arrange
        //Act
        var result = Tenant.Create(faker.Random.String2(10), slug, []);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("O slug deve conter ao menos 3 caracteres.");
    }

    [Fact]
    public void Create_WithSlugAtMinLength_ShouldReturnTenantSuccessfully()
    {
        //Arrange
        //Act
        var result = Tenant.Create(faker.Random.String2(10), "abc", []);
        //Assert
        result.Value.Slug.Should().Be("abc");
    }

    [Fact]
    public void Create_WithInvalidModule_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var result = Tenant.Create(faker.Random.String2(10), faker.Random.String2(10), ["INVALID"]);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Módulo de código 'INVALID' inexistente.");
    }

    [Fact]
    public void Create_WithMultipleInvalidModules_ShouldReturnFailureWithJoinedMessages()
    {
        //Arrange
        //Act
        var result = Tenant.Create(faker.Random.String2(10), faker.Random.String2(10), ["INVALID_1", "INVALID_2"]);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be(
            "Módulo de código 'INVALID_1' inexistente. | " +
            "Módulo de código 'INVALID_2' inexistente.");
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldUpdateName()
    {
        //Arrange
        var tenant = CreateValid();
        var expectedName = faker.Random.String2(10);
        //Act
        var result = tenant.UpdateName(expectedName);
        //Assert
        result.IsSuccess.Should().BeTrue();
        tenant.Name.Should().Be(expectedName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void UpdateName_WithEmptyName_ShouldReturnFailureWithExpectedCodeAndMessage(string? name)
    {
        //Arrange
        var tenant = CreateValid();
        //Act
        var result = tenant.UpdateName(name!);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Um nome deve ser informado para o tenant.");
    }

    [Fact]
    public void UnlockModules_WithValidModule_ShouldAddModule()
    {
        //Arrange
        var tenant = CreateValid();
        IReadOnlyList<string> expectedModules = [Modules.Tables];
        //Act
        var result = tenant.UnlockModules(expectedModules);
        //Assert
        result.IsSuccess.Should().BeTrue();
        tenant.Modules.Select(s => s.ModuleCode).Should().BeEquivalentTo(expectedModules);
    }

    [Fact]
    public void UnlockModules_WithEmptyList_ShouldReturnSuccessAndNotAddModules()
    {
        //Arrange
        var tenant = CreateValid();
        //Act
        var result = tenant.UnlockModules([]);
        //Assert
        result.IsSuccess.Should().BeTrue();
        tenant.Modules.Should().BeEmpty();
    }

    [Fact]
    public void UnlockModules_WithDuplicateModuleDifferentCase_ShouldAddOnlyOnce()
    {
        //Arrange
        var tenant = CreateValid();
        //Act
        var result = tenant.UnlockModules([Modules.Tables, Modules.Tables.ToLowerInvariant()]);
        //Assert
        result.IsSuccess.Should().BeTrue();
        tenant.Modules.Should().ContainSingle();
    }

    [Fact]
    public void UnlockModules_WithAlreadyUnlockedModule_ShouldNotAddDuplicate()
    {
        //Arrange
        var tenant = CreateValid();
        //Act
        tenant.UnlockModules([Modules.Tables]);
        var result = tenant.UnlockModules([Modules.Tables]);
        //Assert
        result.IsSuccess.Should().BeTrue();
        tenant.Modules.Where(w => w.ModuleCode == Modules.Tables).Should().HaveCount(1);
    }

    [Fact]
    public void UnlockModules_WithInvalidModule_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        var tenant = CreateValid();
        //Act
        var result = tenant.UnlockModules(["INVALID"]);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Módulo de código 'INVALID' inexistente.");
    }

    [Fact]
    public void UnlockModules_WithMultipleInvalidModules_ShouldReturnFailureWithJoinedMessages()
    {
        //Arrange
        var tenant = CreateValid();
        //Act
        var result = tenant.UnlockModules(["INVALID_1", "INVALID_2"]);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be(
            "Módulo de código 'INVALID_1' inexistente. | " +
            "Módulo de código 'INVALID_2' inexistente.");
    }

    [Fact]
    public void UnlockModules_WithMixOfValidAndInvalidModules_ShouldReturnFailureAndNotAddAnyModules()
    {
        //Arrange
        var tenant = CreateValid();
        //Act
        var result = tenant.UnlockModules([Modules.Tables, "INVALID"]);
        //Assert
        result.IsSuccess.Should().BeFalse();
        tenant.Modules.Should().BeEmpty();
    }
}