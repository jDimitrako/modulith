using Microsoft.Extensions.Diagnostics.HealthChecks;
using Modulith.NewModule.Infrastructure.Data;

namespace Modulith.NewModule.Api.HealthChecks;

public static class NewModuleHealthChecks
{
    public static IHealthChecksBuilder AddNewModuleHealthChecks(
        this IHealthChecksBuilder builder)
    {
        return builder
            .AddDbContextCheck<NewModuleDbContext>("newmodule-db")
            .AddCheck<NewModuleHealthCheck>("newmodule-health");
    }
}

public class NewModuleHealthCheck : IHealthCheck
{
    private readonly ILogger<NewModuleHealthCheck> _logger;

    public NewModuleHealthCheck(ILogger<NewModuleHealthCheck> logger)
    {
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Add your custom health check logic here
            // For example, check if required services are available
            // or if the module is functioning correctly

            return Task.FromResult(HealthCheckResult.Healthy("NewModule is healthy"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "NewModule health check failed");
            return Task.FromResult(HealthCheckResult.Unhealthy("NewModule is unhealthy", ex));
        }
    }
} 