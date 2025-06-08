namespace Modulith.NewModule.Entities;

public record TodoTitle
{
    public string Value { get; }

    public TodoTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Todo title cannot be empty.", nameof(value));
        Value = value;
    }

    public static implicit operator string(TodoTitle title) => title.Value;
    public static explicit operator TodoTitle(string value) => new TodoTitle(value);
} 