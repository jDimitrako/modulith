using DotNetCore.CAP;
using MediatR;
using Microsoft.Extensions.Logging;
using Modulith.User.Domain.Events;

namespace Modulith.User.Application.DomainEventHandlers;

public class UserDomainEventHandler : 
    ICapSubscribe,
    INotificationHandler<UserCreatedEvent>,
    INotificationHandler<UserUpdatedEvent>,
    INotificationHandler<UserDeactivatedEvent>,
    INotificationHandler<UserActivatedEvent>,
    INotificationHandler<UserRoleChangedEvent>
{
    private readonly ILogger<UserDomainEventHandler> _logger;

    public UserDomainEventHandler(ILogger<UserDomainEventHandler> logger)
    {
        _logger = logger;
    }

    [CapSubscribe("user.created")]
    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User created: {UserId}, Email: {Email}, Name: {FirstName} {LastName}",
            notification.UserId,
            notification.Email,
            notification.FirstName,
            notification.LastName);

        return Task.CompletedTask;
    }

    [CapSubscribe("user.updated")]
    public Task Handle(UserUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User updated: {UserId}, Email: {Email}, Name: {FirstName} {LastName}",
            notification.UserId,
            notification.Email,
            notification.FirstName,
            notification.LastName);

        return Task.CompletedTask;
    }

    [CapSubscribe("user.deactivated")]
    public Task Handle(UserDeactivatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User deactivated: {UserId}, Email: {Email}",
            notification.UserId,
            notification.Email);

        return Task.CompletedTask;
    }

    [CapSubscribe("user.activated")]
    public Task Handle(UserActivatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User activated: {UserId}, Email: {Email}",
            notification.UserId,
            notification.Email);

        return Task.CompletedTask;
    }

    [CapSubscribe("user.role-changed")]
    public Task Handle(UserRoleChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User roles changed: {UserId}, Email: {Email}, Old Roles: {OldRoles}, New Roles: {NewRoles}",
            notification.UserId,
            notification.Email,
            string.Join(", ", notification.OldRoles),
            string.Join(", ", notification.NewRoles));

        return Task.CompletedTask;
    }
} 