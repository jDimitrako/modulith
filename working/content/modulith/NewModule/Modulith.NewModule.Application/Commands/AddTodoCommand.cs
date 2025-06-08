using MediatR;
using Modulith.NewModule.Entities;
using Modulith.NewModule.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using DotNetCore.CAP;

namespace Modulith.NewModule.Application.Commands;

public record AddTodoCommand(string Title, string Description = "", DateTime? DueDate = null) : IRequest<TodoItem>;

public class AddTodoCommandHandler : IRequestHandler<AddTodoCommand, TodoItem>
{
    private readonly ModulithNewModuleDbContext _db;
    private readonly ICapPublisher _capPublisher;
    private readonly IDistributedCache _cache;

    public AddTodoCommandHandler(ModulithNewModuleDbContext db, ICapPublisher capPublisher, IDistributedCache cache)
    {
        _db = db;
        _capPublisher = capPublisher;
        _cache = cache;
    }

    public async Task<TodoItem> Handle(AddTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = new TodoItem(new TodoTitle(request.Title), new TodoDetails { Description = request.Description, DueDate = request.DueDate });
        _db.Add(todo);
        await _db.SaveChangesAsync(cancellationToken);
        // Publish CAP event
        await _capPublisher.PublishAsync("todo.added", new { todo.Id, todo.Title.Value });
        // Cache the new todo in Redis
        await _cache.SetStringAsync($"todo:{todo.Id}", todo.Title.Value);
        return todo;
    }
} 