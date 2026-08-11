using Api.Catalog.Domain;
using Bogus;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Api.Catalog.Tests.Unit.Domain.ResultPattern;

public class ResultExtensionsTests
{
    private static Faker faker = new();
    [Fact]
    public async Task FoldAsync_ParameterizedOnSuccess_WhenSuccessShouldCallOnSuccessAndReturnItsResult()
    {
        //Arrange
        var value = faker.Random.Int(min: 1);
        var expected = faker.Random.String2(10);
        var onSuccess = Substitute.For<Func<int, string>>();
        var onFailure = Substitute.For<Func<Failure, string>>();
        onSuccess.Invoke(value).Returns(expected);
        var resultTask = ValueTask.FromResult(Result<int>.Success(value));
        //Act
        var actual = await resultTask.FoldAsync(onSuccess, onFailure);
        //Assert
        actual.Should().Be(expected);
        onSuccess.Received(1).Invoke(value);
        onFailure.DidNotReceive().Invoke(Arg.Any<Failure>());
    }
    [Fact]
    public async Task FoldAsync_ParameterizedOnSuccess_WhenFailureShouldCallOnFailureAndReturnItsResult()
    {
        //Arrange
        var value = faker.Random.Int(min: 1);
        var expected = faker.Random.String2(10);
        var expectedFailure = ResultTests.GetFailure();
        var onSuccess = Substitute.For<Func<int, string>>();
        var onFailure = Substitute.For<Func<Failure, string>>();
        onFailure.Invoke(expectedFailure).Returns(expected);
        var resultTask = ValueTask.FromResult(Result<int>.Fail(expectedFailure));
        //Act
        var actual = await resultTask.FoldAsync(onSuccess, onFailure);
        //Assert
        actual.Should().Be(expected);
        onFailure.Received(1).Invoke(expectedFailure);
        onSuccess.DidNotReceive().Invoke(value);
    }
    [Fact]
    public async Task FoldAsync_ParameterizedOnSuccess_WhenSuccessShouldExecuteOnSuccessTask()
    {
        //Arrange
        var value = faker.Random.Int(min: 1);
        var onSuccess = Substitute.For<Func<int, Task>>();
        var onFailure = Substitute.For<Func<Failure, Task>>();
        onSuccess.Invoke(value).Returns(Task.CompletedTask);
        var resultTask = Task.FromResult(Result<int>.Success(value));
        //Act
        await resultTask.FoldAsync(onSuccess, onFailure);
        //Assert
        await onSuccess.Received(1).Invoke(value);
        await onFailure.DidNotReceive().Invoke(Arg.Any<Failure>());
    }
    [Fact]
    public async Task FoldAsync_ParameterizedOnSuccess_WhenFailureShouldExecuteOnFailureTask()
    {
        //Arrange
        var value = faker.Random.Int(min: 1);
        var expectedFailure = ResultTests.GetFailure();
        var onSuccess = Substitute.For<Func<int, Task>>();
        var onFailure = Substitute.For<Func<Failure, Task>>();
        onFailure.Invoke(expectedFailure).Returns(Task.CompletedTask);
        var resultTask = Task.FromResult(Result<int>.Fail(expectedFailure));
        //Act
        await resultTask.FoldAsync(onSuccess, onFailure);
        //Assert
        await onSuccess.DidNotReceive().Invoke(value);
        await onFailure.Received(1).Invoke(expectedFailure);
    }
    [Fact]
    public async Task FoldAsync_ParameterlessOnSuccess_WhenSuccessShouldCallOnSuccessAndReturnItsResult()
    {
        //Arrange
        var value = faker.Random.Int(min: 1);
        var expected = faker.Random.String2(10);
        var onSuccess = Substitute.For<Func<string>>();
        var onFailure = Substitute.For<Func<Failure, string>>();
        onSuccess.Invoke().Returns(expected);
        var resultTask = ValueTask.FromResult(Result.Success);
        //Act
        var actual = await resultTask.FoldAsync(onSuccess, onFailure);
        //Assert
        actual.Should().Be(expected);
        onSuccess.Received(1).Invoke();
        onFailure.DidNotReceive().Invoke(Arg.Any<Failure>());
    }
    [Fact]
    public async Task FoldAsync_ParameterlessOnSuccess_WhenFailureShouldCallOnFailureAndReturnItsResult()
    {
        //Arrange
        var value = faker.Random.Int(min: 1);
        var expected = faker.Random.String2(10);
        var expectedFailure = ResultTests.GetFailure();
        var onSuccess = Substitute.For<Func<string>>();
        var onFailure = Substitute.For<Func<Failure, string>>();
        onFailure.Invoke(expectedFailure).Returns(expected);
        var resultTask = ValueTask.FromResult(Result.Fail(expectedFailure));
        //Act
        var actual = await resultTask.FoldAsync(onSuccess, onFailure);
        //Assert
        actual.Should().Be(expected);
        onSuccess.DidNotReceive().Invoke();
        onFailure.Received(1).Invoke(expectedFailure);
    }

}
