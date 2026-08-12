using Api.Catalog.Domain.Entities;
using Api.Catalog.Domain.Models;
using Bogus;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Entities.Catalog;

public class CategoryTests
{
    private static readonly Faker faker = new();
    public static Category CreateValid() => Category.Create(faker.Random.String2(10)).Value;
    [Fact]
    public void Create_WithValidInputs_ShouldReturnCategoryWithExpectedValues()
    {
        //Arrange
        var expectedDescription = faker.Random.String2(10);
        //Act
        var category = Category.Create(expectedDescription).Value;
        //Assert
        category.Description.Should().Be(expectedDescription);
    }
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithEmptyInput_ShouldReturnFailureWithExpectedCodeAndMessage(string? description)
    {
        //Arrange
        //Act
        var result = Category.Create(description!);
        //Assert
        result.Failure.Code.Should().Be(DomainFailureCodes.Validation);
        result.Failure.Message.Should().Be("Uma descrição deve ser fornecida para a categoria.");
    }
    [Fact]
    public void CreateSubCategory_WithValidInputs_ShouldInsertAndReturnItWithExpectedValues()
    {
        //Arrange
        var category = CreateValid();
        var expectedDescription = faker.Random.String2(10);
        //Act
        var sub = category.CreateSubCategory(expectedDescription).Value;
        //Assert
        category.SubCategories.Should().Contain(sub);
        sub.Description.Should().Be(expectedDescription);
    }
    [Fact]
    public void CreateSubCategory_MoreThanOnce_ShouldNotDuplicateAndReturnIt()
    {
        //Arrange
        var category = CreateValid();
        var expectedDescription = faker.Random.String2(10);
        //Act
        var sub1 = category.CreateSubCategory(expectedDescription).Value;
        var sub2 = category.CreateSubCategory(expectedDescription).Value;
        //Assert
        category.SubCategories.Should().HaveCount(1);
        sub1.Should().BeSameAs(sub2);
    }
}
