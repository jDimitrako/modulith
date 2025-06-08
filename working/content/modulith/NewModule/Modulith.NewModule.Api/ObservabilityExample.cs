using Microsoft.Extensions.Logging;
using Prometheus;

namespace Modulith.NewModule.Api;

public class ObservabilityExampleService
{
    private static readonly Counter ExampleCounter = Metrics.CreateCounter("example_counter", "An example Prometheus counter");
    private readonly ILogger<ObservabilityExampleService> _logger;
    public ObservabilityExampleService(ILogger<ObservabilityExampleService> logger) => _logger = logger;

    public void DoSomething()
    {
        ExampleCounter.Inc();
        _logger.LogInformation("Did something! Counter incremented.");
    }
} 