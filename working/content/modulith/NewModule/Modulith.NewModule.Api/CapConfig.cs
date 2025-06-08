using DotNetCore.CAP;

namespace Modulith.NewModule.Api;

public static class CapConfig
{
    public static void AddCapServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCap(x =>
        {
            x.UsePostgreSql(configuration.GetConnectionString("DefaultConnection"));
            x.UseRabbitMQ("rabbitmq", 5672, "guest", "guest");
            x.UseDashboard();
        });
    }
} 