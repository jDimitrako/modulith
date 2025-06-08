using MediatR;
using Microsoft.Extensions.Logging;
using Modulith.NewModule.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Modulith.NewModule.Application.DomainEventHandlers;

public class TodoItemCreatedEventHandler : INotificationHandler<TodoItemCreatedEvent>
{
    private readonly ILogger<TodoItemCreatedEventHandler> _logger;

    public TodoItemCreatedEventHandler(ILogger<TodoItemCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(TodoItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: TodoItemCreatedEvent handled for TodoItem {TodoItemId}", notification.TodoItem.Id);
        // Add any additional business logic or side effects here
        return Task.CompletedTask;
    }
} 