using Microsoft.EntityFrameworkCore;
using Modulith.SharedKernel.Infrastructure;
using MediatR;
using Modulith.SharedKernel.Entities;

namespace Modulith.NewModule.Infrastructure;

public class ModulithNewModuleDbContext : PostgresDbContext
{
    private readonly IMediator _mediator;

    public ModulithNewModuleDbContext(DbContextOptions<ModulithNewModuleDbContext> options, IMediator mediator) 
        : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<Entities.TodoItem> TodoItems => Set<Entities.TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelModeling(modelBuilder);
        // Set the default schema for this module
        modelBuilder.HasDefaultSchema("newmodule"); // Change to your module name

        // Configure TodoItem to use TodoTitle as a owned entity/complex type
        modelBuilder.Entity<Entities.TodoItem>().OwnsOne(t => t.Title);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch domain events
        var domainEntities = ChangeTracker.Entries<AggregateRoot<int>>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        foreach (var entity in domainEntities)
        {
            entity.Entity.ClearDomainEvents();
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
        }

        return result;
    }
} 