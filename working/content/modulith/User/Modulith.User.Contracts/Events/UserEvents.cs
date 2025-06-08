using System;

namespace Modulith.User.Contracts.Events;

public record UserCreatedEvent(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAt);

public record UserUpdatedEvent(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    DateTime UpdatedAt);

public record UserDeactivatedEvent(
    Guid UserId,
    DateTime DeactivatedAt);

public record UserActivatedEvent(
    Guid UserId,
    DateTime ActivatedAt);

public record UserRoleChangedEvent(
    Guid UserId,
    string[] Roles,
    DateTime ChangedAt); 