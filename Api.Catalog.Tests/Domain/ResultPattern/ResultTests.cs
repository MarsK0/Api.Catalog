using Api.Catalog.Domain;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Api.Catalog.Tests.Unit;

public class ResultTests
{
    private static readonly Faker faker = new();
    public static Failure GetFailure() => new(faker.Lorem.Word(), faker.Lorem.Sentence(10));

    public class BaseResult
    {
        public class OnSuccess
        {
            [Fact]
            public void ExplicitOperatorSuccess_BaseInstance_ShouldBeSuccess()
            {
                //Arrange
                //Act
                var result = Result.Success;
                //Assert
                result.IsSuccess.Should().BeTrue();
            }
            [Fact]
            public void ExplicitOperatorSuccess_BaseInstance_ShouldThrowInvalidOperationExceptionWhenTryToAccessFailure()
            {
                //Arrange
                //Act
                var result = Result.Success;
                var action = () => result.Failure;
                //Assert
                action.Should().Throw<InvalidOperationException>().WithMessage("Não há falha em resultado de sucesso.");
            }
        }
        public class OnFailure
        {
            [Fact]
            public void ExplicitOperatorFailure_BaseInstance_ShouldNotBeSuccess()
            {
                //Arrange
                var code = faker.Lorem.Word();
                var message = faker.Lorem.Sentence(10);
                var failure = new Failure(code, message);
                //Act
                var result = Result.Fail(failure);
                //Assert
                result.IsSuccess.Should().BeFalse();
            }
            [Fact]
            public void ExplicitOperatorFailure_BaseInstance_ShouldHaveExpectedFailure()
            {
                //Arrange
                var expectedFailure = GetFailure();
                //Act
                var result = Result.Fail(expectedFailure);
                //Assert
                result.Failure.Should().Be(expectedFailure);
            }
            [Fact]
            public void ImplicitOperatorFailure_BaseInstance_ShouldNotBeSuccess()
            {
                //Arrange
                var failure = GetFailure();
                //Act
                Result result = failure;
                //Assert
                result.IsSuccess.Should().BeFalse();
            }
            [Fact]
            public void ImplicitOperatorFailure_BaseInstance_ShouldHaveExpcetedFailure()
            {
                //Arrange
                var expectedFailure = GetFailure();
                //Act
                Result result = expectedFailure;
                //Assert
                result.Failure.Should().Be(expectedFailure);
            }
        }
    }
    public class ValuedResult
    {
        public class OnSuccess
        {

            [Fact]
            public void ExplicitOperatorSuccess_ValueInstance_ShouldBeSuccess()
            {
                //Arrange
                var value = faker.Random.Int(min: 1);
                //Act
                var result = Result<int>.Success(value);
                //Assert
                result.IsSuccess.Should().BeTrue();
            }
            [Fact]
            public void ExplicitOperatorSuccess_ValueInstance_ShouldHaveExpectedValue()
            {
                //Arrange
                var expectedValue = faker.Random.Int(min: 1);
                //Act
                var result = Result<int>.Success(expectedValue);
                //Assert
                result.Value.Should().Be(expectedValue);
            }
            [Fact]
            public void ExplicitOperatorSuccess_ValueInstance_ShouldThrowInvalidOperationExceptionWhenTryToAccessFailure()
            {
                //Arrange
                var value = faker.Random.Int(min: 1);
                //Act
                var result = Result<int>.Success(value);
                var action = () => result.Failure;
                //Assert
                action.Should().Throw<InvalidOperationException>().WithMessage("Não há falha em resultado de sucesso.");
            }
            [Fact]
            public void ImplicitOperatorSuccess_ValueInstance_ShouldBeSucces()
            {
                //Arrange
                var value = faker.Random.Int(min: 1);
                //Act
                Result<int> result = value;
                //Assert
                result.IsSuccess.Should().BeTrue();
            }
            [Fact]
            public void ImplicitOperatorSuccess_ValueInstance_ShouldHaveExpectedValue()
            {
                //Arrange
                var expectedValue = faker.Random.Int(min: 1);
                //Act
                Result<int> result = expectedValue;
                //Assert
                result.Value.Should().Be(expectedValue);
            }
            [Fact]
            public void ImplicitOperatorSuccess_ValueInstance_ShouldThrowInvalidOperationExceptionWhenTryToAccessFailure()
            {
                //Arrange
                var value = faker.Random.Int(min: 1);
                //Act
                Result<int> result = value;
                var action = () => result.Failure;
                //Assert
                action.Should().Throw<InvalidOperationException>().WithMessage("Não há falha em resultado de sucesso.");
            }
        }
        public class OnFailure
        {
            [Fact]
            public void ExplicitOperatorFailure_ValueInstance_ShouldNotBeSuccess()
            {
                //Arrange
                var failure = GetFailure();
                //Act
                var result = Result<int>.Fail(failure);
                //Assert
                result.IsSuccess.Should().BeFalse();
            }
            [Fact]
            public void ExplicitOperatorFailure_ValueInstance_ShouldHaveExpectedFailure()
            {
                //Arrange
                var expectedFailure = GetFailure();
                //Act
                var result = Result<int>.Fail(expectedFailure);
                //Assert
                result.Failure.Should().Be(expectedFailure);
            }
            [Fact]
            public void ExplicitOperatorFailure_ValueInstance_ShouldThrowInvalidOperationExceptionWhenTryToAccessValue()
            {
                //Arrange
                var failure = GetFailure();
                //Act
                var result = Result<int>.Fail(failure);
                var action = () => result.Value;
                //Assert
                action.Should().Throw<InvalidOperationException>().WithMessage("Não é possível acessar o valor de um resultado de falha.");
            }
            [Fact]
            public void ImplicitOperatorFailure_ValueInstance_ShouldNotBeSuccess()
            {
                //Arrange
                var failure = GetFailure();
                //Act
                Result<int> result = failure;
                //Assert
                result.IsSuccess.Should().BeFalse();
            }
            [Fact]
            public void ImplicitOperatorFailure_ValueInstance_ShouldHaveExpectedFailure()
            {
                //Arrange
                var expectedFailure = GetFailure();
                //Act
                Result<int> result = expectedFailure;
                //Assert
                result.Failure.Should().Be(expectedFailure);
            }
            [Fact]
            public void ImplicitOperatorFailure_ValueInstance_ShouldThrowInvalidOperationExceptionWhenTryToAccessValue()
            {
                var failure = GetFailure();
                //Act
                Result<int> result = failure;
                var action = () => result.Value;
                //Assert
                action.Should().Throw<InvalidOperationException>().WithMessage("Não é possível acessar o valor de um resultado de falha.");
            }
        }
    }
}