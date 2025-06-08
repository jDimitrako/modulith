using DotNetCore.CAP;
using MediatR;
using Microsoft.Extensions.Logging;
using Modulith.User.Domain.Events;
using Modulith.User.Contracts.Events;
using Modulith.User.Domain.Entities;
using System.Threading.Tasks;

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
    private readonly ICapPublisher _capPublisher;

    public UserDomainEventHandler(
        ILogger<UserDomainEventHandler> logger,
        ICapPublisher capPublisher)
    {
        _logger = logger;
        _capPublisher = capPublisher;
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

    public async Task HandleDomainEventsAsync(User user)
    {
        foreach (var domainEvent in user.DomainEvents)
        {
            switch (domainEvent)
            {
                case UserCreatedEvent createdEvent:
                    await _capPublisher.PublishAsync("UserCreated", createdEvent);
                    _logger.LogInformation("Published UserCreated event for user {UserId}", createdEvent.UserId);
                    break;

                case UserUpdatedEvent updatedEvent:
                    await _capPublisher.PublishAsync("UserUpdated", updatedEvent);
                    _logger.LogInformation("Published UserUpdated event for user {UserId}", updatedEvent.UserId);
                    break;

                case UserDeactivatedEvent deactivatedEvent:
                    await _capPublisher.PublishAsync("UserDeactivated", deactivatedEvent);
                    _logger.LogInformation("Published UserDeactivated event for user {UserId}", deactivatedEvent.UserId);
                    break;

                case UserActivatedEvent activatedEvent:
                    await _capPublisher.PublishAsync("UserActivated", activatedEvent);
                    _logger.LogInformation("Published UserActivated event for user {UserId}", activatedEvent.UserId);
                    break;

                case UserRoleChangedEvent roleChangedEvent:
                    await _capPublisher.PublishAsync("UserRoleChanged", roleChangedEvent);
                    _logger.LogInformation("Published UserRoleChanged event for user {UserId}", roleChangedEvent.UserId);
                    break;
            }
        }

        user.ClearDomainEvents();
    }
} 