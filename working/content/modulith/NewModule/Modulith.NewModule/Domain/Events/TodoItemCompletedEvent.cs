using Modulith.SharedKernel.Entities;
using Modulith.NewModule.Entities;

namespace Modulith.NewModule.Domain.Events;

public class TodoItemCompletedEvent : DomainEventBase
{
    public TodoItem TodoItem { get; }

    public TodoItemCompletedEvent(TodoItem todoItem)
    {
        TodoItem = todoItem;
    }
} 