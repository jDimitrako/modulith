using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Instrumentation.EntityFrameworkCore;
using OpenTelemetry.Instrumentation.StackExchangeRedis;
using OpenTelemetry.Instrumentation.SqlClient;
using OpenTelemetry.Instrumentation.EventCounters;
using OpenTelemetry.Instrumentation.Runtime;
using OpenTelemetry.Instrumentation.Process;

namespace Modulith.SharedKernel.Infrastructure.OpenTelemetry;

public static class OpenTelemetryConfig
{
    public static IServiceCollection AddSharedOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName,
        string serviceVersion)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(
                serviceName: serviceName,
                serviceVersion: serviceVersion,
                serviceInstanceId: Environment.MachineName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(serviceName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddStackExchangeRedisInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(configuration["OpenTelemetry:CollectorUrl"] ?? "http://localhost:4317");
                    });
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation()
                    .AddEventCountersInstrumentation()
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(configuration["OpenTelemetry:CollectorUrl"] ?? "http://localhost:4317");
                    });
            });

        return services;
    }
} 