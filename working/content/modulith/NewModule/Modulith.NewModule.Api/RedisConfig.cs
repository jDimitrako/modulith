using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace Modulith.NewModule.Api;

public static class RedisConfig
{
    public static void AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "modulith:";
        });
    }
} 