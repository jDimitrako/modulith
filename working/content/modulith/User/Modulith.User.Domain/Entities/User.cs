using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Modulith.User.Domain.Events;

namespace Modulith.User.Domain.Entities;

public class User : IdentityUser
{
    private readonly List<object> _domainEvents = new();

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    private User() { } // For EF Core

    public static User Create(
        string email,
        string firstName,
        string lastName)
    {
        var user = new User
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            EmailConfirmed = true // You might want to make this configurable
        };

        user._domainEvents.Add(new UserCreatedEvent(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            new List<string> { "User" }));

        return user;
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        
        IsActive = false;
        _domainEvents.Add(new UserDeactivatedEvent(Id, Email!));
    }

    public void Activate()
    {
        if (IsActive) return;
        
        IsActive = true;
        _domainEvents.Add(new UserActivatedEvent(Id, Email!));
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        var oldFirstName = FirstName;
        var oldLastName = LastName;

        FirstName = firstName;
        LastName = lastName;

        _domainEvents.Add(new UserUpdatedEvent(
            Id,
            Email!,
            FirstName,
            LastName,
            new List<string> { "User" }));
    }

    public void UpdateRoles(List<string> newRoles)
    {
        var oldRoles = new List<string> { "User" }; // In a real app, get current roles

        _domainEvents.Add(new UserRoleChangedEvent(
            Id,
            Email!,
            oldRoles,
            newRoles));
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
} 