using Modulith.NewModule.Api.Dtos;
using Modulith.NewModule.Entities;

namespace Modulith.NewModule.Api.Mappers;

public static class TodoItemMapper
{
    public static TodoItemDto ToDto(TodoItem entity) =>
        new TodoItemDto
        {
            Id = entity.Id,
            Title = entity.Title,
            IsComplete = entity.IsComplete
        };
} 