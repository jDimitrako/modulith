using Microsoft.EntityFrameworkCore;
using Modulith.SharedKernel.Infrastructure;

namespace Modulith.NewModule.Infrastructure;

public class ModulithNewModuleDbContext : PostgresDbContext
{
    public ModulithNewModuleDbContext(DbContextOptions<ModulithNewModuleDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Set the default schema for this module
        modelBuilder.HasDefaultSchema("newmodule"); // Change to your module name
        // ... entity configurations ...
    }
} 