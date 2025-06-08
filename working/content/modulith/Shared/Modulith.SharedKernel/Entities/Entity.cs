using System.Collections.Generic;

namespace Modulith.SharedKernel.Entities;

public abstract class Entity<TId>
{
    public TId Id { get; protected set; }

    protected Entity(TId id) => Id = id;

    // For EF Core
    protected Entity() { }
} 