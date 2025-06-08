namespace Modulith.NewModule.Application.Todo;

public interface ITodoCleanupService
{
    Task CleanupOldCompletedTodosAsync();
} 