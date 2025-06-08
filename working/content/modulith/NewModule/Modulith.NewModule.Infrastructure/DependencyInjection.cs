using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modulith.NewModule.Application.Todo;

namespace Modulith.NewModule.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ModulithNewModuleDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ModulithNewModuleDbContext).Assembly.FullName)));

        // Register services
        services.AddScoped<ITodoNotificationService, TodoNotificationService>();
        services.AddScoped<ITodoCleanupService, TodoCleanupService>();

        return services;
    }
} 