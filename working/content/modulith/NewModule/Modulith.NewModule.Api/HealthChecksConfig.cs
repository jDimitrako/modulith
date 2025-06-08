using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Modulith.NewModule.Api;

public static class HealthChecksConfig
{
    public static IServiceCollection AddHealthChecksConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("DefaultConnection")!)
            .AddRedis(configuration.GetConnectionString("Redis")!)
            .AddRabbitmq(configuration.GetConnectionString("RabbitMQ")!)
            .AddCheck<ExampleHealthCheck>("Example")
            .AddUrlGroup(new Uri(configuration["ExternalServices:PaymentGateway"]!), "Payment Gateway")
            .AddUrlGroup(new Uri(configuration["ExternalServices:EmailService"]!), "Email Service");

        services.AddHealthChecksUI(options =>
        {
            options.SetEvaluationTimeInSeconds(15); // Time in seconds between health checks
            options.MaximumHistoryEntriesPerEndpoint(50); // Maximum number of entries to keep in history
            options.SetApiMaxActiveRequests(1); // Maximum number of concurrent requests to the health check API
            options.AddHealthCheckEndpoint("NewModule API", "/health"); // Health check endpoint
        })
        .AddInMemoryStorage(); // Use in-memory storage for health check results

        return services;
    }

    public static IApplicationBuilder UseHealthChecksConfig(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            AllowCachingResponses = false,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        });

        app.UseHealthChecksUI(options =>
        {
            options.UIPath = "/health-ui"; // Health check UI endpoint
            options.ApiPath = "/health-api"; // Health check API endpoint
            options.AddCustomStylesheet("./healthchecks.css"); // Custom CSS for the UI
        });

        return app;
    }
} 