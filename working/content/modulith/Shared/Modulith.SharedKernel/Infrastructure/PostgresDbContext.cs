using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Modulith.SharedKernel.Infrastructure;

public abstract class PostgresDbContext : DbContext
{
    protected PostgresDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Enable pgvector extension
        modelBuilder.HasPostgresExtension("vector");

        // Enable Apache AGE extension
        modelBuilder.HasPostgresExtension("age");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Configure PostgreSQL to use pgvector
        optionsBuilder.UseNpgsql(options => 
        {
            options.UseVector();
        });
    }

    // Add migration support
    public class PostgresDbContextMigrationContext : PostgresDbContext
    {
        public PostgresDbContextMigrationContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
} 