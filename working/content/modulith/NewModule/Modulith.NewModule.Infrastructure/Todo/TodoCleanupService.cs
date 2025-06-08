using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Modulith.NewModule.Infrastructure.Todo;

public class TodoCleanupService : ITodoCleanupService
{
    private readonly ModulithNewModuleDbContext _dbContext;
    private readonly ILogger<TodoCleanupService> _logger;

    public TodoCleanupService(
        ModulithNewModuleDbContext dbContext,
        ILogger<TodoCleanupService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task CleanupOldCompletedTodosAsync()
    {
        var cutoffDate = DateTime.UtcNow.AddMonths(-1);
        var oldCompletedTodos = await _dbContext.TodoItems
            .Where(t => t.IsCompleted && t.CompletedAt < cutoffDate)
            .ToListAsync();

        if (oldCompletedTodos.Any())
        {
            _dbContext.TodoItems.RemoveRange(oldCompletedTodos);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Cleaned up {Count} old completed todos", oldCompletedTodos.Count);
        }
    }
} 