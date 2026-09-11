using Api.Catalog.Application;
using Api.Catalog.Application.Contracts;
using Api.Catalog.Application.Contracts.Contexts;
using Api.Catalog.Application.Entities;
using Api.Catalog.Application.Handlers;
using Api.Catalog.Application.Models;
using Api.Catalog.Tests.Unit.Application.Entities.Auth;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Xunit;

namespace Api.Catalog.Tests.Unit.Application.Handlers;

public class LoginHandlerTests
{
    private static readonly Bogus.Faker faker = new();

    private readonly FakeTimeProvider timeProvider = new(DateTimeOffset.UtcNow);
    private readonly ITenantContext tenantContext = Substitute.For<ITenantContext>();
    private readonly IPasswordHashService passwordHashService = Substitute.For<IPasswordHashService>();
    private readonly ITokenService tokenService = Substitute.For<ITokenService>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IPlatformUserRepo platformUserRepo = Substitute.For<IPlatformUserRepo>();
    private readonly IAccountRepo accountRepo = Substitute.For<IAccountRepo>();
    private readonly IRefreshTokenRepo refreshTokenRepo = Substitute.For<IRefreshTokenRepo>();
    private readonly LoginHandler handler;

    public LoginHandlerTests()
    {
        handler = new LoginHandler(
            timeProvider,
            tenantContext,
            passwordHashService,
            tokenService,
            unitOfWork,
            platformUserRepo,
            accountRepo,
            refreshTokenRepo
        );
    }

    // --- Handle: busca do usuário --------------------------------------------

    [Fact]
    public async Task Handle_WhenPlatformContextAndUserNotFound_ShouldReturnInvalidRequestFailure()
    {
        //Arrange
        tenantContext.IsPlatformContext.Returns(true);
        platformUserRepo.FindByLoginAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((PlatformUser?)null);
        var command = new LoginCommand(faker.Random.String2(10), faker.Random.String2(10), false);
        //Act
        var result = await handler.Handle(command, CancellationToken.None);
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.InvalidRequest);
        result.Failure.Message.Should().Be("Credenciais inválidas.");
    }

    [Fact]
    public async Task Handle_WhenTenantContextAndAccountNotFound_ShouldReturnInvalidRequestFailure()
    {
        //Arrange
        tenantContext.IsPlatformContext.Returns(false);
        accountRepo.FindByLoginAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Account?)null);
        var command = new LoginCommand(faker.Random.String2(10), faker.Random.String2(10), false);
        //Act
        var result = await handler.Handle(command, CancellationToken.None);
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.InvalidRequest);
        result.Failure.Message.Should().Be("Credenciais inválidas.");
    }

    [Fact]
    public async Task Handle_WhenTenantContext_ShouldNotUsePlatformUserRepo()
    {
        //Arrange
        tenantContext.IsPlatformContext.Returns(false);
        accountRepo.FindByLoginAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Account?)null);
        var command = new LoginCommand(faker.Random.String2(10), faker.Random.String2(10), false);
        //Act
        await handler.Handle(command, CancellationToken.None);
        //Assert
        await platformUserRepo.DidNotReceive().FindByLoginAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenPlatformContext_ShouldNotUseAccountRepo()
    {
        //Arrange
        tenantContext.IsPlatformContext.Returns(true);
        platformUserRepo.FindByLoginAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((PlatformUser?)null);
        var command = new LoginCommand(faker.Random.String2(10), faker.Random.String2(10), false);
        //Act
        await handler.Handle(command, CancellationToken.None);
        //Assert
        await accountRepo.DidNotReceive().FindByLoginAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    // --- Handle: senha e status --------------------------------------------

    [Fact]
    public async Task Handle_WhenPasswordDoesNotMatch_ShouldReturnInvalidRequestFailure()
    {
        //Arrange
        var password = faker.Random.String2(10);
        var user = PlatformUserTests.CreateValid();
        tenantContext.IsPlatformContext.Returns(true);
        platformUserRepo.FindByLoginAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);
        passwordHashService.Matches(password, user.PasswordHash).Returns(false);
        var command = new LoginCommand(user.Login, password, false);
        //Act
        var result = await handler.Handle(command, CancellationToken.None);
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.InvalidRequest);
        result.Failure.Message.Should().Be("Credenciais iválidas.");
    }

    [Fact]
    public async Task Handle_WhenPlatformUserIsDisabled_ShouldReturnInvalidRequestFailure()
    {
        //Arrange
        var password = faker.Random.String2(10);
        var user = PlatformUserTests.CreateValid();
        user.Disable();
        tenantContext.IsPlatformContext.Returns(true);
        platformUserRepo.FindByLoginAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);
        passwordHashService.Matches(password, user.PasswordHash).Returns(true);
        var command = new LoginCommand(user.Login, password, false);
        //Act
        var result = await handler.Handle(command, CancellationToken.None);
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.InvalidRequest);
        result.Failure.Message.Should().Be("Credenciais iválidas.");
    }

    [Fact]
    public async Task Handle_WhenAccountIsDisabled_ShouldReturnInvalidRequestFailure()
    {
        //Arrange
        var password = faker.Random.String2(10);
        var account = AccountTests.CreateValidWithPerson();
        account.Disable();
        tenantContext.IsPlatformContext.Returns(false);
        accountRepo.FindByLoginAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(account);
        passwordHashService.Matches(password, account.PasswordHash).Returns(true);
        var command = new LoginCommand(account.Login, password, false);
        //Act
        var result = await handler.Handle(command, CancellationToken.None);
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.InvalidRequest);
        result.Failure.Message.Should().Be("Credenciais iválidas.");
    }

    [Fact]
    public async Task Handle_WhenPersonIsDisabled_ShouldReturnInvalidRequestFailure()
    {
        //Arrange
        var password = faker.Random.String2(10);
        var account = AccountTests.CreateValidWithPerson(personEnabled: false);
        tenantContext.IsPlatformContext.Returns(false);
        accountRepo.FindByLoginAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(account);
        passwordHashService.Matches(password, account.PasswordHash).Returns(true);
        var command = new LoginCommand(account.Login, password, false);
        //Act
        var result = await handler.Handle(command, CancellationToken.None);
        //Assert
        result.Failure.Code.Should().Be(AppFailureCodes.InvalidRequest);
        result.Failure.Message.Should().Be("Credenciais iválidas.");
    }

    // --- Handle: caminho de sucesso -----------------------------------------

    [Fact]
    public async Task Handle_WithValidPlatformCredentials_ShouldReturnSuccessfulLoginResponse()
    {
        //Arrange
        var password = faker.Random.String2(10);
        var user = PlatformUserTests.CreateValid();
        var token = faker.Random.String2(30);
        var tokenExpires = timeProvider.GetUtcNow().AddHours(1).UtcDateTime;
        var (rtValue, rtHash) = (faker.Random.String2(30), faker.Random.String2(30));

        tenantContext.IsPlatformContext.Returns(true);
        platformUserRepo.FindByLoginAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);
        passwordHashService.Matches(password, user.PasswordHash).Returns(true);
        tokenService.GenerateToken(Arg.Any<UserDto>()).Returns((token, tokenExpires));
        tokenService.GenerateRefreshToken().Returns((rtValue, rtHash));

        var command = new LoginCommand(user.Login, password, false);
        //Act
        var result = await handler.Handle(command, CancellationToken.None);
        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Result.Token.Should().Be(token);
        result.Value.Result.Expires.Should().Be(tokenExpires);
        result.Value.Result.PersonId.Should().Be(user.Id);
        result.Value.Result.Name.Should().Be(user.Name);
        result.Value.Result.Email.Should().Be(user.Email);
        result.Value.RefreshTokenValue.Should().Be(rtValue);
        result.Value.RememberMe.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithValidTenantCredentials_ShouldReturnSuccessfulLoginResponse()
    {
        //Arrange
        var password = faker.Random.String2(10);
        var account = AccountTests.CreateValidWithPerson();
        var token = faker.Random.String2(30);
        var tokenExpires = timeProvider.GetUtcNow().AddHours(1).UtcDateTime;
        var (rtValue, rtHash) = (faker.Random.String2(30), faker.Random.String2(30));

        tenantContext.IsPlatformContext.Returns(false);
        accountRepo.FindByLoginAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(account);
        passwordHashService.Matches(password, account.PasswordHash).Returns(true);
        tokenService.GenerateToken(Arg.Any<UserDto>()).Returns((token, tokenExpires));
        tokenService.GenerateRefreshToken().Returns((rtValue, rtHash));

        var command = new LoginCommand(account.Login, password, false);
        //Act
        var result = await handler.Handle(command, CancellationToken.None);
        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Result.PersonId.Should().Be(account.Person.Id);
        result.Value.Result.Name.Should().Be(account.Person.Name);
        result.Value.Result.Email.Should().Be(account.Person.Email);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldAddRefreshTokenAndSaveChanges()
    {
        //Arrange
        var password = faker.Random.String2(10);
        var user = PlatformUserTests.CreateValid();
        tenantContext.IsPlatformContext.Returns(true);
        platformUserRepo.FindByLoginAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);
        passwordHashService.Matches(password, user.PasswordHash).Returns(true);
        tokenService.GenerateToken(Arg.Any<UserDto>()).Returns((faker.Random.String2(30), timeProvider.GetUtcNow().AddHours(1).UtcDateTime));
        tokenService.GenerateRefreshToken().Returns((faker.Random.String2(30), faker.Random.String2(30)));

        var command = new LoginCommand(user.Login, password, false);
        //Act
        await handler.Handle(command, CancellationToken.None);
        //Assert
        refreshTokenRepo.Received(1).Add(Arg.Is<RefreshToken>(rt => rt.UserId == user.Id));
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    // --- Método estático Login: RememberMe e criação do RefreshToken --------

    private static UserDto CreateUserDto(Guid? userId = null)
        => new(
            userId ?? Guid.NewGuid(),
            faker.Random.String2(10),
            faker.Random.String2(10),
            faker.Internet.Email(),
            faker.Random.String2(20),
            true,
            []);

    [Fact]
    public async Task Login_WithRememberMeTrue_ShouldSetRefreshTokenExpiresInThirtyDays()
    {
        //Arrange
        var user = CreateUserDto();
        tokenService.GenerateToken(Arg.Any<UserDto>()).Returns((faker.Random.String2(30), timeProvider.GetUtcNow().AddHours(1).UtcDateTime));
        tokenService.GenerateRefreshToken().Returns((faker.Random.String2(30), faker.Random.String2(30)));
        var expectedExpires = timeProvider.GetUtcNow().AddDays(30);
        //Act
        var result = await LoginHandler.Login(timeProvider, tokenService, unitOfWork, refreshTokenRepo, user, rememberMe: true, CancellationToken.None);
        //Assert
        result.Value.RefreshTokenExpires.Should().Be(expectedExpires);
        result.Value.RememberMe.Should().BeTrue();
    }

    [Fact]
    public async Task Login_WithRememberMeFalse_ShouldSetRefreshTokenExpiresInEightHours()
    {
        //Arrange
        var user = CreateUserDto();
        tokenService.GenerateToken(Arg.Any<UserDto>()).Returns((faker.Random.String2(30), timeProvider.GetUtcNow().AddHours(1).UtcDateTime));
        tokenService.GenerateRefreshToken().Returns((faker.Random.String2(30), faker.Random.String2(30)));
        var expectedExpires = timeProvider.GetUtcNow().AddHours(8);
        //Act
        var result = await LoginHandler.Login(timeProvider, tokenService, unitOfWork, refreshTokenRepo, user, rememberMe: false, CancellationToken.None);
        //Assert
        result.Value.RefreshTokenExpires.Should().Be(expectedExpires);
        result.Value.RememberMe.Should().BeFalse();
    }

    [Fact]
    public async Task Login_WhenRefreshTokenHashIsEmpty_ShouldReturnFailureAndNotPersist()
    {
        //Arrange
        var user = CreateUserDto();
        tokenService.GenerateToken(Arg.Any<UserDto>()).Returns((faker.Random.String2(30), timeProvider.GetUtcNow().AddHours(1).UtcDateTime));
        tokenService.GenerateRefreshToken().Returns((faker.Random.String2(30), string.Empty));
        //Act
        var result = await LoginHandler.Login(timeProvider, tokenService, unitOfWork, refreshTokenRepo, user, rememberMe: false, CancellationToken.None);
        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Failure.Message.Should().Be("Uma hash deve ser informada para a criação do Token.");
        refreshTokenRepo.DidNotReceive().Add(Arg.Any<RefreshToken>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Login_WithValidData_ShouldPersistRefreshTokenWithExpectedValues()
    {
        //Arrange
        var user = CreateUserDto();
        var rtHash = faker.Random.String2(30);
        tokenService.GenerateToken(Arg.Any<UserDto>()).Returns((faker.Random.String2(30), timeProvider.GetUtcNow().AddHours(1).UtcDateTime));
        tokenService.GenerateRefreshToken().Returns((faker.Random.String2(30), rtHash));
        //Act
        await LoginHandler.Login(timeProvider, tokenService, unitOfWork, refreshTokenRepo, user, rememberMe: true, CancellationToken.None);
        //Assert
        refreshTokenRepo.Received(1).Add(Arg.Is<RefreshToken>(rt =>
            rt.UserId == user.UserId &&
            rt.TokenHash == rtHash &&
            rt.RememberMe == true));
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}