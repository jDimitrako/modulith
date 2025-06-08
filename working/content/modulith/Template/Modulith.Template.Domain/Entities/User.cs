using System;

namespace Modulith.Template.Domain.Entities;

public class User
{
    private User() { } // For EF Core

    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public static User Create(Guid id)
    {
        return new User
        {
            Id = id,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update()
    {
        UpdatedAt = DateTime.UtcNow;
    }
} 