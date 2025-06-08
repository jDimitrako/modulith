using System.Collections.Generic;
using System.Linq;

namespace Modulith.SharedKernel.Entities;

public abstract class AggregateRoot<TId> : Entity<TId>
{
    private readonly List<DomainEventBase> _domainEvents = new();

    protected AggregateRoot(TId id) : base(id) { }

    // For EF Core
    protected AggregateRoot() { }

    public IReadOnlyCollection<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(DomainEventBase eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
} 