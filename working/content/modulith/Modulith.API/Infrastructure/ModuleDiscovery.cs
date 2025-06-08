using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Modulith.API.Infrastructure;

public static class ModuleDiscovery
{
    public static IServiceCollection DiscoverAndRegisterModules(
        this IServiceCollection services,
        IConfiguration configuration,
        Assembly[] assemblies)
    {
        var moduleTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IModule).IsAssignableFrom(t))
            .ToList();

        foreach (var moduleType in moduleTypes)
        {
            var module = (IModule)Activator.CreateInstance(moduleType)!;
            module.RegisterModule(services, configuration);
        }

        return services;
    }
}

public interface IModule
{
    void RegisterModule(IServiceCollection services, IConfiguration configuration);
} 