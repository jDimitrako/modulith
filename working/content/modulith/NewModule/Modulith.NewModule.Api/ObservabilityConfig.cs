using Serilog;
using Prometheus;

namespace Modulith.NewModule.Api;

public static class ObservabilityConfig
{
    public static void ConfigureSerilog(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Seq(context.Configuration["Seq:Url"] ?? "http://seq:5341")
            .WriteTo.Sentry(o =>
            {
                o.Dsn = context.Configuration["Sentry:Dsn"];
                o.MinimumBreadcrumbLevel = Serilog.Events.LogEventLevel.Information;
                o.MinimumEventLevel = Serilog.Events.LogEventLevel.Error;
            }));
    }

    public static void UsePrometheusMetrics(this WebApplication app)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapMetrics(); // Exposes /metrics endpoint for Prometheus
        });
    }

    public static void AddHealthChecksWithPrometheus(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .ForwardToPrometheus();
    }
} 