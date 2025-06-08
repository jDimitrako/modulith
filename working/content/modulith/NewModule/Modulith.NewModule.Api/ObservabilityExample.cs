using Microsoft.Extensions.Logging;
using Prometheus;
using Sentry;
using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Modulith.NewModule.Api;

public class ObservabilityExampleService
{
    private static readonly Counter ExampleCounter = Metrics.CreateCounter("example_counter", "An example Prometheus counter");
    private static readonly Counter BusinessEventCounter = Metrics.CreateCounter(
        "business_event_counter",
        "A business KPI counter",
        new CounterConfiguration { LabelNames = new[] { "module" } }
    );
    private static readonly Histogram RequestDuration = Metrics.CreateHistogram("http_request_duration_seconds", "HTTP request duration in seconds");
    private readonly ILogger<ObservabilityExampleService> _logger;
    public ObservabilityExampleService(ILogger<ObservabilityExampleService> logger) => _logger = logger;

    public void DoSomething(string userId, string moduleName)
    {
        using (RequestDuration.NewTimer())
        {
            ExampleCounter.Inc();
            BusinessEventCounter.WithLabels(moduleName).Inc();
            _logger.LogInformation($"Did something in {moduleName}! Counter incremented.");

            // Sentry: Add user context and custom breadcrumb
            SentrySdk.ConfigureScope(scope =>
            {
                scope.User = new User { Id = userId };
                scope.AddBreadcrumb("Did something important", "business");
            });

            // Sentry: Capture a custom event
            SentrySdk.CaptureMessage("Business event occurred", SentryLevel.Info);

            // Sentry: Performance monitoring (transaction)
            var transaction = SentrySdk.StartTransaction("business.operation", "custom-operation");
            try
            {
                // Simulate work
                System.Threading.Thread.Sleep(100);
                transaction.Finish(SpanStatus.Ok);
            }
            catch (Exception ex)
            {
                transaction.Finish(SpanStatus.InternalError);
                SentrySdk.CaptureException(ex);
            }
        }
    }
}

public class ExampleHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // Simulate a healthy check
        return Task.FromResult(HealthCheckResult.Healthy("Example health check is healthy."));
    }
} 