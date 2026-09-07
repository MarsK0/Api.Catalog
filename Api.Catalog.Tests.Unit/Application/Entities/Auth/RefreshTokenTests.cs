using Api.Catalog.Application;
using Api.Catalog.Application.Entities;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace Api.Catalog.Tests.Unit.Application.Entities.Auth;

public class RefreshTokenTests
{
    private static readonly Faker faker = new();
    public static RefreshToken CreateValid(TimeProvider? timeProvider = null, bool rememberMe = false, DateTimeOffset? expires = null)
    {
        var provider = timeProvider ?? new FakeTimeProvider(DateTimeOffset.UtcNow);
        return RefreshToken.Create(
            Guid.NewGuid(),
            faker.Random.String2(20),
            Guid.NewGuid(),
            expires ?? provider.GetUtcNow().AddMinutes(30),
            rememberMe,
            provider
        ).Value;
    }

    [Fact]
    public void Create_WithValidInputs_ShouldReturnRefreshTokenWithExpectedValues()
    {
        //Arrange
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        var expectedUserId = Guid.NewGuid();
        var expectedTokenHash = faker.Random.String2(20);
        var expectedFamilyId = Guid.NewGuid();
        var expectedExpires = timeProvider.GetUtcNow().AddMinutes(30);
        var expectedRememberMe = faker.Random.Bool();
        //Act
        var refreshToken = RefreshToken.Create(expectedUserId, expectedTokenHash, expectedFamilyId, expectedExpires, expectedRememberMe, timeProvider).Value;
        //Assert
        refreshToken.UserId.Should().Be(expectedUserId);
        refreshToken.TokenHash.Should().Be(expectedTokenHash);
        refreshToken.FamilyId.Should().Be(expectedFamilyId);
        refreshToken.Expires.Should().Be(expectedExpires);
        refreshToken.RememberMe.Should().Be(expectedRememberMe);
        refreshToken.IsUsed.Should().BeFalse();
        refreshToken.Revoked.Should().BeFalse();
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        //Act
        var result = RefreshToken.Create(Guid.Empty, faker.Random.String2(20), Guid.NewGuid(), timeProvider.GetUtcNow().AddMinutes(30), false, timeProvider);
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.Validation);
        result.Failure.Message.Should().Be("Deve ser informado um usuário para a criação do Refresh Token.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithEmptyTokenHash_ShouldReturnFailureWithExpectedCodeAndMessage(string? tokenHash)
    {
        //Arrange
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        //Act
        var result = RefreshToken.Create(Guid.NewGuid(), tokenHash!, Guid.NewGuid(), timeProvider.GetUtcNow().AddMinutes(30), false, timeProvider);
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.Validation);
        result.Failure.Message.Should().Be("Uma hash deve ser informada para a criação do Token.");
    }

    [Fact]
    public void Create_WithEmptyFamilyId_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        //Act
        var result = RefreshToken.Create(Guid.NewGuid(), faker.Random.String2(20), Guid.Empty, timeProvider.GetUtcNow().AddMinutes(30), false, timeProvider);
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.Validation);
        result.Failure.Message.Should().Be("Deve ser informado um id para a família do RefreshToken.");
    }

    [Fact]
    public void Create_WithExpiresEqualToNow_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        //Act
        var result = RefreshToken.Create(Guid.NewGuid(), faker.Random.String2(20), Guid.NewGuid(), timeProvider.GetUtcNow(), false, timeProvider);
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.Validation);
        result.Failure.Message.Should().Be("A data de expiração deve ser maior do que o momento atual.");
    }

    [Fact]
    public void Create_WithExpiresBeforeNow_ShouldReturnFailureWithExpectedCodeAndMessage()
    {
        //Arrange
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        //Act
        var result = RefreshToken.Create(Guid.NewGuid(), faker.Random.String2(20), Guid.NewGuid(), timeProvider.GetUtcNow().AddSeconds(-1), false, timeProvider);
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.Validation);
        result.Failure.Message.Should().Be("A data de expiração deve ser maior do que o momento atual.");
    }

    [Fact]
    public void MarkAsUsed_ShouldSetIsUsedToTrue()
    {
        //Arrange
        var refreshToken = CreateValid();
        //Act
        refreshToken.MarkAsUsed();
        //Assert
        refreshToken.IsUsed.Should().BeTrue();
    }

    [Fact]
    public void Revoke_ShouldSetRevokedToTrue()
    {
        //Arrange
        var refreshToken = CreateValid();
        //Act
        refreshToken.Revoke();
        //Assert
        refreshToken.Revoked.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithFreshToken_ShouldReturnTrue()
    {
        //Arrange
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        var refreshToken = CreateValid(timeProvider, expires: timeProvider.GetUtcNow().AddMinutes(5));
        //Act
        var result = refreshToken.IsValid(timeProvider);
        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithUsedToken_ShouldReturnFalse()
    {
        //Arrange
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        var refreshToken = CreateValid(timeProvider, expires: timeProvider.GetUtcNow().AddMinutes(5));
        refreshToken.MarkAsUsed();
        //Act
        var result = refreshToken.IsValid(timeProvider);
        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithRevokedToken_ShouldReturnFalse()
    {
        //Arrange
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        var refreshToken = CreateValid(timeProvider, expires: timeProvider.GetUtcNow().AddMinutes(5));
        refreshToken.Revoke();
        //Act
        var result = refreshToken.IsValid(timeProvider);
        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithExpiresExactlyAtNow_ShouldReturnFalse()
    {
        //Arrange
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        var expires = timeProvider.GetUtcNow().AddSeconds(10);
        var refreshToken = CreateValid(timeProvider, expires: expires);
        //Act
        timeProvider.SetUtcNow(expires);
        var result = refreshToken.IsValid(timeProvider);
        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithNowBeforeExpires_ShouldReturnTrue()
    {
        //Arrange
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        var expires = timeProvider.GetUtcNow().AddSeconds(10);
        var refreshToken = CreateValid(timeProvider, expires: expires);
        //Act
        timeProvider.SetUtcNow(expires.AddSeconds(-1));
        var result = refreshToken.IsValid(timeProvider);
        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithNowAfterExpires_ShouldReturnFalse()
    {
        //Arrange
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        var expires = timeProvider.GetUtcNow().AddSeconds(10);
        var refreshToken = CreateValid(timeProvider, expires: expires);
        //Act
        timeProvider.SetUtcNow(expires.AddSeconds(1));
        var result = refreshToken.IsValid(timeProvider);
        //Assert
        result.Should().BeFalse();
    }
}