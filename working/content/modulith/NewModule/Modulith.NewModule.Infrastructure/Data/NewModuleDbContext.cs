using Microsoft.EntityFrameworkCore;
using Modulith.NewModule.Domain.Entities;

namespace Modulith.NewModule.Infrastructure.Data;

public class NewModuleDbContext : DbContext
{
    public NewModuleDbContext(DbContextOptions<NewModuleDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
        });
    }
} 