using System.Collections.Generic;

namespace Modulith.NewModule.Api.Dtos;

public class TodoListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<TodoItemDto> Items { get; set; } = new();
} 