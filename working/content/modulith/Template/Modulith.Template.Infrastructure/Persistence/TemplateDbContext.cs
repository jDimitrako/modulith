using Microsoft.EntityFrameworkCore;
using Modulith.Template.Domain.Entities;

namespace Modulith.Template.Infrastructure.Persistence;

public class TemplateDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public TemplateDbContext(DbContextOptions<TemplateDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
        });
    }
} 