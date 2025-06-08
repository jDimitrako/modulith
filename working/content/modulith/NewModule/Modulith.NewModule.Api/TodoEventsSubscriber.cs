using DotNetCore.CAP;
using Microsoft.Extensions.Logging;

namespace Modulith.NewModule.Api;

public class TodoEventsSubscriber
{
    private readonly ILogger<TodoEventsSubscriber> _logger;
    public TodoEventsSubscriber(ILogger<TodoEventsSubscriber> logger) => _logger = logger;

    [CapSubscribe("todo.added")]
    public void OnTodoAdded(dynamic todo)
    {
        _logger.LogInformation("Received todo.added event: {Id} - {Title}", todo.Id, todo.Title.Value);
        // Additional logic here
    }
} 