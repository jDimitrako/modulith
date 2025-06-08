using Microsoft.EntityFrameworkCore;

namespace Modulith.NewModule.Infrastructure
{
    public class ModulithNewModuleDbContext : DbContext
    {
        public DbSet<Entities.TodoItem> TodoItems => Set<Entities.TodoItem>();
    }
} 