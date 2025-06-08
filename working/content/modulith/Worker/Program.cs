using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.Redis.StackExchange;
using Modulith.NewModule.Infrastructure;
using Modulith.SharedKernel.Infrastructure.OpenTelemetry;

namespace Modulith.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;

                services.AddSharedOpenTelemetry(
                    configuration,
                    "Modulith.Worker", // Service name for OpenTelemetry
                    "1.0.0"            // Service version
                );

                // Add NewModule Infrastructure services (for Hangfire jobs)
                services.AddInfrastructureServices(configuration);

                // Add Hangfire services
                services.AddHangfire(config =>
                {
                    config.UsePostgreSqlStorage(configuration.GetConnectionString("Postgres"));
                    config.UseRedisStorage(configuration.GetConnectionString("Redis"));
                    config.UseRecommendedSerializerSettings();
                });

                // Add the processing server as IHostedService
                services.AddHangfireServer(serverOptions =>
                {
                    serverOptions.ServerName = "Modulith.Worker";
                    serverOptions.Queues = new[] { "default", "critical" };
                    serverOptions.WorkerCount = Environment.ProcessorCount * 5;
                });
            });
} 