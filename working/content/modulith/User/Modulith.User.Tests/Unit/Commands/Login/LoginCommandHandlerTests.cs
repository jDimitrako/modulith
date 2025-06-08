using Microsoft.AspNetCore.Identity;
using Moq;
using Modulith.User.Application.Commands.Login;
using Modulith.User.Domain.Entities;
using Modulith.User.Infrastructure.Services;
using Xunit;

namespace Modulith.User.Tests.Unit.Commands.Login;

public class LoginCommandHandlerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object,
            null, null, null, null, null, null, null, null);

        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
        _signInManagerMock = new Mock<SignInManager<User>>(
            _userManagerMock.Object,
            contextAccessorMock.Object,
            userPrincipalFactoryMock.Object,
            null, null, null, null);

        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();

        _handler = new LoginCommandHandler(
            _signInManagerMock.Object,
            _userManagerMock.Object,
            _jwtTokenGeneratorMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ShouldReturnTokens()
    {
        // Arrange
        var command = new LoginCommand(
            "test@example.com",
            "Password123!",
            false);

        var user = User.Create(
            command.Email,
            "John",
            "Doe");

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(user, command.Password, false))
            .ReturnsAsync(SignInResult.Success);

        _jwtTokenGeneratorMock
            .Setup(x => x.GenerateToken(user))
            .Returns("jwt-token");

        _jwtTokenGeneratorMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal("jwt-token", result.Token);
        Assert.Equal("refresh-token", result.RefreshToken);
        Assert.Null(result.ErrorMessage);

        _userManagerMock.Verify(
            x => x.UpdateAsync(user),
            Times.Once);
        _userManagerMock.Verify(
            x => x.SetAuthenticationTokenAsync(
                user,
                "Modulith",
                "RefreshToken",
                "refresh-token"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidCredentials_ShouldReturnFailure()
    {
        // Arrange
        var command = new LoginCommand(
            "test@example.com",
            "WrongPassword",
            false);

        var user = User.Create(
            command.Email,
            "John",
            "Doe");

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(user, command.Password, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Invalid email or password", result.ErrorMessage);
        Assert.Null(result.Token);
        Assert.Null(result.RefreshToken);

        _userManagerMock.Verify(
            x => x.UpdateAsync(It.IsAny<User>()),
            Times.Never);
        _userManagerMock.Verify(
            x => x.SetAuthenticationTokenAsync(
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_DeactivatedUser_ShouldReturnFailure()
    {
        // Arrange
        var command = new LoginCommand(
            "test@example.com",
            "Password123!",
            false);

        var user = User.Create(
            command.Email,
            "John",
            "Doe");
        user.Deactivate();

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Account is deactivated", result.ErrorMessage);
        Assert.Null(result.Token);
        Assert.Null(result.RefreshToken);

        _signInManagerMock.Verify(
            x => x.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Never);
    }
} 