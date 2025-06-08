using Microsoft.Extensions.Logging;

namespace Modulith.NewModule.Infrastructure.Todo;

public class TodoNotificationService : ITodoNotificationService
{
    private readonly ILogger<TodoNotificationService> _logger;

    public TodoNotificationService(ILogger<TodoNotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendTodoCreatedNotificationAsync(int todoId, string title)
    {
        _logger.LogInformation("Sending notification for newly created todo: {TodoId} - {Title}", todoId, title);
        // Implement actual notification logic (email, push notification, etc.)
        return Task.CompletedTask;
    }

    public Task SendTodoReminderNotificationAsync(int todoId, string title)
    {
        _logger.LogInformation("Sending reminder notification for todo: {TodoId} - {Title}", todoId, title);
        // Implement actual notification logic (email, push notification, etc.)
        return Task.CompletedTask;
    }
} 