using System.Collections.Generic;

namespace Modulith.NewModule.Entities;

public class TodoList
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<TodoItem> Items { get; set; } = new();
} 