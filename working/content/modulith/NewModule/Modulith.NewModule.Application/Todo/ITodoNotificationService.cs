namespace Modulith.NewModule.Application.Todo;

public interface ITodoNotificationService
{
    Task SendTodoCreatedNotificationAsync(int todoId, string title);
    Task SendTodoReminderNotificationAsync(int todoId, string title);
} 