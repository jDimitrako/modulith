using Modulith.SharedKernel.Entities;
using Modulith.NewModule.Entities;

namespace Modulith.NewModule.Domain.Events;

public class TodoItemCreatedEvent : DomainEventBase
{
    public TodoItem TodoItem { get; }

    public TodoItemCreatedEvent(TodoItem todoItem)
    {
        TodoItem = todoItem;
    }
} 