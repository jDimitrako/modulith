using MediatR;
using System;

namespace Modulith.SharedKernel.Entities;

public abstract class DomainEventBase : INotification
{
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
} 