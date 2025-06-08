using DotNetCore.CAP;

namespace Modulith.User.Domain.Events;

public record UserCreatedEvent(
    string UserId,
    string Email,
    string FirstName,
    string LastName,
    List<string> Roles) : ICapSubscribe;

public record UserUpdatedEvent(
    string UserId,
    string Email,
    string FirstName,
    string LastName,
    List<string> Roles) : ICapSubscribe;

public record UserDeactivatedEvent(
    string UserId,
    string Email) : ICapSubscribe;

public record UserActivatedEvent(
    string UserId,
    string Email) : ICapSubscribe;

public record UserRoleChangedEvent(
    string UserId,
    string Email,
    List<string> OldRoles,
    List<string> NewRoles) : ICapSubscribe; 