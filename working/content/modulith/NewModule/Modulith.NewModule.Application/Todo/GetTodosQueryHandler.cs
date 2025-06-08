using MediatR;
using Microsoft.EntityFrameworkCore;
using Modulith.NewModule.Infrastructure;
using Modulith.SharedKernel.Infrastructure.Caching;

namespace Modulith.NewModule.Application.Todo;

public record GetTodosQuery : IRequest<IEnumerable<TodoItem>>;

public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, IEnumerable<TodoItem>>
{
    private readonly ModulithNewModuleDbContext _dbContext;
    private readonly ICacheService _cache;
    private const string CacheKey = "todos:all";

    public GetTodosQueryHandler(
        ModulithNewModuleDbContext dbContext,
        ICacheService cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<IEnumerable<TodoItem>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        return await _cache.GetOrSetAsync(
            CacheKey,
            async () =>
            {
                var todos = await _dbContext.TodoItems
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync(cancellationToken);

                // Schedule a background job to clean up old completed todos
                BackgroundJob.Enqueue<ITodoCleanupService>(x => x.CleanupOldCompletedTodosAsync());

                return todos;
            },
            TimeSpan.FromMinutes(5),
            cancellationToken);
    }
} 