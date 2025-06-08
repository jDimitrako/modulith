using FluentAssertions;
using Modulith.NewModule.Entities;
using System;
using Xunit;

namespace Modulith.NewModule.Tests;

public class TodoItemTests
{
    [Fact]
    public void Constructor_Should_Throw_If_Title_Is_Empty()
    {
        Action act = () => new TodoItem("");
        act.Should().Throw<ArgumentException>().WithMessage("*Title is required.*");
    }

    [Fact]
    public void MarkComplete_Should_Set_IsComplete_To_True()
    {
        var todo = new TodoItem("Test");
        todo.MarkComplete();
        todo.IsComplete.Should().BeTrue();
    }

    [Fact]
    public void MarkComplete_Should_Throw_If_Already_Complete()
    {
        var todo = new TodoItem("Test");
        todo.MarkComplete();
        Action act = () => todo.MarkComplete();
        act.Should().Throw<InvalidOperationException>().WithMessage("*already complete*");
    }

    [Fact]
    public void UpdateDetails_Should_Set_Description_And_DueDate()
    {
        var todo = new TodoItem("Test");
        todo.UpdateDetails("desc", new DateTime(2024, 1, 1));
        todo.Details.Description.Should().Be("desc");
        todo.Details.DueDate.Should().Be(new DateTime(2024, 1, 1));
    }
} 