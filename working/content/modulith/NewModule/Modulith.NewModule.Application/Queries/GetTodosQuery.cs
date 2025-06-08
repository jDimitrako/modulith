using MediatR;
using Modulith.NewModule.Entities;
using Modulith.NewModule.Infrastructure;

namespace Modulith.NewModule.Application.Queries;

public record GetTodosQuery() : IRequest<List<TodoItem>>;

public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, List<TodoItem>>
{
    private readonly ModulithNewModuleDbContext _db;
    public GetTodosQueryHandler(ModulithNewModuleDbContext db) => _db = db;

    public async Task<List<TodoItem>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        return await _db.Set<TodoItem>().ToListAsync(cancellationToken);
    }
} 