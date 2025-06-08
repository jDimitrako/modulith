using MediatR;
using Modulith.NewModule.Entities;
using Modulith.NewModule.Infrastructure;

namespace Modulith.NewModule.Application.Commands;

public record AddTodoCommand(string Title) : IRequest<TodoItem>;

public class AddTodoCommandHandler : IRequestHandler<AddTodoCommand, TodoItem>
{
    private readonly ModulithNewModuleDbContext _db;
    public AddTodoCommandHandler(ModulithNewModuleDbContext db) => _db = db;

    public async Task<TodoItem> Handle(AddTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = new TodoItem { Title = request.Title };
        _db.Add(todo);
        await _db.SaveChangesAsync(cancellationToken);
        return todo;
    }
} 