using FluentAssertions;
using Modulith.NewModule.Api.Dtos;
using Modulith.NewModule.Api.Mappers;
using Modulith.NewModule.Entities;
using Xunit;

namespace Modulith.NewModule.Tests;

public class TodoItemMapperTests
{
    [Fact]
    public void ToDto_Should_Map_Entity_To_Dto()
    {
        var entity = new TodoItem { Id = 1, Title = "Test", IsComplete = true };
        var dto = TodoItemMapper.ToDto(entity);
        dto.Id.Should().Be(1);
        dto.Title.Should().Be("Test");
        dto.IsComplete.Should().BeTrue();
    }
} 