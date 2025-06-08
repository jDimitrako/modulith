using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Modulith.User.Domain.Events;
using Modulith.User.Contracts.Events;
using System;
using System.Collections.Generic;

namespace Modulith.User.Domain.Entities;

public class User : IdentityUser
{
    private readonly List<object> _domainEvents = new();

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string[] Roles { get; private set; } = Array.Empty<string>();

    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    private User() { } // For EF Core

    public static User Create(
        string email,
        string passwordHash,
        string firstName,
        string lastName)
    {
        var user = new User
        {
            UserName = email,
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            IsActive = true,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow,
            Roles = new[] { "User" }
        };

        user._domainEvents.Add(new UserCreatedEvent(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            user.CreatedAt));

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
        UpdatedAt = DateTime.UtcNow;
        _domainEvents.Add(new UserDeactivatedEvent(Id, UpdatedAt.Value));
    }

    public void Activate()
    {
        if (IsActive) return;
        
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        _domainEvents.Add(new UserActivatedEvent(Id, UpdatedAt.Value));
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        var oldFirstName = FirstName;
        var oldLastName = LastName;

        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;

        _domainEvents.Add(new UserUpdatedEvent(
            Id,
            Email!,
            FirstName,
            LastName,
            UpdatedAt.Value));
    }

    public void ConfirmEmail()
    {
        if (EmailConfirmed) return;

        EmailConfirmed = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRoles(string[] roles)
    {
        var oldRoles = Roles;
        Roles = roles;
        UpdatedAt = DateTime.UtcNow;

        _domainEvents.Add(new UserRoleChangedEvent(
            Id,
            Roles,
            UpdatedAt.Value));
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
} 