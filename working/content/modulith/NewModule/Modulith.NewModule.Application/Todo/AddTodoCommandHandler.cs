using MediatR;
using Modulith.NewModule.Infrastructure;
using Modulith.SharedKernel.Infrastructure.Caching;
using Hangfire;

namespace Modulith.NewModule.Application.Todo;

public record AddTodoCommand(string Title, string? Description) : IRequest<TodoItem>;

public class AddTodoCommandHandler : IRequestHandler<AddTodoCommand, TodoItem>
{
    private readonly ModulithNewModuleDbContext _dbContext;
    private readonly ICacheService _cache;
    private const string CacheKey = "todos:all";

    public AddTodoCommandHandler(
        ModulithNewModuleDbContext dbContext,
        ICacheService cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<TodoItem> Handle(AddTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = new TodoItem(request.Title, request.Description);
        _dbContext.TodoItems.Add(todo);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Invalidate the cache
        await _cache.RemoveAsync(CacheKey, cancellationToken);

        // Schedule a background job to send notifications
        BackgroundJob.Enqueue<ITodoNotificationService>(x => 
            x.SendTodoCreatedNotificationAsync(todo.Id, todo.Title));

        // Schedule a reminder for 24 hours later if the todo is not completed
        BackgroundJob.Schedule<ITodoNotificationService>(
            x => x.SendTodoReminderNotificationAsync(todo.Id, todo.Title),
            TimeSpan.FromHours(24));

        return todo;
    }
} 