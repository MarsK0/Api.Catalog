using Api.Catalog.Domain.Models;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.Models;

public class DomainResultFailuresTests
{
    private static readonly Faker faker = new();
    [Fact]
    public void Validation_WithMessage_ShouldReturnExpectedFailureCodeAndMessage()
    {
        //Arrange
        var expectedCode = DomainFailureCodes.Validation;
        var expectedMessage = faker.Lorem.Sentence(10);
        //Act
        var failure = DomainResultFailures.Validation(expectedMessage);
        //Assert
        failure.Code.Should().Be(expectedCode);
        failure.Message.Should().Be(expectedMessage);
    }
    [Fact]
    public void EntityNotFound_WithMessage_ShouldReturnExpectedFailureCodeAndMessage()
    {
        //Arrange
        var expectedCode = DomainFailureCodes.EntityNotFound;
        var expectedMessage = faker.Lorem.Sentence(10);
        //Act
        var failure = DomainResultFailures.EntityNotFound(expectedMessage);
        //Assert
        failure.Code.Should().Be(expectedCode);
        failure.Message.Should().Be(expectedMessage);
    }
}
