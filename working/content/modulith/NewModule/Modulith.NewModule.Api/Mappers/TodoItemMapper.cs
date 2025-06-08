using Modulith.NewModule.Api.Dtos;
using Modulith.NewModule.Entities;

namespace Modulith.NewModule.Api.Mappers;

public static class TodoItemMapper
{
    public static TodoItemDto ToDto(TodoItem entity) =>
        new TodoItemDto
        {
            Id = entity.Id,
            Title = entity.Title.Value,
            IsComplete = entity.IsComplete,
            Description = entity.Details.Description,
            DueDate = entity.Details.DueDate,
            Status = entity.IsComplete ? "Complete" : "Pending"
        };
} 