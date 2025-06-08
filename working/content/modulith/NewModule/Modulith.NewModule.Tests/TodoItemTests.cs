using FluentAssertions;
using Modulith.NewModule.Entities;
using Xunit;

namespace Modulith.NewModule.Tests;

public class TodoItemTests
{
    [Fact]
    public void MarkComplete_Should_Set_IsComplete_To_True()
    {
        var todo = new TodoItem { Title = "Test" };
        todo.MarkComplete();
        todo.IsComplete.Should().BeTrue();
    }
} 