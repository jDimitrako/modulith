using AutoMapper;
using FluentAssertions;
using Modulith.NewModule.Api;
using Modulith.NewModule.Api.Dtos;
using Modulith.NewModule.Entities;
using Xunit;

namespace Modulith.NewModule.Tests;

public class AutoMapperProfileTests
{
    [Fact]
    public void AutoMapperProfile_Should_Map_TodoItem_To_TodoItemDto()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        var mapper = config.CreateMapper();
        var entity = new TodoItem { Id = 2, Title = "AutoMap", IsComplete = false };
        var dto = mapper.Map<TodoItemDto>(entity);
        dto.Id.Should().Be(2);
        dto.Title.Should().Be("AutoMap");
        dto.IsComplete.Should().BeFalse();
    }

    [Fact]
    public void AutoMapperProfile_Should_Map_Nested_And_Custom_Properties()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        var mapper = config.CreateMapper();
        var entity = new TodoItem
        {
            Id = 3,
            Title = "Advanced",
            IsComplete = true,
            Details = new TodoDetails { Description = "Desc", DueDate = DateTime.Today }
        };
        var dto = mapper.Map<TodoItemDto>(entity);
        dto.Description.Should().Be("Desc");
        dto.DueDate.Should().Be(DateTime.Today);
        dto.Status.Should().Be("Complete");
    }

    [Fact]
    public void AutoMapperProfile_Should_Map_Collections()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        var mapper = config.CreateMapper();
        var list = new TodoList
        {
            Id = 1,
            Name = "List",
            Items = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "A", IsComplete = false },
                new TodoItem { Id = 2, Title = "B", IsComplete = true }
            }
        };
        var dto = mapper.Map<TodoListDto>(list);
        dto.Items.Should().HaveCount(2);
        dto.Items[0].Title.Should().Be("A");
        dto.Items[1].Status.Should().Be("Complete");
    }
} 