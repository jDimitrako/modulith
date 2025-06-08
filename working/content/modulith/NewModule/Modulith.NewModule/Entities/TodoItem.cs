namespace Modulith.NewModule.Entities;

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsComplete { get; private set; }
    public TodoDetails Details { get; set; } = new();

    public void MarkComplete() => IsComplete = true;
} 