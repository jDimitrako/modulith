using DotNetCore.CAP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Modulith.SharedKernel;

public static class CapConfiguration
{
    public static void AddSharedCap(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCap(x =>
        {
            x.UsePostgreSql(configuration.GetConnectionString("DefaultConnection"));
            x.UseRabbitMQ("rabbitmq", 5672, "guest", "guest");
            x.UseDashboard();
            // Optionally: x.UseSchema("sharedcap");
        });
    }
} 