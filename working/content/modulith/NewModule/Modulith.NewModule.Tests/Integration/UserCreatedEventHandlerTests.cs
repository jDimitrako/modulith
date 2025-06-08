using Microsoft.Extensions.Logging;
using Moq;
using Modulith.NewModule.Application.IntegrationEvents;
using Modulith.NewModule.Domain.Entities;
using Modulith.NewModule.Domain.Interfaces;
using Modulith.User.Contracts.Events;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Modulith.NewModule.Tests.Integration;

public class UserCreatedEventHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<UserCreatedEventHandler>> _loggerMock;
    private readonly UserCreatedEventHandler _handler;

    public UserCreatedEventHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<UserCreatedEventHandler>>();
        _handler = new UserCreatedEventHandler(_userRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WhenUserDoesNotExist_ShouldCreateNewUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var @event = new UserCreatedEvent(userId, "test@example.com", "John", "Doe", DateTime.UtcNow);

        _userRepositoryMock
            .Setup(x => x.ExistsAsync(userId))
            .ReturnsAsync(false);

        // Act
        await _handler.HandleAsync(@event);

        // Assert
        _userRepositoryMock.Verify(
            x => x.AddAsync(It.Is<User>(u => u.Id == userId)),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenUserExists_ShouldNotCreateNewUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var @event = new UserCreatedEvent(userId, "test@example.com", "John", "Doe", DateTime.UtcNow);

        _userRepositoryMock
            .Setup(x => x.ExistsAsync(userId))
            .ReturnsAsync(true);

        // Act
        await _handler.HandleAsync(@event);

        // Assert
        _userRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<User>()),
            Times.Never);
    }
} 