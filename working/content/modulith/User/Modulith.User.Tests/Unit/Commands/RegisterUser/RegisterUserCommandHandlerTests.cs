using Microsoft.AspNetCore.Identity;
using Moq;
using Modulith.User.Application.Commands.RegisterUser;
using Modulith.User.Domain.Entities;
using Xunit;

namespace Modulith.User.Tests.Unit.Commands.RegisterUser;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object,
            null, null, null, null, null, null, null, null);

        _handler = new RegisterUserCommandHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateUser()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            "John",
            "Doe");

        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), command.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "User"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        _userManagerMock.Verify(
            x => x.CreateAsync(It.IsAny<User>(), command.Password),
            Times.Once);
        _userManagerMock.Verify(
            x => x.AddToRoleAsync(It.IsAny<User>(), "User"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_UserCreationFails_ShouldReturnFailure()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "Password123!",
            "John",
            "Doe");

        var errors = new[]
        {
            new IdentityError { Code = "DuplicateEmail", Description = "Email already exists" }
        };

        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), command.Password))
            .ReturnsAsync(IdentityResult.Failed(errors));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains(errors, e => e.Code == "DuplicateEmail");
        _userManagerMock.Verify(
            x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()),
            Times.Never);
    }
} 