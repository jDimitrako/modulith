using Modulith.SharedKernel.Entities;
using System;

namespace Modulith.NewModule.Entities;

public class TodoItem : AggregateRoot<int>
{
    public TodoTitle Title { get; private set; }
    public bool IsComplete { get; private set; }
    public TodoDetails Details { get; private set; } = new();

    // Constructor for creation
    public TodoItem(TodoTitle title, TodoDetails? details = null)
    {
        Id = 0; // Set a default value or handle ID generation if not from DB
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Details = details ?? new TodoDetails();
        AddDomainEvent(new TodoItemCreatedEvent(this));
    }

    // For EF Core
    private TodoItem() { }

    public void MarkComplete()
    {
        if (IsComplete)
            throw new InvalidOperationException("Todo is already complete.");
        IsComplete = true;
        AddDomainEvent(new TodoItemCompletedEvent(this));
    }

    public void UpdateDetails(string description, DateTime? dueDate)
    {
        Details = new TodoDetails { Description = description, DueDate = dueDate };
    }
} 