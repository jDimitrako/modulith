using DotNetCore.CAP;
using Microsoft.Extensions.Caching.Distributed;

namespace Modulith.NewModule.Application.Commands
{
    public class AddTodoCommandHandler : IRequestHandler<AddTodoCommand, Todo>
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

        public async Task<Todo> Handle(AddTodoCommand request, CancellationToken cancellationToken)
        {
            var todo = new Todo
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                Completed = request.Completed
            };

            _db.Todos.Add(todo);
            await _db.SaveChangesAsync(cancellationToken);

            // Publish CAP event
            await _capPublisher.PublishAsync("todo.added", new { todo.Id, todo.Title });

            // Cache the new todo in Redis
            await _cache.SetStringAsync($"todo:{todo.Id}", todo.Title);

            return todo;
        }
    }
} 