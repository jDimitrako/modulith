using DotNetCore.CAP;

namespace Modulith.NewModule.Api;

public class CapEventPublisher
{
    private readonly ICapPublisher _capBus;
    public CapEventPublisher(ICapPublisher capBus) => _capBus = capBus;

    public async Task PublishExampleEventAsync()
    {
        await _capBus.PublishAsync("example.event", new { Message = "Hello from CAP!" });
    }
}

public class CapEventSubscriber
{
    [CapSubscribe("example.event")]
    public void HandleExampleEvent(dynamic data)
    {
        // Handle the event (inbox pattern)
        Console.WriteLine($"Received event: {data.Message}");
    }
} 