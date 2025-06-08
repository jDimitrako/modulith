using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Modulith.API.Infrastructure;

public class ModuleConfiguration
{
    public string Name { get; set; } = string.Empty;
    public string BasePath { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
    public Dictionary<string, string> Settings { get; set; } = new();
}

public static class ModuleConfigurationExtensions
{
    public static IServiceCollection AddModuleConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ModuleConfiguration>(configuration.GetSection("Modules"));
        return services;
    }

    public static ModuleConfiguration GetModuleConfiguration(
        this IServiceProvider serviceProvider,
        string moduleName)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var moduleConfig = new ModuleConfiguration();
        configuration.GetSection($"Modules:{moduleName}").Bind(moduleConfig);
        return moduleConfig;
    }
} 