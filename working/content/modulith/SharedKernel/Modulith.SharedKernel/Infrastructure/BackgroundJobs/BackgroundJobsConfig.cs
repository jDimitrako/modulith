using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.Dashboard;

namespace Modulith.SharedKernel.Infrastructure.BackgroundJobs;

public static class BackgroundJobsConfig
{
    public static IServiceCollection AddSharedBackgroundJobs(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHangfire(config =>
        {
            config.UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
            });

            // Configure Hangfire to use the same JSON serialization settings as the application
            config.UseSerializerSettings(new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        });

        // Add the processing server as IHostedService
        services.AddHangfireServer(options =>
        {
            options.WorkerCount = Environment.ProcessorCount * 5;
            options.Queues = new[] { "default", "critical" };
        });

        return services;
    }

    public static IApplicationBuilder UseSharedBackgroundJobs(
        this IApplicationBuilder app,
        IConfiguration configuration)
    {
        app.UseHangfireDashboard("/jobs", new DashboardOptions
        {
            Authorization = new[]
            {
                new HangfireCustomBasicAuthenticationFilter
                {
                    User = configuration["Hangfire:Username"] ?? "admin",
                    Pass = configuration["Hangfire:Password"] ?? "admin"
                }
            }
        });

        return app;
    }
}

public class HangfireCustomBasicAuthenticationFilter : IDashboardAuthorizationFilter
{
    public string User { get; set; } = string.Empty;
    public string Pass { get; set; } = string.Empty;

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        string header = httpContext.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(header) || !header.StartsWith("Basic "))
        {
            return false;
        }

        var authValues = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(header.Substring(6))).Split(':');
        if (authValues.Length != 2)
        {
            return false;
        }

        return authValues[0] == User && authValues[1] == Pass;
    }
} 