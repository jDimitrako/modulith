using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Modulith.NewModule.Application.Commands;
using Modulith.NewModule.Entities;
using Modulith.NewModule.Infrastructure;
using NSubstitute;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Modulith.NewModule.Tests;

public class AddTodoCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Add_TodoItem()
    {
        var options = new DbContextOptionsBuilder<ModulithNewModuleDbContext>()
            .UseInMemoryDatabase(databaseName: "AddTodoTestDb")
            .Options;
        var db = new ModulithNewModuleDbContext(options, Substitute.For<IMediator>());
        var capPublisher = Substitute.For<DotNetCore.CAP.ICapPublisher>();
        var cache = Substitute.For<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();

        var handler = new AddTodoCommandHandler(db, capPublisher, cache);
        var command = new AddTodoCommand("Test Todo", "Description", DateTime.Today);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Title.Value.Should().Be("Test Todo");
        db.TodoItems.Should().ContainSingle(x => x.Title.Value == "Test Todo");
    }

    [Fact]
    public async Task Handle_Should_Publish_CAP_Event_And_Cache_Todo()
    {
        var options = new DbContextOptionsBuilder<ModulithNewModuleDbContext>()
            .UseInMemoryDatabase(databaseName: "AddTodoTestDb2")
            .Options;
        var db = new ModulithNewModuleDbContext(options, Substitute.For<IMediator>());
        var capPublisher = Substitute.For<DotNetCore.CAP.ICapPublisher>();
        var cache = Substitute.For<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
        var handler = new AddTodoCommandHandler(db, capPublisher, cache);
        var command = new AddTodoCommand("Test Todo 2", "Another Description", DateTime.Now);

        var result = await handler.Handle(command, CancellationToken.None);

        await capPublisher.Received(1).PublishAsync("todo.added", Arg.Is<object>(o => o.GetPropertyValue<string>("Title") == "Test Todo 2"), Arg.Any<CancellationToken>());
        await cache.Received(1).SetStringAsync($"todo:{result.Id}", result.Title.Value, Arg.Any<DistributedCacheEntryOptions>(), Arg.Any<CancellationToken>());
    }
} 