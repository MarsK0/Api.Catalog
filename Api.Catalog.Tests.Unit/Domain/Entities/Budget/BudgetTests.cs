using Bogus;
using Xunit;
using Microsoft.Extensions.Time.Testing;
using FluentAssertions;
using Api.Catalog.Domain.Models;
using Api.Catalog.Domain.Entities;

namespace Api.Catalog.Tests.Unit.Domain.Entities.Budget;

public class BudgetTests
{
    private static readonly Faker faker = new();
    private static readonly DateTimeOffset fakeNow = new(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly FakeTimeProvider fakeTimeProvider = new(fakeNow);
    private static Api.Catalog.Domain.Entities.Budget CreateValid()
    {
        return Api.Catalog.Domain.Entities.Budget
            .Create(
                DateTimeOffset.UtcNow.AddDays(1),
                faker.Internet.Email(),
                Guid.NewGuid()
            ).Value;
    }
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Create_WithValidInput_ShouldReturnBudgetWithExpectedValues(int daysOffset)
    {
        //Arrange
        var expectedValidUntil = fakeNow.AddDays(daysOffset);
        var expectedEmail = faker.Internet.Email();
        var expectedUserId = Guid.NewGuid();
        //Act
        var budget = Api.Catalog.Domain.Entities.Budget
            .Create(
                expectedValidUntil,
                expectedEmail,
                expectedUserId,
                fakeTimeProvider
            ).Value;
        //Assert
        budget.ValidUntil.Should().Be(expectedValidUntil);
        budget.UserEmail.Should().Be(expectedEmail);
        budget.PersonId.Should().Be(expectedUserId);
    }
    [Fact]
    public void Create_WithValidInputAndNullableNulls_ShouldReturnBudgetWithExpectedValues()
    {
        //Arrange
        var expectedValidUntil = fakeNow;
        //Act
        var budget = Api.Catalog.Domain.Entities.Budget
            .Create(
                expectedValidUntil,
                null,
                null,
                fakeTimeProvider
            ).Value;
        //Assert
        budget.ValidUntil.Should().Be(expectedValidUntil);
        budget.UserEmail.Should().BeNull();
        budget.PersonId.Should().BeNull();
    }
    [Fact]
    public void Create_WithValidUntilInferiorNow_ReturnsFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        //Act
        var result = Api.Catalog.Domain.Entities.Budget.Create(
            fakeNow.AddDays(-1),
            null,
            null,
            fakeTimeProvider
        );
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("A data de validade do orçamento deve ser superior a agora.");
    }
    [Fact]
    public void AddItem_WithValidItems_ShouldInsertInItems()
    {
        //Arrange
        var budget = CreateValid();
        IEnumerable<BudgetItem> items = [
            BudgetItemTests.CreateValid(),
            BudgetItemTests.CreateValid(),
            BudgetItemTests.CreateValid(),
            BudgetItemTests.CreateValid(),
            BudgetItemTests.CreateValid(),
        ];
        //Act
        foreach(var item in items)
            budget.AddItem(item);
        //Assert
        budget.Items.Should().BeEquivalentTo(items);
    }
}
