using DotNetCore.CAP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modulith.SharedKernel;

namespace Modulith.NewModule.Api;

public static class CapConfig
{
    public static void AddCapServices(this IServiceCollection services, IConfiguration configuration, bool useDefaultCap = true)
    {
        if (useDefaultCap)
        {
            // Use shared CAP configuration
            services.AddSharedCap(configuration);
        }
        else
        {
            // Module-specific CAP configuration (override)
            services.AddCap(x =>
            {
                x.UsePostgreSql(configuration.GetConnectionString("ModuleSpecificDb"));
                x.UseRabbitMQ("rabbitmq", 5672, "guest", "guest", "module_vhost");
                x.UseDashboard();
                // Optionally: x.UseSchema("modulecap");
            });
        }
    }
} 