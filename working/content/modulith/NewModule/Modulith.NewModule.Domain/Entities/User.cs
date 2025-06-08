using Modulith.SharedKernel.Domain;

namespace Modulith.NewModule.Domain.Entities;

public class User : Entity
{
    private User() { } // For EF Core

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